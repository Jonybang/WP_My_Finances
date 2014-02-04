using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Finances
{
    public class LastStatus
    {
        //Класс для сериализации текущей информации на главном окне
        [XmlIgnore()] 
        private double currentSumm;

        public double CurrentSumm
        {
            get
            {
                currentSumm = 0;
                try
                {
                    foreach (Account acc in AccountList)
                        currentSumm += acc.Summ;
                }
                catch (NullReferenceException)
                {
                }               
                return currentSumm; 
            }
            set
            {
                currentSumm = value;
            }
        }

        public ObservableCollection<Account> AccountList { get; set; }

        public LastStatus() { }
        [XmlIgnore()] 
        private static LastStatus lastStatus = new LastStatus();

        public static LastStatus GetLastStatus()
        {
            if (lastStatus.AccountList == null)
                lastStatus.AccountList = new ObservableCollection<Account>();
            return lastStatus;
        }
        public static void SetLastStatus(LastStatus lS)
        {
            lastStatus = lS;
        }
    }
}
