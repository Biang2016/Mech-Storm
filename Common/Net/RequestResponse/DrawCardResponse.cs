using System.Collections;
using System.Collections.Generic;

public class DrawCardResponse : Response
{
    public int clientId;
    public int cardId;

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
        cardId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += "[clientId]" + clientId;
        log += "[cardId]" + cardId;
        return log;
    }
}