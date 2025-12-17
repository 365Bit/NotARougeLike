using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.InteropServices;
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
        // else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        // {
        //     saveDirectory = Path.Combine(new string[] { home, "Saved Games", "NotARogueLike" });
        // }
        // else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        // {
        //     saveDirectory = Path.Combine(new string[] { home, ".local", "share", "not_a_rogue_like" });
        // }
        else
        {
            throw new NotImplementedException("There is no savegame location implemented for the current operating system.\nSupported are: Linux and MS Windows");
        }
    }

    /// <summary>
    /// Subscribes an object to the savegame system. 
    /// This must be done before an object can be saved or loaded.
    /// Throws an exception if the object has no member that is marked as saveable.
    /// Only fields and properties are stored
    /// </summary>
    /// <typeparam name="T">The Type of the object to Save</typeparam>
    /// <param name="self">reference to the object. Note that no reference is kept so the object can be garbage collected.</param>
    /// <param name="name">the name under witch the object is stored. A name must be unique, throws an DuplicateNameException otherwise. Defaults to class name</param>
    public static void subscribe<T>(T self, string name = null)
    {
        if (name == null)
        {
            name = self.GetType().Name;
        }
        cleanUpSubscriptions();
        if (saveables.Any(s => { return s.Item1 == name; }))
        {
            return;
            throw new DuplicateNameException($"{name} already subscribed for gamesaves");
        }
        if (!Writer.getSaveableMemebrs(self).Any())
        {
            throw new UnsavableException($"The Object {self} has no member marked as saveable");
        }
        WeakReference reference = new WeakReference(self);
        saveables.Add((name, reference));
    }

    /// <summary>
    /// Stores all subscribed objects onto disk
    /// </summary>
    public static void save()
    {
        Debug.Log("start saving");
        Directory.CreateDirectory(saveDirectory);

        Dictionary<string, System.Object> toSave = new Dictionary<string, System.Object>();

        toSave.Add("save time", DateTime.Now.ToString());

        cleanUpSubscriptions();
        toSave.Add("data", (Dictionary<string, System.Object>)saveables.ToDictionary(s => s.Item1, s => s.Item2.Target));

        Writer.write(Path.Combine(saveDirectory, "save.json"), toSave);

        Debug.Log("finish saving");

    }

    public static void load()
    {
        Debug.Log("start loading");

        var loadPath = Path.Combine(saveDirectory, "save.json");
        if (!File.Exists(loadPath))
        {
            Debug.LogWarning($"No savegame in {loadPath}");
        }
        else
        {
            var loader = new Loader(loadPath);
            cleanUpSubscriptions();
            Debug.Log($"found save from {loader.loaded["save time"]}");
            var data = loader.loaded["data"];
            foreach (var saveable in saveables)
            {
                LoadedItem loadedObject;
                try
                {
                    loadedObject = data[saveable.Item1];
                }
                catch (IndexOutOfRangeException){continue;}

                foreach (MemberInfo m in Writer.getSaveableMemebrs(saveable.Item2.Target))
                {
                    System.Object obj = saveable.Item2.Target;
                    LoadedItem loadedValue;
                    try
                    {
                        loadedValue = loadedObject[m.Name];
                    }
                    catch (IndexOutOfRangeException) { continue; }

                    if (m.MemberType == MemberTypes.Field)
                    {
                        ((FieldInfo)m).SetValue(obj, loadedValue.getValue(((FieldInfo)m).FieldType));
                    }
                    else if (m.MemberType == MemberTypes.Property)
                    {
                        ((PropertyInfo)m).SetValue(obj,loadedValue.getValue(((PropertyInfo)m).PropertyType));
                    }
                    else
                    {
                        throw new UnsavableException($"Only Fields and Properties are sAveable, not {m.MemberType}");
                    }
                }

                Debug.Log($"Loaded {saveable.Item1}");
            }
        }

        Debug.Log("finish loading");

    }

    /// <summary>
    /// Removes all Subscriptions from Object that are no longer alive
    /// </summary>
    public static void cleanUpSubscriptions()
    {
        saveables.RemoveAll(s => { return !s.Item2.IsAlive; });

    }

}

interface ISaveable
{

}

class SaveAble : Attribute
{
    
}
