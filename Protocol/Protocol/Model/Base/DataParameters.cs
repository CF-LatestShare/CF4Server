using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    /// <summary>
    /// ProtocolDefine 4319 请求通道的数据类型；
    /// </summary>
    [MessagePackObject]
    public sealed class DataParameters: IDataContract
    {
        public object this[byte messageKey]
        {
            get
            {
                object varlue;
                Messages.TryGetValue(messageKey, out varlue);
                return varlue;
            }
            set
            {
                Messages.TryAdd(messageKey, value);
            }
        }
        [Key(0)]
        public byte OperationCode { get; set; }
        [Key(1)]
        public Dictionary<byte, object> Messages { get; set; }
        [Key(2)]
        public short ReturnCode { get; set; }
        public DataParameters() { }
        public DataParameters(byte operationCode)
        {
            this.OperationCode = operationCode;
        }
        public DataParameters(byte operationCode, Dictionary<byte, object> messages) : this(operationCode)
        {
            this.Messages = messages;
        }
        public void SetMessages(Dictionary<byte, object> messages)
        {
            this.Messages = messages;
        }
    }
}
