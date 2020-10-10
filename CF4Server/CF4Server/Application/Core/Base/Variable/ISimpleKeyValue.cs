﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosServer
{
    public interface ISimpleKeyValue<TKey, TValue>
    {
        bool TryGetValue(TKey key, out TValue value);
        bool ContainsKey(TKey key);
        bool TryRemove(TKey key);
        bool TryRemove(TKey key,out TValue value);
        bool TryAdd(TKey key, TValue value);
    }
}