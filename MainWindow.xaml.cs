using System;
using Wpf_1._9.Enums;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Wpf_1._9.Entity;
using System.Windows.Media;
using System.Linq;
using System.Windows.Shapes;

namespace Wpf_1._9
{

    public partial class MainWindow : Window
    {

        private List<Data> _datas;
        public MainWindow()
        {
            InitializeComponent();

            Init();            
        }

        List <StrategyType> _strategies = new List<StrategyType>()
        {
            StrategyType.FIX,
            StrategyType.CAPITALIZATION,
            StrategyType.PROGRESS,
            StrategyType.DOWNGRADE
        };

        Random _random = new Random();

        public void Init()
        {
            _comboBox.ItemsSource = _strategies;
            _comboBox.SelectionChanged += _comboBox_SelectionChanged;

            _comboBox.SelectedIndex = 0;

            _depo.Text = "100000";
            _startlot.Text = "10";
            _take.Text = "300";
            _stop.Text = "100";
            _comiss.Text = "5";
            _countTrades.Text = "1000";
            _percentProfit.Text = "30";
            _go.Text = "5000";
            _minStartPercent.Text = "20";
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _datas = Calculate();
            Draw(_datas);
        }
        private void _comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_datas != null && _datas.Any())
            {
                Draw(_datas);
            }
        }

        private List<Data> Calculate()
        {
            decimal depoStart = GetDecimalFromString(_depo.Text);
            decimal startLot = GetIntFromString(_startlot.Text);
            decimal take = GetDecimalFromString(_take.Text);
            decimal stop = GetDecimalFromString(_stop.Text);
            decimal comiss = GetDecimalFromString(_comiss.Text);
            decimal countTrades = GetIntFromString(_countTrades.Text);
            decimal percentProfit = GetDecimalFromString(_percentProfit.Text);
            decimal go = GetDecimalFromString(_go.Text);
            decimal minStartPercent = GetDecimalFromString(_minStartPercent.Text);

            List<Data> datas = new List<Data>();

            foreach (StrategyType type in _strategies)
            {
                datas.Add(new Data(depoStart, type));
            }
                      
            int lotPercent = (int)startLot;
            decimal percent = startLot * go * 100 / depoStart;

            decimal multiply = take / stop;
            int lotProgress = CalculateLot(depoStart, minStartPercent, go);

            int lotDown = (int)startLot;

            for (int i = 0; i < countTrades; i++)
            {
                int rnd = _random.Next(1, 100);

                if (rnd <= percentProfit)
                {
                    datas[0].ResultDepo += (take - comiss) * startLot;
                    datas[1].ResultDepo += (take - comiss) * lotPercent;

                    int newLot = CalculateLot(datas[1].ResultDepo, percent, go);

                    if (lotPercent < newLot) lotPercent = newLot;

                    datas[2].ResultDepo += (take - comiss) * lotProgress;

                    lotProgress = CalculateLot(depoStart, minStartPercent * multiply, go);

                    datas[3].ResultDepo += (take - comiss) * lotDown;
                    lotDown = (int)startLot;
                }
                else
                {
                    datas[0].ResultDepo -= (stop + comiss) * startLot;
                    datas[1].ResultDepo -= (stop + comiss) * lotPercent;

                    datas[2].ResultDepo -= (stop + comiss) * lotProgress;

                    lotProgress = CalculateLot(depoStart, minStartPercent, go);

                    datas[3].ResultDepo -= (stop + comiss) * lotDown;
                    lotDown /= 2;
                    if (lotDown == 0) lotDown = 1;
                }
            }

            _dataGrid.ItemsSource = datas;

            return datas;
        }

        private void Draw(List<Data> datas)
        {
            _canvas.Children.Clear();

            int index = _comboBox.SelectedIndex;

            List<decimal> listEquity = datas[index].ListEquty;

            int count = listEquity.Count;
            decimal maxEquity = listEquity.Max();
            decimal minEquity = listEquity.Min();

            double stepX = _canvas.ActualWidth / count;
            double koef = (double)(maxEquity - minEquity) / _canvas.ActualHeight;

            double x1 = 0;
            double y1 = 0;

            for (int i = 0; i < count; i++)
            {
                double x2 = i * stepX;
                double y2 = _canvas.ActualHeight - (double)(listEquity[i] - minEquity) / koef;

                Line line = new Line()
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = Brushes.Black,  // Цвет линии
                    StrokeThickness = 2      // Толщина линии
                };

                _canvas.Children.Add(line);

                x1 = x2;
                y1 = y2;
            }
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_datas != null && _datas.Any())
            {
                Draw(_datas);  // Перерисовываем график с новыми размерами
            }
        }

        private int CalculateLot(decimal currentDepo, decimal percent, decimal go)
        {
            if (percent > 100) { percent = 100; }
            decimal lot = currentDepo / go / 100 * percent;

            return (int)lot;
        }

        private decimal GetDecimalFromString(string str)
        {
            if (decimal.TryParse(str, out decimal result)) return result;

            return 0;

        }

        private int GetIntFromString(string str)
        {
            if (int.TryParse(str, out int result)) return result;

            return 0;

        }
    }

       
   
}
