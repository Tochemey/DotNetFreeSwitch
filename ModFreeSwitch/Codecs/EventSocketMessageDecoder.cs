using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Messages;
using NLog;

namespace ModFreeSwitch.Codecs {
    /// <summary>
    ///     FreeSwitch message decoder.
    /// </summary>
    public class EventSocketMessageDecoder : ReplayingDecoder<State> {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Line feed character
        /// </summary>
        private const byte lineFeed = 10;

        /// <summary>
        ///     Maximum header size to read.
        /// </summary>
        private readonly int _maxHeaderSize;

        /// <summary>
        ///     This is very useful during freeSwitch outbound_mode when we issue the connect command to freeSwitch to retrieve
        ///     connected call data.
        /// </summary>
        private readonly bool _treatUnknownHeadersAsBody;

        /// <summary>
        ///     Decoded freeSwitch message
        /// </summary>
        private EventSocketMessage _currentMessage;

        public EventSocketMessageDecoder(int maxHeaderSize) : base(new State(true, false)) { _maxHeaderSize = maxHeaderSize; }

        public EventSocketMessageDecoder(int maxHeaderSize, bool treatUnknownHeadersAsBody) : base(new State(true, false)) {
            _maxHeaderSize = maxHeaderSize;
            _treatUnknownHeadersAsBody = treatUnknownHeadersAsBody;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output) {
            if (logger.IsDebugEnabled) logger.Debug("Decode(): State[{0}] ", State);

            if (State.ReadHeader) {
                if (_currentMessage == null) _currentMessage = new EventSocketMessage();

                //  read '\n' terminated lines until reach a single '\n'
                bool reachedDoubleLF = false;
                while (!reachedDoubleLF) {
                    // this will read or fail
                    string headerLine = ReadLineFeedOrFail(input, _maxHeaderSize);
                    if (logger.IsDebugEnabled) logger.Debug("read header line [{0}]", headerLine);
                    if (!string.IsNullOrEmpty(headerLine)) {
                        string[] headerParts = HeaderParser.SplitHeader(headerLine);
                        string part0 = headerParts[0];
                        if (string.IsNullOrEmpty(part0)) {
                            if (_treatUnknownHeadersAsBody) _currentMessage.BodyLines.Add(headerLine);
                            else throw new DecoderException("Unhandled ESL header[" + headerParts[0] + ']');
                        }
                        _currentMessage.Headers.Add(headerParts[0], headerParts[1]);
                    }
                    else reachedDoubleLF = true;
                    // do not read in this line again
                    Checkpoint();
                }
                // have read all headers - check for content-length
                if (_currentMessage.HasContentLength()) {
                    Checkpoint(new State(false, true));
                    if (logger.IsDebugEnabled) logger.Debug("have content-length, decoding body ..");
                }
                else {
                    // end of message
                    Checkpoint(new State(true, false));
                    // send message upstream
                    output.Add(_currentMessage);
                    _currentMessage = null;
                }
            }
            else if (State.ReadBody) {
                /*
                 *   read the content-length specified
                 */
                int contentLength = _currentMessage.ContentLength();
                IByteBuffer bodyBytes = input.ReadBytes(contentLength);
                if (logger.IsDebugEnabled) logger.Debug("read [{0}] body bytes", bodyBytes.WriterIndex);
                // most bodies are line based, so split on LF
                while (bodyBytes.IsReadable()) {
                    string bodyLine = ReadLine(bodyBytes, contentLength);
                    if (logger.IsDebugEnabled) logger.Debug("read body line [{0}]", bodyLine);
                    _currentMessage.BodyLines.Add(bodyLine);
                }

                // end of message
                Checkpoint(new State(true, false));
                // send message upstream
                output.Add(_currentMessage);
                _currentMessage = null;
            }
            else throw new DecoderException("Illegal state: [" + State + ']');
        }

        private static string ReadLineFeedOrFail(IByteBuffer buffer, int maxLineLength) {
            StringBuilder sb = new StringBuilder(64);
            while (true) {
                // this read might fail
                byte nextByte = buffer.ReadByte();
                if (nextByte == lineFeed) return sb.ToString();
                // Abort decoding if the decoded line is too large.
                if (sb.Length >= maxLineLength) throw new TooLongFrameException("ESL header line is longer than " + maxLineLength + " bytes.");
                sb.Append((char) nextByte);
            }
        }

        private static string ReadLine(IByteBuffer buffer, int maxLineLength) {
            StringBuilder sb = new StringBuilder(64);
            while (buffer.IsReadable()) {
                // this read should always succeed
                byte nextByte = buffer.ReadByte();
                if (nextByte == lineFeed) return sb.ToString();
                // Abort decoding if the decoded line is too large.
                if (sb.Length >= maxLineLength) throw new TooLongFrameException("ESL message line is longer than " + maxLineLength + " bytes.");
                sb.Append((char) nextByte);
            }
            return sb.ToString();
        }
    }

    /// <summary>
    ///     FreeSwitch message decoding stages. It will read first the header followed by the body.
    /// </summary>
    public struct State {
        public bool ReadHeader;
        public bool ReadBody;

        public State(bool readingHeader, bool readingBody) {
            ReadBody = readingBody;
            ReadHeader = readingHeader;
        }
    }
}