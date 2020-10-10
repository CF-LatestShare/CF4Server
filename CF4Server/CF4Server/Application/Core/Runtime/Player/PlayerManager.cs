using System;
using System.Collections;
using System.Collections.Generic;
using Cosmos;
using Protocol;

namespace CosmosServer
{
    [CustomeModule]
    public class PlayerManager : Module<PlayerManager>
    {
        /// <summary>
        /// 玩家字典；
        /// 玩家ID->玩家；
        /// </summary>
        Dictionary<int, PlayerEntity> playerDict = new Dictionary<int, PlayerEntity>();
        /// <summary>
        /// 回收的玩家对象缓存；
        /// </summary>
        Queue<PlayerEntity> playerPoolQueue = new Queue<PlayerEntity>();
        int playerIndex;
        public override void OnInitialization()
        {
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_ROOM, PlayerCmdHandler);
        }
        public bool TryAddPlayer(int sessionId,out PlayerEntity playerEntity)
        {
#if SERVER
            var result = playerPoolQueue.TryDequeue(out var pe);
#else
            PlayerEntity pe=null;
            bool result = false;
            if (playerPoolQueue.Count > 0)
            {
                pe = playerPoolQueue.Dequeue();
                result = true;
            }
#endif
            if (!result)
                pe = new PlayerEntity();
            pe.SetPlayerId(playerIndex++);
            pe.SessionId = sessionId;
            playerEntity = pe;
            return playerDict.TryAdd(pe.PlayerId, pe);
        }
        /// <summary>
        ///移除但是不回收； 
        /// </summary>
        public bool TryRemovePlayer(int playerId, out PlayerEntity playerEntity)
        {
            return playerDict.Remove(playerId, out playerEntity);
        }
        /// <summary>
        ///移除且回收； 
        /// </summary>
        public bool TryRemovePlayer(PlayerEntity playerEntity)
        {
            playerDict.Remove(playerEntity.PlayerId);
            playerEntity.Dispose();
            playerPoolQueue.Enqueue(playerEntity);
            return true;
        }
        public bool HasPlayer(int playerId)
        {
            return playerDict.ContainsKey(playerId);
        }
        public bool TryGetPlayer(int playerId, out PlayerEntity playerEntity)
        {
            return playerDict.TryGetValue(playerId, out playerEntity);
        }
        void PlayerCmdHandler(OperationData opData)
        {

        }
    }
}