
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

class LoadedItem
{

    System.Collections.Generic.Dictionary<string, LoadedItem> dict;
    List<LoadedItem> list;

    string rawData;
    public LoadedItem(Dictionary<string, LoadedItem> data)
    {
        dict = data;
    }

    public LoadedItem(List<LoadedItem> data)
    {
        list = data;
    }

    public LoadedItem(string data)
    {
        rawData = data;
    }

    bool isDict()
    {
        return dict != null;
    }

    bool isList()
    {
        return list != null;
    }

    bool isValue()
    {
        return rawData != null;
    }

    public override string ToString()
    {
        if (isDict())
            return dict.ToString();
        else if (isList())
            return list.ToString();
        else
            return rawData;
    }

    public LoadedItem this[int key]
    {
        get
        {
            if (!isList())
                throw new LoadException($"{ToString()} is not a List");
            return list[key];
        }
    }


    public LoadedItem this[string key]
    {
        get
        {
            if (!isDict())
                throw new LoadException($"{ToString()} is not a Dictionary");
            return dict[key];
        }
    }

    public Object getValue(Type type)
    {
        if (rawData == "null")
        {
            return null;
        }
        if (typeof(IEnumerable<Object>).IsConvertibleTo(type,false))
        {
            
        }
        try
        {
            var value = System.Convert.ChangeType(this.rawData, type);
            if (value != null)
                return value;
        }
        catch (InvalidCastException) { }
        throw new LoadException($"cannot convert {ToString()} to type {type.Name}");
    }
}
