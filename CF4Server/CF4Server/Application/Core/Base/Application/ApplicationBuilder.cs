using Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosServer
{
    public class ApplicationBuilder : IApplicationBuilder
    {
        /// <summary>
        /// 服务器帧数；
        /// 换算成毫秒为125ms/tick;
        /// </summary>
        public const int TICKRATE = 4;
        /// <summary>
        /// 每个tick所持续的毫秒；
        /// </summary>
        public static readonly int _MSPerTick;
        static ApplicationBuilder()
        {
            _MSPerTick = 1000 / TICKRATE;
        }
    }
}
