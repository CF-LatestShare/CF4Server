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
        public bool TryAddPlayer(int sessionId, out PlayerEntity playerEntity)
        {
#if SERVER
            var result = playerPoolQueue.TryDequeue(out var pe);
#else
            PlayerEntity pe = null;
            bool result = false;
            if (playerPoolQueue.Count > 0)
            {
                pe = playerPoolQueue.Dequeue();
                result = true;
            }
#endif
            if (!result)
                pe = new PlayerEntity();
            pe.SessionId = sessionId;
            var canAdd = playerDict.TryAdd(pe.SessionId, pe);
            if (canAdd)
            {
                pe.SetPlayerId(playerIndex++);
            }
            playerEntity = pe;
            return canAdd;
        }
        public bool TryAddPlayer(int sessionId, int playerId, out PlayerEntity playerEntity)
        {
#if SERVER
            var result = playerPoolQueue.TryDequeue(out var pe);
#else
            PlayerEntity pe = null;
            bool result = false;
            if (playerPoolQueue.Count > 0)
            {
                pe = playerPoolQueue.Dequeue();
                result = true;
            }
#endif
            if (!result)
                pe = new PlayerEntity();
            pe.SessionId = sessionId;
            var canAdd = playerDict.TryAdd(pe.SessionId, pe);
            if (canAdd)
            {
                pe.SetPlayerId(playerId);
            }
            playerEntity = pe;
            return canAdd;
        }
        public bool TryAddPlayer(PlayerEntity playerEntity)
        {
            var result = playerDict.TryAdd(playerEntity.SessionId, playerEntity);
            return result;
        }
        /// <summary>
        ///移除但是不回收PlayerEntity； 
        /// </summary>
        public bool TryRemovePlayer(int sessionId, out PlayerEntity playerEntity)
        {
            return playerDict.Remove(sessionId, out playerEntity);
        }
        /// <summary>
        ///移除且回收； 
        /// </summary>
        public bool TryRemovePlayer(PlayerEntity playerEntity)
        {
            playerDict.Remove(playerEntity.SessionId);
            playerEntity.Dispose();
            playerPoolQueue.Enqueue(playerEntity);
            return true;
        }
        public bool HasPlayer(int playerId)
        {
            return playerDict.ContainsKey(playerId);
        }
        public bool TryGetPlayer(int sessionId, out PlayerEntity playerEntity)
        {
            return playerDict.TryGetValue(sessionId, out playerEntity);
        }
    }
}