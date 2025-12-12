
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

class Writer : IDisposable
{
    public StreamWriter stream
    {
        private set;
        get;
    }
    bool disposed = false;
    public int indent = 0;

    public Writer(string path)
    {
        stream = new StreamWriter(path, false);
        stream.WriteLine("{");
    }

    public void writeValue<T>(string name, T value)
    {
        stream.Write(new string('\n', indent));
        stream.Write($"\"{name}\":");
        write(value);
    }
    public void writeValue(string name)
    {
        stream.Write(new string('\n', indent));
        stream.Write($"\"{name}\":null");
    }

    public void write(ISaveable obj)
    {
        stream.WriteLine("{");
        indent++;
        bool first = true;
        var type=obj.GetType();
        foreach (var member in type.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            if (!member.GetCustomAttributes(typeof(SaveAble)).Any())
            {
                continue;
            }
            if (first)
            {
                first = false;
            }
            else
            {
                stream.WriteLine(",");
            }
            if (member.MemberType == MemberTypes.Field)
            {
                writeValue(member.Name,((FieldInfo)member).GetValue(obj));
            }
            if (member.MemberType == MemberTypes.Property)
            {
                writeValue(member.Name,((PropertyInfo)member).GetValue(obj));
            }
        }
        indent--;
        stream.WriteLine("}");
    }



    public void write<T>(IList<T> collection)
    {
        stream.WriteLine("[");
        foreach (T item in collection)
        {
            write(item);
        }
        stream.WriteLine("]");
    }

    public void write(long num)
    {
        
    }

    public void write<T>(T data)
    {
        stream.Write($"#Not Implemented data Type {typeof(T).Name}");
    }


    public void Dispose()
    {
        if (disposed)
            return;

        stream.WriteLine("}");
        stream.Close();
    }

    ~Writer()
    {
        Dispose();
    }

}
