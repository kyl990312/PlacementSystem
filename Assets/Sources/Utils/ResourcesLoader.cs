using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesLoader 
{ 
    public static bool Load<T>(string path, out T result)
        where T : Object
    {
        result = Resources.Load(path) as T;
        return result != null;
    }
}
