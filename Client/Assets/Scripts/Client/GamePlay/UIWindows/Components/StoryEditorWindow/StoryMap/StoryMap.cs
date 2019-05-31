using System.Collections.Generic;
using UnityEngine;

public class StoryMap : MonoBehaviour
{
    [SerializeField] private Transform StoryMapTransform;
    private List<StoryMapRoute> StoryMapRoutes = new List<StoryMapRoute>();

    internal void Initialize(int roundCount, float routeLength)
    {
        foreach (StoryMapRoute route in StoryMapRoutes)
        {
            route.PoolRecycle();
        }

        StoryMapRoutes.Clear();

        Vector2 a = new Vector2(1, 0);
        Vector2 b = new Vector2(0.5f, 0.866f);
        Vector2 c = new Vector2(-0.5f, 0.866f);
        Vector2[] nodes = new Vector2[(roundCount + 1) * roundCount * 3 + 1];
        Vector2[] directions = new[] {a, b, c, -a, -b, -c, a};
        int index = 0;
        nodes[index++] = Vector2.zero;

        for (int round = 1; round <= roundCount; round++)
        {
            for (int i = 0; i < 6; i++)
            {
                nodes[index++] = round * directions[i] * routeLength;
                for (int middle = 1; middle <= round - 1; middle++)
                {
                    nodes[index++] = ((middle) * directions[i] + (round - middle) * directions[i + 1]) * routeLength;
                }
            }
        }

        for (int i = 1; i <= 6; i++)
        {
            GenerateLine(nodes, 0, i);
        }

        int start = 1;
        for (int round = 1; round <= roundCount; round++)
        {
            start += 6 * (round - 1);
            for (int i = 0; i < round * 6 - 1; i++)
            {
                GenerateLine(nodes, start + i, start + i + 1);
            }

            GenerateLine(nodes, start + round * 6 - 1, start);
        }

        for (int i = 1; i < nodes.Length; i++)
        {
            if (i <= 6)
            {
                //角点
            }
            else
            {
                for (int round = 1; round < roundCount; round++)
                {
                    int end = (1 + round) * round * 3 / 2;
                    if (i > end)
                    {
                        if ((i - end - 1) % round == 0)
                        {
                            //角点
                        }
                        else
                        {
                            //边点
                        }

                        break;
                    }
                }
            }
        }

        for (int round = 0; round < roundCount; round++)
        {
        }
    }

    private void GenerateLine(Vector2[] nodes, int startIndex, int endIndex)
    {
        StoryMapRoute r = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StoryMapRoute].AllocateGameObject<StoryMapRoute>(StoryMapTransform);
        r.Refresh(nodes[startIndex], nodes[endIndex], 7f);
        StoryMapRoutes.Add(r);
    }
}