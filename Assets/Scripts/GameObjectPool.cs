using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    GameObject[] gameObjectPool;
    bool[] isUsed;
    //Todo Temp public
    public int capacity;
    public int used;
    public int empty;
    public int notUsed;

    GameObject gameObjectPrefab;
    Vector3 gameObjectDefaultPosition;
    Quaternion gameObjectDefaultRotation;
    Vector3 gameObjectDefaultScale;

    public static Vector3 GameObjectPoolPosition = new Vector3(-30f, -30f, 0f);

    public void Initiate(GameObject prefab, int initialCapacity)
    {
        transform.position = GameObjectPoolPosition;
        gameObjectPrefab = prefab;
        gameObjectDefaultPosition = gameObjectPrefab.transform.position;
        gameObjectDefaultRotation = gameObjectPrefab.transform.rotation;
        gameObjectDefaultScale = gameObjectPrefab.transform.localScale;
        gameObjectPool = new GameObject[initialCapacity];
        isUsed = new bool[initialCapacity];
        capacity = initialCapacity;
        empty = capacity;
    }

    public GameObject AllocateGameObject(Transform parent)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (!isUsed[i])
            {
                if (gameObjectPool[i])
                {
                    gameObjectPool[i].transform.parent = parent;
                    gameObjectPool[i].transform.localPosition = gameObjectDefaultPosition;
                    gameObjectPool[i].transform.localRotation = gameObjectDefaultRotation;
                    gameObjectPool[i].transform.localScale = gameObjectDefaultScale;
                    used++;
                    notUsed--;
                }
                else
                {
                    gameObjectPool[i] = Instantiate(gameObjectPrefab, parent);
                    empty--;
                    used++;
                }
                isUsed[i] = true;
                return gameObjectPool[i];
            }
        }
        expandCapacity();
        return AllocateGameObject(parent);
    }

    public void RecycleGameObject(GameObject recGameObject)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (gameObjectPool[i] == recGameObject)
            {
                isUsed[i] = false;
                recGameObject.transform.parent = transform;
                recGameObject.transform.localPosition = gameObjectDefaultPosition;
                used--;
                notUsed++;
                return;
            }
        }
        Destroy(recGameObject, 0.1f);
    }

    void expandCapacity()
    {
        GameObject[] new_gameObjectPool = new GameObject[capacity * 2];
        bool[] new_isUsed = new bool[capacity * 2];

        for (int i = 0; i < capacity; i++)
        {
            new_gameObjectPool[i] = gameObjectPool[i];
            new_isUsed[i] = isUsed[i];
        }
        capacity *= 2;
        empty = capacity - used - notUsed;
        gameObjectPool = new_gameObjectPool;
        isUsed = new_isUsed;
    }

}
