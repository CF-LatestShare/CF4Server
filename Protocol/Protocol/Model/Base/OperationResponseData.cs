using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    public class OperationResponseData : OperationData
    {
        public OperationResponseData() { }
        public OperationResponseData(byte opCode):base(opCode){}
        [Key(3)]
        public byte LatestCmd { get; set; }
        public override OperationData DeepClone()
        {
            return new OperationResponseData()
            {
                DataContract = this.DataContract,
                OperationCode = this.OperationCode,
                Cmd = this.Cmd,
                LatestCmd = this.LatestCmd
            };
        }
        public override void Dispose()
        {
            base.Dispose();
            LatestCmd = 0;
        }
    }
}
