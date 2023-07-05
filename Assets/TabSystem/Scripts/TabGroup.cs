using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TabGroup : MonoBehaviour
{
    public Color tabIdle;
    public Color tabHover;
    public Color tabSeleted;

    List<TabButton> _tabButtons;
    [SerializeField]List<GameObject> _tabActiveGo;
    TabButton _selectTab;
    public void Register(TabButton tabButton)
    {
        if (_tabButtons == null) _tabButtons = new List<TabButton>();
        _tabButtons.Add(tabButton);
    }

    public void OnTabEnter(TabButton tabButton)
    {
        ResetTapButton();
        if(tabButton != _selectTab)
            tabButton.background.color = tabHover;
    }

    public void OnTabExit(TabButton tabButton)
    {
        ResetTapButton();
    }

    public void OnTabSelected(TabButton tabButton)
    {
        tabButton.background.color = tabSeleted;
        int idx = tabButton.transform.GetSiblingIndex();
        if(_selectTab != null)
        {
            int beforeIdx = _selectTab.transform.GetSiblingIndex();
            _tabActiveGo[beforeIdx].SetActive(false);
        }
        _tabActiveGo[idx].SetActive(true);
        _selectTab = tabButton;
        ResetTapButton();
    }

    void ResetTapButton()
    {
        foreach (var tabButton in _tabButtons)
        {
            // 正常不会使用Color会造成性能问题
            if(tabButton != _selectTab)
                tabButton.background.color = tabIdle;
        }
    }
}
