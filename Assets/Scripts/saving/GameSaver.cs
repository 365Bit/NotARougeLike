using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using System.Data;
using System.Reflection;

class GameSaver
{
    private static List<(string, WeakReference)> saveables = new List<(string, WeakReference)>();
    private static string saveDirectory;


    static GameSaver()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        if (Application.isEditor)
        {
            saveDirectory = Path.Combine("game_saves", "not_a_rogue_like");
        }
        else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            saveDirectory = Path.Combine(new string[] { home, "Saved Games", "NotARogueLike" });
        }
        else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            saveDirectory = Path.Combine(new string[] { home, ".local", "share", "not_a_rogue_like" });
        }
        else
        {
            throw new NotImplementedException("There is no savegame location implemented for the current operating system.\nSupported are: Linux and MS Windows");
        }
    }

    /// <summary>
    /// Subscribes an object to the savegame system. 
    /// This must be done before an object can be saved or loaded.
    /// </summary>
    /// <typeparam name="T">The Type of the object to Save</typeparam>
    /// <param name="self">reference to the object. Note that no reference is kept so the object can be garbage collected.</param>
    /// <param name="name">the name under witch the object is stored. A name must be unique, throws an DuplicateNameException otherwise. Defaults to class name</param>
    public static void subscribe<T>(T self, string name = null)
    {
        if (name == null)
        {
            name = typeof(T).Name;
        }
        saveables.RemoveAll(s => { return !s.Item2.IsAlive; });
        if (saveables.Any(s => { return s.Item1 == name; }))
        {
            return;
            throw new DuplicateNameException($"{name} already subscribed for gamesaves");
        }
        var reference = new WeakReference(self);
        saveables.Add((name, reference));
    }

    /// <summary>
    /// Stores all subscribed objects onto disk
    /// </summary>
    public static void save()
    {
        Debug.Log("start saving");
        Directory.CreateDirectory(saveDirectory);
        using (var file = new Writer(Path.Combine(saveDirectory, "save.json")))
        {

            file.stream.WriteLine("\"data\":{");
            file.indent+=1;
            for (int i = 0; i < saveables.Count; i++)
            {
                var saveable = saveables[i];
                //remove everyone that was subscribed but has been deleted since
                if (!saveable.Item2.IsAlive)
                {
                    saveables.RemoveAt(i--);
                    continue;
                }
                file.writeValue(saveable.Item1, saveable.Item2.Target);
                if (i < saveables.Count - 1)
                {
                    file.stream.Write(",");
                }
            }
            file.indent--;
            file.stream.WriteLine("}");
        }
        Debug.Log("finish saving");
    }





}

interface ISaveable
{
    
}

class SaveAble : Attribute
{
}
