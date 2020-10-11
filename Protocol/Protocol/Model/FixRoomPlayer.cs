using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    [MessagePackObject]
    public class FixRoomPlayer: IDataContract
    {
        [Key(0)]
        public int RoomId { get; set; }
        [Key(1)]
        public FixPlayer Player { get; set; }
    }
}
