using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Bonus_BudgetLifeEnergyMixed : Bonus
{
    public int TotalValue;

    public override int PicID { get; set; } = (int) AllCards.SpecialPicIDs.Skills;

    public Bonus_BudgetLifeEnergyMixed(int totalValue) : base(BonusTypes.BudgetLifeEnergyMixed)
    {
        TotalValue = totalValue;
    }

    public override string GetDesc()
    {
        Dictionary<BonusTypes, string> dic = BonusDescRaw[LanguageManager_Common.GetCurrentLanguage()];
        return Utils.HighlightStringFormat(dic[BonusType], AllColors.ColorDict[AllColors.ColorType.CardHighLightColor], TotalValue);
    }

    protected override void ChildrenExportToXML(XmlElement my_ele)
    {
        base.ChildrenExportToXML(my_ele);
        my_ele.SetAttribute("value", TotalValue.ToString());
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(TotalValue);
    }

    public override Bonus Clone()
    {
        return new Bonus_BudgetLifeEnergyMixed(TotalValue);
    }

    public static BudgetLifeEnergyComb GetBonusFromMixedBonus(int value, int seed)
    {
        Random.InitState(seed);

        int minValue = Mathf.CeilToInt(0.8f * value / 25);
        int maxValue = Mathf.CeilToInt(1.5f * value / 25);
        int curCount_25 = Random.Range(minValue, maxValue + 1);

        int lifeEnergyTotal = Random.Range(0, curCount_25 / 2 + 1);
        int budget = (curCount_25 - lifeEnergyTotal) * 25;
        int life = Random.Range(0, lifeEnergyTotal + 1);
        int energy = lifeEnergyTotal - life;

        if (budget == 0 && life == 0 && energy == 0)
        {
            return null;
        }

        BudgetLifeEnergyComb BudgetLifeEnergyComb = new BudgetLifeEnergyComb(budget, life, energy);
        return BudgetLifeEnergyComb;
    }

    public BudgetLifeEnergyComb GetBudgetLifeEnergyComb(List<BudgetLifeEnergyComb> exceptionBudgetLifeEnergyComb)
    {
        BudgetLifeEnergyComb comb = null;
        bool needRetry = true;
        int retryTime = 100;
        while (needRetry && retryTime > 0)
        {
            comb = GetBonusFromMixedBonus(TotalValue, retryTime);
            if (comb == null)
            {
                needRetry = true;
            }
            else
            {
                needRetry = false;
                foreach (BudgetLifeEnergyComb c in exceptionBudgetLifeEnergyComb)
                {
                    if (c.HaveSameMeaningTo(comb))
                    {
                        needRetry = true;
                    }
                }

                if (needRetry)
                {
                    comb = null;
                }
            }

            retryTime--;
        }

        return comb;
    }

    public List<Bonus> GetBonusListFromBonusComb(BudgetLifeEnergyComb comb)
    {
        List<Bonus> res = new List<Bonus>();
        if (comb.Budget != 0)
        {
            Bonus_Budget bb = new Bonus_Budget(comb.Budget);
            res.Add(bb);
        }

        if (comb.LifeUpperLimit != 0)
        {
            Bonus_LifeUpperLimit bl = new Bonus_LifeUpperLimit(comb.LifeUpperLimit);
            res.Add(bl);
        }

        if (comb.EnergyUpperLimit != 0)
        {
            Bonus_EnergyUpperLimit be = new Bonus_EnergyUpperLimit(comb.EnergyUpperLimit);
            res.Add(be);
        }

        return res;
    }

    public class BudgetLifeEnergyComb
    {
        public int Budget;
        public int LifeUpperLimit;
        public int EnergyUpperLimit;

        public BudgetLifeEnergyComb(int budget, int lifeUpperLimit, int energyUpperLimit)
        {
            Budget = budget;
            LifeUpperLimit = lifeUpperLimit;
            EnergyUpperLimit = energyUpperLimit;
        }

        public bool HaveSameMeaningTo(BudgetLifeEnergyComb o)
        {
            if (Budget <= o.Budget && LifeUpperLimit <= o.LifeUpperLimit && EnergyUpperLimit <= o.EnergyUpperLimit)
            {
                return true;
            }

            if (Budget >= o.Budget && LifeUpperLimit >= o.LifeUpperLimit && EnergyUpperLimit >= o.EnergyUpperLimit)
            {
                return true;
            }

            return Budget == o.Budget && LifeUpperLimit == o.LifeUpperLimit && EnergyUpperLimit == o.EnergyUpperLimit;
        }

        public List<Bonus> GenerateBonuses()
        {
            List<Bonus> res = new List<Bonus>();
            if (Budget > 0)
            {
                res.Add(new Bonus_Budget(Budget));
            }

            if (LifeUpperLimit > 0)
            {
                res.Add(new Bonus_LifeUpperLimit(LifeUpperLimit));
            }

            if (EnergyUpperLimit > 0)
            {
                res.Add(new Bonus_EnergyUpperLimit(EnergyUpperLimit));
            }

            return res;
        }
    }
}