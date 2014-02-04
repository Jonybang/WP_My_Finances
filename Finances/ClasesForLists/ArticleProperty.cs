using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Finances.ClasesForLists
{
    public class ArticleProperty : IComparable
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int TapNumber { get; set; }

        public double Summ { get; set; }

        public ArticleProperty(string name, int id)
        {
            Summ = 0;
            TapNumber = 0;
            Name = name;
            ID = id;
        }
        public ArticleProperty()
        {
        }
        public int CompareTo(object obj)
        {
            ArticleProperty artProp = obj as ArticleProperty;
            if (artProp == null)
            {
                throw new ArgumentException("Object is not Article Name");
            }
            return this.ID.CompareTo(artProp.ID);
        }
    }
}

