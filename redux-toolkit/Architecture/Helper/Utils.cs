using System;
using System.Collections.Generic;
using Architecture.Base;

namespace Architecture
{
    public static class Utils
    {
        public static string GetOriginalAction(string actionType)
        {
            string fulfilledSuffix = "/" + Constants.Fulfilled;
            string rejectSuffix = "/" + Constants.Rejected;
            string pendingSuffix = "/" + Constants.Pending;

            return actionType.Replace(fulfilledSuffix, "").Replace(rejectSuffix, "").Replace(pendingSuffix, "");
        }

        public static string CreateRejectAction(string actionType)
        {
            return string.Format("{0}/{1}", actionType, Constants.Rejected);
        }

        public static string CreatePendingAction(string actionType)
        {
            return string.Format("{0}/{1}", actionType, Constants.Pending);
        }

        public static string CreateFulfilledAction(string actionType)
        {
            return string.Format("{0}/{1}", actionType, Constants.Fulfilled);
        }

        public static bool IsFulfilled(string actionType)
        {
            return actionType != null && actionType.Contains(Constants.Fulfilled);
        }

        public static bool IsPending(string actionType)
        {
            return actionType != null && actionType.Contains(Constants.Pending);
        }

        public static bool IsReject(string actionType)
        {
            return actionType != null && actionType.Contains(Constants.Rejected);
        }

        public static bool IsFunc(Type type)
        {
            return type.FullName != null && type.FullName.Contains("System.Func");
        }

        public static bool IsJavaObject(Type type)
        {
            return type.FullName != null && type.FullName.Contains("System.Collections.Generic");
        }
    }
}