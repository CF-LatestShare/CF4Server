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
        public bool Enterable { get { return PlayerCount < _MaxPlayer; } }
        public bool Full { get; private set; }
        /// <summary>
        /// 当前房间帧指令数据字典；
        /// </summary>
#if SERVER
        Dictionary<int, Dictionary<int, FixInput>> tickCmdDict = new Dictionary<int, Dictionary<int, FixInput>>();
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
        Dictionary<int, FixPlayer> fixPlayerDict = new Dictionary<int, FixPlayer>();
        PlayerManager playerMgrInstance;
#endif
        FixPlayer player = new FixPlayer();
        FixRoomPlayer roomPlayer = new FixRoomPlayer();
        public RoomEntity()
        {
#if SERVER
            msgMgrInstance = GameManager.CustomeModule<MessageManager>();
            playerMgrInstance = GameManager.CustomeModule<PlayerManager>();
            cmdOpData.OperationCode = ProtocolDefine.OPERATION_PLYAERINPUT;
#else
#endif
        }
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
            tickCmdDict.TryAdd(inputs.Tick, inputs);
        }
#endif
#if SERVER
        /// <summary>
        ///缓存输入指令； 
        /// </summary>
        public void OnPlayerInput(FixInput input)
        {
            Utility.Debug.LogInfo($"RoomId：{RoomId},FixInput：{input}");
            //若玩家延迟，发送的帧落后当前帧数，则将帧转换为当前帧；
            if (input.Tick < tick)
            {
                input.Tick = tick;
            }
            var result = tickCmdDict.TryGetValue(tick, out var dict);
            if (!result)
            {
                tickCmdDict.Add(tick, new Dictionary<int, FixInput>());
                if( tickCmdDict[tick].TryAdd(input.SessionId, input))
                {
                    //Utility.Debug.LogInfo($"RoomId：{RoomId},存储帧：{tick}");
                }
            }
            else
            {
                dict.TryAdd(input.SessionId, input);
            }
        }
#endif
        public void Enter(PlayerEntity playerEntity)
        {
            var result = playerDict.TryAdd(playerEntity.SessionId, playerEntity);
            if (result)
            {
#if SERVER
                FixPlayer fixPlayer = new FixPlayer() { PlayerId = playerEntity.PlayerId, SessionId = playerEntity.SessionId };
                fixPlayerDict.Add(fixPlayer.SessionId, fixPlayer);
                {
                    var opData = new OperationData() { OperationCode = ProtocolDefine.OPERATION_PLAYERENTER, DataContract = fixPlayer };
                    broadcastCmdHandler?.Invoke(opData);
                }
                {
                    FixRoomEntity fre = new FixRoomEntity();
                    fre.RoomId = RoomId;
                    fre.MSPerTick= ApplicationBuilder._MSPerTick;
                    fre.Players = fixPlayerDict.Values.ToList<FixPlayer>();
                    msgMgrInstance.SendCommandMessage
                    (playerEntity.SessionId, ProtocolDefine.OPERATION_ENTERROOM, fre, ProtocolDefine.RETURN_SUCCESS);
                }
                BroadcastCmdHandler += playerEntity.SendCommadMessage;
            }
#else
                BroadcastCmdHandler += playerEntity.UpdateEntity;
            }
#endif
            Utility.Debug.LogInfo($"加入房间 RoomId:{RoomId} ,PlayerEntity : {playerEntity}");
        }
        public void Exit(PlayerEntity playerEntity)
        {
            var result = playerDict.Remove(playerEntity.SessionId);
            if (result)
            {
#if SERVER
                FixPlayer fixPlayer = new FixPlayer() { PlayerId = playerEntity.PlayerId, SessionId = playerEntity.SessionId };
                BroadcastCmdHandler -= playerEntity.SendCommadMessage;
                {
                    var opData = new OperationData() { OperationCode = ProtocolDefine.OPERATION_PLAYEREXIT, DataContract = fixPlayer };
                    broadcastCmdHandler?.Invoke(opData);
                    fixPlayerDict.Remove(playerEntity.SessionId);
                }
                playerEntity.SendCommadMessage
                    (ProtocolDefine.OPERATION_EXITROOM, roomPlayer, ProtocolDefine.RETURN_SUCCESS);
                playerEntity.Dispose();
            }
#else
                playerEntity.Dispose();
                BroadcastCmdHandler -= playerEntity.UpdateEntity;
            }
#endif
            Utility.Debug.LogInfo($"离开房间 RoomId:{RoomId} ,PlayerEntity : {playerEntity}");
        }
        public void OnRefresh()
        {
            if (!IsAlive)
                return;
            //if (!Full)
            //    return;
            //广播当前帧，并进入下一帧；
            var result = tickCmdDict.TryGetValue(tick, out var cmds);
            playerInputSets.Tick = tick;
            playerInputSets.InputDict = cmds;
            playerInputSets.TS = Utility.Time.MillisecondTimeStamp();
            cmdOpData.DataContract = playerInputSets;
            broadcastCmdHandler?.Invoke(cmdOpData);
            tick++;
            if (result)
            {
               // Utility.Debug.LogWarning($"房间 RoomId:{RoomId} 找到帧，广播tick:{tick}");
            }
            else
            {
               // Utility.Debug.LogWarning($"房间 RoomId:{RoomId} 空帧，广播tick:{tick}");
            }
        }
        public void Clear()
        {
            Utility.Debug.LogInfo($"房间 RoomId:{RoomId} 被回收");
            RoomId = 0;
            tick = 0;
            IsAlive = false;
            playerInputSets.Clear();
            foreach (var player in playerDict.Values)
            {
                player.Dispose();
            }
            playerDict.Clear();
            broadcastCmdHandler = null;
#if SERVER
            fixPlayerDict.Clear();
#endif
        }
    }
}
