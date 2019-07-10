using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabControl_Panel : PoolObject
{
    public Transform Container;

    private List<PoolObject> ContainerChildren = new List<PoolObject>();

    [SerializeField] private Button AddNewButton;
    [SerializeField] private Text AddNewButtonText;

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(AddNewButtonText, "StoryEditorPanel_AddNewButtonText");
    }

    public void Initialize(UnityAction onAddButtonClick)
    {
        AddNewButton.onClick.RemoveAllListeners();
        AddNewButton.onClick.AddListener(onAddButtonClick);
    }

    public override void PoolRecycle()
    {
        foreach (PoolObject po in ContainerChildren)
        {
            po.PoolRecycle();
        }

        AddNewButton.onClick.RemoveAllListeners();
        ContainerChildren.Clear();
        base.PoolRecycle();
    }
}