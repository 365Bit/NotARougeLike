
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;

class Writer : IDisposable
{
    private StreamWriter stream;
    bool disposed = false;
    public int indent = 0;

    private Writer(string path)
    {
        try
        {
            File.Open(path, FileMode.Truncate).Close();
        }
        catch (FileNotFoundException) { }

        stream = new StreamWriter(path, false);
    }

    void writePair(string name, System.Object value)
    {
        stream.Write(new string('\t', indent));
        stream.Write($"\"{name}\":");
        write(value);
    }


    public static void write(string path, Dictionary<string, System.Object> data)
    {
        using (Writer writer = new Writer(path))
        {
            writer.write(data);
        }
    }

    void write(System.Object data)
    {
        var type = data.GetType();
        if (data == null)
        {
            stream.Write("null");
        }
        else if (data is string str)
        {
            stream.Write($"\"{str}\"");
        }
        else if (data is bool value)
        {
            stream.Write('"');
            if (value)
                stream.Write("true");
            else
                stream.Write("false");
            stream.Write('"');
        }
        else if (data is IConvertible num)
        {
            stream.Write($"\"{num.ToString()}\"");
        }
        else if (data is System.Collections.Generic.Dictionary<string, Object> dict)
        {
            writeDict(dict);
        }
        else if (data is System.Collections.IEnumerable list)
        {
            writeList(list);
        }
        else if (getSaveableMemebrs(data).Count() > 0)
        {
            writeObject(data);
        }
        else
        {
            stream.Write($"\"#Not Implemented data Type {data.GetType().Name}#\"");
        }
    }

    private IEnumerable<MemberInfo> getSaveableMemebrs(Object obj)
    {
        return obj.GetType().GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(
            m => m.GetCustomAttributes(typeof(SaveAble)).Any()
        );
    }

    void writeObject(Object obj)
    {
        writeDict((System.Collections.Generic.Dictionary<string, Object>)getSaveableMemebrs(obj).ToDictionary(m => m.Name, m =>
        {
            if (m.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)m).GetValue(obj);
            }
            else if (m.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo)m).GetValue(obj);
            }
            else
            {
                throw new UnsavableException($"Only Fields and Properties are sAveable, not {m.MemberType}");
            }
        }));
    }

    void writeDict(Dictionary<string, Object> dict)
    {
        stream.WriteLine("{");
        indent++;
        bool first = true;
        var type = dict.GetType();
        foreach (var item in dict)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                stream.WriteLine(",");
            }
            writePair(item.Key, item.Value);
        }
        indent--;
        stream.WriteLine(new string('\t', indent) + "}");
    }

    void writeList(System.Collections.IEnumerable collection)
    {
        stream.WriteLine("[");
        indent++;
        bool first = true;
        foreach (var item in collection)
        {
            if (!first)
                stream.WriteLine(",");
            first = false;
            stream.Write(new string('\t', indent));
            write(item);
        }
        stream.WriteLine("]");
    }




    public void Dispose()
    {
        if (disposed)
            return;

        stream.Close();
    }

    ~Writer()
    {
        Dispose();
    }

}


class UnsavableException : Exception
{
    public UnsavableException()
    {

    }

    public UnsavableException(string message) : base(message)
    {

    }

    public UnsavableException(string message, Exception innerException) : base(message, innerException)
    {

    }
}