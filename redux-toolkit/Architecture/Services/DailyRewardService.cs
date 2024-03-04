using RSG;
using UniRx;
using UnityEngine;

namespace Architecture.Services
{
    public static class DailyRewardService
    {
        public static Promise<string> FetchDailyRewards()
        {
            return ObservableWWW.Get(ServerHelper.GetUrl("dailyrewards"), ServerHelper.GetHeader()).ToPromise();
        }

        public static Promise<string> Claim()
        {
            //Params
            WWWForm postParams = new WWWForm();
            postParams.AddField("FIELDS", "FIELDS");

            //Post to server
            return ObservableWWW.Post(ServerHelper.GetUrl("dailyrewards/claim"), postParams, ServerHelper.GetHeader())
                .ToPromise();
        }
    }
}