public class GamePlaySettings
{
    public static int MaxHandCard = 30;
    public static int DrawCardPerRound = 2;
    public static int FirstDrawCard = 0;
    public static int SecondDrawCard = 0;
    public static int MaxRetinueNumber = 6;

    public static int MaxHeroNumber = 4;

    public static int BeginMetal = 0;
    public static int MaxMetal = 10;
    public static int MetalIncrease = 1;

    public static bool SuffleBuild = true;

    public static int ServerDefaultCoin = 30000;
    public static int ServerDefaultLife = 999;
    public static int ServerDefaultLifeMax = 999;
    public static int ServerDefaultLifeMin = 50;
    public static int ServerDefaultEnergy = 10;
    public static int ServerDefaultEnergyMax = 50;

    public static int ServerDefaultDrawCardNum = 2;
    public static int ServerMinDrawCardNum = 1;
    public static int ServerMaxDrawCardNum = 5;

    public static int PlayerDefaultCoin = 15000;
    public static int PlayerDefaultLife = 100;
    public static int PlayerDefaultLifeMax = 200;
    public static int PlayerDefaultLifeMin = 50;
    public static int PlayerDefaultEnergy = 10;
    public static int PlayerDefaultEnergyMax = 50;

    public static int PlayerDefaultDrawCardNum = 2;
    public static int PlayerMinDrawCardNum = 1;
    public static int PlayerMaxDrawCardNum = 5;

    public static int LifeToCoin = 50;
    public static int EnergyToCoin = 50;
    public static int[] DrawCardNumToCoin = new[] {0, 10000, 15000, 18000, 23000, 26000};

    public static int PlayerDefaultMaxCoin
    {
        get { return PlayerDefaultCoin + (PlayerDefaultLife - PlayerDefaultLifeMin) * LifeToCoin + PlayerDefaultEnergy * EnergyToCoin + DrawCardNumToCoin[PlayerDefaultDrawCardNum] - DrawCardNumToCoin[PlayerMinDrawCardNum]; }
    }

    public static int ServerDefaultMaxCoin
    {
        get { return ServerDefaultCoin + (ServerDefaultLife - ServerDefaultLifeMin) * LifeToCoin + ServerDefaultEnergy * EnergyToCoin + DrawCardNumToCoin[ServerDefaultDrawCardNum] - DrawCardNumToCoin[ServerMinDrawCardNum]; }
    }
}