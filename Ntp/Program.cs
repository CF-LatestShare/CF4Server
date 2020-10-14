using GuerrillaNtp;
using System;
using System.Net;
using System.Net.Sockets;

namespace Ntp
{
    class Program
    {
        static void Main(string[] args)
        {
            // query the SNTP server
            TimeSpan offset;
            try
            {
                using (var ntp = new NtpClient(Dns.GetHostAddresses("pool.ntp.org")[0]))
                    offset = ntp.GetCorrectionOffset();
            }
            catch (Exception ex)
            {
                // timeout or bad SNTP reply
                offset = TimeSpan.Zero;
            }

            // use the offset throughout your app
            DateTime accurateTime = DateTime.UtcNow + offset;
            Console.WriteLine(accurateTime);
            Console.ReadLine();
        }
    }
}