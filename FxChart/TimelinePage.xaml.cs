using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Charting;

namespace FxChart
{
    public partial class TimelinePage 
    {
        public TimelinePage()
        {
            InitializeComponent();
            InitializeChart();
            InitializeDateLabels();
        }

        private void InitializeDateLabels()
        {
            this.YesterdayTextBlock.Text = DateTime.Today.AddDays(-1).ToString("dd.MM.yyyy");
            this.TodayTextBlock.Text = DateTime.Today.ToString("dd.MM.yyyy");
            this.TomorrowTextBlock.Text = DateTime.Today.AddDays(1).ToString("dd.MM.yyyy");
        }

        private void InitializeChart()
        {
            this.RadChart1.LayoutUpdated += this.RadChart1_LayoutUpdated;

            var seriesMapping = GetLineSeries();
            RadChart1.SeriesMappings.Add(seriesMapping);

            RadChart1.Background = new SolidColorBrush(Colors.Transparent);
            RadChart1.DefaultView.ChartLegend.Visibility = Visibility.Collapsed;
            RadChart1.DefaultView.ChartArea.Padding= new Thickness(0);

            AxisX axisX = new AxisX()
                             {
                                 AutoRange = false,
                                 IsDateTime = true,
                                 Visibility = Visibility.Collapsed,
                                 MinValue = DateTime.Today.AddDays(-1).ToOADate(),
                                 MaxValue = DateTime.Today.AddDays(1).ToOADate()
                             };

            RadChart1.DefaultView.ChartArea.AxisX = axisX;

            AxisY axisY = new AxisY()
            {
                AutoRange = false,
                IsZeroBased = true,
                Visibility = Visibility.Collapsed,
                MinValue = 0,
                MaxValue = 4,
                StripLinesVisibility = Visibility.Collapsed
            };

            RadChart1.DefaultView.ChartArea.AxisY = axisY;
            RadChart1.ItemsSource = GetData();
        }

        void RadChart1_LayoutUpdated(object sender, EventArgs e)
        {
            var labelsPanel = this.RadChart1.FindChildByType<LabelsPanel>();
            if (labelsPanel == null || labelsPanel.Children.Count == 0)
                return;

            this.RadChart1.LayoutUpdated -= this.RadChart1_LayoutUpdated;

            foreach (SeriesItemLabel seriesItemLabel in labelsPanel.Children)
            {
                double leftAdjustment = -(seriesItemLabel.ActualWidth / 2 + 20);
                seriesItemLabel.Margin = new Thickness(leftAdjustment, -40, 0, 0);
            }
        }

        private SeriesMapping GetLineSeries()
        {
            Style pathStyle = new Style(typeof (Path));
            //pathStyle1.Setters.Add(new Setter(Shape.StrokeDashArrayProperty, "1"));
            pathStyle.Setters.Add(new Setter(Shape.StrokeThicknessProperty, 0));

            Style lineStyle = new Style(typeof (SelfDrawingSeries));
            lineStyle.Setters.Add(new Setter(SelfDrawingSeries.BorderLineStyleProperty, pathStyle));

            SeriesMapping seriesMapping = new SeriesMapping();
            seriesMapping.ItemMappings.Add(new ItemMapping("YValue", DataPointMember.YValue));
            seriesMapping.ItemMappings.Add(new ItemMapping("ActivityDateTime", DataPointMember.XValue));
            seriesMapping.ItemMappings.Add(new ItemMapping("LabelTime", DataPointMember.Label));

            var lineDefinition =  new LineSeriesDefinition() { SeriesStyle = lineStyle, ShowItemLabels = true, ShowPointMarks = true};
            lineDefinition.Appearance.PointMark.Fill = this.Resources["BlueBrush"] as SolidColorBrush;
            lineDefinition.PointMarkItemStyle =  this.Resources["CustomPointMark"] as Style;

            lineDefinition.SeriesItemLabelStyle =  this.Resources["SeriesItemLabelStyle"] as Style;
            seriesMapping.SeriesDefinition = lineDefinition;

            return seriesMapping;
        }

        private IList<TimelineData> GetData()
        {
            return new List<TimelineData>(10)
                {
                    new TimelineData() {ActivityDateTime = DateTime.Today.AddHours(-16), YValue = 1},
                    new TimelineData() {ActivityDateTime = DateTime.Today.AddHours(-12), YValue = 1},
                    new TimelineData() {ActivityDateTime = DateTime.Today.AddHours(-8), YValue = 1},
                    new TimelineData() {ActivityDateTime = DateTime.Today.AddHours(-4), YValue = 1},
                    new TimelineData() {ActivityDateTime = DateTime.Today.AddHours(8), YValue = 1},
                    new TimelineData() {ActivityDateTime = DateTime.Today.AddHours(10), YValue = 1},
                    new TimelineData() {ActivityDateTime = DateTime.Today.AddHours(12), YValue = 1},
                    new TimelineData() {ActivityDateTime = DateTime.Today.AddHours(16), YValue = 1},
                }; 
        }
    }

    public class TimelineData
    {
        public int YValue { get; set; }
        public DateTime ActivityDateTime { get; set; }

        public string LabelTime { get { return ActivityDateTime.ToString("h tt"); } }
    }
}
