using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Protocol
{
    [MessagePackObject]
    public class FixInputSet
    {
        [Key(0)]
        public int Tick { get; set; }
        [Key(1)]
        public int RoomId { get; set; }
        [Key(2)]
        public List<FixInput> InputSet { get; set; }
        public void Clear()
        {
            Tick = 0;
            RoomId = 0;
            InputSet = null;
        }
    }
}
