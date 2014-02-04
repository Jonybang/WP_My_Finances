using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using Finances.ClasesForLists;
using System.Globalization;
using System.Runtime.Serialization;

namespace Finances
{
    public class AccountsObservableRangeCollection 
    {
        public AccountsObservableRangeCollection()
        { }

        public AccountsObservableRangeCollection(IOrderedEnumerable<ArticleProperty> obj)
        {
            AccountsList = new ObservableRangeCollection<ArticleProperty>();
            foreach (ArticleProperty aP in obj)
                AccountsList.Add(new ArticleProperty
                { 
                    ID=aP.ID,
                    Name = aP.Name,
                    TapNumber = aP.TapNumber,
                    Summ = aP.Summ
                });
            this.SetRecordDayStr();
        }

        public AccountsObservableRangeCollection(ObservableRangeCollection<ArticleProperty> obj, string recordDayStr="")
        {
            if (((ObservableRangeCollection<ArticleProperty>)obj).Count() == 0)
            {
                AccountsList = new ObservableRangeCollection<ArticleProperty>
                        {
                            new ArticleProperty("На руках",1),
                            new ArticleProperty("Сбербанк",2),
                        };
                SetRecordDayStr();
                return;
            }

            AccountsList = new ObservableRangeCollection<ArticleProperty>();

            foreach (ArticleProperty aP in obj)
                AccountsList.Add(new ArticleProperty
                {
                    ID = aP.ID,
                    Name = aP.Name,
                    TapNumber = aP.TapNumber,
                    Summ = aP.Summ
                });
            if(recordDayStr=="")
                SetRecordDayStr();
            else
                RecordDayStr = recordDayStr;            
        }

        public ObservableRangeCollection<ArticleProperty> AccountsList { get; set; }

        #region TotalSumm
        private double totalSumm;
        public double TotalSumm
        {
            get
            {
                totalSumm = 0;
                foreach (ArticleProperty articleProperty in AccountsList)
                    totalSumm += articleProperty.Summ;

                return totalSumm;
            }
            set
            {
                totalSumm=value;
            }
        }
        #endregion

        public string RecordDayStr { get; set; }
        public string FullDateTimeStr { get; set; }

        public string SetRecordDayStr(int n = 0)
        {
            FullDateTimeStr = DateTime.Today.AddDays(-n).ToString("g", new CultureInfo("ru-RU"));
            return RecordDayStr = DateTime.Today.AddDays(-n).ToString("d MMM", new CultureInfo("ru-RU"));
        }

        public DateTime GetRecordDay()
        {
            return DateTime.Parse(FullDateTimeStr);
        }
    }
}
