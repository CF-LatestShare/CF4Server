using Cosmos;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
#if !SERVER
using UnityEngine;
#endif
namespace CosmosServer
{
    [MessagePackObject]
    public class PlayerEntity : IDisposable
    {
        [Key(0)]
        public int SessionId { get; set; }

#if !SERVER
        PlayerEntityAgent agent;
#endif
        [Key(1)]
        public int PlayerId { get; set; }

        public void Dispose()
        {
#if !SERVER
            GameManagerAgent.KillObject(agent.gameObject);
#endif
        }
        public void SetPlayerId(int id)
        {
#if !SERVER
            var go = Facade.LoadResPrefab<PlayerEntityAgent>(true);
            agent.SessionId = SessionId; ;
#else
            PlayerId = id;
#endif
        }
        public void SendCommadMessage(OperationData data)
        {
#if !SERVER
#else
            GameManager.CustomeModule<MessageManager>().SendCommandMessage(SessionId, data);
#endif
        }
        public void SendCommadMessage<T>(byte opCode, T data, byte cmd = 0)
        {
#if !SERVER
#else
            GameManager.CustomeModule<MessageManager>().SendCommandMessage(SessionId,opCode, data,cmd);
#endif
        }
    }
}