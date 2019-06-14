using System;

public class Player : ILife
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

    #region Metal

    protected virtual void OnMetalChanged(int change)
    {
    }

    protected virtual void OnMetalIncrease(int change)
    {
    }

    protected virtual void OnMetalReduce(int change)
    {
    }

    protected virtual void OnMetalUsed(int change)
    {
    }

    protected virtual void OnMaxMetalChanged(int change)
    {
    }

    protected virtual void OnMaxMetalIncrease(int change)
    {
    }

    protected virtual void OnMaxMetalReduce(int change)
    {
    }

    protected virtual void MetalChange(int change, bool trigger = true)
    {
        if (change > 0)
        {
            change = Math.Min(metalMax - metalLeft, change);
            metalLeft += change;
            if (trigger) OnMetalIncrease(change);
        }
        else
        {
            change = Math.Max(-metalLeft, change);
            metalLeft += change;
            if (trigger) OnMetalReduce(-change);
        }

        if (trigger) OnMetalChanged(change);
    }

    public void MetalMaxChange(int change, bool trigger = true)
    {
        if (change > 0)
        {
            metalMax += change;
            if (trigger) OnMaxMetalIncrease(change);
        }
        else
        {
            change = Math.Max(-metalLeft, change);
            metalMax += change;

            if (trigger) OnMaxMetalReduce(-change);
            if (metalMax < metalLeft)
            {
                int metalChange = metalLeft - metalMax;
                metalLeft = metalMax;
                if (trigger) OnMetalChanged(metalChange);
            }
        }

        if (trigger) OnMaxMetalChanged(change);
    }

    public void AddMetal(int addMetal)
    {
        if (addMetal <= 0) return;
        MetalChange(addMetal);
    }

    public void UseMetal(int useMetal)
    {
        if (useMetal <= 0) return;
        MetalChange(-useMetal);
        OnMetalUsed(useMetal);
    }

    public void ReduceMetal(int reduceMetal)
    {
        if (reduceMetal <= 0) return;
        MetalChange(-reduceMetal);
    }

    public void AddAllMetal()
    {
        MetalChange(metalMax - metalLeft);
    }

    public void UseAllMetal()
    {
        int useMetal = metalLeft;
        MetalChange(-metalLeft);
        OnMetalUsed(useMetal);
    }

    #endregion

    #region Life

    protected virtual void OnLifeChanged(int change, bool isOverflow)
    {
    }

    protected virtual void OnHeal(int change, bool isOverflow)
    {
    }

    protected virtual void OnDamage(int change)
    {
    }

    protected virtual void OnMaxLifeChanged(int change)
    {
    }

    protected virtual void OnMaxLifeIncrease(int change)
    {
    }

    protected virtual void OnMaxLifeReduce(int change)
    {
    }

    protected virtual void LifeChange(int change, bool trigger = true)
    {
        bool isOverflow = lifeLeft + change > lifeMax;
        if (change > 0)
        {
            change = Math.Min(lifeMax - lifeLeft, change);
            lifeLeft += change;
            if (trigger) OnHeal(change, isOverflow);
        }
        else
        {
            change = Math.Max(-lifeLeft, change);
            lifeLeft += change;
            if (trigger) OnDamage(-change);
        }

        if (trigger) OnLifeChanged(change, isOverflow);
    }

    protected virtual void MaxLifeChange(int change, bool trigger = true)
    {
        if (change > 0)
        {
            lifeMax += change;
            if (trigger) OnMaxLifeIncrease(change);
        }
        else
        {
            change = Math.Max(-lifeLeft, change);
            lifeMax += change;

            if (trigger) OnMaxLifeReduce(-change);
            if (lifeMax < lifeLeft)
            {
                int lifeChange = lifeLeft - lifeMax;
                lifeLeft = lifeMax;
                if (trigger) OnLifeChanged(lifeChange, false);
            }
        }

        if (trigger) OnMaxLifeChanged(change);
    }

    #region ILife

    public void AddLife(int addValue)
    {
        MaxLifeChange(addValue, false);
        LifeChange(addValue, false);
    }

    public void Heal(int healValue)
    {
        LifeChange(healValue);
    }

    public void Damage(int damage)
    {
        LifeChange(-damage);
    }

    public void Change(int changeValue)
    {
        LifeChange(changeValue);
    }

    public void HealAll()
    {
        LifeChange(lifeMax - lifeLeft);
    }

    public void ChangeMaxLife(int change)
    {
        MaxLifeChange(change);
    }

    #endregion

    #endregion

    #region Energy

    protected virtual void OnEnergyChanged(int change, bool isOverflow)
    {
    }

    protected virtual void OnEnergyIncrease(int change, bool isOverflow)
    {
    }

    protected virtual void OnEnergyReduce(int change)
    {
    }

    protected virtual void OnEnergyUsed(int change)
    {
    }

    protected virtual void OnMaxEnergyChanged(int change)
    {
    }

    protected virtual void OnMaxEnergyIncrease(int change)
    {
    }

    protected virtual void OnMaxEnergyReduce(int change)
    {
    }

    protected virtual bool EnergyChange(int change, bool trigger = true)
    {
        bool isOverflow = energyLeft + change > energyMax;
        if (change > 0)
        {
            change = Math.Min(energyMax - energyLeft, change);
            energyLeft += change;
            if (trigger) OnEnergyIncrease(change, isOverflow);
        }
        else
        {
            change = Math.Max(-energyLeft, change);
            energyLeft += change;
            if (trigger) OnEnergyReduce(-change);
        }

        if (trigger) OnEnergyChanged(change, isOverflow);
        return isOverflow;
    }

    protected virtual void EnergyMaxChange(int change, bool trigger = true)
    {
        if (change > 0)
        {
            energyMax += change;
            if (trigger) OnMaxEnergyIncrease(change);
        }
        else
        {
            change = Math.Max(-energyLeft, change);
            energyMax += change;

            if (trigger) OnMaxEnergyReduce(-change);
            if (energyMax < energyLeft)
            {
                int energyChange = energyLeft - energyMax;
                energyLeft = energyMax;
                if (trigger) OnEnergyChanged(energyChange, false);
            }
        }

        if (trigger) OnMaxEnergyChanged(change);
    }

    public void AddEnergy(int addEnergy)
    {
        if (addEnergy < 0) return;
        EnergyChange(addEnergy);
    }

    public void UseEnergy(int useEnergy)
    {
        if (useEnergy <= 0) return;

        EnergyChange(-useEnergy);
        OnEnergyUsed(useEnergy);
    }

    public void ReduceEnergy(int reduceEnergy)
    {
        if (reduceEnergy <= 0) return;
        EnergyChange(-reduceEnergy);
    }

    public void AddAllEnergy()
    {
        EnergyChange(energyMax - energyLeft);
    }

    public void UseAllEnergy()
    {
        int useEnergy = energyLeft;
        EnergyChange(-energyLeft);
        OnEnergyUsed(useEnergy);
    }

    #endregion
}