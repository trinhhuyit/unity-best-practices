using System;
using System.Collections.Generic;
using Architecture.Base;

namespace Architecture.Helper
{
    public static class ReduxHelper
    {
        public static Func<ReduxAction> CreateAction(string type, object payload, Dictionary<string, object> meta = null)
        {
            return () => ReduxAction.Create(type, payload, meta);
        }
        
        public static Func<ReduxAction> CreateAction(string type)
        {
            return () => ReduxAction.Create(type);
        }
    }
}