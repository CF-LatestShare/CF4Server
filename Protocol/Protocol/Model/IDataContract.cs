using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    [Union(0, typeof(FixInput))]
    [Union(1, typeof(FixPlayer))]
    [Union(2, typeof(FixVector3))]
    [Union(3, typeof(FixRoom))]
    [Union(4, typeof(FixRoomPlayer))]
    [Union(5, typeof(FixInputSet))]
    public interface IDataContract
    {
    }
}
