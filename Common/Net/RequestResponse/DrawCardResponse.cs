using System.Collections;
using System.Collections.Generic;

public class DrawCardResponse : Response
{
    public int clientId;
    public int cardId;
    public bool isShow;

    public override int GetProtocol()
    {
        return NetProtocols.DRAW_CARD;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        if (reader.ReadByte() == 0x01)
        {
            isShow = true;
        }
        else
        {
            isShow = false;
            cardId = reader.ReadSInt32();
        }
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += "[clientId]" + clientId;
        if (isShow)
        {
            log += "[cardId]" + cardId;
        }
        return log;
    }
}