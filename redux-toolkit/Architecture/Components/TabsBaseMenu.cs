using System.Collections.Generic;
using UIComponents.Inventory.Components;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.Components
{
    public class TabsBaseMenu : BaseMenu
    {
        [SerializeField] protected List<TabButton> _tabsButton;
        [SerializeField] protected List<GameObject> _tabs;
        
        protected void InitTabs()
        {
            SetShopTabsActive(0);

            foreach (var tab in _tabsButton)
            {
                tab.Button.OnClickAsObservable().Subscribe(_ => SetShopTabsActive(tab.transform.GetSiblingIndex()));
            }
        }

        protected void SetShopTabsActive(int currentTabSelectedIndex)
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                _tabs[i].gameObject.SetActive(i == currentTabSelectedIndex); 
                _tabsButton[i].SetActive(i != currentTabSelectedIndex);
            }
        }
    }
}