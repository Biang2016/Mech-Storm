using System.Collections;
using System.Collections.Generic;

public class ServerInfoRequest : Request
{
    public int infoNumber;

    public ServerInfoRequest()
    {

    }

    public ServerInfoRequest(int infoNumber)
    {
        this.infoNumber = infoNumber;
    }

    public override int GetProtocol()
    {
        return NetProtocols.INFO_NUMBER;
    }

    public override string GetProtocolName()
    {
        return "INFO_NUMBER";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(infoNumber);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        this.infoNumber = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        foreach (System.Reflection.FieldInfo fi in typeof(InfoNumbers).GetFields())
        {
            if ((int)(fi.GetRawConstantValue()) == infoNumber)
            {
                log += "[infoNumber]" + fi.Name;
                break;
            }
        }
        return log;

    }


}
