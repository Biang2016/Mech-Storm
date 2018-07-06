using System.Collections;
using System.Collections.Generic;

public class TestConnectRequest : Request
{
    int testNumber;

    public override int GetProtocol()
    {
        return NetProtocols.TEST_CONNECT;
    }

    public TestConnectRequest(int testNumber)
    {
        this.testNumber = testNumber;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt64(testNumber);
    }
}