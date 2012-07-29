using System;
using System.Collections.Generic;
using System.Globalization;
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
            BarChart.Palette = ChartPalettes.Arctic;

            this.SetLegend(BarLegend, BarChart.Palette, BarChart.Series[0]);

            var data = GetData();
            
            var summaryByDay = (from d in data
                                 group d by new {d.Type, d.ActivityDateTime.Date}
                                 into grp
                                        select new MultiBarChartSummary()
                                        {
                                            Type = grp.Key.Type,
                                            ActivityDate = grp.Key.Date,
                                            Count = grp.Count()
                                        }
                                        ).ToList().OrderBy(x => x.Type);

            this.BarChart.Series[0].ItemsSource = summaryByDay.Where(x => x.Type == Type.Brazil);
            this.BarChart.Series[1].ItemsSource = summaryByDay.Where(x => x.Type == Type.China);
            this.BarChart.Series[2].ItemsSource = summaryByDay.Where(x => x.Type == Type.Italy);
            this.BarChart.Series[3].ItemsSource = summaryByDay.Where(x => x.Type == Type.GB);
            this.BarChart.Series[4].ItemsSource = summaryByDay.Where(x => x.Type == Type.USA);

            var summary = (from d in data
                            group d by new { d.Type }
                                into grp
                                select new MultiBarChartSummary()
                                {
                                    Type = grp.Key.Type,
                                    Count = grp.Count()
                                }
                            ).ToList()
                            .OrderByDescending(x => x.Count);

            this.GoldCountTextBlock.Text = summary.ElementAt(0).Count.ToString();
            this.GoldDescriptionTextBlock.Text = summary.ElementAt(0).Type.ToString();

            this.SilverCountTextBlock.Text = summary.ElementAt(1).Count.ToString();
            this.SilverDescriptionTextBlock.Text = summary.ElementAt(1).Type.ToString();

            this.BronzeCountTextBlock.Text = summary.ElementAt(2).Count.ToString();
            this.BronzeDescriptionTextBlock.Text = summary.ElementAt(2).Type.ToString();
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
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(-16), Type = Type.China},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(-12), Type = Type.GB},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(-8), Type = Type.GB},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(-4), Type = Type.Brazil},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(-3), Type = Type.Brazil},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(-2), Type = Type.USA},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(8), Type = Type.Italy},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(10),Type = Type.GB},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(12), Type = Type.China},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(16), Type = Type.GB},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(10),Type = Type.GB},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(12), Type = Type.China},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(16), Type = Type.Brazil},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(10),Type = Type.GB},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(12), Type = Type.China},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(16), Type = Type.USA},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(10),Type = Type.USA},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(12), Type = Type.USA},
                    new MultiBarChartData() {ActivityDateTime = DateTime.Today.AddHours(16), Type = Type.USA}
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
        Brazil,
        China,
        Italy,
        GB,
        USA
    }
}
