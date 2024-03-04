using System.Collections.Generic;
using System.Linq;
using Architecture.Base;
using Architecture.Model;
using Architecture.Reducers;
using Helper;
using UniRx;
using UnityEngine;

namespace Architecture.Components
{
    public class BaseMenu : MonoBehaviour
    {
        private bool _justOpen = false;

        protected virtual void OnClose()
        {
        }

        protected virtual void OnOpen()
        {
        }

        protected virtual bool CanOpen()
        {
            return true;
        }

        protected virtual void OnFirstTimeGetData(Dictionary<string, object> data)
        {
        }

        protected virtual void OnGetData(Dictionary<string, object> data)
        {
        }

        protected virtual void OnReduxUpdate(State state)
        {
        }

        private void Display(bool isDisplay)
        {
//            if (!CanOpen() && isDisplay) return;

            if (this == null ||
                this.gameObject == null ||
                this.gameObject.Equals(null) ||
                this.Equals(null) ||
                this.gameObject.activeInHierarchy == isDisplay) return;

            this.gameObject.SetActive(isDisplay);
        }

        private void Responsive()
        {
            if ((Screen.width == 1200 && Screen.height == 1920) || (Screen.width == 600 && Screen.height == 960))
            {
                this.transform.localScale = Vector3.one * 0.9f;
            }
        }

        private void ReduxHandler(MenuType menuType)
        {
            Redux.Subject
                .StartWith(Redux.State)
                .Subscribe(state =>
                {
                    if (Ultilities.IsObjectDead(this)) return;
                    MenuModel exitsMenuModel = state.MenusState.ListMenu.FirstOrDefault(x => x.Type == menuType);
                    if (exitsMenuModel == null)
                    {
                        Display(false);
                        return;
                    }

                    bool isOpen = exitsMenuModel.On;
                    if (isOpen && CanOpen())
                    {
                        if (!this.gameObject.activeInHierarchy)
                        {
                            Display(true);
                            if (exitsMenuModel.Data != null) OnFirstTimeGetData(exitsMenuModel.Data);
                            _justOpen = true;
                        }

                        OnReduxUpdate(state);
                        if (_justOpen)
                        {
                            _justOpen = false;
                            OnOpen();
                        }
                        else
                        {
                            if (exitsMenuModel.Data != null) OnGetData(exitsMenuModel.Data);
                        }
                    }
                    else
                    {
                        if (this.gameObject.activeInHierarchy)
                        {
                            OnClose();
                            Display(false);
                        }
                    }

                    //THIS MUST BE PLACED HERE DUE TO SOME CASE THIS FUNCTION WILL BE CALLED WHEN SCENE IS SWITCHING
                    ;
                })
                .AddTo(this);
        }

        protected void InitBase(MenuType menuType)
        {
            this.ReduxHandler(menuType);
            this.Responsive();
        }
    }
}