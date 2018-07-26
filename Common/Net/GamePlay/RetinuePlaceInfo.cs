using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct RetinuePlaceInfo
{
    public int clientId;
    public int retinuePlaceIndex;

    public RetinuePlaceInfo(int clientId, int retinuePlaceIndex)
    {
        this.clientId = clientId;
        this.retinuePlaceIndex = retinuePlaceIndex;
    }
}