using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ModFreeSwitch.Messages;
using NLog;

namespace ModFreeSwitch.Codecs {
    /// <summary>
    ///     EslFrameDecoder.
    ///     Look for \n\n in your receive buffer
    ///     Examine data for existence of Content-Length
    ///     If NOT present, process event and remove from receive buffer
    ///     IF present, Shift buffer to remove 'header'
    ///     Evaluate content-length value
    ///     Loop until receive buffer size is >= Content-length
    ///     Extract content-length bytes from buffer and process
    /// </summary>
    public sealed class EslFrameDecoder : ReplayingDecoder<EslFrameDecoder.DecoderState> {
        /// <summary>
        ///     EslFrameDecoder State. This help us to decode the FreeSwitch message based upon the different parts.
        ///     We all know that according to their documentation every freeSwitch has two parts.
        ///     <see cref="http://wiki.freeswitch.org/wiki/Event_List#Minimum_event_information" />
        /// </summary>
        public enum DecoderState {
            ReadHeader,
            ReadBody
        }

        private const char LINE_FEED_CHAR = '\n';

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     This is very useful during freeSwitch outbound_mode when we issue the connect command to freeSwitch to retrieve
        ///     connected call data.
        /// </summary>
        private readonly bool _treatUnknownHeadersAsBody;

        private EslMessage _actualMessage;

        public EslFrameDecoder() : this(false) {}

        public EslFrameDecoder(bool treatUnknownHeadersAsBody)
            : this(DecoderState.ReadHeader, treatUnknownHeadersAsBody) {}

        public EslFrameDecoder(DecoderState initialState,
            bool treatUnknownHeadersAsBody) : base(initialState) {
            _treatUnknownHeadersAsBody = treatUnknownHeadersAsBody;
        }

        protected override void Decode(IChannelHandlerContext context,
            IByteBuffer input,
            List<object> output) {
            // first let us display the state in debug mode
            if (_logger.IsDebugEnabled) _logger.Debug("Decode(): State[{0}] ", State);

            switch (State) {
                /**
                 * Let us read the message head.
                 */
                case DecoderState.ReadHeader:
                    // Let us initialize the actual message to decode
                    SetActualMessage();

                    // Read the headers
                    ReadHeader(input);

                    // have read all headers - check whether we can read the message body or not
                    if (!CanReadBody()) { CompleteDecoding(output); }
                    break;
                case DecoderState.ReadBody:
                    /**
                     * At this stage we are reading the message body based upon the content length in the header.
                     */
                    ReadBody(input, output);

                    break;
                default:
                    throw new DecoderException("Illegal state: [" + State + ']');
            }
        }

        private void SetActualMessage() {
            if (_actualMessage == null) _actualMessage = new EslMessage();
        }

        private bool CanReadBody() {
            if (!_actualMessage.HasContentLength()) return false;
            Checkpoint(DecoderState.ReadBody);
            if (_logger.IsDebugEnabled)
                _logger.Debug("have content-length, decoding body ..");
            return true;
        }

        private void CompleteDecoding(List<object> output) {
            // complete the decoding. Get ready for the next message
            Checkpoint(DecoderState.ReadHeader);
            // send message upstream
            output.Add(_actualMessage);
            _actualMessage = null;
        }

        private void ReadBody(IByteBuffer buffer,
            List<object> output) {
            var maxlength = _actualMessage.ContentLength();
            // Let us check whether we do not have another message on he line
            var currentReaderIndex = buffer.ReaderIndex;
            var writerIndex = buffer.WriterIndex;
            var len = maxlength + currentReaderIndex;

            if (len <= writerIndex) {
                // read the body bytes
                var bodyBytes = buffer.ReadBytes(maxlength);
                if (_logger.IsDebugEnabled)
                    _logger.Debug("read [{0}] body bytes", bodyBytes.WriterIndex);
                // most bodies are line based, so split on LF
                while (bodyBytes.IsReadable()) {
                    var bodyLine = ReadLine(bodyBytes, maxlength);
                    if (_logger.IsDebugEnabled)
                        _logger.Debug("read body line [{0}]", bodyLine);
                    _actualMessage.BodyLines.Add(bodyLine);
                }

                CompleteDecoding(output);
            }
            else {
                _logger.Debug("reading more bytes...");
                RequestReplay();
            }
        }

        private void ReadHeader(IByteBuffer buffer) {
            var reachedDoubleLf = false;
            while (!reachedDoubleLf) {
                var headerLine = ReadLine(buffer);
                if (_logger.IsDebugEnabled)
                    _logger.Debug("read header line [{0}]", headerLine);
                if (!string.IsNullOrEmpty(headerLine)) {
                    var headerParts = EslHeaderParser.SplitHeader(headerLine);
                    if (headerParts == null || headerParts.Length == 0) continue;
                    var headerName = headerParts[0];
                    if (string.IsNullOrEmpty(headerName)) {
                        if (_treatUnknownHeadersAsBody)
                            _actualMessage.BodyLines.Add(headerLine);
                        else
                            throw new DecoderException(
                                "Unhandled FreeSwitch message header[" + headerParts[0] +
                                ']');
                    }

                    _actualMessage.Headers.Add(headerName.Trim(LINE_FEED_CHAR),
                        Uri.UnescapeDataString(headerParts[1])
                            .Trim(LINE_FEED_CHAR));
                }
                else {
                    reachedDoubleLf = true;
                }
                Checkpoint();
            }
        }

        private static string ReadLine(IByteBuffer buffer) {
            var sb = new StringBuilder();
            while (buffer.IsReadable()) {
                var nextByte = buffer.ReadByte();
                if ((char) nextByte == LINE_FEED_CHAR) return sb.ToString();
                sb.Append((char) nextByte);
            }
            return sb.ToString();
        }

        private static string ReadLine(IByteBuffer buffer,
            int maxLineLength) {
            var sb = new StringBuilder();
            while (buffer.IsReadable()) {
                // this read should always succeed
                var nextByte = buffer.ReadByte();
                if ((char) nextByte == LINE_FEED_CHAR) return sb.ToString();
                // Abort decoding if the decoded line is too large.
                if (sb.Length >= maxLineLength)
                    throw new TooLongFrameException(
                        "FreeSwitch message line is longer than " + maxLineLength +
                        " bytes.");
                sb.Append((char) nextByte);
            }
            return sb.ToString();
        }
    }
}