using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnectRequest : Request
{
    long testNumber;

    public override int GetProtocol()
    {
        return NetProtocols.TEST_CONNECT;
    }

    public TestConnectRequest(long testNumber)
    {
        this.testNumber = testNumber;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt64(testNumber);
    }
}