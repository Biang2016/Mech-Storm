using System.Collections.Generic;
using UnityEngine;

public class ChapterMap : MonoBehaviour
{
    [SerializeField] private Transform ChapterMapRoutesTransform;
    [SerializeField] private Transform ChapterMapNodesTransform;
    private List<ChapterMapRoute> ChapterMapRoutes = new List<ChapterMapRoute>();
    private List<ChapterMapNode> ChapterMapNodes = new List<ChapterMapNode>();

    private float lineWidth = 7f;
    private Vector2[] nodeLocations;

    internal void Initialize(int roundCount, float routeLength, float lineWidth)
    {
        this.lineWidth = lineWidth;
        foreach (ChapterMapRoute route in ChapterMapRoutes)
        {
            route.PoolRecycle();
        }

        foreach (ChapterMapNode node in ChapterMapNodes)
        {
            node.PoolRecycle();
        }

        ChapterMapRoutes.Clear();

        Vector2 a = new Vector2(1, 0);
        Vector2 b = new Vector2(0.5f, 0.866f);
        Vector2 c = new Vector2(-0.5f, 0.866f);
        nodeLocations = new Vector2[(roundCount + 1) * roundCount * 3 + 1 + 12];
        Vector2[] directions = new[] {a, b, c, -a, -b, -c, a};
        int index = 0;
        nodeLocations[index++] = Vector2.zero;

        // 画点
        for (int round = 1; round <= roundCount; round++)
        {
            for (int i = 0; i < 6; i++)
            {
                nodeLocations[index++] = round * directions[i] * routeLength;
                for (int middle = 1; middle <= round - 1; middle++)
                {
                    nodeLocations[index++] = ((middle) * directions[i + 1] + (round - middle) * directions[i]) * routeLength;
                }
            }
        }

        //六角点BOSS
        for (int i = 0; i < 6; i++)
        {
            nodeLocations[index++] = (roundCount + 1) * directions[i] * routeLength;
        }

        //六边中点宝藏
        for (int i = 0; i < 6; i++)
        {
            nodeLocations[index++] = (((roundCount + 1) / 2.0f) * directions[i + 1] + ((roundCount + 1) / 2.0f) * directions[i]) * routeLength;
        }

        //生成关卡Button
        foreach (Vector2 nl in nodeLocations)
        {
            ChapterMapNode cmn = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ChapterMapNode].AllocateGameObject<ChapterMapNode>(ChapterMapNodesTransform);
            cmn.Initialize(0);
            ChapterMapNodes.Add(cmn);
            cmn.transform.localPosition = nl;
        }

        // 画线
        for (int i = 1; i <= 6; i++)
        {
            GenerateLine(0, i);
        }

        int start = 1;
        for (int round = 1; round <= roundCount; round++)
        {
            start += 6 * (round - 1);
            for (int i = 0; i < round * 6 - 1; i++)
            {
                GenerateLine(start + i, start + i + 1);
            }

            GenerateLine(start + round * 6 - 1, start);
        }

        for (int i = 1; i < nodeLocations.Length; i++)
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
                            GenerateLine(i, i + round * 6);
                            GenerateLine(i, i + round * 6 + 1);
                            GenerateLine(i, next_end);
                        }
                        else //其他角点
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                GenerateLine(i, i + round * 6 + j + cornerIndex - 1);
                            }
                        }
                    }
                    else //边点
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            GenerateLine(i, i + round * 6 + j + cornerIndex);
                        }
                    }

                    break;
                }
            }
        }

        // 六角点 BOSS
        int end1 = (roundCount - 1) * roundCount * 6 / 2;
        int end2 = (roundCount + 1) * roundCount * 6 / 2;
        for (int i = 0; i < 6; i++)
        {
            int node_index = end1 + 1 + roundCount * i;
            GenerateLine(node_index, end2 + (i + 1));
        }

        // 六边中点宝藏
        for (int i = 0; i < 6; i++)
        {
            int node_index = end1 + 1 + roundCount * i + roundCount / 2;
            GenerateLine(node_index, end2 + i + 6 + 1);
        }
    }

    private void GenerateLine(int startIndex, int endIndex)
    {
        ChapterMapRoute r = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ChapterMapRoute].AllocateGameObject<ChapterMapRoute>(ChapterMapRoutesTransform);
        r.Refresh(nodeLocations[startIndex], nodeLocations[endIndex], lineWidth);
        ChapterMapRoutes.Add(r);
    }
}