using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
namespace CosmosServer
{
    public class PollingEventCore:ConcurrentEventCore<short,object,PollingEventCore>
    {
    }
}
