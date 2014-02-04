using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Finances.ClasesForLists
{
    public class Article
    {
        [XmlIgnore()] 
        bool isIncome;

        public int NameID { get; set; }        
        public string Name { get; set; }
        public int AccountID { get; set; }
        public string Account { get; set; }
        public double Number { get; set; }
        public int DateID { get; set; }
        public bool IsIncome {
            get
            {
                return isIncome;
            }
            set
            {
                isIncome = value;

                if (isIncome)
                    ArrowPath = new Uri("/Images/green_arrow.png", UriKind.Relative).ToString();
                else
                    ArrowPath = new Uri("/Images/red_arrow.png", UriKind.Relative).ToString();
            }
        }
        public string ArrowPath { get; set; }

        [XmlIgnore]
        public DateTime ADateTime { get; set; }

        // For serialization.
        [XmlElement]
        public string FullDateTime
        {
            get { return ADateTime.ToString("g", new CultureInfo("ru-RU")); }
            set { ADateTime = DateTime.Parse(value); }
        }

        public string StrDate 
        { 
            get 
            {
                ADateTime = DateTime.Parse(FullDateTime);
                return ADateTime.ToString("d MMM", new CultureInfo("ru-RU"));
            }
        }
    }
}
