using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleOperationRecord
{
    //游戏行为记录
    private static BattleOperationRecord _bop;

    public static BattleOperationRecord BOP
    {
        get
        {
            if (_bop == null) _bop = new BattleOperationRecord();
            return _bop;
        }
    }

    int GameObjectID = 0;

    public int GetGameObjectID()
    {
        return ++GameObjectID;
    }

    public List<Operation> Operations = new List<Operation>();
}