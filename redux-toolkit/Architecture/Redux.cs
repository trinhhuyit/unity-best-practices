using System.Collections;
using System.Collections.Generic;
using Architecture.Reducers;
using Architecture.Base;
using Architecture.Model;
using Architecture.Services;
using Unidux;
using UniRx;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Architecture
{
    public sealed class Redux : SingletonMonoBehaviour<Redux>, IStoreAccessor
    {
        public TextAsset InitialStateJson;
        private Store<State> _store;

        public IStoreObject StoreObject
        {
            get { return Store; }
        }

        public static State State
        {
            get { return Store.State; }
        }

        public static Subject<State> Subject
        {
            get { return Store.Subject; }
        }

        private static State InitialState
        {
            get
            {
                return Instance.InitialStateJson != null
                    ? UniduxSetting.Serializer.Deserialize(
                        System.Text.Encoding.UTF8.GetBytes(Instance.InitialStateJson.text),
                        typeof(State)
                    ) as State
                    : new State();
            }
        }


        public static void Reset()
        {
            Store.State = new State();
            Player.LoadActionCreator();
        }

        public static Store<State> Store
        {
            get
            {
                return Instance._store = Instance._store ??
                                         new Store<State>(InitialState,
                                             new Errors.Reducer(),
                                             new User.Reducer(),
                                             new Items.Reducer(),
                                             new Loading.Reducer(),
                                             new Monsters.Reducer(),
                                             new EggBlocks.Reducer(),
                                             new Designs.Reducer(),
                                             new Map.Reducer(),
                                             new DailyRewards.Reducer(),
                                             new Menus.Reducer(),
                                             new Campaigns.Reducer(),
                                             new Rank.Reducer(),
                                             new Gameplay.Reducer(),
                                             new Player.Reducer()
                                         );
            }
        }

        public static object Dispatch<TAction>(TAction action)
        {
            return Store.Dispatch(action);
        }

        private void Start()
        {
            Store.ApplyMiddlewares(
                MiddleWares.Authentication,
                MiddleWares.RunOnceActionStart,
                MiddleWares.Promise,
                MiddleWares.RunOnceActionFinish,
                MiddleWares.GlobalError,
                MiddleWares.Logger,
                MiddleWares.GlobalLoading,
                MiddleWares.CrashReport
            );
            DatabaseService.Init();
            Player.LoadActionCreator();
        }

        private void OnApplicationQuit()
        {
            DatabaseService.Clear();
        }

        private void Update()
        {
            Store.Update();
            
            
            //Handle for Android back key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (User.IsLogin() && 
                    !Menus.IsOpening(MenuType.SelectName) && 
                    !Menus.IsOpening(MenuType.SelectMonster))
                {
                    if (Menus.IsOpening()) Menus.CloseAllMenu();
                    else Application.Quit();
                }
                else
                {
                    Application.Quit();
                }
                
            }
            
        }
    }
}