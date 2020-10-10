using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Protocol
{
    [MessagePackObject]
    public class FixInputSet
    {
        [Key(2)]
        public int SessionId { get; set; }
        [Key(2)]
        public FixPlayerSet Players{ get; set; }
        [Key(3)]
        public int Tick { get; set; }
        [Key(4)]
        public int RoomId { get; set; }
        [Key(5)]
        public List<FixInput> InputSet { get; set; }
        public void Clear()
        {
            Tick = 0;
            RoomId = 0;
            InputSet = null;
            SessionId = 0;
        }
    }
}
