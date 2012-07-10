using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows.Browser;

using FxChart.Models;

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
            GetHistoricData();
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

            var incYield = chartData.Union(coupon.Select(c => new ChartModel() { Age = c.Age, Rate = c.Rate, Yield = c.Yield}))
                .Select(x => x);

            var data = incYield.ToList();

            DataContext = data;
        }
    }
}