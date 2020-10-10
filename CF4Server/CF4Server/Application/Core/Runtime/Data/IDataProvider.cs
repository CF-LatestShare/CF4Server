using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosServer
{
    public interface IDataProvider
    {
        object LoadData();
        object ParseData();
    }
}
