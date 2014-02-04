using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Finances.ClasesForLists
{
    public class AccountsComparison
    {
        public AccountsComparison()
        {
            CurrentName = currAcc.Name;
            CurrentSumm = currAcc.Summ;
            PreviouslySumm = prevAcc.Summ;
        }

        public double CurrentSumm { get; set; }
        public double PreviouslySumm { get; set; }

        public string CurrentName { get; set; }
    }
}
