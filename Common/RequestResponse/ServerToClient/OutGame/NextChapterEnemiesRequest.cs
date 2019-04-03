using System.Collections.Generic;

public class NextChapterEnemiesRequest : ServerRequestBase
{
    public int ChapterID;
    public List<int> NextChapterEnemyPicIDs = new List<int>();

    public NextChapterEnemiesRequest()
    {
    }

    public NextChapterEnemiesRequest(int chapterID, List<int> nextLevelEnemyPicIDs)
    {
        ChapterID = chapterID;
        NextChapterEnemyPicIDs = nextLevelEnemyPicIDs;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.NEXT_CHAPTER_ENEMYINFO_REQUSET;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ChapterID);
        writer.WriteSInt32(NextChapterEnemyPicIDs.Count);
        foreach (int picID in NextChapterEnemyPicIDs)
        {
            writer.WriteSInt32(picID);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ChapterID = reader.ReadSInt32();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            NextChapterEnemyPicIDs.Add(reader.ReadSInt32());
        }
    }
}