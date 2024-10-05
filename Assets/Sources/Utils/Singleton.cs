using System;
using UnityEngine;

public class Singleton<T>
    where T : class
{
    static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = Activator.CreateInstance<T>();
            return _instance;
        }
    }
}

public class MonoSingleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    static T _instance;
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if(_instance == null)
                {
                    var gObj = new GameObject(typeof(T).ToString());
                    _instance = gObj.AddComponent<T>();
                }
            }

            return _instance;
        }
    }
}
