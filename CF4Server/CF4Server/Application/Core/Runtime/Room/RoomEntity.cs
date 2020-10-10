using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos;
using Cosmos.Network;
using Protocol;
namespace CosmosServer
{
    public class RoomEntity : IRefreshable, IReference
    {
        public int RoomId { get; private set; }
        /// <summary>
        /// 当前房间是否可用；
        /// </summary>
        public bool IsAlive { get; private set; }
        /// <summary>
        /// 当前房间内玩家人数；
        /// </summary>
        public int PlayerCount { get { return playerDict.Count; } }
        public bool IsFull { get { return PlayerCount >= _MaxPlayer; } }
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 当前房间帧指令数据字典；
        /// </summary>
#if SERVER
        Dictionary<int, List<FixInput>> tickCmdDict = new Dictionary<int, List<FixInput>>();
#else
        Dictionary<int, InputSetProtocol> tickCmdDict = new Dictionary<int, InputSetProtocol>();
#endif
        FixInputSet inputSetProtocol = new FixInputSet();
        /// <summary>
        /// 玩家字典；
        /// 玩家ID->玩家；
        /// </summary>
        Dictionary<int, PlayerEntity> playerDict = new Dictionary<int, PlayerEntity>();
        /// <summary>
        /// 广播数据委托；
        /// </summary>
        Action<OperationData> broadcastCmdHandler;
        event Action<OperationData> BroadcastCmdHandler
        {
            add { broadcastCmdHandler += value; }
            remove
            {
                try { broadcastCmdHandler -= value; }
                catch (Exception e) { Utility.Debug.LogError(e); }
            }
        }
        OperationData cmdOpData = new OperationData();
        readonly int _MaxPlayer = 6;
        /// <summary>
        /// 游戏逻辑帧;
        /// </summary>
        int tick = 0;
        MessageManager msgMgrInstance;
        PlayerManager playerMgrInstance;
        FixRoom roomProtol = new FixRoom();
        public RoomEntity()
        {
            msgMgrInstance = GameManager.CustomeModule<MessageManager>();
            playerMgrInstance = GameManager.CustomeModule<PlayerManager>();
        }
        public void Oninit(int roomId)
        {
            this.RoomId = roomId;
            inputSetProtocol.RoomId = roomId;
            IsAlive = true;
            roomProtol.RoomId = roomId;
        }
#if !SERVER
        public void CacheInputCmds(InputSetProtocol inputs)
        {
            //玩家收到当前服务器帧数据；
            if (inputs.Tick > tick)
                inputs.Tick = tick;
            var result = tickCmdDict.TryGetValue(inputs.Tick, out var set);
            if (!result)
                tickCmdDict.Add(inputs.Tick, inputs);
        }
#endif
#if SERVER
        /// <summary>
        ///缓存输入指令； 
        /// </summary>
        public void CacheInputCmd(FixInput input)
        {
            //若玩家延迟，发送的帧落后当前帧数，则将帧转换为当前帧；
            if (input.Tick < tick)
                input.Tick = tick;
            var result = tickCmdDict.TryGetValue(input.Tick, out var set);
            if (!result)
                tickCmdDict.Add(input.Tick, new List<FixInput>() { input });
            set.Add(input);
        }
#endif
        public void Enter(PlayerEntity playerEntity )
        {
            var result = playerDict.TryAdd(playerEntity.PlayerId, playerEntity);
            if (result)
            {
                BroadcastCmdHandler += playerEntity.SendCommadMessage;
                msgMgrInstance.SendCommandMessage
                    (playerEntity.SessionId, ProtocolDefine.OPERATION_ROOM, 
                    roomProtol,ProtocolDefine.CMD_ACK) ;
            }
        }
        public void Exit(PlayerEntity playerEntity)
        {
            var result = playerDict.Remove(playerEntity.PlayerId);
            if (result)
            {
                BroadcastCmdHandler -= playerEntity.SendCommadMessage;
                playerEntity.SendCommadMessage
                    (ProtocolDefine.OPERATION_ROOM, roomProtol, ProtocolDefine.CMD_ACK);
            }
        }
        public void RunGame()
        {
            IsRunning = true;
        }
            /// <summary>
            ///玩家通过指令加入； 
            /// </summary>
            public void Join(FixInput cmd)
        {
            playerMgrInstance.TryAddPlayer(cmd.SessionId,out var pe);
            pe.SessionId = cmd.SessionId;
            if (playerDict.ContainsKey(pe.PlayerId))
                return;
            //1、发送ACK
            cmd.Cmd = ProtocolDefine.CMD_ACK;
            msgMgrInstance.SendCommandMessage(cmd.SessionId,ProtocolDefine.OPERATION_ROOM, cmd);
            BroadcastCmdHandler += pe.SendCommadMessage;
            //2、广播房间内所有玩家的信息；
            foreach (var p in playerDict.Values)
            {
                p.SendCommadMessage(ProtocolDefine.OPERATION_ROOM,playerDict.Values.ToList<PlayerEntity>());
            }
            playerDict.Add(pe.PlayerId, pe);
            Utility.Debug.LogWarning($"发送加入房间ACK ,SessionId{cmd.SessionId}");
        }
        /// <summary>
        ///玩家离开； 
        /// </summary>
        public void Leave(FixInput cmd )
        {
            if (!playerDict.ContainsKey(cmd.PlayerId))
                return;
            var pe = playerDict[cmd.PlayerId];
            //1、发送ACK
            cmd.Cmd = ProtocolDefine.CMD_ACK;
           msgMgrInstance.SendCommandMessage(cmd.SessionId,ProtocolDefine.OPERATION_PLYAERINPUT,cmd);
            BroadcastCmdHandler -= pe.SendCommadMessage;
            playerDict.Remove(cmd.PlayerId);
            //2、广播房间内离开玩家的信息；
            foreach (var p in playerDict.Values)
            {
                p.SendCommadMessage(ProtocolDefine.OPERATION_PLYAERINPUT,pe);
            }
            pe.Dispose();
        }
        public void OnRefresh()
        {
            if (!IsRunning)
                return;
            //广播当前帧，并进入下一帧；
            var result = tickCmdDict.TryGetValue(tick, out var cmds);
            if (result)
            {
#if SERVER
                inputSetProtocol.Tick = tick;
                inputSetProtocol.InputSet = cmds;
#endif
                cmdOpData.DataContract = inputSetProtocol;
                broadcastCmdHandler?.Invoke(cmdOpData);
            }
            tick++;
        }
        public void Clear()
        {
            RoomId = 0;
            tick = 0;
            IsAlive = false;
            inputSetProtocol.Clear();
        }
    }
}
