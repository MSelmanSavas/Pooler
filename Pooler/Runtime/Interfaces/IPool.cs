
using System;
using System.Collections;
using Pooler.Config.Data;
using Pooler.Request.Data;

namespace Pooler
{
    public interface IPool
    {
        bool Initialize(BasePoolConfigData configData);
        System.Type GetConfigDataType();
        System.Type GetRequestDataType();
        bool CheckRequestDataType(object requestData);
        T1 GetPoolObj<T1, T2>(T2 requestData) where T1 : class, IPoolElement where T2 : BasePoolRequestData;
        bool ReturnPoolObj<T>(T poolElement) where T : IPoolElement;
        IEnumerator GetPoolEnumerator();
        BasePoolConfigData GetPoolAsConfigData();
        public Action<IPool> OnCapacityChange { get; set; }
    }
}
