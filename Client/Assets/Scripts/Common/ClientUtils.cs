using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ClientUtils
{
    public static Color HTMLColorToColor(string htmlColor)
    {
        Color cl = new Color();
        ColorUtility.TryParseHtmlString(htmlColor, out cl);
        return cl;
    }
}