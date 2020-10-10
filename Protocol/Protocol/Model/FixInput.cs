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
        [Key(2)]
        public int SessionId { get; set; }
        [Key(3)]
        public int PlayerId { get; set; }
        [Key(4)]
        /// <summary>
        /// CMD指令；
        /// 全部指令为：SYN，FIN，MSG，ACK
        /// </summary>
        public byte Cmd { get; set; }
        [Key(5)]
        public int RoomId { get; set; }
        [Key(6)]
        public int Tick { get; set; }
        [Key(7)]
        public FixVector3 Position { get; set; }
        [Key(8)]
        public  FixVector3 Rotation { get; set; }
        [Key(9)]
        public bool ShiftDown { get; set; }
    }
}
