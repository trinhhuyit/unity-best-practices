using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.Model;
using Architecture.Reducers;
using Architecture.Services;
using Unidux;
using UnityEngine;

namespace Architecture.Base
{
    [Serializable]
    public class State : StateBase
    {
        public readonly User.UserState UserState = new User.UserState();
        public readonly Items.ItemsState ItemsState = new Items.ItemsState();
        public readonly Loading.LoadingState LoadingState = new Loading.LoadingState();
        public readonly Monsters.MonstersState MonstersState = new Monsters.MonstersState();
        public readonly Menus.MenusState MenusState = new Menus.MenusState();
        public readonly EggBlocks.EggBlocksState EggBlocksState = new EggBlocks.EggBlocksState();
        public readonly Map.MapState MapState = new Map.MapState();
        public readonly DailyRewards.DailyRewardsState DailyRewardsState = new DailyRewards.DailyRewardsState();
        public readonly Campaigns.CampaignsState CampaignsState = new Campaigns.CampaignsState();
        public readonly Gameplay.GameplayState GameplayState = new Gameplay.GameplayState();
        public readonly Rank.RankState RankState = new Rank.RankState();
        public readonly Errors.ErrorsState ErrorsState = new Errors.ErrorsState();
        public PlayerData PlayerState = new PlayerData();
    }
}