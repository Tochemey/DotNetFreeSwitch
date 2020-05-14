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

namespace Core.Commands
{
    /// <summary>
    /// Record command. Record is used for recording messages, like in a voicemail system 
    /// </summary>
    public sealed class RecordCommand : BaseCommand
    {
        public RecordCommand()
        {
            SilenceHit = 3;
        }

        public override string Argument => $"{RecordFile} {TimeLimit} {SilenceTreshold} {SilenceHit}";

        public override string Command => "record";

        /// <summary>
        /// File to record 
        /// </summary>
        public string RecordFile { set; get; }

        /// <summary>
        /// how many seconds of audio below silence_thresh will be tolerated before the recording
        /// stops. When omitted, the default value is 3.
        /// </summary>
        public long SilenceHit { set; get; }

        /// <summary>
        /// the energy level below which is considered silence. 
        /// </summary>
        public long SilenceTreshold { set; get; }

        /// <summary>
        /// the maximum duration of the recording in seconds. 
        /// </summary>
        public long TimeLimit { set; get; }
    }
}