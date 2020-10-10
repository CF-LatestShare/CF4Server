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
#if SERVER
        Dictionary<int, RoomEntity> roomDict = new Dictionary<int, RoomEntity>();
        Queue<RoomEntity> roomPoolQueue = new Queue<RoomEntity>();
        int updateInterval = ApplicationBuilder._MSPerTick;
        long latestTime;
        int roomIndex;
#else
        RoomEntity roomEntity = new RoomEntity();
#endif
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_PLYAERINPUT, PlayerInputCommand);
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
        bool DespawnRoom(int roomId)
        {
#if SERVER
            var result = roomDict.Remove(roomId, out var room);
            if (!result)
                return false;
            room.Clear();
            RoomRefreshHandler -= room.OnRefresh;
            roomPoolQueue.Enqueue(room);
            return true;
#else
            roomEntity.Clear();
            return true;
#endif
        }

        void PlayerInputCommand(OperationData operationData)
        {
#if SERVER
            var cmd = operationData.DataContract as FixInput;
            switch (cmd.Cmd)
            {
                case ProtocolDefine.CMD_SYN:
                    {
                        if (cmd.RoomId != 0)
                        {
                            var result = roomDict.TryGetValue(cmd.RoomId, out var room);
                            if (result)
                                room.Join(cmd);
                            else
                            {
                                if (SpawnRoom(out  room))
                                {
                                    room.Join(cmd);
                                }
                            }
                        }
                        else
                        {
                            List<RoomEntity> roomSet = new List<RoomEntity>();
                            roomSet.AddRange(roomDict.Values);
                            var enterableRoom= roomSet.Find((r) => r.IsFull);
                            enterableRoom.Join(cmd);
                        }
                    }
                    break;
                case ProtocolDefine.CMD_FIN:
                    {
                        var result = roomDict.TryGetValue(cmd.RoomId, out var room);
                        if (result)
                            room.Leave(cmd);
                        if (room.PlayerCount == 0)
                            DespawnRoom(cmd.RoomId);
                    }
                    break;
                case ProtocolDefine.CMD_MSG:
                    {
                        //转发到具体房间；
                        roomDict.TryGetValue(cmd.RoomId, out var room);
                        room?.CacheInputCmd(cmd);
                    }
                    break;
                case ProtocolDefine.CMD_ACK:
                    {
                        //这部分逻辑由客户端实现；
                    }
                    break;
#else
         try
            {
                var cmd = Utility.MessagePack.ToObject<InputProtocol>(buffer);
                switch (cmd.Cmd)
                {
                    case ProtocolDefine.CMD_ACK:
                        {
                            //这部分逻辑由客户端实现；
                            roomEntity.Join(cmd);
                            Utility.Debug.LogInfo("接收CMD_ACK");
                        }
                        break;
                }
            }
            catch (Exception e) { Utility.Debug.LogError(e); }

            try
            {
                var cmds = Utility.MessagePack.ToObject<InputSetProtocol>(buffer);
                roomEntity.CacheInputCmds(cmds);
                roomEntity.OnRefresh();
            }
            catch (Exception e){Utility.Debug.LogError(e); }
#endif
            }
        }
    }
}
