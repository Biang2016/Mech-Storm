using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    PoolObject[] gameObjectPool; //对象池
    bool[] isUsed;//已使用的对象
    //Todo Temp public
    public int capacity; //对象池容量，根据场景中可能出现的最多数量的该对象预估一个容量
    public int used; //已使用多少个对象
    public int notUsed; //多少个对象已实例化但未使用
    public int empty; //对象池中未实例化的空位置的个数

    PoolObject gameObjectPrefab;

    //记录对象原始的位置、旋转、缩放，以便还原
    Vector3 gameObjectDefaultPosition;
    Quaternion gameObjectDefaultRotation;
    Vector3 gameObjectDefaultScale;

    public static Vector3 GameObjectPoolPosition = new Vector3(-30f, -30f, 0f);

    public void Initiate(PoolObject prefab, int initialCapacity)
    {
        transform.position = GameObjectPoolPosition;
        gameObjectPrefab = prefab;
        gameObjectDefaultPosition = gameObjectPrefab.transform.position;
        gameObjectDefaultRotation = gameObjectPrefab.transform.rotation;
        gameObjectDefaultScale = gameObjectPrefab.transform.localScale;
        gameObjectPool = new PoolObject[initialCapacity];
        isUsed = new bool[initialCapacity];
        capacity = initialCapacity;
        empty = capacity;
    }

    public T AllocateGameObject<T>(Transform parent) where T : PoolObject
    {
        for (int i = 0; i < capacity; i++)
        {
            if (!isUsed[i])
            {
                if (gameObjectPool[i])
                {
                    gameObjectPool[i].transform.SetParent(parent);
                    gameObjectPool[i].transform.localPosition = gameObjectDefaultPosition;
                    gameObjectPool[i].transform.localRotation = gameObjectDefaultRotation;
                    gameObjectPool[i].transform.localScale = gameObjectDefaultScale;
                    used++;
                    notUsed--;
                }
                else
                {
                    gameObjectPool[i] = Instantiate(gameObjectPrefab, parent);
                    gameObjectPool[i].name = gameObjectPrefab.name + "_" + i;//便于调试的时候分辨对象
                    gameObjectPool[i].SetObjectPool(this);
                    empty--;
                    used++;
                }
                isUsed[i] = true;
                return (T)gameObjectPool[i];
            }
        }
        expandCapacity();
        return AllocateGameObject<T>(parent);
    }

    public void RecycleGameObject(PoolObject recGameObject)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (gameObjectPool[i] == recGameObject)
            {
                isUsed[i] = false;
                recGameObject.transform.SetParent(transform);
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
        PoolObject[] new_gameObjectPool = new PoolObject[capacity * 2];
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
