using System;

namespace Protocol
{
    public class ProtocolDefine
    {
        #region Port
        /// <summary>
        /// 请求通道；
        /// </summary>
        public const ushort PORT_REQUEST = 4319;
        /// <summary>
        /// 输入同步通道；
        /// </summary>
        public const ushort PORT_INPUT = 4320;
        #endregion

        #region CMD
        /// <summary>
        /// CMD指令，若无必要勿动；
        /// </summary>
        public const byte CMD_MSG = 0;
        public const byte CMD_SYN = 1;
        public const byte CMD_ACK = 2;
        public const byte CMD_FIN = 3;
        #endregion

        #region Operation
        /// <summary>
        /// PlayerInput;
        /// </summary>
        public const byte OPERATION_PLYAERINPUT = 15;
        /// <summary>
        /// Room;
        /// </summary>
        public const byte OPERATION_ROOM = 16;
        #endregion
    }
}
