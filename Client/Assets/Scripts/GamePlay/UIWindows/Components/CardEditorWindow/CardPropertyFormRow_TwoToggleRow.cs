using System.Collections.Generic;
using UnityEngine.Events;

public class CardPropertyFormRow_TwoToggleRow : CardPropertyFormRow
{
    public CardPropertyFormRow_Toggle ToggleLeft;
    public CardPropertyFormRow_Toggle ToggleRight;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        ToggleLeft = null;
        ToggleRight = null;
    }

    protected override void Child_Initialize(string labelStrKey, UnityAction<string> onValueChangeAction, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
    }

    protected override void SetValue(string value_str)
    {
    }

    public override void SetReadOnly(bool isReadOnly)
    {
        base.SetReadOnly(isReadOnly);
        ToggleLeft.SetReadOnly(isReadOnly);
        ToggleLeft.SetReadOnly(isReadOnly);
    }
}