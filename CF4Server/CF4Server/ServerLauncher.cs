using Cosmos;
using System;
using System.Runtime.InteropServices;
using ProtocolCore;
using System.Threading.Tasks;

namespace CosmosServer
{
    public class ServerLauncher
    {
        public delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        static ControlCtrlDelegate newDelegate = new ControlCtrlDelegate(HandlerRoutine);
        public static bool HandlerRoutine(int CtrlType)
        {
            Utility.Debug.LogInfo("Server Shutdown !");//按控制台关闭按钮关闭 
            return false;
        }
        static string ip = "127.0.0.1";
        static int port = 8513;
        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(newDelegate, true);
#if DEBUG
            Utility.Debug.SetHelper(new ConsoleDebugHelper("Debug_LockStep"));
#else
            Utility.Debug.SetHelper(new ConsoleDebugHelper("Release_LockStep"));
#endif
            Utility.MessagePack.SetHelper(new ImplMessagePackHelper());
            Utility.Debug.LogInfo("Server Start Running !");
            GameManager.NetworkManager.Connect(ip, port, System.Net.Sockets.ProtocolType.Udp);
            GameManager.InitCustomeModule(typeof(ServerLauncher).Assembly);
            Task.Run(GameManagerAgent.Instance.OnRefresh);
            while (true) { }
        }
    }
}
