using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct CardDeckInfo
{
    public int CardNumber;

    public int[] CardIDs;

    public int[] BeginRetinueIDs;

    public CardDeckInfo(int[] cardIDs, int[] beginRetinueIDs)
    {
        CardIDs = cardIDs;
        CardNumber = cardIDs.Length;
        BeginRetinueIDs = beginRetinueIDs;
    }
}