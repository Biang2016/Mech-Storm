using System;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Bonus : IClone<Bonus>
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BonusTypes
    {
        UnlockCardByID,
        UnlockCardByLevelNum,
        LifeUpperLimit,
        EnergyUpperLimit,
        Budget,
    }

    public BonusTypes BonusType;

    public virtual int PicID { get; set; } = (int) AllCards.EmptyCardTypes.EmptyCard;

    public Bonus(BonusTypes mBonusType)
    {
        BonusType = mBonusType;
    }

    public virtual string GetDesc()
    {
        return "";
    }

    protected static Dictionary<string, Dictionary<BonusTypes, string>> BonusDescRaw = new Dictionary<string, Dictionary<BonusTypes, string>>
    {
        {
            "zh", new Dictionary<BonusTypes, string>
            {
                {BonusTypes.UnlockCardByID, "解锁卡片[{0}]"},
                {BonusTypes.UnlockCardByLevelNum, "解锁一张稀有度为{0}的卡片"},
                {BonusTypes.LifeUpperLimit, "生命上限{0}"},
                {BonusTypes.EnergyUpperLimit, "能量上限{0}"},
                {BonusTypes.Budget, "预算{0}"},
            }
        },
        {
            "en", new Dictionary<BonusTypes, string>
            {
                {BonusTypes.UnlockCardByID, "Unlock card [{0}]"},
                {BonusTypes.UnlockCardByLevelNum, "Unlock card of rare [{0}]"},
                {BonusTypes.LifeUpperLimit, "Life {0}"},
                {BonusTypes.EnergyUpperLimit, "Energy {0}"},
                {BonusTypes.Budget, "Budget {0}"},
            }
        }
    };

    public virtual Bonus Clone()
    {
        return new Bonus(BonusType);
    }

    public virtual void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) BonusType);
    }

    public static Bonus Deserialize(DataStream reader)
    {
        BonusTypes type = (BonusTypes) reader.ReadSInt32();
        Bonus bonus = null;
        switch (type)
        {
            case BonusTypes.UnlockCardByID:
            {
                int cardID = reader.ReadSInt32();
                bonus = new Bonus_UnlockCardByID(cardID);
                break;
            }
            case BonusTypes.UnlockCardByLevelNum:
            {
                int levelNum = reader.ReadSInt32();
                bonus = new Bonus_UnlockCardByLevelNum(levelNum);
                break;
            }
            case BonusTypes.LifeUpperLimit:
            {
                int lifeUpperLimit = reader.ReadSInt32();
                bonus = new Bonus_LifeUpperLimit(lifeUpperLimit);
                break;
            }
            case BonusTypes.EnergyUpperLimit:
            {
                int energyUpperLimit = reader.ReadSInt32();
                bonus = new Bonus_EnergyUpperLimit(energyUpperLimit);
                break;
            }
            case BonusTypes.Budget:
            {
                int budget = reader.ReadSInt32();
                bonus = new Bonus_Budget(budget);
                break;
            }
        }

        return bonus;
    }

    public void ExportToXML(XmlElement parent_ele)
    {
        XmlDocument doc = parent_ele.OwnerDocument;
        XmlElement bonus_ele = doc.CreateElement("BonusInfo");
        parent_ele.AppendChild(bonus_ele);

        bonus_ele.SetAttribute("bonusType", BonusType.ToString());
        ChildrenExportToXML(bonus_ele);
    }

    protected virtual void ChildrenExportToXML(XmlElement my_ele)
    {
    }

    public static Bonus GenerateBonusFromXML(XmlNode node_Bonus, out bool needRefresh)
    {
        needRefresh = false;
        BonusTypes type = (BonusTypes) Enum.Parse(typeof(BonusTypes), node_Bonus.Attributes["bonusType"].Value);

        switch (type)
        {
            case BonusTypes.UnlockCardByID:
            {
                int cardID = int.Parse(node_Bonus.Attributes["cardID"].Value);
                if (!AllCards.CardDict.ContainsKey(cardID))
                {
                    needRefresh = true;
                    return null;
                }

                return new Bonus_UnlockCardByID(cardID);
            }
            case BonusTypes.UnlockCardByLevelNum:
            {
                int levelNum = int.Parse(node_Bonus.Attributes["levelNum"].Value);
                return new Bonus_UnlockCardByLevelNum(levelNum);
            }
            case BonusTypes.LifeUpperLimit:
            {
                int lifeUpperLimit = int.Parse(node_Bonus.Attributes["lifeUpperLimit"].Value);
                return new Bonus_LifeUpperLimit(lifeUpperLimit);
            }
            case BonusTypes.EnergyUpperLimit:
            {
                int energyUpperLimit = int.Parse(node_Bonus.Attributes["energyUpperLimit"].Value);
                return new Bonus_EnergyUpperLimit(energyUpperLimit);
            }
            case BonusTypes.Budget:
            {
                int budget = int.Parse(node_Bonus.Attributes["budget"].Value);
                return new Bonus_Budget(budget);
            }
        }

        return null;
    }
}