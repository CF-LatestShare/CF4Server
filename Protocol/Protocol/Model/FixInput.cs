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
    public class FixInput : IDataContract
    {
        [Key(0)]
        public int SessionId { get; set; }
        [Key(1)]
        public int PlayerId { get; set; }
        [Key(2)]
        public int RoomId { get; set; }
        [Key(3)]
        public int Tick { get; set; }
        [Key(4)]
        public FixVector3 Position { get; set; }
        [Key(5)]
        public FixVector3 Rotation { get; set; }
        [Key(6)]
        public bool ShiftDown { get; set; }
        [Key(7)]
        public long TS { get; set; }
        public FixInput DeepClone()
        {
            return new FixInput()
            {
                SessionId = this.SessionId,
                PlayerId = this.PlayerId,
                RoomId = this.RoomId,
                Tick = this.Tick,
                Position = this.Position,
                Rotation = this.Rotation,
                TS = this.TS
            };
        }
    }
}
