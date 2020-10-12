using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosServer
{
    /// <summary>
    /// 战斗模块；
    /// 仅用于消息转发；
    /// </summary>
    [CustomeModule]
    public class BattleManager : Module<BattleManager>
    {

#if SERVER
        int updateInterval = ApplicationBuilder._MSPerTick;
        long latestTime;
        int roomIndex;
#else
        RoomEntity roomEntity = new RoomEntity();
#endif
        public override void OnPreparatory()
        {
            //CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_PLYAERINPUT, PlayerInputCommand);
#if SERVER
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_TESTCAHNNEL, TestChannelHandler);
            latestTime = Utility.Time.MillisecondTimeStamp() + updateInterval;
#endif
        }
        public override void OnRefresh()
        {
#if SERVER
            var now = Utility.Time.MillisecondTimeStamp();
            if (now >= latestTime)
            {
                //广播当前帧，并进入下一帧；
                latestTime = now + updateInterval;
                //roomRefreshHandler?.Invoke();
            }
#else

#endif
        }
        void PlayerInputCommand(OperationData opData)
        {
#if SERVER

#else

            try
            {
                var cmds = opData.DataContract as FixInputSet;
                roomEntity.OnPlayersInput(cmds);
                roomEntity.OnRefresh();
            }
            catch (Exception e){Utility.Debug.LogError(e); }
#endif
        }
#if SERVER
        void TestChannelHandler(OperationData opData)
        {
            try
            {
                Utility.Debug.LogWarning(opData.DataContract);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);

            }
        }
#endif
    }
}
