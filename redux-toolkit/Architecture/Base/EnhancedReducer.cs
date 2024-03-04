using System;
using Unidux;

namespace Architecture.Base
{
    using JavaObject = System.Collections.Generic.Dictionary<string, object>;
    using ActionHandler = Func<State, ReduxAction, State>;

    public class EnhancedReducer : ReducerBase<State, ReduxAction>
    {
        public override State Reduce(State state,
            ReduxAction reduxAction)
        {
            if (HandlerTree != null)
            {
                //Handle if redux action type already a key in tree
                if (HandlerTree.ContainsKey(reduxAction.Type))
                {
                    ActionHandler handler = HandlerTree[reduxAction.Type] as ActionHandler;
                    if (handler != null) state = handler(state, reduxAction);
                    return state;
                }
                
                //Find in tree whether handler for redux action type have been implemented
                foreach (string key in HandlerTree.Keys)
                {
                    if (reduxAction.Type.Contains(key))
                    {
                        if (Utils.IsFunc(HandlerTree[key].GetType())) //2nd level only
                        {
                            ActionHandler handler = HandlerTree[key] as ActionHandler;
                            if (handler != null) state = handler(state, reduxAction);
                            return state;
                        }
                        if (Utils.IsJavaObject(HandlerTree[key].GetType())) //2nd level only
                        {
                            string suffix = reduxAction.Type.Replace(key, "").TrimStart('/');
                            
                            JavaObject javaObject = HandlerTree[key] as JavaObject;
                            if (javaObject == null || !javaObject.ContainsKey(suffix)) continue;
                            
                            ActionHandler handler = javaObject[suffix] as ActionHandler;
                            if (handler != null) state = handler(state, reduxAction);
                            
                            return state;
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return state;
        }
    }
}