using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Finances
{
    public class Account
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Summ { get; set; }

        public Account(int id)
        {
            ID = id;
            Name = "Name";
            Summ = 0;
        }

        public Account()
        {
        }
    }
}
