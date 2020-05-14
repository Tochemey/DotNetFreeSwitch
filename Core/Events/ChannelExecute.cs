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

using Core.Messages;

namespace Core.Events
{
    /// <summary>
    ///     Function have been executed on channel
    /// </summary>
    public class ChannelExecute : FsEvent
    {
        public ChannelExecute(FsMessage message) : base(message) { }

        /// <summary>
        ///     Gets application to execute.
        /// </summary>
        public string Application => this["Application"];

        /// <summary>
        ///     Gets arguments for the application
        /// </summary>
        public string ApplicationData => this["Application-Data"];

        /// <summary>
        ///     Gets response from the application
        /// </summary>
        protected string ApplicationResponse => this["Application-Response"];

        public override string ToString() { return "ChannelExecute(" + Application + ", '" + ApplicationData + "')"; }
    }
}