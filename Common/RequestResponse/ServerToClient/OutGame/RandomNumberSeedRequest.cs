public class RandomNumberSeedRequest : ServerRequestBase
{
    public int randomNumberSeed;

    public RandomNumberSeedRequest()
    {
    }

    public RandomNumberSeedRequest(int randomNumberSeed)
    {
        this.randomNumberSeed = randomNumberSeed;
    }

    public override int GetProtocol()
    {
        return NetProtocols.RANDOM_NUMBER_SEED_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "RANDOM_NUMBER_SEED_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(randomNumberSeed);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        randomNumberSeed = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [randomNumberSeed]=" + randomNumberSeed;
        return log;
    }
}