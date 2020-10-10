using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Protocol
{
    [MessagePackObject]
    public class FixRoom
    {
        [Key(0)]
        public int RoomId { get; set; }
    }
}
