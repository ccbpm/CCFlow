using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Silverlight;

namespace BP.En
{
    /// <summary>
    /// 实体类
    /// </summary>
    public class Entity
    {
        public DataRow HisDR = null;
        public string GetValStringByKey(string key)
        {
            return this.HisDR[key] as string;
        }
        public int GetValIntByKey(string key)
        {
            return  int.Parse( this.HisDR[key] as string);
        }
        public float GetValFloatByKey(string key)
        {
            return int.Parse(this.HisDR[key] as string);
        }
    }
    public class Entities : Collection<Entity>
    {
    }

    
    ///// <summary>
    ///// 集合类
    ///// </summary>
    //public class Entities : System.Collections.ICollection
    //{
    //    /// <summary>
    //    /// Entities
    //    /// </summary>
    //    public Entities()
    //    {
    //    }
    //    //int Add(object value);
    //    //void Clear();
    //    //bool Contains(object value);
    //    //int IndexOf(object value);
    //    //void Insert(int index, object value);
    //    //void Remove(object value);
    //    //void RemoveAt(int index);
    //    //bool IsFixedSize { get; }
    //    //bool IsReadOnly { get; }
    //    //object this[int index]
    //    //{
    //    //    get;
    //    //    set;
    //    //}
    //}
}
