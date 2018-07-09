using System.Collections;
using System.Collections.Generic;

public class ServerWarningRequest : Request
{
    public int warningNumber;

    public ServerWarningRequest(){
        
    }

    public ServerWarningRequest(int warningNumber)
    {
        this.warningNumber = warningNumber;
    }

    public override int GetProtocol()
    {
        return NetProtocols.WARNING_NUMBER;
    }

	public override string GetProtocolName()
	{
        return "WARNING_NUMBER";
	}

	public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(warningNumber);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        this.warningNumber = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        foreach (System.Reflection.FieldInfo fi in typeof(WarningNumbers).GetFields())
        {
            if ((int)(fi.GetRawConstantValue()) == warningNumber)
            {
                log += " [warningNumber] " + fi.Name;
                break;
            }
        }
        return log;
    }
}
