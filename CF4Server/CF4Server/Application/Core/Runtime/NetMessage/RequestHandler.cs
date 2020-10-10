using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
using Protocol;

namespace CosmosServer
{
    public abstract class RequestHandler:IBehaviour
    {
        public abstract byte OpCode { get; protected set; }
        /// <summary>
        /// 处理消息；
        /// 此方法在外部执行时为异步；
        /// </summary>
        /// <param name="packet">消息体</param>
        /// <returns></returns>
        public abstract DataParameters Handle(DataParameters packet);
        /// <summary>
        /// 空虚函数；
        /// </summary>
        public virtual void OnInitialization() { }
        /// <summary>
        /// 空虚函数；
        /// </summary>
        public virtual void OnTermination(){}
    }
}
