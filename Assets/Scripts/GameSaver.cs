using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.InteropServices;

class GameSaver
{
    private static List<WeakReference> saveables = new List<WeakReference>();
    private static string savePath;

    static GameSaver()
    {
        var home=Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            savePath=Path.Combine(new string[]{home,"Saved Games","NotARogueLike"});
        }else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            savePath=Path.Combine(new string[]{home,".local","share","not_a_rogue_like"});
        }
        else
        {
            throw new NotImplementedException("There is no savegame location implemented for the current operating system.\nSupported are: Linux and MS Windows");
        }

    } 
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
