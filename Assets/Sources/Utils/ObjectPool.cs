using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T>
    where T : class, new()
{
    private List<T> _pool = new List<T>();
    private int _capacity;
    
    public int Capacity
    {
        get => _capacity;
        set => _capacity = value;
    }

    public T Get()
    {
        if (_pool.Count == 0)
        {
            if (!CreateObject(out var instance))
                return null;
            _pool.Add(instance);
        }

        var obj = _pool[0];
        _pool.RemoveAt(0);
        return obj;
    }

    public void Return(T obj)
    {
        if (_capacity <= _pool.Count)
            return;
        _pool.Add(obj);
    }

    protected virtual bool CreateObject(out T instance)
    {
        instance = new T();
        return instance != null;
    }
}

public class GameObjectPool : ObjectPool<GameObject>
{
    GameObject _src;
    public GameObjectPool(GameObject src) : base()
    {
        _src = src;
    }

    protected override bool CreateObject(out GameObject instance)
    {
        instance = null;
        if (_src == null)
            return false;
        instance = GameObject.Instantiate(_src);
        return instance != null;
    }
}
