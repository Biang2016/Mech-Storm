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