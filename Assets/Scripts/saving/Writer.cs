
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;

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
        File.Open(path,FileMode.Truncate).Close();
        stream = new StreamWriter(path, false);
        stream.WriteLine("{");
    }

    public void writePair<T>(string name, T value)
    {
        stream.Write(new string('\t', indent));
        stream.Write($"\"{name}\":");
        write<T>(value);
    }
    public void writePair(string name)
    {
        stream.Write(new string('\n', indent));
        stream.Write($"\"{name}\":null");
    }

    
    public void write<T>(T data)
    {
        var type = data.GetType();
        if (type.GetInterfaces().Contains(typeof(ISaveable)))
        {
            
        }
        stream.Write($"\"#Not Implemented data Type {typeof(T).Name}#\"");
    }

    private IEnumerable<MemberInfo> getSaveableMemebrs(Object obj)
    {
        return obj.GetType().GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(
            m=>m.GetCustomAttributes(typeof(SaveAble)).Any()
        );
    }

    public void writeObject<T>(T obj)where T:ISaveable
    {
        stream.WriteLine("{");
        indent++;
        bool first = true;
        var type=obj.GetType();
        foreach ()
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
                writePair(member.Name,((FieldInfo)member).GetValue(obj));
            }
            if (member.MemberType == MemberTypes.Property)
            {
                writePair(member.Name,((PropertyInfo)member).GetValue(obj));
            }
        }
        indent--;
        stream.WriteLine("}");
    }


    public void write(long num)
    {
        
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
