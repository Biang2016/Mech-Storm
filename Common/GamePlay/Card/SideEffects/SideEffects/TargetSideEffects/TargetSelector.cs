using System.Collections.Generic;

public class TargetSelector
{
    public static string GetDescOfTargetSelector(TargetRange targetRange, TargetSelect targetSelect, int choiceCount)
    {
        string textKey = "TargetRange_" + targetSelect + "_" + targetRange;
        string rawDesc = "";
        rawDesc = LanguageManager_Common.GetText(textKey);
        if (rawDesc != null && rawDesc.Contains("{0}"))
        {
            if (targetSelect == TargetSelect.Multiple || targetSelect == TargetSelect.MultipleRandom)
            {
                rawDesc = string.Format(rawDesc, choiceCount);
            }
        }

        return rawDesc;
    }

    public enum TargetSelectorTypes
    {
        MechBased,
        EveryMechBased,
        LifeBased,
        ShipBased,
        EquipBased,
    }

    public static Dictionary<TargetSelectorTypes, Dictionary<TargetSelect, List<TargetRange>>> TargetSelectorPresets =>
        new Dictionary<TargetSelectorTypes, Dictionary<TargetSelect, List<TargetRange>>>
        {
            {TargetSelectorTypes.MechBased, MechBasedSelector},
            {TargetSelectorTypes.EveryMechBased, EveryMechBasedSelector},
            {TargetSelectorTypes.LifeBased, LifeBasedSelector},
            {TargetSelectorTypes.ShipBased, ShipBasedSelector},
            {TargetSelectorTypes.EquipBased, EquipBasedSelector},
        };

    public static Dictionary<TargetSelect, List<TargetRange>> MechBasedSelector =>
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

    public static Dictionary<TargetSelect, List<TargetRange>> EveryMechBasedSelector =>
        new Dictionary<TargetSelect, List<TargetRange>>
        {
            {
                TargetSelect.Single, new List<TargetRange>
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

    public static Dictionary<TargetSelect, List<TargetRange>> EquipBasedSelector =>
        new Dictionary<TargetSelect, List<TargetRange>>
        {
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
        };
}