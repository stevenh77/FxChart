﻿using System.IO;
using System.Web;

namespace FxChart.Web
{
    public class FutureService : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var dataPath = HttpContext.Current.Server.MapPath("DataFeeds/json/future.json");

            using (var reader = new StreamReader(dataPath))
            {
                var result = reader.ReadToEnd();
                context.Response.ContentType = "text/json";
                context.Response.Write(result);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}