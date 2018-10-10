public class GamePlaySettings
{
    public static int MaxHandCard = 30;
    public static int DrawCardPerRound = 2;
    public static int FirstDrawCard = 5;
    public static int SecondDrawCard = 6;
    public static int MaxRetinueNumber = 6;

    public static int MaxHeroNumber = 3;

    public static int BeginMetal = 1;
    public static int MaxMetal = 10;
    public static int MetalIncrease = 1;

    public static bool SuffleBuild = true;

    public static int PlayerDefaultCoin = 15000;
    public static int PlayerDefaultLife = 100;
    public static int PlayerDefaultLifeMax = 200;
    public static int PlayerDefaultLifeMin = 50;
    public static int PlayerDefaultEnergy = 10;
    public static int PlayerDefaultEnergyMax = 50;

    public static int LifeToCoin = 50;
    public static int EnergyToCoin = 50;

    public static int[] DrawCardNumToCoin = new[] {0, 10000, 15000, 18000, 23000, 26000};
    public static int PlayerDefaultDrawCardNum = 2;
    public static int PlayerMinDrawCardNum = 1;
    public static int PlayerMaxDrawCardNum = 5;

    public static int PlayerDefaultMaxCoin
    {
        get { return PlayerDefaultCoin + (PlayerDefaultLife - PlayerDefaultLifeMin) * LifeToCoin + PlayerDefaultEnergy * EnergyToCoin + DrawCardNumToCoin[PlayerDefaultDrawCardNum] - DrawCardNumToCoin[PlayerMinDrawCardNum]; }
    }

    public static string SoldierCardColor ="#404040";
    public static string HeroCardColor = "#00F1FF";
    public static string EnergyCardColor = "#3686FF";
    public static string SpellCardColor = "#78FF4E";
    public static string WeaponCardColor = "#FF0000";
    public static string ShieldCardColor = "#FFE325";
    public static string PackCardColor = "#0049BC";
    public static string MACardColor = "#7F8AFF";
    public static string CardHightLightColor = "#FFFF00";
    public static string CardImportantColor = "#FF9732";
}