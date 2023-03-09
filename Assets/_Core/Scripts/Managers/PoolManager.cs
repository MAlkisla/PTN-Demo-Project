using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum PoolObjectType
{
    Soldier1,
    Soldier2,
    Soldier3
}

[Serializable]
public class PoolInfo
{
    public PoolObjectType type;
    public int amount = 0;
    public GameObject prefab;
    public Transform container;
    [HideInInspector] public List<GameObject> pool = new List<GameObject>();
}

public class PoolManager : MonoBehaviour
{
    [SerializeField] private List<PoolInfo> listOfPool;


    #region Singleton

    private static PoolManager _instance;

    public static PoolManager Instance => _instance;


    private void Awake()
    {
        _instance = this;
    }

    #endregion

    private void Start()
    {
        foreach (var poolInfo in listOfPool)
        {
            FillPool(poolInfo);
        }
    }

    private void FillPool(PoolInfo poolInfo)
    {
        for (int i = 0; i < poolInfo.amount; i++)
        {
            var poolObj = Instantiate(poolInfo.prefab, poolInfo.container);
            poolObj.SetActive(false);
            poolInfo.pool.Add(poolObj);
        }
    }

    public GameObject GetPoolObject(PoolObjectType poolObjectType)
    {
        var selectedPool = GetPoolByType(poolObjectType);
        var pool = selectedPool.pool;

        GameObject instance;

        if (pool.Count > 0)
        {
            instance = pool[pool.Count - 1];
            pool.Remove(instance);
        }
        else
        {
            instance = Instantiate(selectedPool.prefab, selectedPool.container);
        }

        return instance;
    }

    public void CoolObject(GameObject obj, PoolObjectType type)
    {
        obj.SetActive(false);

        var selectedType = GetPoolByType(type);
        var selectedPool = selectedType.pool;

        if (!selectedPool.Contains(obj))
        {
            selectedPool.Add(obj);
        }
    }

    private PoolInfo GetPoolByType(PoolObjectType poolObjectType)
    {
        foreach (var poolInfo in listOfPool)
        {
            if (poolInfo.type == poolObjectType) return poolInfo;
        }

        return null;
    }
}