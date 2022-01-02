using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler instance;

    [SerializeField] private List<Pool> pools;
    [SerializeField] private Dictionary<string, List<GameObject>> poolDict;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        poolDict = new Dictionary<string, List<GameObject>>();

        foreach (Pool pool in pools)
        {
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < pool.size; ++i)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            poolDict.Add(pool.tag, objectPool);
        }
    }

    public void SpawnFromPool (string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDict.ContainsKey(tag))
        {
            Debug.LogWarning("No pool with such tag exists: " + tag);
            return;
        }

        bool allObjectsActive = true;
        for (int i = 0; i < poolDict[tag].Count; ++i)
        {
            if (!poolDict[tag][i].activeInHierarchy)
            {
                allObjectsActive = false;
                poolDict[tag][i].transform.position = position;
                poolDict[tag][i].transform.rotation = rotation;
                poolDict[tag][i].SetActive(true);
                break;
            }
        }

        if (allObjectsActive)
        {
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool != null)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                poolDict[tag].Add(obj);
            }
        }
    }

    public void ClearPooledObjects()
    {
        foreach (Pool pool in pools)
            for (int i = 0; i < poolDict[pool.tag].Count; ++i)
                poolDict[pool.tag][i].SetActive(false);
    }
}
