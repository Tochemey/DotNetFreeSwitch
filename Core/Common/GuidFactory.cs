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

namespace Core.Common
{
    public class GuidFactory
    {
        private static GuidFactory _instance = new GuidFactory();

        /// <summary>
        ///     Assign a new factory
        /// </summary>
        /// <param name="factory">Factory to use</param>
        public static void Assign(GuidFactory factory)
        {
            _instance = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>
        ///     Generate a new GUID
        /// </summary>
        /// <returns>Guid of some sort.</returns>
        public static Guid Create()
        {
            return _instance.CreateInternal();
        }

        /// <summary>
        ///     Generate a new GUID using the assign implementation
        /// </summary>
        /// <returns>Guid of some sort</returns>
        protected virtual Guid CreateInternal()
        {
            return Guid.NewGuid();
        }
    }
}