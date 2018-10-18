using UnityEngine;
using UnityEngine.UI;

internal class PlayerBuff : PoolObject
{
    public override void PoolRecycle()
    {
        PictureId = 0;
        BuffId = 0;
        buffValue = 0;
        BuffValueText.text = "";
        base.PoolRecycle();
    }

    internal int PictureId;
    internal int BuffId;
    private int buffValue;

    internal int BuffValue
    {
        get { return buffValue; }

        set
        {
            buffValue = value;
            BuffValueText.text = value == 0 ? "" : value.ToString();
            if (value >= 0)
            {
                BuffValueChangeAnim.SetTrigger("Jump");
            }
        }
    }

    public void Initialize(int picId, int buffId, int buffValue)
    {
        PictureId = picId;
        BuffId = buffId;
        BuffValue = buffValue;
        BuffAnim.SetTrigger("Add");
    }

    public void OnRemove()
    {
        BuffAnim.SetTrigger("Remove");
    }

    [SerializeField] private Text BuffValueText;
    [SerializeField] private Text BuffDescText;
    [SerializeField] private Animator BuffDescAnim;
    [SerializeField] private Animator BuffValueChangeAnim;
    [SerializeField] private Animator BuffAnim;
}