using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnectResponse : Response
{
    public long testNumber;

    public override int GetProtocol()
    {
        return NetProtocols.TEST_CONNECT;
    }

    public override string GetProtocolName()
    {
        return this.GetType().FullName;
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        testNumber = (long) reader.ReadInt64();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += "[testNumber]" + testNumber;
        return log;
    }
}