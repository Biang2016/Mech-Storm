using System.Collections.Generic;

public class TargetSelector
{
    public static string GetDescOfTargetSelector(TargetRange targetRange, TargetSelect targetSelect)
    {
        string textKey = "TargetRange_" + targetSelect + "_" + targetRange;
        return LanguageManager_Common.GetText(textKey);
    }

    public enum TargetSelectorTypes
    {
        RetinueBased,
        LifeBased,
        ShipBased,
    }

    public static Dictionary<TargetSelectorTypes, Dictionary<TargetSelect, List<TargetRange>>> TargetSelectorPresets =>
        new Dictionary<TargetSelectorTypes, Dictionary<TargetSelect, List<TargetRange>>>
        {
            {TargetSelectorTypes.RetinueBased, RetinueBasedSelector},
            {TargetSelectorTypes.LifeBased, LifeBasedSelector},
            {TargetSelectorTypes.ShipBased, ShipBasedSelector},
        };

    public static Dictionary<TargetSelect, List<TargetRange>> RetinueBasedSelector =>
        new Dictionary<TargetSelect, List<TargetRange>>
        {
            {
                TargetSelect.All, new List<TargetRange>
                {
                    TargetRange.Heroes,
                    TargetRange.Soldiers,
                    TargetRange.Mechs,
                    TargetRange.SelfHeroes,
                    TargetRange.SelfSoldiers,
                    TargetRange.SelfMechs,
                    TargetRange.EnemyHeroes,
                    TargetRange.EnemySoldiers,
                    TargetRange.EnemyMechs,
                }
            },
            {
                TargetSelect.Multiple, new List<TargetRange>
                {
                    TargetRange.Heroes,
                    TargetRange.Soldiers,
                    TargetRange.Mechs,
                    TargetRange.SelfHeroes,
                    TargetRange.SelfSoldiers,
                    TargetRange.SelfMechs,
                    TargetRange.EnemyHeroes,
                    TargetRange.EnemySoldiers,
                    TargetRange.EnemyMechs,
                }
            },
            {
                TargetSelect.MultipleRandom, new List<TargetRange>
                {
                    TargetRange.Heroes,
                    TargetRange.Soldiers,
                    TargetRange.Mechs,
                    TargetRange.SelfHeroes,
                    TargetRange.SelfSoldiers,
                    TargetRange.SelfMechs,
                    TargetRange.EnemyHeroes,
                    TargetRange.EnemySoldiers,
                    TargetRange.EnemyMechs,
                }
            },
            {
                TargetSelect.Single, new List<TargetRange>
                {
                    TargetRange.Self,
                    TargetRange.Heroes,
                    TargetRange.Soldiers,
                    TargetRange.Mechs,
                    TargetRange.SelfHeroes,
                    TargetRange.SelfSoldiers,
                    TargetRange.SelfMechs,
                    TargetRange.EnemyHeroes,
                    TargetRange.EnemySoldiers,
                    TargetRange.EnemyMechs,
                }
            },
            {
                TargetSelect.SingleRandom, new List<TargetRange>
                {
                    TargetRange.Heroes,
                    TargetRange.Soldiers,
                    TargetRange.Mechs,
                    TargetRange.SelfHeroes,
                    TargetRange.SelfSoldiers,
                    TargetRange.SelfMechs,
                    TargetRange.EnemyHeroes,
                    TargetRange.EnemySoldiers,
                    TargetRange.EnemyMechs,
                }
            },
        };

    public static Dictionary<TargetSelect, List<TargetRange>> LifeBasedSelector =>
        new Dictionary<TargetSelect, List<TargetRange>>
        {
            {
                TargetSelect.All, new List<TargetRange>
                {
                    TargetRange.Heroes,
                    TargetRange.Soldiers,
                    TargetRange.Mechs,
                    TargetRange.SelfHeroes,
                    TargetRange.SelfSoldiers,
                    TargetRange.SelfMechs,
                    TargetRange.EnemyHeroes,
                    TargetRange.EnemySoldiers,
                    TargetRange.EnemyMechs,
                    TargetRange.AllLife,
                    TargetRange.SelfLife,
                    TargetRange.EnemyLife,
                    TargetRange.Ships,
                }
            },
            {
                TargetSelect.Multiple, new List<TargetRange>
                {
                    TargetRange.Heroes,
                    TargetRange.Soldiers,
                    TargetRange.Mechs,
                    TargetRange.SelfHeroes,
                    TargetRange.SelfSoldiers,
                    TargetRange.SelfMechs,
                    TargetRange.EnemyHeroes,
                    TargetRange.EnemySoldiers,
                    TargetRange.EnemyMechs,
                    TargetRange.AllLife,
                    TargetRange.SelfLife,
                    TargetRange.EnemyLife,
                    TargetRange.Ships,
                }
            },
            {
                TargetSelect.MultipleRandom, new List<TargetRange>
                {
                    TargetRange.Heroes,
                    TargetRange.Soldiers,
                    TargetRange.Mechs,
                    TargetRange.SelfHeroes,
                    TargetRange.SelfSoldiers,
                    TargetRange.SelfMechs,
                    TargetRange.EnemyHeroes,
                    TargetRange.EnemySoldiers,
                    TargetRange.EnemyMechs,
                    TargetRange.AllLife,
                    TargetRange.SelfLife,
                    TargetRange.EnemyLife,
                    TargetRange.Ships,
                }
            },
            {
                TargetSelect.Single, new List<TargetRange>
                {
                    TargetRange.Self,
                    TargetRange.Heroes,
                    TargetRange.Soldiers,
                    TargetRange.Mechs,
                    TargetRange.SelfHeroes,
                    TargetRange.SelfSoldiers,
                    TargetRange.SelfMechs,
                    TargetRange.EnemyHeroes,
                    TargetRange.EnemySoldiers,
                    TargetRange.EnemyMechs,
                    TargetRange.AllLife,
                    TargetRange.SelfLife,
                    TargetRange.EnemyLife,
                    TargetRange.Ships,
                    TargetRange.EnemyShip,
                    TargetRange.SelfShip,
                }
            },
            {
                TargetSelect.SingleRandom, new List<TargetRange>
                {
                    TargetRange.Heroes,
                    TargetRange.Soldiers,
                    TargetRange.Mechs,
                    TargetRange.SelfHeroes,
                    TargetRange.SelfSoldiers,
                    TargetRange.SelfMechs,
                    TargetRange.EnemyHeroes,
                    TargetRange.EnemySoldiers,
                    TargetRange.EnemyMechs,
                    TargetRange.AllLife,
                    TargetRange.SelfLife,
                    TargetRange.EnemyLife,
                    TargetRange.Ships,
                }
            },
        };

    public static Dictionary<TargetSelect, List<TargetRange>> ShipBasedSelector =>
        new Dictionary<TargetSelect, List<TargetRange>>
        {
            {
                TargetSelect.All, new List<TargetRange>
                {
                    TargetRange.Ships,
                }
            },
            {
                TargetSelect.Multiple, new List<TargetRange>
                {
                    TargetRange.Ships,
                }
            },
            {
                TargetSelect.MultipleRandom, new List<TargetRange>
                {
                    TargetRange.Ships,
                }
            },
            {
                TargetSelect.Single, new List<TargetRange>
                {
                    TargetRange.Ships,
                    TargetRange.EnemyShip,
                    TargetRange.SelfShip,
                }
            },
            {
                TargetSelect.SingleRandom, new List<TargetRange>
                {
                    TargetRange.Ships,
                }
            },
        };
}