using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AffixManager : MonoSingletion<AffixManager>
{
    [SerializeField] private Transform AffixPanel;
    [SerializeField] private VerticalLayoutGroup VerticalLayoutGroup;
    [SerializeField] private Animator AffixPanelAnim;

    private List<Affix> Affixs = new List<Affix>();
    private HashSet<AffixType> AffixTypes = new HashSet<AffixType>();

    void Start()
    {
    }

    public void ShowAffixPanel(List<AffixType> affixTypes)
    {
        ClearAllAffixs();
        AddAffixs(affixTypes);
        AffixPanelAnim.SetTrigger("Show");
    }

    public void HideAffixPanel()
    {
        AffixPanelAnim.SetTrigger("Hide");
    }

    private void AddAffixs(List<AffixType> affixTypes)
    {
        foreach (AffixType affixType in affixTypes)
        {
            AddAffix(affixType);
        }
    }

    private void AddAffix(AffixType affixType)
    {
        if (!AffixTypes.Contains(affixType))
        {
            AffixTypes.Add(affixType);
            Affix newAffix = GameObjectPoolManager.Instance.Pool_AffixPool.AllocateGameObject<Affix>(AffixPanel);
            newAffix.Initialize(affixType);
            Affixs.Add(newAffix);
        }
    }

    private void RemoveAffix(AffixType affixType)
    {
        if (AffixTypes.Contains(affixType))
        {
            AffixTypes.Remove(affixType);
            foreach (Affix affix in Affixs)
            {
                if (affix.AffixType == affixType)
                {
                    Affixs.Remove(affix);
                    affix.PoolRecycle();
                }
            }
        }
    }

    public void ClearAllAffixs()
    {
        AffixTypes.Clear();
        foreach (Affix affix in Affixs)
        {
            affix.PoolRecycle();
        }

        Affixs.Clear();
    }

}