using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Protocol
{
    [MessagePackObject]
    public class FixPlayerSet
    {
        [Key(0)]
        public List<FixPlayer> PlayerList { get; set; }
    }
}
