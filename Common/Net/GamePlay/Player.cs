using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Player
{
    public Player(int costMax, int costLeft)
    {
        CostMax = costMax;
        CostLeft = costLeft;
    }

    private int costMax;

    public int CostMax
    {
        get { return costMax; }
        set
        {
            costMax = value;
            OnCostChanged();
        }
    }

    private int costLeft;

    public int CostLeft
    {
        get { return costLeft; }
        set
        {
            costLeft = value;
            OnCostChanged();
        }
    }

    public virtual void OnCostChanged()
    {
    }

    public void AddCost(int addCostNumber)
    {
        CostLeft += addCostNumber;
    }

    public void UseCost(int useCostNumber)
    {
        CostLeft -= useCostNumber;
    }

    public void AddCostMax(int addCostNumber)
    {
        CostMax += addCostNumber;
    }

    public void ReduceCostMax(int useCostNumber)
    {
        CostMax -= useCostNumber;
    }
}