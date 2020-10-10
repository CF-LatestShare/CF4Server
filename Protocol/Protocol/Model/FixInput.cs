using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Protocol
{
    /// <summary>
    /// 测试用输入协议；
    /// </summary>
    [MessagePackObject]
    public class FixInput
    {
        [Key(0)]
        public FixPlayer  Player{ get; set; }
        [Key(1)]
        public int RoomId { get; set; }
        [Key(2)]
        public int Tick { get; set; }
        [Key(3)]
        public FixVector3 Position { get; set; }
        [Key(4)]
        public  FixVector3 Rotation { get; set; }
        [Key(5)]
        public bool ShiftDown { get; set; }
    }
}
