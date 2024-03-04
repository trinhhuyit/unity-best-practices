using System.Collections.Generic;
using Architecture.Model;

namespace Architecture.Base
{
    // actions must have a type and may include a payload
    public class ReduxAction
    {
        public string Type { get; set; }
        public object Payload { get; set; }
        public Dictionary<string, object> Meta { get; set; }

        public static ReduxAction Create(string type, object payload = null, Dictionary<string, object> meta = null)
        {
            return new ReduxAction()
            {
                Type = type,
                Payload = payload,
                Meta = meta
            };
        }
    }
}