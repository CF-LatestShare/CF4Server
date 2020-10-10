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
        Dictionary<int, FixInputSet> tickCmdDict = new Dictionary<int, FixInputSet>();
#endif
        FixInputSet playerInputSets = new FixInputSet();
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
#if SERVER
        MessageManager msgMgrInstance;
        PlayerManager playerMgrInstance;
#endif
        FixPlayer player = new FixPlayer();
        FixRoomPlayer roomPlayer = new FixRoomPlayer();
#if SERVER
        public RoomEntity()
        {
            msgMgrInstance = GameManager.CustomeModule<MessageManager>();
            playerMgrInstance = GameManager.CustomeModule<PlayerManager>();
        }
#endif
        public void Oninit(int roomId)
        {
            this.RoomId = roomId;
            playerInputSets.RoomId = roomId;
            IsAlive = true;
            roomPlayer.RoomId = roomId;
            roomPlayer.Player = player;
        }
#if !SERVER
        public void OnPlayersInput(FixInputSet inputs)
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
        public void OnPlayerInput(FixInput input)
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
        public void Enter(PlayerEntity playerEntity)
        {
            var result = playerDict.TryAdd(playerEntity.PlayerId, playerEntity);
            if (result)
            {
#if SERVER
                player.PlayerId = playerEntity.PlayerId;
                player.SessionId= playerEntity.SessionId;
                BroadcastCmdHandler += playerEntity.SendCommadMessage;
                msgMgrInstance.SendCommandMessage
                    (playerEntity.SessionId, ProtocolDefine.OPERATION_ENTERROOM, roomPlayer, ProtocolDefine.RETURN_SUCCESS);
            }
            else
                playerEntity.SendCommadMessage
                     (ProtocolDefine.OPERATION_EXITROOM, roomPlayer, ProtocolDefine.RETURN_ALREADYEXISTS);
#else
                BroadcastCmdHandler += playerEntity.SendCommadMessage;
            }
#endif
            Utility.Debug.LogWarning($"加入房间 ,PlayerEntity{playerEntity}");
        }
        public void Exit(PlayerEntity playerEntity)
        {
            var result = playerDict.Remove(playerEntity.PlayerId);
            if (result)
            {
#if SERVER
                player.PlayerId = playerEntity.PlayerId;
                player.SessionId = playerEntity.SessionId;
                BroadcastCmdHandler -= playerEntity.SendCommadMessage;
                playerEntity.SendCommadMessage
                    (ProtocolDefine.OPERATION_EXITROOM, roomPlayer, ProtocolDefine.RETURN_SUCCESS);
                playerEntity.Dispose();
            }
            else
                playerEntity.SendCommadMessage
                      (ProtocolDefine.OPERATION_EXITROOM, roomPlayer, ProtocolDefine.RETURN_NOTFOUND);
#else
                BroadcastCmdHandler -= playerEntity.SendCommadMessage;
            }
#endif
            Utility.Debug.LogWarning($"离开房间 ,PlayerEntity{playerEntity}");
        }
        public void RunGame()
        {
            IsRunning = true;
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
                playerInputSets.Tick = tick;
                playerInputSets.InputSet = cmds;
#endif
                cmdOpData.DataContract = playerInputSets;
                broadcastCmdHandler?.Invoke(cmdOpData);
            }
            tick++;
        }
        public void Clear()
        {
            RoomId = 0;
            tick = 0;
            IsAlive = false;
            playerInputSets.Clear();
        }
    }
}
