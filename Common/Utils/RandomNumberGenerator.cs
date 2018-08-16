using System;

public class RandomNumberGenerator
{
    public RandomNumberGenerator(int seed)
    {
        Seed = seed;
        SyncRandom = new Random(Seed);
    }

    public Random SyncRandom;

    public int UseTime = 0;
    public int Seed = 0;

    public Random UseRandom()
    {
        UseTime++;
        return SyncRandom;
    }
}