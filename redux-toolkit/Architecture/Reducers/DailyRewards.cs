using System;
using System.Collections.Generic;
using Architecture.Base;
using Architecture.Helper;
using Architecture.Model;
using Architecture.Services;
using Newtonsoft.Json;

namespace Architecture.Reducers
{
    using ActionHandler = Func<State, ReduxAction, State>;
    using JavaObject = Dictionary<string, object>;

    public class DailyRewards
    {
        [Serializable]
        public class DailyRewardsState
        {
            public List<DailyRewardModel> DailyRewards { get; set; }
        }

        public const string FetchAll = "DAILY_REWARDS/FETCH_ALL";
        public const string Claim = "DAILY_REWARDS/CLAIM";

        //Action//
        private static readonly Func<Func<ReduxAction>>
            FetchAllAction = () =>
                ReduxHelper.CreateAction(FetchAll, DailyRewardService.FetchDailyRewards());

        private static readonly Func<List<DailyRewardItemModel>, Func<ReduxAction>>
            ClaimAction = lstClaimRewards =>
                ReduxHelper.CreateAction(Claim, DailyRewardService.Claim(), new JavaObject
                {
                    {"lstClaimRewards", lstClaimRewards}
                });

        //Action Creator//
        public static void FetchAllActionCreator()
        {
            ReduxAction action = FetchAllAction()();
            Redux.Dispatch(action);
        }

        public static void ClaimActionCreator(List<DailyRewardItemModel> lstClaimRewards)
        {
            ReduxAction action = ClaimAction(lstClaimRewards)();
            Redux.Dispatch(action);
        }

        //Reducer//
        public sealed class Reducer : EnhancedReducer
        {
            public Reducer()
            {
                HandlerTree = new JavaObject
                {
                    {
                        FetchAll, (new JavaObject
                        {
                            {Constants.Fulfilled, FetchAllActionHandler}
                        })
                    }
                };
            }

            //ALL MONSTER, ITEMS DESIGN WILL NOT BE STORE IN STATE DUE TO HEAVY LOAD//
            private static readonly ActionHandler FetchAllActionHandler =
                (state, action) =>
                {
                    state.DailyRewardsState.DailyRewards =
                        JsonConvert.DeserializeObject<List<DailyRewardModel>>(action.Payload as String);
                    return state;
                };
        }
    }
}