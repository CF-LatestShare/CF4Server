using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Protocol
{
    [MessagePackObject]
    public class FixRoom: IDataContract
    {
        [Key(0)]
        public int RoomId { get; set; }
    }
}
