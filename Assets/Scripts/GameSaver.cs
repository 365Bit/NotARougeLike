using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

class GameSaver
{
    private static List<WeakReference> saveables = new List<WeakReference>();
    public static void subscribe<T>(T self)
    {
        var reference = new WeakReference(self);
        saveables.Append(reference);
        Debug.Log("Subscribed");
    }

    public static void save()
    {
        Debug.Log("start saving");
        StreamWriter file = new StreamWriter("save.json");
        file.WriteLine("#NotARougeLike savegame");
        for (int i = 0; i < saveables.Count; i++)
        {
            var saveable = saveables[i];
            //remove everyone that was subscribed but has been deleted since
            if (!saveable.IsAlive)
            {
                saveables.RemoveAt(i--);
                Debug.Log($"Removed dead. {i} remaining");
                continue;
            }
            file.WriteLine(JsonUtility.ToJson(saveable.Target));

        }
        file.Close();
        Debug.Log("finish saving");
    }

}
