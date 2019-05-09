using System;

public abstract class SideEffectValue : IClone<SideEffectValue>
{
    public string Name;
    public ValueTypes ValueType;

    public enum ValueTypes
    {
        ConstInt, //常量int值,不会随倍率变化而变化,包括枚举
        MultipliedInt, //可变int值，会随增益等效果而变化
        Bool,
        String,
    }

    public SideEffectValue(string name, ValueTypes valueType)
    {
        Name = name;
        ValueType = valueType;
    }

    public void SetValue(string value)
    {
        switch (ValueType)
        {
            case ValueTypes.ConstInt:
            {
                SideEffectValue_ConstInt s = (SideEffectValue_ConstInt) this;
                if (s.EnumType == typeof(CardDeck))
                {
                    if (!AllCards.CardDict.ContainsKey(int.Parse(value)))
                    {
                        return;
                    }

                    ((SideEffectValue_ConstInt) this).Value = int.Parse(value);
                }
                else if (s.EnumType != null)
                {
                    s.Value = (int) Enum.Parse(s.EnumType, value);
                }
                else
                {
                    ((SideEffectValue_ConstInt) this).Value = int.Parse(value);
                }

                break;
            }
            case ValueTypes.MultipliedInt:
            {
                ((SideEffectValue_MultipliedInt) this).Value = int.Parse(value);
                break;
            }
            case ValueTypes.Bool:
            {
                ((SideEffectValue_Bool) this).Value = value == "True";
                break;
            }
            case ValueTypes.String:
            {
                ((SideEffectValue_String) this).Value = value;
                break;
            }
        }
    }

    public abstract SideEffectValue Clone();

    public virtual void Serialize(DataStream writer)
    {
        writer.WriteString8(Name);
        writer.WriteSInt32((int) ValueType);
    }

    public static SideEffectValue BaseDeserialize(DataStream reader)
    {
        string name = reader.ReadString8();
        ValueTypes valueType = (ValueTypes) reader.ReadSInt32();
        SideEffectValue sev = null;
        switch (valueType)
        {
            case ValueTypes.ConstInt:
            {
                int value = reader.ReadSInt32();
                string enumType_str = reader.ReadString8();
                Type enumType = null;

                if (!enumType_str.Equals("null"))
                {
                    enumType = Type.GetType(enumType_str);
                }

                sev = new SideEffectValue_ConstInt(name, value, enumType);
                break;
            }
            case ValueTypes.MultipliedInt:
            {
                int value = reader.ReadSInt32();
                sev = new SideEffectValue_MultipliedInt(name, value);
                break;
            }
            case ValueTypes.Bool:
            {
                bool value = reader.ReadByte() == 0x01;
                sev = new SideEffectValue_Bool(name, value);
                break;
            }
            case ValueTypes.String:
            {
                string value = reader.ReadString8();
                sev = new SideEffectValue_String(name, value);
                break;
            }
        }

        return sev;
    }
}

public class SideEffectValue_ConstInt : SideEffectValue
{
    public int Value;
    public Type EnumType;

    public SideEffectValue_ConstInt(string name, int value, Type enumType = null) : base(name, ValueTypes.ConstInt)
    {
        Value = value;
        EnumType = enumType;
    }

    public override SideEffectValue Clone()
    {
        return new SideEffectValue_ConstInt(Name, Value, EnumType);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(Value);
        writer.WriteString8(EnumType == null ? "null" : EnumType.ToString());
    }
}

public class SideEffectValue_MultipliedInt : SideEffectValue
{
    public int Value;

    public SideEffectValue_MultipliedInt(string name, int value) : base(name, ValueTypes.MultipliedInt)
    {
        Value = value;
    }

    public override SideEffectValue Clone()
    {
        return new SideEffectValue_MultipliedInt(Name, Value);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(Value);
    }
}

public class SideEffectValue_Bool : SideEffectValue
{
    public bool Value;

    public SideEffectValue_Bool(string name, bool value) : base(name, ValueTypes.Bool)
    {
        Value = value;
    }

    public override SideEffectValue Clone()
    {
        return new SideEffectValue_Bool(Name, Value);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteByte((byte) (Value ? 0x01 : 0x00));
    }
}

public class SideEffectValue_String : SideEffectValue
{
    public string Value;

    public SideEffectValue_String(string name, string value) : base(name, ValueTypes.String)
    {
        Value = value;
    }

    public override SideEffectValue Clone()
    {
        return new SideEffectValue_String(Name, Value);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteString8(Value);
    }
}