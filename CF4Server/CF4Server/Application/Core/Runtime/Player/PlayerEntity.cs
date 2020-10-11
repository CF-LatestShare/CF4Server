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
    public class PlayerEntity : IDisposable
    {
        public int SessionId { get; set; }

#if !SERVER
        PlayerEntityAgent agent;
#endif
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
            Facade.CustomeModule<NetManager>().SendCommandMessage(data);
#else
            GameManager.CustomeModule<MessageManager>().SendCommandMessage(SessionId, data);
#endif
        }
        public void SendCommadMessage<T>(byte opCode, T data, short returnCode = 0)
            where T : IDataContract
        {
#if !SERVER
            Facade.CustomeModule<NetManager>().SendCommandMessage(opCode,data);
#else
            GameManager.CustomeModule<MessageManager>().SendCommandMessage(SessionId, opCode, data, returnCode);
#endif
        }
#if !SERVER
        public void UpdateEntity(OperationData data)
        {
            agent.UpdateEntity(data.DataContract);
        }
#endif
        public override string ToString()
        {
            return $"SessionId:{SessionId} ; PlayerId:{PlayerId}";
        }
    }
}