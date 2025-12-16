using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Loader
{

    public LoadedItem loaded
    {
        get;
        private set;
    }
    StreamReader reader;

    public Loader(string path)
    {
        var content = new StringBuilder((int)new FileInfo(path).Length);
        reader = new StreamReader(path);
        for (var read = reader.Read(); read >= 0;)
        {
            char c = (char)read;
            if (c == ' ' || c == '\t' || c =='\r' || c=='\n')
            {
                continue;
            }
            content.Append(c);
        }
        reader.Close();
        loaded = load(content.ToString());
    }

    private LoadedItem load(string data)
    {
        if (data[0] == '{' && data[-1] == '}')
        {
            var dict = new Dictionary<string, LoadedItem>();
            foreach (var line in data.Substring(1, data.Length - 2).Split(','))
            {
                var item = line.Split(':');
                dict.Add(item[0], load(item[1]));
            }
            return new LoadedItem(dict);
        }
        else if (data[0] == '[' && data[-1] == ']')
        {
            var list = new List<LoadedItem>();
            foreach (var item in data.Substring(1, data.Length - 2).Split(','))
            {
                list.Add(load(item));
            }
            return new LoadedItem(list);
        }else if (data == "null")
        {
            return null;
        }
        else
        {
            throw new LoadException("could not deserialize:\n" + data);
        }
    }

}

class LoadedItem
{

    Dictionary<string,LoadedItem> dict;
    List<LoadedItem> list;

    string rawData;
    public LoadedItem(Dictionary<string, LoadedItem> data)
    {
        dict=data;
    }

    public LoadedItem(List<LoadedItem> data)
    {
        list=data;
    }

    public LoadedItem(string data)
    {
        rawData=data;
    }

    bool isDict()
    {
        return dict!=null;
    }

    bool isList()
    {
        return list!=null;
    }

    bool isValue()
    {
        return rawData!=null;
    }

    string dataToString()
    {
        if(isDict())
            return dict.ToString();
        else if(isList())
            return list.ToString();
        else
            return rawData;
    }

    public LoadedItem this[int key]
    {
        get
        {
            if(!isList())
                throw new LoadException($"{dataToString()} is not a List");
            return list[key];
        }
    }


    public LoadedItem this[string key]
    {
        get
        {
            if(!isList())
                throw new LoadException($"{dataToString()} is not a Dictionary");
            return dict[key];
        }
    }

    T convertTo<T>()
    {
        throw new LoadException($"cannot convert {dataToString()} to type {typeof(T).Name}");
    }
}

class LoadException : Exception
{
    public LoadException()
    {

    }

    public LoadException(string message) : base(message)
    {

    }
}