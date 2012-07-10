using System.IO;
using System.Web;

namespace FxChart.Web
{
    public class RangeService : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var dataPath = HttpContext.Current.Server.MapPath("DataFeeds/json/range.json");

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