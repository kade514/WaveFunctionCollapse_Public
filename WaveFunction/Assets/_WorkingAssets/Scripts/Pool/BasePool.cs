using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public abstract class BasePool<T> : MonoBehaviour where T : Object, IPoolable
{
    public static BasePool<T> Instance;
    public T ObjectPrefab;
    public int ObjectsToSpawn;
    public List<T> ActiveObjects;
    public List<T> InactiveObjects;

    public virtual void Awake()
    {
        Instance = this;

        for (var i = 0; i < ObjectsToSpawn; i++)
        {
            SpawnItem<T>();
        }
    }

    public virtual void SpawnItem<T>() where T : Object
    {
        var temp = Instantiate(ObjectPrefab, transform);
        temp.gameObject.SetActive(false);
        InactiveObjects.Add(temp);
    }

    public abstract T GetPoolableObject();
    public abstract void ReturnPoolableObject(T instance);
}
