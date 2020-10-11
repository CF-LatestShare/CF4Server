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
        public void SetVector(Vector3 vector)
        {
            X = Mathf.FloorToInt(vector.x);
            Y = Mathf.FloorToInt(vector.y);
            Z = Mathf.FloorToInt(vector.z);
        }
        public Vector3 GetVector()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
