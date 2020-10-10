using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Protocol
{
    [MessagePackObject]
    public class OperationData:IDisposable
    {
        [Key(0)]
        public byte OperationCode { get; set; }
        [Key(1)]
        public object DataContract { get; set; }
        [Key(2)]
        public short ReturnCode { get; set; }
        public OperationData() { }
        public OperationData(byte operationCode)
        {
            OperationCode = operationCode;
        }
        public virtual OperationData DeepClone()
        {
            return new OperationData()
            {
                DataContract = this.DataContract,
                OperationCode = this.OperationCode,
                ReturnCode = this.ReturnCode
            };
        }
        public virtual void Dispose()
        {
            OperationCode = 0;
            DataContract = null;
            ReturnCode = 0;
        }
    }
}
