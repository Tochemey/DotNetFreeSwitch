/*
    Copyright [2016] [Arsene Tochemey GANDOTE]

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using DotNetFreeSwitch.Common;

namespace DotNetFreeSwitch.Commands
{
    /// <summary>
    ///     Playback wrapper
    /// </summary>
    public sealed class PlaybackCommand : BaseCommand
    {
        public PlaybackCommand(string audioFile,
            IList<ChannelVariable> variables,
            long loop)
        {
            if (string.IsNullOrEmpty(audioFile)) throw new ArgumentNullException(nameof(audioFile));
            AudioFile = audioFile;
            Variables = variables;
            Loop = loop;
        }

        public PlaybackCommand(string audioFile,
            IList<ChannelVariable> variables)
        {
            if (string.IsNullOrEmpty(audioFile)) throw new ArgumentNullException(nameof(audioFile));
            AudioFile = audioFile;
            Variables = variables;
            Loop = 1;
        }

        public PlaybackCommand(string audioFile)
        {
            if (string.IsNullOrEmpty(audioFile)) throw new ArgumentNullException(nameof(audioFile));
            AudioFile = audioFile;
            Variables = new List<ChannelVariable>();
            Loop = 1;
        }

        protected override string Argument => ToString();

        /// <summary>
        ///     Audio file to play
        /// </summary>
        public string AudioFile { get; }

        public override string Command => "playback";

        /// <summary>
        ///     The number of time to play the audio file. Please bear in mind that we will be using
        ///     sendmsg to play audio file. This one will be very helpful.
        /// </summary>
        public long Loop { get; }

        /// <summary>
        ///     Playback additional variables to add to the channel while playing the audio file
        /// </summary>
        public IList<ChannelVariable> Variables { get; }

        public override string ToString()
        {
            var variables = Variables != null && Variables.Count > 0
                ? Variables.Aggregate(string.Empty,
                    (current,
                        variable) => current + (variable + ","))
                : string.Empty;
            if (variables.Length > 0)
                variables = "{" + variables.Remove(variables.Length - 1,
                    1) + "}";
            return $"{variables}{AudioFile}";
        }
    }
}