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
        public byte Cmd { get; set; }
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
                Cmd = this.Cmd
            };
        }
        public virtual void Dispose()
        {
            OperationCode = 0;
            DataContract = null;
            Cmd = 0;
        }
    }
}
