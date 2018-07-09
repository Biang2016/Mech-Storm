using System.Collections;
using System.Collections.Generic;

public class TestConnectRequest : Request
{
    int testNumber;

    public TestConnectRequest()
    {

    }

    public TestConnectRequest(int testNumber)
    {
        this.testNumber = testNumber;
    }

    public override int GetProtocol()
    {
        return NetProtocols.TEST_CONNECT;
    }

	public override string GetProtocolName()
	{
        return "TEST_CONNECT";
	}

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt64(testNumber);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        testNumber = (int)reader.ReadInt64();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [testNumber] " + testNumber;
        return log;
    }
}