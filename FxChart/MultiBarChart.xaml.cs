using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using Telerik.Windows.Controls.ChartView;

namespace FxChart
{
    public partial class MultiBarChart
    {
        
        public MultiBarChart()
        {
            InitializeComponent();
            BarChart.Palette = ChartPalettes.Metro;

            this.SetLegend(BarLegend, BarChart.Palette, BarChart.Series[0]);

            var data = GetData();
            
            var summary = (from d in data
                         group d by new {d.Type, d.ActivityDateTime.Date}
                         into grp
                                select new MultiBarChartSummary()
                                {
                                    Type = grp.Key.Type,
                                    ActivityDate = grp.Key.Date,
                                    Count = grp.Count()
                                }).ToList();


            this.BarChart.Series[0].ItemsSource = summary.Where(x => x.Type == Type.Option);
            this.BarChart.Series[1].ItemsSource = summary.Where(x => x.Type == Type.BRC);
            this.BarChart.Series[2].ItemsSource = summary.Where(x => x.Type == Type.Finer);
            this.BarChart.Series[3].ItemsSource = summary.Where(x => x.Type == Type.Futures);
            this.BarChart.Series[4].ItemsSource = summary.Where(x => x.Type == Type.FinerS);
        }

        private void SetLegend(StackPanel legend, ChartPalette palette, ChartSeries series)
        {
            int index = 0;
            foreach (StackPanel panel in legend.Children)
            {
                Rectangle rectangle = panel.Children[0] as Rectangle;
                PaletteEntry? entry = palette.GetEntry(series, index);

                if ((rectangle != null) && (entry != null)) 
                    rectangle.Fill = entry.Value.Fill;

                index++;
            }
        }

         private IList<MultiBarChartData> GetData()
        {
            return new List<MultiBarChartData>()
                {
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(-16), Type = Type.FinerS},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(-12), Type = Type.BRC},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(-8), Type = Type.BRC},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(-4), Type = Type.Finer},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(-3), Type = Type.Finer},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(-2), Type = Type.Futures},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(8), Type = Type.Option},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(10),Type = Type.BRC},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(12), Type = Type.FinerS},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(16), Type = Type.BRC},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(10),Type = Type.BRC},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(12), Type = Type.FinerS},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(16), Type = Type.Finer},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(10),Type = Type.BRC},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(12), Type = Type.FinerS},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(16), Type = Type.Futures},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(10),Type = Type.Futures},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(12), Type = Type.Futures},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(16), Type = Type.Futures}
                }; 
        }
    }

    public class MultiBarChartSummary
    {
        public int Count{ get; set; }
        public Type Type { get; set; }
        public DateTime ActivityDate { get; set; }
    }

    public class MultiBarChartData
    {
        public DateTime ActivityDateTime { get; set; }
        public Type Type { get; set; }
    }

    public enum Type
    {
        Finer,
        FinerS,
        Option,
        BRC,
        Futures
    }
}
