using System.Xml;

public class GamePlaySettings
{
    public static GamePlaySettings OnlineGamePlaySettings = new GamePlaySettings(
        defaultCoin: 15000,
        defaultLife: 100,
        defaultLifeMax: 200,
        defaultLifeMin: 50,
        defaultEnergy: 10,
        defaultEnergyMax: 50,
        defaultDrawCardNum: 2,
        minDrawCardNum: 1,
        maxDrawCardNum: 5
    );

    public static GamePlaySettings ServerGamePlaySettings = new GamePlaySettings(
        defaultCoin: 30000,
        defaultLife: 100,
        defaultLifeMax: 999,
        defaultLifeMin: 10,
        defaultEnergy: 10,
        defaultEnergyMax: 50,
        defaultDrawCardNum: 2,
        minDrawCardNum: 1,
        maxDrawCardNum: 5
    );

    public static int SystemMinLife = 1;
    public static int SystemMaxLife = 10000;
    public static int SystemMaxEnergy = 200;
    public static int SystemMaxMetal = 10;
    public static int SystemMaxDrawCardNum = 5;
    public static int SystemMinDrawCardNum = 1;

    public static int MaxHandCard = 30;
    public static int FirstDrawCard = 3;
    public static int SecondDrawCard = 3;
    public static int MaxMechNumber = 6;

    public static int MaxHeroNumber = 4;

    public static int MaxMetal = 10;
    public static int MetalIncrease = 1;

    public static int LifeToCoin = 50;
    public static int EnergyToCoin = 50;
    public static int[] DrawCardNumToCoin = new[] {0, 10000, 15000, 18000, 23000, 26000};

    public int DefaultCoin = 1500;
    public int DefaultLife = 100;
    public int DefaultLifeMax = 1000;
    public int DefaultLifeMin = 20;
    public int DefaultEnergy = 10;
    public int DefaultEnergyMax = 50;

    public int DefaultDrawCardNum = 2;
    public int MinDrawCardNum = 1;
    public int MaxDrawCardNum = 5;

    public GamePlaySettings()
    {
    }

    public GamePlaySettings(int defaultCoin, int defaultLife, int defaultLifeMax, int defaultLifeMin, int defaultEnergy, int defaultEnergyMax, int defaultDrawCardNum, int minDrawCardNum, int maxDrawCardNum)
    {
        DefaultCoin = defaultCoin;
        DefaultLife = defaultLife;
        DefaultLifeMax = defaultLifeMax;
        DefaultLifeMin = defaultLifeMin;
        DefaultEnergy = defaultEnergy;
        DefaultEnergyMax = defaultEnergyMax;
        DefaultDrawCardNum = defaultDrawCardNum;
        MinDrawCardNum = minDrawCardNum;
        MaxDrawCardNum = maxDrawCardNum;
    }

    public int DefaultMaxCoin
    {
        get { return DefaultCoin + (DefaultLife - DefaultLifeMin) * LifeToCoin + DefaultEnergy * EnergyToCoin + DrawCardNumToCoin[DefaultDrawCardNum] - DrawCardNumToCoin[MinDrawCardNum]; }
    }

    public static GamePlaySettings GetGamePlaySettingsFromXML(XmlNode node_gps)
    {
        int DefaultCoin = int.Parse(node_gps.Attributes["DefaultCoin"].Value);
        int DefaultLife = int.Parse(node_gps.Attributes["DefaultLife"].Value);
        int DefaultLifeMax = int.Parse(node_gps.Attributes["DefaultLifeMax"].Value);
        int DefaultLifeMin = int.Parse(node_gps.Attributes["DefaultLifeMin"].Value);
        int DefaultEnergy = int.Parse(node_gps.Attributes["DefaultEnergy"].Value);
        int DefaultEnergyMax = int.Parse(node_gps.Attributes["DefaultEnergyMax"].Value);

        int DefaultDrawCardNum = int.Parse(node_gps.Attributes["DefaultDrawCardNum"].Value);
        int MinDrawCardNum = int.Parse(node_gps.Attributes["MinDrawCardNum"].Value);
        int MaxDrawCardNum = int.Parse(node_gps.Attributes["MaxDrawCardNum"].Value);

        GamePlaySettings gps = new GamePlaySettings();
        gps.DefaultCoin = DefaultCoin;
        gps.DefaultLife = DefaultLife;
        gps.DefaultLifeMax = DefaultLifeMax;
        gps.DefaultLifeMin = DefaultLifeMin;
        gps.DefaultEnergy = DefaultEnergy;
        gps.DefaultEnergyMax = DefaultEnergyMax;

        gps.DefaultDrawCardNum = DefaultDrawCardNum;
        gps.MinDrawCardNum = MinDrawCardNum;
        gps.MaxDrawCardNum = MaxDrawCardNum;
        return gps;
    }

    public void ExportToXML(XmlElement parent_ele)
    {
        XmlDocument doc = parent_ele.OwnerDocument;
        XmlElement gps_ele = doc.CreateElement("GamePlaySettings");
        parent_ele.AppendChild(gps_ele);

        gps_ele.SetAttribute("DefaultCoin", DefaultCoin.ToString());
        gps_ele.SetAttribute("DefaultDrawCardNum", DefaultDrawCardNum.ToString());
        gps_ele.SetAttribute("DefaultEnergy", DefaultEnergy.ToString());
        gps_ele.SetAttribute("DefaultEnergyMax", DefaultEnergyMax.ToString());
        gps_ele.SetAttribute("DefaultLife", DefaultLife.ToString());
        gps_ele.SetAttribute("DefaultLifeMax", DefaultLifeMax.ToString());
        gps_ele.SetAttribute("DefaultLifeMin", DefaultLifeMin.ToString());
        gps_ele.SetAttribute("MaxDrawCardNum", MaxDrawCardNum.ToString());
        gps_ele.SetAttribute("MinDrawCardNum", MinDrawCardNum.ToString());
    }

    public GamePlaySettings Clone()
    {
        GamePlaySettings gps = new GamePlaySettings(
            defaultCoin: DefaultCoin,
            defaultLife: DefaultLife,
            defaultLifeMax: DefaultLifeMax,
            defaultLifeMin: DefaultLifeMin,
            defaultEnergy: DefaultEnergy,
            defaultEnergyMax: DefaultEnergyMax,
            defaultDrawCardNum: DefaultDrawCardNum,
            minDrawCardNum: MinDrawCardNum,
            maxDrawCardNum: MaxDrawCardNum
        );
        return gps;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(DefaultCoin);
        writer.WriteSInt32(DefaultLife);
        writer.WriteSInt32(DefaultLifeMax);
        writer.WriteSInt32(DefaultLifeMin);
        writer.WriteSInt32(DefaultEnergy);
        writer.WriteSInt32(DefaultEnergyMax);

        writer.WriteSInt32(DefaultDrawCardNum);
        writer.WriteSInt32(MinDrawCardNum);
        writer.WriteSInt32(MaxDrawCardNum);
    }

    public static GamePlaySettings Deserialize(DataStream reader)
    {
        GamePlaySettings res = new GamePlaySettings();
        res.DefaultCoin = reader.ReadSInt32();
        res.DefaultLife = reader.ReadSInt32();
        res.DefaultLifeMax = reader.ReadSInt32();
        res.DefaultLifeMin = reader.ReadSInt32();
        res.DefaultEnergy = reader.ReadSInt32();
        res.DefaultEnergyMax = reader.ReadSInt32();

        res.DefaultDrawCardNum = reader.ReadSInt32();
        res.MinDrawCardNum = reader.ReadSInt32();
        res.MaxDrawCardNum = reader.ReadSInt32();
        return res;
    }
}