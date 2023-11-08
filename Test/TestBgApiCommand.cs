using System;
using DotNetFreeSwitch.Commands;
using Xunit;

namespace Test
{
    public class TestBgApiCommand
    {
        [Fact]
        public void ParseCommand()
        {

            // create the command
            var playback = "user/1001 playback:/usr/local/freeswitch/sounds/broadcast/no_prisoner_info.wav,hangup inline";
            var bgCommand = new BgApiCommand("originate", playback);

            var expected = "bgapi originate user/1001 playback:/usr/local/freeswitch/sounds/broadcast/no_prisoner_info.wav,hangup inline";
            Assert.Equal(expected, bgCommand.ToString());

        }
    }
}

