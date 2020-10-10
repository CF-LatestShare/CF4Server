using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
namespace CosmosServer
{
#if SERVER
    [CustomeModule]
#endif
    public class DataManager : Module<DataManager>, ISimpleKeyValue<Type, object>
    {
        /// <summary>
        /// 间隔秒；
        /// </summary>
        int intervalSec = 1200;
        long latestRefreshTime;
        IDataProvider dataProvider;
        /// <summary>
        /// 对象字典；
        /// </summary>
        Dictionary<Type, object> dataDict;
        /// <summary>
        /// json数据字典；
        /// </summary>
        Dictionary<string, string> jsonDict;
        public override void OnInitialization()
        {
            InitProvider();
            latestRefreshTime = Utility.Time.SecondNow() + intervalSec;
        }
        /// <summary>
        /// 覆写轮询函数；
        /// 自动更新服务器数据类型；
        /// </summary>
        public override void OnRefresh()
        {
            if (IsPause)
                return;
            var now = Utility.Time.SecondNow();
            if (now >= latestRefreshTime)
            {
                jsonDict = dataProvider?.LoadData() as Dictionary<string, string>;
                dataDict = dataProvider?.ParseData() as Dictionary<Type, object>;
                latestRefreshTime = now + intervalSec;
            }
        }
        public bool ContainsKey(Type key)
        {
            return dataDict.ContainsKey(key);
        }
        public bool TryAdd(Type key, object value)
        {
            return dataDict.TryAdd(key, value);
        }
        public bool TryGetValue(Type key, out object value)
        {
            return dataDict.TryGetValue(key, out value);
        }
        public bool TryRemove(Type key)
        {
            return dataDict.Remove(key);
        }
        public bool TryRemove(Type key, out object value)
        {
            return dataDict.Remove(key, out value);
        }
        public bool ContainsKey<T>()
            where T : class
        {
            return ContainsKey(typeof(T));
        }
        public bool TryAdd<T>(T value)
            where T : class
        {
            return TryAdd(typeof(T), value);
        }
        public bool TryGetValue<T>(out T value)
            where T : class
        {
            value = default;
            object data;
            var result = TryGetValue(typeof(T), out data);
            if (result)
                value = data as T;
            return result;
        }
        public bool TryRemove<T>()
            where T : class
        {
            return TryRemove(typeof(T));
        }
        public bool TryRemove<T>(out T value)
            where T : class
        {
            value = default;
            object data;
            var result = TryRemove(typeof(T), out data);
            if (result)
                value = data as T;
            return result;
        }
        public bool TryAdd(string key, string value)
        {
            return jsonDict.TryAdd(key, value);
        }
        public bool TryGetValue(string key, out string value)
        {
            return jsonDict.TryGetValue(key, out value);
        }
        public bool TryRemove(string key)
        {
            return jsonDict.Remove(key);
        }
        public bool TryRemove(string key, out string value)
        {
            return jsonDict.Remove(key, out value);
        }
        public bool ContainsKey(string key)
        {
            return jsonDict.ContainsKey(key);
        }
        void InitProvider()
        {
            var obj = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute>(typeof(IDataProvider));
            dataProvider = obj as IDataProvider;
            jsonDict = dataProvider?.LoadData() as Dictionary<string, string>;
            dataDict = dataProvider?.ParseData() as Dictionary<Type, object>;
        }
    }
}
