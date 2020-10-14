using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    [MessagePackObject]
    public class FixRoomEntity:IDataContract
    {
        [Key(0)]
        public int RoomId { get; set; }
        [Key(1)]
        public List<FixPlayer> Players { get; set; }
        [Key(2)]
        public int MSPerTick { get; set; }
    }
}
