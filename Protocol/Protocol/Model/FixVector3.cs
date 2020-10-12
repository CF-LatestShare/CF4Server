using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public struct FixVector3 : IDataContract
    {
        [Key(0)]
        public int X { get; set; }
        [Key(1)]
        public int Y { get; set; }
        [Key(2)]
        public int Z { get; set; }
        public FixVector3 SetVector(Vector3 vector)
        {
            X = Mathf.FloorToInt(vector.x * 1000);
            Y = Mathf.FloorToInt(vector.y * 1000);
            Z = Mathf.FloorToInt(vector.z * 1000);
            return this;
        }
        public Vector3 GetVector()
        {
            return new Vector3(X / 1000, Y / 1000, Z / 1000);
        }
    }
}
