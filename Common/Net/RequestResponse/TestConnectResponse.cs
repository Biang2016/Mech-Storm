using System.Collections;
using System.Collections.Generic;

public class TestConnectResponse : Response
{
    public int testNumber;

    public override int GetProtocol()
    {
        return NetProtocols.TEST_CONNECT;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        testNumber = (int) reader.ReadInt64();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += "[testNumber]" + testNumber;
        return log;
    }
}