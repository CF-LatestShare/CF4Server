using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Protocol
{
    [MessagePackObject]
    public class FixInputSet : IDataContract
    {
        [Key(0)]
        public int Tick { get; set; }
        [Key(1)]
        public int RoomId { get; set; }
        [Key(2)]
        public Dictionary<int, FixInput> InputDict { get; set; }
        [Key(3)]
        public long TS { get; set; }
        public void Clear()
        {
            Tick = 0;
            RoomId = 0;
            InputDict = null;
            TS = 0;
        }
    }
}
