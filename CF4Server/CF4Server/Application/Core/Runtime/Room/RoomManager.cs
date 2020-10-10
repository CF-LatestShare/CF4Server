using System;
using System.Collections.Generic;
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
            playerMgrInstance = GameManager.CustomeModule<PlayerManager>();
#if SERVER
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
                roomRefreshHandler?.Invoke();
            }
#else

#endif
        }
        void EnterRoomHandler(OperationData opData)
        {
            //系统分配房间；
            FixPlayer player = opData.DataContract as FixPlayer;
            List<RoomEntity> roomSet = new List<RoomEntity>();
            roomSet.AddRange(roomDict.Values);
            var enterableRoom = roomSet.Find((r) => r.IsFull);
            if (enterableRoom == null)
                SpawnRoom(out enterableRoom);
            playerMgrInstance.TryAddPlayer(player.SessionId, out var pe);
            enterableRoom.Enter(pe);
        }
        void ExitRoomHandler(OperationData opData)
        {
            var rp = opData.DataContract as FixRoomPlayer;
            roomDict.TryGetValue(rp.RoomId, out var roomEntity);
            playerMgrInstance.TryGetPlayer(rp.Player.PlayerId, out var pe);
            roomEntity.Exit(pe);
            if (roomEntity.PlayerCount == 0)
                DespawnRoom(roomEntity);
        }
        void PlayerInputHandler(OperationData opData)
        {
#if SERVER
            var input = opData.DataContract as FixInput;
            var result = roomDict.TryGetValue(input.RoomId, out var room);
            if (result)
                room.OnPlayerInput(input);
#else

#endif
        }
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
