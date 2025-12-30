
using System;
using System.Collections.Generic;

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

        if (type.IsArray)
        {
            if (!isList())
            {
                throw new LoadException($"{ToString()} is not a List");
            }

            Type elementType = type.GetElementType();
            Array array = Array.CreateInstance(elementType, list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                array.SetValue(list[i].getValue(elementType), i);
            }

            return array;
        }

        try
        {
            return Convert.ChangeType(rawData, type);
        }
        catch
        {
            throw new LoadException($"cannot convert {ToString()} to type {type.Name}");
        }
    }
}
