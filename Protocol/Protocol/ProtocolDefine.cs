using System;

namespace Protocol
{
    public class ProtocolDefine
    {
        #region Port  Range[4096 , 4224] ; Count:128;
        /// <summary>
        /// 请求通道；
        /// </summary>
        public const ushort PORT_REQUEST = 4319;
        /// <summary>
        /// 输入同步通道；
        /// </summary>
        public const ushort PORT_INPUT = 4320;
        #endregion

        #region CMD Range[0 , 15] ; Count:16;
        /// <summary>
        /// CMD指令;
        /// 若无必要，勿动；
        /// </summary>
        public const byte CMD_MSG = 1;
        public const byte CMD_SYN = 2;
        public const byte CMD_ACK = 3;
        public const byte CMD_FIN = 4;
        #endregion

        #region Operation Range[16 , 255] ; Count:240;
        /// <summary>
        /// 测试通道；
        /// </summary>
        public const byte OPERATION_TESTCAHNNEL = 16;
        /// <summary>
        /// PlayerInput;
        /// </summary>
        public const byte OPERATION_PLYAERINPUT = 17;
        /// <summary>
        /// EnterRoom;
        /// </summary>
        public const byte OPERATION_ENTERROOM = 18;
        /// <summary>
        /// ExitRoom;
        /// </summary>
        public const byte OPERATION_EXITROOM = 19;
        #endregion

        #region ReturnCode  Range[256 , 319] ; Count:64;
        /// <summary>
        /// 成功；
        /// </summary>
        public const short RETURN_SUCCESS = 257;
        /// <summary>
        /// 错误
        /// </summary>
        public const short RETURN_ERROR = 258;
        /// <summary>
        /// 失败；
        /// </summary>
        public const short RETURN_FAIL = 259;
        /// <summary>
        /// 空；
        /// </summary>
        public const short RETURN_EMPTY = 260;
        /// <summary>
        /// 操作码无效；
        /// </summary>
        public const short RETURN_INVALIDOPERATION = 261;
        /// <summary>
        /// 数据无效；
        /// </summary>
        public const short RETURN_INVALIDDATA = 262;
        /// <summary>
        /// 条款（Item）已存在；
        /// </summary>
        public const short RETURN_ALREADYEXISTS = 263;
        /// <summary>
        /// 未查询到条款（Item）；
        /// </summary>
        public const short RETURN_NOTFOUND = 264;
        #endregion
    }
}
