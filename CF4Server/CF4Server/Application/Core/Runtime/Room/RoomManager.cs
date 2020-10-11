using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Cosmos;
using Protocol;

namespace CosmosServer
{
    [CustomeModule]
    public class RoomManager : Module<RoomManager>
    {

#if SERVER
        Action roomRefreshHandler;
        event Action RoomRefreshHandler
        {
            add { roomRefreshHandler += value; }
            remove
            {
                try { roomRefreshHandler -= value; }
                catch (Exception e) { Utility.Debug.LogError(e); }
            }
        }
        Dictionary<int, RoomEntity> roomDict = new Dictionary<int, RoomEntity>();
        Queue<RoomEntity> roomPoolQueue = new Queue<RoomEntity>();
        int updateInterval = ApplicationBuilder._MSPerTick;
        long latestTime;
        int roomIndex;
#else
        RoomEntity roomEntity = new RoomEntity();
#endif
        PlayerManager playerMgrInstance;
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_ENTERROOM, EnterRoomHandler);
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_EXITROOM, ExitRoomHandler);
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_PLYAERINPUT, PlayerInputHandler);
#if SERVER
            playerMgrInstance = GameManager.CustomeModule<PlayerManager>();
            latestTime = Utility.Time.MillisecondTimeStamp() + updateInterval;
#else
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_PLAYERENTER, PlayerEnterHandler);
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_PLAYEREXIT, PlayerExitHandler);
            playerMgrInstance = Facade.CustomeModule<PlayerManager>();
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
                roomRefreshHandler?.Invoke();
            }
#else

#endif
        }
        void EnterRoomHandler(OperationData opData)
        {
            try
            {
#if SERVER
                //系统分配房间；
                FixPlayer player = opData.DataContract as FixPlayer;
                var enterableRoom = Utility.Algorithm.Find(roomDict.Values.ToArray(), (r) => r.Enterable);
                if (enterableRoom == null)
                {
                    SpawnRoom(out var er);
                    enterableRoom = er;
                    Utility.Debug.LogWarning($"未查找到可进入房间：当前房间数量：{roomDict.Count},roomIndex:{roomIndex};");
                }
                else
                {
                    Utility.Debug.LogWarning($"查找到可进入房间 RoomId:{enterableRoom.RoomId}；当前房间数量：{roomDict.Count}");
                }
                playerMgrInstance.TryAddPlayer(player.SessionId, out var pe);
                enterableRoom.Enter(pe);
#else
                var rp = opData.DataContract as FixRoomEntity;
                if (!roomEntity.IsAlive)
                    roomEntity.Oninit(rp.RoomId);
                for (int i = 0; i < rp.Players.Count; i++)
                {
                    var result = playerMgrInstance.TryAddPlayer(rp.Players[i].SessionId, rp.Players[i].PlayerId, out var pe);
                    if (result)
                        roomEntity.Enter(pe);
                }


#endif
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
        void ExitRoomHandler(OperationData opData)
        {
            try
            {
#if SERVER
                var rp = opData.DataContract as FixRoomPlayer;
                var hasRoom = roomDict.TryGetValue(rp.RoomId, out var roomEntity);
                if (!hasRoom)
                    return;
                var result = playerMgrInstance.TryGetPlayer(rp.Player.SessionId, out var pe);
                {
                    roomEntity.Exit(pe);
                    if (roomEntity.PlayerCount <= 0)
                        DespawnRoom(roomEntity);
                }
#else
                FixRoomPlayer rp = opData.DataContract as FixRoomPlayer;
                roomEntity.Clear();
                playerMgrInstance.ClearAll();
#endif
            }
            catch (Exception e)
            {

                Utility.Debug.LogError(e);
            }
        }
        void PlayerInputHandler(OperationData opData)
        {
            try
            {
#if SERVER
                var input = opData.DataContract as FixInput;
                var result = roomDict.TryGetValue(input.RoomId, out var room);
                if (result)
                    room.OnPlayerInput(input);
#else

#endif
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }

        }
#if !SERVER
        void PlayerEnterHandler(OperationData opData)
        {
            try
            {
                FixPlayer fp = opData.DataContract as FixPlayer;
                var result = playerMgrInstance.TryAddPlayer(fp.SessionId, fp.PlayerId, out var pe);
                if (result)
                    roomEntity.Enter(pe);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
        void PlayerExitHandler(OperationData opData)
        {
            try
            {
                FixPlayer fp = opData.DataContract as FixPlayer;
                var result = playerMgrInstance.TryRemovePlayer(fp.SessionId, out var pe);
                if (result)
                    roomEntity.Exit(pe);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
#endif
        bool SpawnRoom(out RoomEntity room)
        {
#if SERVER
            var result = roomPoolQueue.TryDequeue(out room);
            if (!result)
                room = new RoomEntity();
            room.Oninit(roomIndex++);
            RoomRefreshHandler += room.OnRefresh;
            return roomDict.TryAdd(room.RoomId, room);
#else
            room = roomEntity;
            return true;
#endif
        }
        bool DespawnRoom(RoomEntity room)
        {
#if SERVER
            var result = roomDict.Remove(room.RoomId);
            if (!result)
                return false;
            room.Clear();
            RoomRefreshHandler -= room.OnRefresh;
            roomPoolQueue.Enqueue(room);
            return true;
#else
            room.Clear();
            return true;
#endif
        }

    }
}