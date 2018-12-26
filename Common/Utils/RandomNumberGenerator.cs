public class RandomNumberGenerator
{
    public RandomNumberGenerator(int seed)
    {
        InitSeed = seed;
        Seed = seed;
    }

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


    int pre;

    int rand()
    {
        int ret = (Seed * 7361238 + Seed % 20037 * 1244 + pre * 12342 + 378211) * (Seed + 134543);
        pre = Seed;
        Seed = ret;
        return ret;
    }

    public int Range(int low, int high)
    {
        if (low > high) return 0;
        int len = high - low;
        int rand = this.rand();
        if (rand < 0) rand = -rand;
        int res = rand % len + low;
        return res;
    }
}