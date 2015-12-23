using System;
using System.Collections.Generic;
using System.Linq;
using ModFreeSwitch.Common;

namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     Playback wrapper
    /// </summary>
    public sealed class PlaybackCommand : BaseCommand {
        private readonly string _audioFile;
        private readonly long _loop;
        private readonly IList<EventSocketChannelVariable> _variables;

        public PlaybackCommand(string audioFile, IList<EventSocketChannelVariable> variables, long loop) {
            if (string.IsNullOrEmpty(audioFile)) throw new ArgumentNullException("audioFile");
            _audioFile = audioFile;
            _variables = variables;
            _loop = loop;
        }

        public PlaybackCommand(string audioFile, IList<EventSocketChannelVariable> variables) {
            if (string.IsNullOrEmpty(audioFile)) throw new ArgumentNullException("audioFile");
            _audioFile = audioFile;
            _variables = variables;
            _loop = 1;
        }

        public PlaybackCommand(string audioFile) {
            if (string.IsNullOrEmpty(audioFile)) throw new ArgumentNullException("audioFile");
            _audioFile = audioFile;
            _variables = new List<EventSocketChannelVariable>();
            _loop = 1;
        }

        /// <summary>
        ///     Audio file to play
        /// </summary>
        public string AudioFile { get { return _audioFile; } }

        /// <summary>
        ///     The number of time to play the audio file. Please bear in mind that we will be using sendmsg to play audio file.
        ///     This one will be very helpful.
        /// </summary>
        public long Loop { get { return _loop; } }

        /// <summary>
        ///     Playback additional variables to add to the channel while playing the audio file
        /// </summary>
        public IList<EventSocketChannelVariable> Variables { get { return _variables; } }

        public override string Command { get { return "playback"; } }

        public override string Argument { get { return ToString(); } }

        public override string ToString() {
            string variables = (_variables != null && _variables.Count > 0) ? _variables.Aggregate(string.Empty, (current, variable) => current + (variable + ",")) : string.Empty;
            if (variables.Length > 0) variables = "{" + variables.Remove(variables.Length - 1, 1) + "}";
            return string.Format("{0}{1}", variables, _audioFile);
        }
    }
}