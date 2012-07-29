
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls.Charting;

namespace FxChart
{
    public partial class DoughnutPage
    {
        public DoughnutPage()
        {
            InitializeComponent();

            var definition = new DoughnutSeriesDefinition();
            definition.ItemStyle = this.Resources["CustomDoughnut"] as Style;
            definition.Appearance.StrokeThickness = 0;
            SeriesMapping m = new SeriesMapping {SeriesDefinition = definition};
            // { LegendDisplayMode = LegendDisplayMode.DataPointLabel };

            m.ItemMappings.Add(new ItemMapping("Count", DataPointMember.YValue));
            m.ItemMappings.Add(new ItemMapping("Type", DataPointMember.XCategory));
            m.ItemMappings.Add(new ItemMapping("Type", DataPointMember.Label));
            m.ItemMappings.Add(new ItemMapping("Type", DataPointMember.LegendLabel));

            //m.SeriesDefinition.ItemLabelFormat = "#XCAT\n#Y%";
            m.SeriesDefinition.ShowItemToolTips = true;

            m.SeriesDefinition.InteractivitySettings.HoverScope = InteractivityScope.Item;
            m.SeriesDefinition.InteractivitySettings.SelectionScope = InteractivityScope.Item;
            m.SeriesDefinition.InteractivitySettings.SelectionMode = ChartSelectionMode.Single;

            RadDoughnutChart.DefaultView.ChartLegend.BorderBrush = new SolidColorBrush(Colors.Transparent);

            //_DoughnutChart.InvDoughnutChart.SeriesMappings.Clear();
            RadDoughnutChart.SeriesMappings.Add(m);
            this.RadDoughnutChart.ItemsSource = GetData();
        }

        private IList<DoughnutData> GetData()
        {
            return new List<DoughnutData>()
                {
                    new DoughnutData() {Type = Type.Brazil, Count=10},
                    new DoughnutData() {Type = Type.China, Count=10},
                    new DoughnutData() {Type = Type.Italy, Count=5},
                    new DoughnutData() {Type = Type.GB, Count=20},
                    new DoughnutData() {Type = Type.USA, Count=15}
                }; 
        }
    }

    public class DoughnutData
    {
        public Type Type { get; set; }
        public int Count { get; set; }
    }
}
