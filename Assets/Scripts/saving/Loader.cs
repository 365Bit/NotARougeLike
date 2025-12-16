using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        for (int read =' '; (read=reader.Read()) >= 0;)
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
        if (data.First() == '{' && data.Last() == '}')
        {
            var dict = new Dictionary<string, LoadedItem>();
            foreach (var line in data.Substring(1, data.Length - 1).Split(','))
            {
                var item = splitTuple(line);
                dict.Add(item.Item1,load(item.Item2));
            }
            return new LoadedItem(dict);
        }
        else if (data.First() == '[' && data.Last() == ']')
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

    private Tuple<string,string> splitTuple(string tuple)
    {
        bool inString=false;
        for(int i = 0; i < tuple.Length; i++)
        {
            if (tuple[i] == '"' && tuple.ElementAtOrDefault(i - 1) != '\\')
            {
                inString=!inString;
            }else if (!inString && tuple[i] == ':')
            {
                return new Tuple<string, string>(tuple.Substring(0,i),tuple.Substring(i+1));
            }
        }
        throw new LoadException($"The string\"{tuple}\" cannot be loaded as a tuple. No colon was found");
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
            if(!isDict())
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