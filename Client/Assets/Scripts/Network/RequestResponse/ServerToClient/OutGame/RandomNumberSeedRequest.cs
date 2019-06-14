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

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.RANDOM_NUMBER_SEED_REQUEST;
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
}