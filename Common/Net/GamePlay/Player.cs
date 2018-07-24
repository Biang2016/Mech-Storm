using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Player
{
    public Player(int costLeft, int costMax)
    {
        this.costMax = costMax;
        this.costLeft = costLeft;
        OnCostChanged();
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
}