using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf_1._9.Enums;

namespace Wpf_1._9.Entity
{
    public class Data
    {

        public Data(decimal depoStart, StrategyType strategyType)
        {
            StrategyType = strategyType;
            Depo = depoStart;
        }


        public StrategyType StrategyType { get; set; }

        public decimal Depo
        {
            get => _depo;

            set
            {
                _depo = value;
                ResultDepo = value;
            }
        }
        decimal _depo;

        public decimal ResultDepo
        {
            get => _resultDepo;

            set
            {
                _resultDepo = value;
                Profit = ResultDepo - Depo;
                if (Depo != 0 )
                {
                    PercentProfit = Profit * 100 / Depo;
                }

                ListEquty.Add(ResultDepo);

                CalcDrawDawn();
            }
        }
        decimal _resultDepo;

        public decimal Profit { get; set; }

        public decimal PercentProfit { get; set; }

        public decimal MaxDrowDown
        {
            get => _maxDrowDown;

            set
            {
                _maxDrowDown = value;
                CalcPrecentDrawDawn();
            }
        }
        decimal _maxDrowDown;


        public decimal PercentDrowDown { get; set; }


        public List<decimal> ListEquty = new List<decimal>();

        private decimal _max = 0;
        private decimal _min = 0;

        private void CalcDrawDawn()
        {
            if (_max < ResultDepo)
            {
                _max = ResultDepo;
                _min = ResultDepo;
            }

            if (_min > ResultDepo)
            {
                _min = ResultDepo;
                if (MaxDrowDown < _max - _min)
                {
                    MaxDrowDown = _max - _min;
                }
            }
        }

        private void CalcPrecentDrawDawn()
        {
            decimal percent = MaxDrowDown * 100 / ResultDepo;
            if (percent > PercentDrowDown) PercentDrowDown = Math.Round(percent, 2);
        }

    }
}
