public class Player
{
    public Player(int costLeft, int costMax, int lifeLeft, int lifeMax, int magicLeft, int magicMax)
    {
        this.costMax = costMax;
        this.costLeft = costLeft;
        this.lifeLeft = lifeLeft;
        this.lifeMax = lifeMax;
        this.magicLeft = magicLeft;
        this.magicMax = magicMax;
        OnCostChanged();
        OnLifeChanged();
        OnMagicChanged();
    }

    private int costMax;

    public int CostMax
    {
        get { return costMax; }
    }

    private int costLeft;

    public int CostLeft
    {
        get { return costLeft; }
    }

    private int magicMax;

    public int MagicMax
    {
        get { return magicMax; }
    }

    private int magicLeft;

    public int MagicLeft
    {
        get { return magicLeft; }
    }

    private int lifeMax;

    public int LifeMax
    {
        get { return lifeMax; }
    }

    private int lifeLeft;

    public int LifeLeft
    {
        get { return lifeLeft; }
    }

    protected virtual void OnCostChanged()
    {
    }

    protected virtual void AddCost(int addCostNumber)
    {
        costLeft += addCostNumber;
        OnCostChanged();
    }

    protected virtual void AddCostMax(int addCostNumber)
    {
        costMax += addCostNumber;
        OnCostChanged();
    }

    protected virtual void OnLifeChanged()
    {
    }

    protected virtual void AddLife(int addLifeNumber)
    {
        lifeLeft += addLifeNumber;
        OnLifeChanged();
    }

    protected virtual void OnMagicChanged()
    {
    }

    protected virtual void AddMagic(int addMagicNumber)
    {
        magicLeft += addMagicNumber;
        OnMagicChanged();
    }
}