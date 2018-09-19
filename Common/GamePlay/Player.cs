public class Player
{
    public Player(string username, int metalLeft, int metalMax, int lifeLeft, int lifeMax, int energyLeft, int energyMax)
    {
        this.Username = username;
        this.metalMax = metalMax;
        this.metalLeft = metalLeft;
        this.lifeLeft = lifeLeft;
        this.lifeMax = lifeMax;
        this.energyLeft = energyLeft;
        this.energyMax = energyMax;
    }

    public string Username;

    private int metalMax;

    public int MetalMax
    {
        get { return metalMax; }
    }

    private int metalLeft;

    public int MetalLeft
    {
        get { return metalLeft; }
    }

    private int energyMax;

    public int EnergyMax
    {
        get { return energyMax; }
    }

    private int energyLeft;

    public int EnergyLeft
    {
        get { return energyLeft; }
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

    protected virtual void OnMetalChanged(int change)
    {
    }

    protected virtual void AddMetal(int addMetalNumber)
    {
        metalLeft += addMetalNumber;
        OnMetalChanged(addMetalNumber);
    }

    protected virtual void AddMetalMax(int addMetalNumber)
    {
        metalMax += addMetalNumber;
        OnMetalChanged(addMetalNumber);
    }

    protected virtual void OnLifeChanged(int change)
    {
    }

    protected virtual void AddLife(int addLifeNumber)
    {
        lifeLeft += addLifeNumber;
        OnLifeChanged(addLifeNumber);
    }

    protected virtual void OnEnergyChanged(int change)
    {
    }

    protected virtual void AddEnergy(int addEnergyNumber)
    {
        energyLeft += addEnergyNumber;
        OnEnergyChanged(addEnergyNumber);
    }
}