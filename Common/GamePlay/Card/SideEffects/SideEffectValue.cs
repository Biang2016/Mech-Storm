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
                SideEffectValue_ContInt s = (SideEffectValue_ContInt) this;
                if (s.EnumType != null)
                {
                    s.Value = (int) Enum.Parse(s.EnumType, value);
                }
                else
                {
                    ((SideEffectValue_ContInt) this).Value = int.Parse(value);
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
                sev = new SideEffectValue_ContInt(name, value);
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

public class SideEffectValue_ContInt : SideEffectValue
{
    public int Value;
    public Type EnumType;

    public SideEffectValue_ContInt(string name, int value, Type enumType = null) : base(name, ValueTypes.ConstInt)
    {
        Value = value;
        EnumType = enumType;
    }

    public override SideEffectValue Clone()
    {
        return new SideEffectValue_ContInt(Name, Value, EnumType);
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
}