using System;

public class RandomNumberGenerator
{
    public RandomNumberGenerator(int seed)
    {
        InitSeed = seed;
        Seed = seed;
    }

    private int UseTime = 0;
    private int InitSeed = 0;
    private int Seed = 0;

    public int GetInitSeed()
    {
        return InitSeed;
    }

    public int GetSeed()
    {
        return Seed;
    }

    public int Range(int down,int up)
    {
        UseTime++;
        Seed = a * Seed % m;
        int res = Seed % (up - down) + down;
        return res;
    }

    private int a = 7;
    private int m = 2147483647;
}