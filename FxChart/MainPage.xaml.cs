﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using FxChart.Models;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Charting;
using Range = FxChart.Models.Range;

namespace FxChart
{
    public partial class MainPage
    {
        private IList<Price> historic;
        private IList<Price> future;
        private IList<Range> range;
        private IList<Coupon> coupon;

        public MainPage()
        {
            InitializeComponent();

            //this.RadChart1.DefaultView.ChartArea.AnimationSettings = new AnimationSettings() { TotalSeriesAnimationDuration = new TimeSpan(0, 0, 0, 0, 500) };
            RadChart1.LayoutUpdated += this.RadChart1_LayoutUpdated;
            
            GetHistoricData();
        }

        private void RadChart1_LayoutUpdated(object sender, EventArgs e)
        {
            RadChart1.LayoutUpdated -= this.RadChart1_LayoutUpdated;

            var plotAreaPanel = this.RadChart1.DefaultView.ChartArea.ChildrenOfType<ClipPanel>().FirstOrDefault();
            if (plotAreaPanel == null) return;
            
            plotAreaPanel.MouseEnter += OnPlotAreaPanel_MouseEnter;
            plotAreaPanel.MouseMove += OnPlotAreaPanel_MouseMove;
            plotAreaPanel.MouseLeave += OnPlotAreaPanel_MouseLeave;

        }

        void OnPlotAreaPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            RadChart1.DefaultView.ChartArea.Annotations[0].Visibility = Visibility.Visible;
            RadChart1.DefaultView.ChartArea.Annotations[1].Visibility = Visibility.Visible;
        }

        private void OnPlotAreaPanel_MouseMove(object sender, MouseEventArgs e)
        {
            var plotAreaPanel = sender as ClipPanel;
            var position = e.GetPosition(plotAreaPanel);

            var x = this.RadChart1.DefaultView.ChartArea.AxisX.ConvertPhysicalUnitsToData(position.X);
            var y = this.RadChart1.DefaultView.ChartArea.AxisY.ConvertPhysicalUnitsToData(position.Y);

            ((CustomGridLine) RadChart1.DefaultView.ChartArea.Annotations[0]).XIntercept = x;
            ((CustomGridLine) RadChart1.DefaultView.ChartArea.Annotations[1]).YIntercept = y;

            this.textX.Text = string.Format("Age: {0:N0} days", x - 91);
            this.textY.Text = string.Format("Rate: {0:N2}", y);
        }

        private void OnPlotAreaPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            RadChart1.DefaultView.ChartArea.Annotations[0].Visibility = Visibility.Collapsed;
            RadChart1.DefaultView.ChartArea.Annotations[1].Visibility = Visibility.Collapsed;
        }

        private void GetHistoricData()
        {
            var web = new WebClient();
            web.OpenReadCompleted += (s, e) => 
            {
                historic = ConvertJson<IList<Price>>(e);
                GetFutureData();
            }; 
            web.OpenReadAsync(new Uri(HtmlPage.Document.DocumentUri, "HistoricService.ashx"));
        }

        private void GetFutureData()
        {
            var web = new WebClient();
            web.OpenReadCompleted += (s, e) =>
            {
                future = ConvertJson<IList<Price>>(e);
                GetRangeData();
            };
            web.OpenReadAsync(new Uri(HtmlPage.Document.DocumentUri, "FutureService.ashx"));
        }

        private void GetRangeData()
        {
            var web = new WebClient();
            web.OpenReadCompleted += (s, e) =>
                {
                    range = ConvertJson<IList<Range>>(e);
                    GetCouponData();
                };

            web.OpenReadAsync(new Uri(HtmlPage.Document.DocumentUri, "RangeService.ashx"));
        }

        private void GetCouponData()
        {
            var web = new WebClient();
            web.OpenReadCompleted += (s, e) =>
                {
                    coupon = ConvertJson<IList<Coupon>>(e);
                    SetupChartData();
                };

            web.OpenReadAsync(new Uri(HtmlPage.Document.DocumentUri, "CouponService.ashx"));
        }

        private T ConvertJson<T>(OpenReadCompletedEventArgs e)
        {
            if (e.Error != null) throw e.Error;
            var jsonSerializer = new DataContractJsonSerializer(typeof(T));
            return (T)jsonSerializer.ReadObject(e.Result);
        }

        private void SetupChartData()
        {
            var prices = 
                (historic.Select(h => new ChartModel() {Age = h.Age, Rate = h.Rate}))
                .Union(future.Select(f => new ChartModel() { Age = f.Age, Future = f.Rate}))
                .Select(x => new ChartModel() 
                    { 
                        Age= x.Age, 
                        Rate = x.Rate, 
                        Future = x.Future, 
                        LowerBottom = x.LowerBottom,
                        LowerTop = x.LowerTop,
                        UpperBottom = x.UpperBottom,
                        UpperTop = x.LowerTop
                    });

            var chartData = 
                from p in prices
                    join r in range on p.Age equals r.Age into temp from temp2 in temp.DefaultIfEmpty()
                select new ChartModel()
                    {
                        Age = p.Age,
                        Rate = p.Rate,
                        Future = p.Future,
                        LowerBottom = temp2 == null ? null : temp2.LowerBottom,
                        LowerTop = temp2 == null ? null : temp2.LowerTop ,
                        UpperBottom = temp2 == null ? null : temp2.UpperBottom,
                        UpperTop = temp2 == null ? null : temp2.UpperTop
                    };

            var withYield = chartData.Union(coupon.Select(c => new ChartModel() { Age = c.Age, Coupon = c}))
                                     .Select(x => x)
                                     .OrderBy(x => x.Age);

            var data = withYield.ToList();
            DataContext = data;
        }

        private void ChartArea_ItemClick(object sender, ChartItemClickEventArgs e)
        {
            if (e.DataPoint.BubbleSize == 0) return;
            
            // grab some of the datapoint info and fire an event (to display a new window)
            e.DataPoint.BubbleSize = e.DataPoint.BubbleSize * 5;

            var coupon = ((e.DataPoint.DataItem as ChartModel).Coupon as Coupon);

            MessageBox.Show(string.Format("Trade details:\n\nAge:\t{0} days\nRate:\t{1:N4}\nYield:\t{2} %", coupon.Age, coupon.Rate, coupon.Yield));
        }
    }
}
