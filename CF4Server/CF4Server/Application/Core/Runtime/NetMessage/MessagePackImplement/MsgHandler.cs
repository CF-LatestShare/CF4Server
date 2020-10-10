using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosServer
{
    public class MsgHandler : RequestHandler
    {
        DataParameters param=new DataParameters();
        public override byte OpCode { get; protected set; } = 1;

        public override DataParameters Handle(DataParameters packet)
        {
            Utility.Debug.LogInfo(packet.Messages[0]);
            param.OperationCode = OpCode;
            param.Messages = new Dictionary<byte, object>()
            {
                {0,"服务器消息发送返回" }
            };
            return param;
        }
    }
}
