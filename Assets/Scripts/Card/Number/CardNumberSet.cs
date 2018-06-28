using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CardNumberSet : MonoBehaviour, IGameObjectPool {
    GameObjectPool gameObjectPool;

    public void PoolRecycle()
    {
        hasSign = false;
        gameObjectPool.RecycleGameObject(gameObject);
        MyNumberSize= NumberSize.Big;
    }
    GameObjectPool childrenPool;

    void Awake()
    {
        hasSign = false;
        gameObjectPool = GameObjectPoolManager.GOPM.Pool_CardNumberSetPool;
    }

    void Start()
    {

    }

    public void initiate(int number, NumberSize numberSize, TextAlign textAlign)
    {
        digitCount = 0;
        MyNumberSize = numberSize;
        Number = number;
        MyTextAlign = textAlign;
    }

    private bool hasSign = false;
    private char m_firstSign;
    public void initiate(char firstSign, int number, NumberSize numberSize, TextAlign textAlign)
    {
        m_firstSign = firstSign;
        hasSign = true;

        digitCount = 0;
        MyNumberSize = numberSize;
        if (number > 999) {
            Debug.Log("有符号的数字不超过3位数");
            return;
        }
        Number = number;
        MyTextAlign = textAlign;
    }

    [SerializeField]
    private NumberSize myNumberSize;
    public NumberSize MyNumberSize {
        get {
            return myNumberSize;
        }

        set {
            myNumberSize = value;
            setNumberSize(value);
        }
    }

    public float IntervalFactor;
    void setNumberSize(NumberSize value)
    {
        switch (value) {
            case NumberSize.Small:
                childrenPool = GameObjectPoolManager.GOPM.Pool_CardSmallNumberPool;
                for (int i = 0; i < 4; i++)
                {
                    if (cardNumbers[i] != null)
                    {
                        cardNumbers[i].PoolRecycle();
                        cardNumbers[i] = null;
                    }
                }
                interval = -0.6f * IntervalFactor;
                Number = Number;
                initiateDigitPlace();
                break;
            case NumberSize.Medium:
                childrenPool = GameObjectPoolManager.GOPM.Pool_CardMediumNumberPool;
                for (int i = 0; i < 4; i++)
                {
                    if (cardNumbers[i] != null)
                    {
                        cardNumbers[i].PoolRecycle();
                        cardNumbers[i] = null;
                    }
                }
                interval = -0.9f * IntervalFactor;
                Number = Number;
                initiateDigitPlace();
                break;
            case NumberSize.Big:
                childrenPool = GameObjectPoolManager.GOPM.Pool_CardBigNumberPool;
                for (int i = 0; i < 4; i++)
                {
                    if (cardNumbers[i] != null)
                    {
                        cardNumbers[i].PoolRecycle();
                        cardNumbers[i] = null;
                    }
                }
                interval = -1f * IntervalFactor;
                Number = Number;
                initiateDigitPlace();
                break;
        }
    }

    CardNumber[] cardNumbers = new CardNumber[4];
    int[] digits = new int[4];
    private int digitCount = 0;

    [SerializeField]
    private int number;
    public int Number {
        get {
            return number;
        }

        set {
            if (value < 0) {
                Debug.Log("CardNumberSet(" + name + ")" + ":number is set to " + value);
                number = 0;
            } else if (hasSign && value > 999) {
                Debug.Log("有符号的数字不超过3位数");
                number = 999;
            } else if (value > 9999) {
                Debug.Log("CardNumberSet(" + name + ")" + ":number is set to " + value);
                number = 9999;
            }
            number = value;
            setNumberSet();
            if(hasSign) setFirstSign(m_firstSign);
            setTextAlign();
        }
    }

    void setNumberSet()
    {
        digits[0] = Number / 1000;
        digits[1] = Number % 1000 / 100;
        digits[2] = Number % 100 / 10;
        digits[3] = Number % 10;
        if (digits[0] > 0) {
            setNumber(0, digits[0]);
            setNumber(1, digits[1]);
            setNumber(2, digits[2]);
            setNumber(3, digits[3]);
            digitCount = 4;
        } else if (digits[1] > 0) {
            setNumber(0, -1);
            setNumber(1, digits[1]);
            setNumber(2, digits[2]);
            setNumber(3, digits[3]);
            digitCount = 3;
        } else if (digits[2] > 0) {
            setNumber(0, -1);
            setNumber(1, -1);
            setNumber(2, digits[2]);
            setNumber(3, digits[3]);
            digitCount = 2;
        } else if (digits[3] >= 0) {
            setNumber(0, -1);
            setNumber(1, -1);
            setNumber(2, -1);
            setNumber(3, digits[3]);
            digitCount = 1;
        }
    }

    void setFirstSign(char firstSign)
    {
        if(!cardNumbers[3 - digitCount]) {
            cardNumbers[3 - digitCount] = childrenPool.AllocateGameObject(transform).GetComponent<CardNumber>();
        }
        cardNumbers[3 - digitCount].SetSign(firstSign);
    }

    void clearFisrSign()
    {

    }

    void setNumber(int digitPlace, int value)
    {
        if (!cardNumbers[digitPlace])
        {
            if (value == -1) return;
            cardNumbers[digitPlace] = childrenPool.AllocateGameObject(transform).GetComponent<CardNumber>();
            cardNumbers[digitPlace].Number = value;
        }
        else if (value == -1)
        {
            cardNumbers[digitPlace].PoolRecycle();
            cardNumbers[digitPlace] = null;
        }
        else
        {
            cardNumbers[digitPlace].Number = value;
        }
    }

    Vector3 digit4_thousand_position_L;
    Vector3 digit4_hundreds_position_L;
    Vector3 digit4_tens_position_L;
    Vector3 digit4_unit_position_L;

    Vector3 digit3_hundreds_position_L;
    Vector3 digit3_tens_position_L;
    Vector3 digit3_unit_position_L;

    Vector3 digit2_tens_position_L;
    Vector3 digit2_unit_position_L;

    Vector3 digit1_unit_position_L;

    Vector3 digit4_thousand_position_R;
    Vector3 digit4_hundreds_position_R;
    Vector3 digit4_tens_position_R;
    Vector3 digit4_unit_position_R;

    Vector3 digit3_hundreds_position_R;
    Vector3 digit3_tens_position_R;
    Vector3 digit3_unit_position_R;

    Vector3 digit2_tens_position_R;
    Vector3 digit2_unit_position_R;

    Vector3 digit1_unit_position_R;

    Vector3 digit4_thousand_position_C;
    Vector3 digit4_hundreds_position_C;
    Vector3 digit4_tens_position_C;
    Vector3 digit4_unit_position_C;

    Vector3 digit3_hundreds_position_C;
    Vector3 digit3_tens_position_C;
    Vector3 digit3_unit_position_C;

    Vector3 digit2_tens_position_C;
    Vector3 digit2_unit_position_C;

    Vector3 digit1_unit_position_C;

    private float interval;
    void initiateDigitPlace()
    {
        digit4_thousand_position_L = new Vector3(interval * -4, 0f, 0f);
        digit4_hundreds_position_L = new Vector3(interval * -3, 0f, 0f);
        digit4_tens_position_L = new Vector3(interval * -2, 0f, 0f);
        digit4_unit_position_L = new Vector3(interval * -1, 0f, 0f);

        digit3_hundreds_position_L = new Vector3(interval * -3, 0f, 0f);
        digit3_tens_position_L = new Vector3(interval * -2, 0f, 0f);
        digit3_unit_position_L = new Vector3(interval * -1, 0f, 0f);

        digit2_tens_position_L = new Vector3(interval * -2, 0f, 0f);
        digit2_unit_position_L = new Vector3(interval * -1, 0f, 0f);

        digit1_unit_position_L = new Vector3(interval * -1, 0f, 0f);

        digit4_thousand_position_R = new Vector3(interval * 0, 0f, 0f);
        digit4_hundreds_position_R = new Vector3(interval * 1, 0f, 0f);
        digit4_tens_position_R = new Vector3(interval * 2, 0f, 0f);
        digit4_unit_position_R = new Vector3(interval * 3, 0f, 0f);

        digit3_hundreds_position_R = new Vector3(interval * 0, 0f, 0f);
        digit3_tens_position_R = new Vector3(interval * 1, 0f, 0f);
        digit3_unit_position_R = new Vector3(interval * 2, 0f, 0f);

        digit2_tens_position_R = new Vector3(interval * 0, 0f, 0f);
        digit2_unit_position_R = new Vector3(interval * 1, 0f, 0f);

        digit1_unit_position_R = new Vector3(interval * 0, 0f, 0f);

        digit4_thousand_position_C = new Vector3(interval * -2, 0f, 0f);
        digit4_hundreds_position_C = new Vector3(interval * -1, 0f, 0f);
        digit4_tens_position_C = new Vector3(interval * 0, 0f, 0f);
        digit4_unit_position_C = new Vector3(interval * 1, 0f, 0f);

        digit3_hundreds_position_C = new Vector3(interval * -1.5f, 0f, 0f);
        digit3_tens_position_C = new Vector3(interval * -0.5f, 0f, 0f);
        digit3_unit_position_C = new Vector3(interval * 0.5f, 0f, 0f);

        digit2_tens_position_C = new Vector3(interval * -1, 0f, 0f);
        digit2_unit_position_C = new Vector3(interval * 0, 0f, 0f);

        digit1_unit_position_C = new Vector3(interval * -0.5f, 0f, 0f);
    }

    public enum TextAlign {
        Left = 0,
        Right = 1,
        Center = 2
    }

    private TextAlign myTextAlign = TextAlign.Right;
    internal TextAlign MyTextAlign {
        get {
            return myTextAlign;
        }

        set {
            myTextAlign = value;
            setTextAlign();
        }
    }

    void setTextAlign()
    {
        switch (MyTextAlign) {
            case TextAlign.Left:
                if (digitCount == 4 || (digitCount == 3 && hasSign)) {
                    cardNumbers[0].transform.localPosition = digit4_thousand_position_L;
                    cardNumbers[1].transform.localPosition = digit4_hundreds_position_L;
                    cardNumbers[2].transform.localPosition = digit4_tens_position_L;
                    cardNumbers[3].transform.localPosition = digit4_unit_position_L;
                } else if (digitCount == 3 || (digitCount == 2 && hasSign)) {
                    cardNumbers[1].transform.localPosition = digit3_hundreds_position_L;
                    cardNumbers[2].transform.localPosition = digit3_tens_position_L;
                    cardNumbers[3].transform.localPosition = digit3_unit_position_L;
                } else if (digitCount == 2 || (digitCount == 1 && hasSign)) {
                    cardNumbers[2].transform.localPosition = digit2_tens_position_L;
                    cardNumbers[3].transform.localPosition = digit2_unit_position_L;
                } else {
                    cardNumbers[3].transform.localPosition = digit1_unit_position_L;
                }
                break;
            case TextAlign.Right:
                if (digitCount == 4 || (digitCount == 3 && hasSign)) {
                    cardNumbers[0].transform.localPosition = digit4_thousand_position_R;
                    cardNumbers[1].transform.localPosition = digit4_hundreds_position_R;
                    cardNumbers[2].transform.localPosition = digit4_tens_position_R;
                    cardNumbers[3].transform.localPosition = digit4_unit_position_R;
                } else if (digitCount == 3 || (digitCount == 2 && hasSign)) {
                    cardNumbers[1].transform.localPosition = digit3_hundreds_position_R;
                    cardNumbers[2].transform.localPosition = digit3_tens_position_R;
                    cardNumbers[3].transform.localPosition = digit3_unit_position_R;
                } else if (digitCount == 2 || (digitCount == 1 && hasSign)) {
                    cardNumbers[2].transform.localPosition = digit2_tens_position_R;
                    cardNumbers[3].transform.localPosition = digit2_unit_position_R;
                } else {
                    cardNumbers[3].transform.localPosition = digit1_unit_position_R;
                }
                break;
            case TextAlign.Center:
                if (digitCount == 4 || (digitCount == 3 && hasSign)) {
                    cardNumbers[0].transform.localPosition = digit4_thousand_position_C;
                    cardNumbers[1].transform.localPosition = digit4_hundreds_position_C;
                    cardNumbers[2].transform.localPosition = digit4_tens_position_C;
                    cardNumbers[3].transform.localPosition = digit4_unit_position_C;
                } else if (digitCount == 3 || (digitCount == 2 && hasSign)) {
                    cardNumbers[1].transform.localPosition = digit3_hundreds_position_C;
                    cardNumbers[2].transform.localPosition = digit3_tens_position_C;
                    cardNumbers[3].transform.localPosition = digit3_unit_position_C;
                } else if (digitCount == 2 || (digitCount == 1 && hasSign)) {
                    cardNumbers[2].transform.localPosition = digit2_tens_position_C;
                    cardNumbers[3].transform.localPosition = digit2_unit_position_C;
                } else {
                    cardNumbers[3].transform.localPosition = digit1_unit_position_C;
                }
                break;
        }
    }

}
