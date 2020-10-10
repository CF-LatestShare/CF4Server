using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    [MessagePackObject]
    public class FixRoomPlayer
    {
        [Key(0)]
        public FixRoom Room { get; set; }
        [Key(1)]
        public FixPlayer Player { get; set; }
    }
}
