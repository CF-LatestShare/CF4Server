using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
namespace CosmosServer
{
    //==========================================//
    //消息处理流程：
    //1、消息从4319口进入此管理类，由MessageHandler进行解码；
    //2、消息解码为MessagePack对象后，根据opCode派发到具体
    //3、消息处理者完成消息处理后，则返回处理完成的消息；
    //4、发送处理好的消息；
    //==========================================
#if SERVER
    [CustomeModule]
    public class MessageManager : Module<MessageManager>
    {
        Dictionary<int, RequestHandler> handlerDict = new Dictionary<int, RequestHandler>();
        Queue<OperationData> opDataQueue = new Queue<OperationData>();
        public override void OnInitialization()
        {
            NetworkMsgEventCore.Instance.AddEventListener(ProtocolDefine.PORT_REQUEST, RequestHandler);
            NetworkMsgEventCore.Instance.AddEventListener(ProtocolDefine.PORT_INPUT, CommandHandler);
            InitHandler();
        }
        public void SendCommandMessage<T>(long sessionId, T packet)
            where T : OperationData
        {
            var buffer = Utility.MessagePack.ToByteArray(packet);
            if (buffer != null)
            {
                UdpNetMessage msg = UdpNetMessage.EncodeMessageAsync(sessionId, ProtocolDefine.PORT_INPUT, buffer).Result;
                GameManager.NetworkManager.SendNetworkMessage(msg);
            }
        }
        public void SendCommandMessage<T>(long sessionId, byte opCode, T data, short returnCode = 0)
        {
            var opData = SpawnOpData(opCode, data, returnCode);
            var buffer = Utility.MessagePack.ToByteArray(opData);
            if (buffer != null)
            {
                UdpNetMessage msg = UdpNetMessage.EncodeMessageAsync(sessionId, ProtocolDefine.PORT_INPUT, buffer).Result;
                GameManager.NetworkManager.SendNetworkMessage(msg);
            }
            DespawnOpData(opData);
        }
        /// <summary>
        /// 初始化消息处理者；
        /// </summary>
        void InitHandler()
        {
            var handlerType = typeof(RequestHandler);
            Type[] types = Assembly.GetAssembly(handlerType).GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (handlerType.IsAssignableFrom(types[i]))
                {
                    if (types[i].IsClass && !types[i].IsAbstract)
                    {
                        var handler = Utility.Assembly.GetTypeInstance(types[i]) as RequestHandler;
                        handler.OnInitialization();
                        handlerDict.Add(handler.OpCode, handler);
                    }
                }
            }
        }
        /// <summary>
        ///  处理从系统通讯通道 （MSG_PORT）接收到的消息，并解包成MessagePack对象；
        ///  解包完成后，派发消息到具体的消息处理者；
        ///  处理者完成处理后，对消息进行发送；
        /// </summary>
        /// <param name="netMsg">数据</param>
        void RequestHandler(INetworkMessage netMsg)
        {
            try
            {
                //这里解码成明文后进行反序列化得到packet数据；
                var msgPack = Utility.MessagePack.ToObject<DataParameters>(netMsg.ServiceMsg);
                if (msgPack == null)
                    return;
                RequestHandler handler;
                var exist = handlerDict.TryGetValue(msgPack.OperationCode, out handler);
                if (exist)
                {
                    var mp = handler.Handle(msgPack);
                    if (mp != null)
                    {
                        SendResponseMessage(netMsg, mp);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
        void CommandHandler(INetworkMessage netMsg)
        {
            try
            {
                var opData = Utility.MessagePack.ToObject<OperationData>(netMsg.ServiceMsg);
                if (opData == null)
                    return;
                CommandEventCore.Instance.Dispatch(opData.OperationCode, opData);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
        async Task HandleMessageAsync(INetworkMessage netMsg)
        {
            await Task.Run(() => { RequestHandler(netMsg); });
        }
        void SendResponseMessage(INetworkMessage netMsg, DataParameters packet)
        {
            var buffer = Utility.MessagePack.ToByteArray(packet);
            if (buffer != null)
            {
                UdpNetMessage msg = UdpNetMessage.EncodeMessageAsync(netMsg.Conv, ProtocolDefine.PORT_REQUEST, buffer).Result;
                GameManager.NetworkManager.SendNetworkMessage(msg);
            }
        }
        OperationData SpawnOpData<T>(byte opCode, T packet, short returnCode = 0)
        {
            var result = opDataQueue.TryDequeue(out var opData);
            if (!result)
                opData = new OperationData();
            opData.DataContract = packet;
            opData.OperationCode = opCode;
            opData.ReturnCode = returnCode;
            return opData;
        }
        void DespawnOpData(OperationData opData)
        {
            opData.Dispose();
            opDataQueue.Enqueue(opData);
        }
    }
#endif
}
