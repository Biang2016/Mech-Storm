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

    internal int BuffId;
    private int buffValue;
    private bool hasNumberShow;

    internal int BuffValue
    {
        get { return buffValue; }

        set
        {
            buffValue = value;
            BuffValueText.text = value == 0 ? "" : value.ToString();

        }
    }

    private int pictureId;

    internal int PictureId
    {
        get { return pictureId; }

        set
        {
            pictureId = value;
            ClientUtils.ChangePicture(Image, pictureId);
        }
    }

    public bool HasNumberShow
    {
        get { return hasNumberShow; }

        set
        {
            hasNumberShow = value;
            BuffValuePanel.enabled = hasNumberShow;
            BuffValueText.enabled = hasNumberShow;
        }
    }

    public void UpdateValue(int picId, int buffId, int buffValue, bool hasNumberShow = true)
    {
        PictureId = picId;
        BuffId = buffId;
        BuffValue = buffValue;
        if (buffValue >= 0)
        {
            BuffAnim.SetTrigger("Jump");
        }
        HasNumberShow = hasNumberShow;
        BuffAnim.SetTrigger("Add");
    }

    public void OnRemove()
    {
        BuffAnim.SetTrigger("Remove");
    }


    [SerializeField] private Image BuffValuePanel;
    [SerializeField] private Text BuffValueText;
    [SerializeField] private Text BuffDescText;
    [SerializeField] private Animator BuffDescAnim;
    [SerializeField] private Animator BuffAnim;
    [SerializeField] private Image Image;
}