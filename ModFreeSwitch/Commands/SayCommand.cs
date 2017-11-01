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

using ModFreeSwitch.Common;
using System;

namespace ModFreeSwitch.Commands
{
    /// <summary>
    /// say. The say application will use the pre-recorded sound files to read or say various things
    /// like dates, times, digits, etc. The say application can read digits and numbers as well as
    /// dollar amounts, date/time values and IP addresses. It can also spell out alpha-numeric text,
    /// including punctuation marks
    /// </summary>
    public sealed class SayCommand : BaseCommand
    {
        public SayCommand(string text)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException(nameof(text));
            Text = text;
            SayType = SayTypes.MESSAGES;
            SayMethod = SayMethods.PRONOUNCED;
            Gender = SayGenders.FEMININE;
        }

        public override string Argument => Language + " " + SayType + " " + SayMethod.ToString().Replace("_",
                                                       "/") + " " + Gender + " " + Text;

        public override string Command => "say";

        /// <summary>
        /// Gender <see cref="SayGenders"/> 
        /// </summary>
        public SayGenders Gender { set; get; }

        /// <summary>
        /// Language or Module 
        /// </summary>
        public string Language { set; get; }

        /// <summary>
        /// Method <see cref="SayMethods"/> 
        /// </summary>
        public SayMethods SayMethod { set; get; }

        /// <summary>
        /// Type <see cref="SayTypes"/> 
        /// </summary>
        public SayTypes SayType { set; get; }

        /// <summary>
        /// The actual text to read. 
        /// </summary>
        public string Text { get; }
    }
}