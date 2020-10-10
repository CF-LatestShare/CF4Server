using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;

namespace CosmosServer
{
    [CustomeModule]
    public class LockSetpManager : Module<LockSetpManager>
    {
        readonly int updateInterval = ApplicationBuilder._MSPerTick;
        long latestUpdateTime;
        public override void OnInitialization()
        {
            //NetworkMsgEventCore.Instance.AddEventListener(ProtocolDefine.PORT_INPUT, HandleMessage);
        }
        public override void OnRefresh()
        {
            var now = Utility.Time.MillisecondTimeStamp();
            if (now < latestUpdateTime)
                return;
            latestUpdateTime = now + updateInterval;
            PollingEventCore.Instance.Dispatch(PollingDefine.POLLING_LOGIC);
        }
        //void HandleMessage(INetworkMessage netMsg)
        //{
        //    IMessagePacket packet = NetMessageSerializer.Deserialize(netMsg.ServiceMsg);
        //}
        //void SendMessage(INetworkMessage netMsg, IMessagePacket packet)
        //{
        //    var packetBuffer = NetMessageSerializer.Serialize(packet);
        //    UdpNetMessage msg = UdpNetMessage.EncodeMessageAsync(netMsg.Conv, netMsg.OperationCode, packetBuffer).Result;
        //    GameManager.NetworkManager.SendNetworkMessage(msg);
        //}
    }
}
