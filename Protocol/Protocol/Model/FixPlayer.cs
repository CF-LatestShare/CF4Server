using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Protocol
{
    [MessagePackObject]
    public class FixPlayer : IDataContract, IDisposable
    {
        [Key(0)]
        public int SessionId { get; set; }
        [Key(1)]
        public int PlayerId { get; set; }
        public void Dispose()
        {
            SessionId = 0;
            PlayerId = 0;
        }
        public override string ToString()
        {
            return $"SessionId:{SessionId} ; PlayerId:{PlayerId}";
        }
    }
}
