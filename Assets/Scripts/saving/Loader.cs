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
        bool inString=false;
        for (int read =' '; (read=reader.Read()) >= 0;)
        {
            char c = (char)read;
            if (c == '\\' &&inString)
            {
                read=reader.Read();
            }else if (c == '\"')
            {
                inString=!inString;
            }else if ((c == ' ' || c == '\t' || c =='\r' || c=='\n')&&!inString)
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
            foreach (var line in splitList(data.Substring(1, data.Length - 1)))
            {
                var item = splitTuple(line);
                dict.Add(item.Item1,load(item.Item2));
            }
            return new LoadedItem(dict);
        }
        else if (data.First() == '[' && data.Last() == ']')
        {
            var list = new List<LoadedItem>();
            foreach (var item in splitList(data.Substring(1, data.Length - 1)))
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
            string newData;
            if (data.First() == '\"' && data.Last() == '\"')
            {
                newData=data.Substring(1,data.Length-2);
            }
            else
            {
                newData=data;
            }
            return new LoadedItem(newData);
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
                return new Tuple<string, string>(tuple.Substring(0+1,i-2),tuple.Substring(i+1));
            }
        }
        throw new LoadException($"The string\"{tuple}\" cannot be loaded as a tuple. No colon was found");
    }

    private List<string> splitList(string data)
    {
        List<string> output = new List<string>();
        int laststart=0;
        bool inString=false;
        int dict=0;
        int list=0;
        for(int i = 0; i < data.Length; i++)
        {
            if (data[i] == '\\')
            {
                i++;
                continue;
            }else if (data[i] == '"')
            {
                inString=!inString;
            }else if (inString)
            {
                continue;
            }else if (data[i] == '{')
            {
                dict++;
            }else if (data[i] == '}')
            {
                dict--;
            }else if (data[i] == '[')
            {
                list++;
            }else if (data[i] == ']')
            {
                list--;
            }else if (data[i] == ','&&dict==0&&list==0)
            {
                output.Add(data.Substring(laststart,i-laststart-1));
                laststart=i+1;
            }
            else
            {
                
            }
        }
        output.Add(data.Substring(laststart,data.Length-laststart-1));
        return output;
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