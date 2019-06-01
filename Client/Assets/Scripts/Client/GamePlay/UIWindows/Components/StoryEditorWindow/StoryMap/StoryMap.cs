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
                    nodes[index++] = ((middle) * directions[i + 1] + (round - middle) * directions[i]) * routeLength;
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
            for (int round = 1; round < roundCount; round++)
            {
                int last_end = (round - 1) * round * 6 / 2;
                int this_end = (round + 1) * round * 6 / 2;
                int next_end = (round + 2) * (round + 1) * 6 / 2;
                if (i <= this_end)
                {
                    int cornerIndex = (i - last_end - 1) / round;
                    bool isCornerIndex = (i - last_end - 1) % round == 0;
                    if (isCornerIndex)
                    {
                        if (cornerIndex == 0) // 第一个点
                        {
                            GenerateLine(nodes, i, i + round * 6);
                            GenerateLine(nodes, i, i + round * 6 + 1);
                            GenerateLine(nodes, i, next_end);
                        }
                        else //其他角点
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                GenerateLine(nodes, i, i + round * 6 + j + cornerIndex - 1);
                            }
                        }
                    }
                    else //边点
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            GenerateLine(nodes, i, i + round * 6 + j + cornerIndex);
                        }
                    }

                    break;
                }
            }
        }
    }

    private void GenerateLine(Vector2[] nodes, int startIndex, int endIndex)
    {
        StoryMapRoute r = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StoryMapRoute].AllocateGameObject<StoryMapRoute>(StoryMapTransform);
        r.Refresh(nodes[startIndex], nodes[endIndex], 7f);
        StoryMapRoutes.Add(r);
    }
}