using System;
using System.Collections.Generic;
using System.Linq;
using ModFreeSwitch.Common;

namespace ModFreeSwitch.Commands {
    /// <summary>
    ///     Playback wrapper
    /// </summary>
    public sealed class PlaybackCommand : BaseCommand {
        public PlaybackCommand(string audioFile,
            IList<EslChannelVariable> variables,
            long loop) {
            if (string.IsNullOrEmpty(audioFile))
                throw new ArgumentNullException("audioFile");
            AudioFile = audioFile;
            Variables = variables;
            Loop = loop;
        }

        public PlaybackCommand(string audioFile,
            IList<EslChannelVariable> variables) {
            if (string.IsNullOrEmpty(audioFile))
                throw new ArgumentNullException("audioFile");
            AudioFile = audioFile;
            Variables = variables;
            Loop = 1;
        }

        public PlaybackCommand(string audioFile) {
            if (string.IsNullOrEmpty(audioFile))
                throw new ArgumentNullException("audioFile");
            AudioFile = audioFile;
            Variables = new List<EslChannelVariable>();
            Loop = 1;
        }

        /// <summary>
        ///     Audio file to play
        /// </summary>
        public string AudioFile { get; }

        /// <summary>
        ///     The number of time to play the audio file. Please bear in mind that we will be using sendmsg to play audio file.
        ///     This one will be very helpful.
        /// </summary>
        public long Loop { get; }

        /// <summary>
        ///     Playback additional variables to add to the channel while playing the audio file
        /// </summary>
        public IList<EslChannelVariable> Variables { get; }

        public override string Command {
            get { return "playback"; }
        }

        public override string Argument {
            get { return ToString(); }
        }

        public override string ToString() {
            var variables = Variables != null && Variables.Count > 0
                ? Variables.Aggregate(string.Empty,
                    (current,
                        variable) => current + (variable + ","))
                : string.Empty;
            if (variables.Length > 0)
                variables = "{" + variables.Remove(variables.Length - 1, 1) + "}";
            return string.Format("{0}{1}", variables, AudioFile);
        }
    }
}