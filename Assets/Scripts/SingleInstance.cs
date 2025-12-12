using UnityEngine;
using System.Collections.Generic;

// ensures that only one object of this name exists at a time
public class SingleInstance : MonoBehaviour
{
    public static Dictionary<string, GameObject> instances;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 
        if (instances == null) {
            instances = new();
        }

        // this object is a duplicate
        if (instances.ContainsKey(gameObject.name)) {
            GameObject.Destroy(gameObject);
            return;
        }

        instances[gameObject.name] = gameObject;
        DontDestroyOnLoad(gameObject);
    }
}
