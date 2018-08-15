using System.Collections;
using System.Collections.Generic;

public class GameStopByLeaveRequest : ServerRequestBase
{
    public int clientId;

    public GameStopByLeaveRequest()
    {

    }

    public GameStopByLeaveRequest(int clientId)
    {
        this.clientId = clientId;
    }
    public override int GetProtocol()
    {
        return NetProtocols.GAME_STOP_BY_LEAVE_REQUEST;
    }

	public override string GetProtocolName()
	{
        return "GAME_STOP_BY_LEAVE_REQUEST";
	}

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        return log;
    }
}