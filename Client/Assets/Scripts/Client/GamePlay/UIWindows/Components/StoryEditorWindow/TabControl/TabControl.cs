using System.Collections.Generic;
using UnityEngine;

public class TabControl : MonoBehaviour
{
    [SerializeField] private Transform TabsContainer;
    [SerializeField] private Transform PanelsContainer;

    private Dictionary<string, TabControl_Panel> PanelsDict = new Dictionary<string, TabControl_Panel>();
    private Dictionary<string, TabControl_TabButton> TabsDict = new Dictionary<string, TabControl_TabButton>();

    public void Reset()
    {
        foreach (KeyValuePair<string, TabControl_Panel> kv in PanelsDict)
        {
            kv.Value.PoolRecycle();
        }

        PanelsDict.Clear();

        foreach (KeyValuePair<string, TabControl_TabButton> kv in TabsDict)
        {
            kv.Value.PoolRecycle();
        }

        TabsDict.Clear();
    }

    public void Initialize()
    {
        Reset();
    }

    private string CurSelectedTabButtonStrKey = "";

    public Transform AddTab(string tabTitleStrKey)
    {
        if (PanelsDict.ContainsKey(tabTitleStrKey))
        {
            return PanelsDict[tabTitleStrKey].Container;
        }

        TabControl_TabButton btn = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.TabControl_TabButton].AllocateGameObject<TabControl_TabButton>(TabsContainer);
        btn.Initialize(tabTitleStrKey, delegate { SelectTab(tabTitleStrKey); });
        TabsDict.Add(tabTitleStrKey, btn);
        TabControl_Panel panel = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.TabControl_Panel].AllocateGameObject<TabControl_Panel>(PanelsContainer);
        PanelsDict.Add(tabTitleStrKey, panel);
        return panel.Container;
    }

    public Transform GetPanelTransform(string tabTitleStrKey)
    {
        PanelsDict.TryGetValue(tabTitleStrKey, out TabControl_Panel panel);
        if (panel) return panel.Container;
        return null;
    }

    public void SelectTab(string tabTitleStrKey)
    {
        if (CurSelectedTabButtonStrKey != tabTitleStrKey)
        {
            foreach (KeyValuePair<string, TabControl_Panel> kv in PanelsDict)
            {
                kv.Value.gameObject.SetActive(false);
            }

            PanelsDict[tabTitleStrKey].gameObject.SetActive(true);

            foreach (KeyValuePair<string, TabControl_TabButton> kv in TabsDict)
            {
                kv.Value.IsSelected = false;
            }

            TabsDict[tabTitleStrKey].IsSelected = true;

            CurSelectedTabButtonStrKey = tabTitleStrKey;
        }
    }
}