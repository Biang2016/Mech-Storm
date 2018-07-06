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
        AddAllCost();
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



    public bool UseCost(int useCostNumber)
    {
        if (CostLeft >= useCostNumber)
        {
            CostLeft -= useCostNumber;
            return true;
        }

        return false;
    }

    public void AddCostWithoutLimit(int addCostValue)
    {
        CostLeft += addCostValue;
    }

    public void AddCostWithinMax(int addCostValue)
    {
        if (CostMax - CostLeft > addCostValue)
            CostLeft += addCostValue;
        else
            CostLeft = CostMax;
    }

    public void UseAllCost()
    {
        CostLeft = 0;
    }

    public void AddAllCost()
    {
        CostLeft = CostMax;
    }


}