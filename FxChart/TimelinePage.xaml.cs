using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls.Charting;

namespace FxChart
{
    public partial class TimelinePage 
    {
        public TimelinePage()
        {
            InitializeComponent();
            var seriesMapping = GetLineSeries();
            //RadChart1.SeriesMappings.Add(seriesMapping);



            RadChart1.DefaultView.ChartArea.AxisX.IsDateTime = true;
            RadChart1.DefaultView.ChartArea.AxisX.MinValue = DateTime.Today.AddDays(-1).ToOADate();
            RadChart1.DefaultView.ChartArea.AxisX.MaxValue = DateTime.Today.AddDays(1).ToOADate();
            RadChart1.ItemsSource = GetData();
        }

        private SeriesMapping GetLineSeries()
        {
            Style pathStyle1 = new Style(typeof (Path));
            //pathStyle1.Setters.Add(new Setter(Shape.StrokeDashArrayProperty, "1"));
            //pathStyle1.Setters.Add(new Setter(Shape.StrokeProperty, new SolidColorBrush(Colors.Cyan)));
            pathStyle1.Setters.Add(new Setter(Shape.StrokeThicknessProperty, 0));

            Style lineStyle1 = new Style(typeof (SelfDrawingSeries));
            lineStyle1.Setters.Add(new Setter(SelfDrawingSeries.BorderLineStyleProperty, pathStyle1));

            SeriesMapping seriesMapping = new SeriesMapping();
            seriesMapping.ItemMappings.Add(new ItemMapping("YValue", DataPointMember.YValue));
            seriesMapping.SeriesDefinition = new LineSeriesDefinition()
                                                 {SeriesStyle = lineStyle1, ShowItemLabels = true, ShowPointMarks = true};
            return seriesMapping;
        }

        private IList<TimelineData> GetData()
        {
            return new List<TimelineData>(10)
                {
                    new TimelineData() {ActivityTime = DateTime.Today.AddHours(-16), YValue = 1},
                    new TimelineData() {ActivityTime = DateTime.Today.AddHours(-12), YValue = 1},
                    new TimelineData() {ActivityTime = DateTime.Today.AddHours(-8), YValue = 1},
                    new TimelineData() {ActivityTime = DateTime.Today.AddHours(-4), YValue = 1},
                    new TimelineData() {ActivityTime = DateTime.Today.AddHours(8), YValue = 1},
                    new TimelineData() {ActivityTime = DateTime.Today.AddHours(10), YValue = 1},
                    new TimelineData() {ActivityTime = DateTime.Today.AddHours(12), YValue = 1},
                    new TimelineData() {ActivityTime = DateTime.Today.AddHours(-16), YValue = 1},
                }; 
        }
    }

    public class TimelineData
    {
        public int YValue { get; set; }
        public DateTime ActivityTime { get; set; }
    }
}
