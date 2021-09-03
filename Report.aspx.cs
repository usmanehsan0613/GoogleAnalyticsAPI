using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Web.Services;
using System.Web.Script.Services;

using System.Web.Script.Serialization;

public partial class Report : System.Web.UI.Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        //Response.Write(Server.MapPath("~"));
        // MyGoogle gs = new MyGoogle();
        //  Response.Write(String.Format("{0:n0}", Convert.ToInt32(gs.FetchResults("/main/ar/index.asp"))));
        // FetchResults();
    }

    [WebMethod]
    [ScriptMethod]
    public static JsResponse FetchData(ClientData data)
    {
        JsResponse res = new JsResponse();

        res.html = data.page_path;
        res.responseCode = 200;
         MyGoogle gs = new MyGoogle();
       
        res.html = String.Format("{0:n0}", Convert.ToInt32(gs.FetchResults(data.page_path)));

        return res;
    }

    


    public class JsResponse
    {
        public string html;
        public int responseCode;

        public JsResponse()
        {

        }
    }

    public class ClientData
    {
        public string page_path;
    }

    public class MyGoogle
    {
        


        public string FetchResults(string pageP)
        {
            List<string> metricV = new List<string>();
           
            string metricVC = "0"; string StartDateInput = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var filterClause = new DimensionFilterClause();

            var filter = new DimensionFilter
            {
                DimensionName = "ga:pagepath",
                Operator__ = "EXACT",
                Expressions = new List<string>
                {
                    (string) pageP
                } 
            };

             
            filterClause.Filters = new List<DimensionFilter> { filter };

            /*var metrics = new List<Metric>();
            metrics.Add(new Metric { Expression = "ga:avgSessionDuration", Alias = "Avg. Session Duration" });
            metrics.Add(new Metric { Expression = "ga:sessions", Alias = "Sessions" });
            metrics.Add(new Metric { Expression = "ga:pageviewsPerSession", Alias = "Pageviews Per Session" }); 
            */

			string srvieFile = @"D:\googleAPI2\files\service-account-credentials.json"; 

            var credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(srvieFile)
                .CreateScoped(new[] { Google.Apis.AnalyticsReporting.v4.AnalyticsReportingService.Scope.AnalyticsReadonly });

            using (var analytics = new Google.Apis.AnalyticsReporting.v4.AnalyticsReportingService(new Google.Apis.Services.BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            }))
            {
                var request = analytics.Reports.BatchGet(new GetReportsRequest
                {
                    // StartDate = "2021-06-01", EndDate = "2021-06-20" }
                    ReportRequests = new[] {
                        new ReportRequest{
                            DateRanges = new[] { new DateRange{ StartDate = StartDateInput, EndDate = "today" } },
                            Dimensions = new[] { new Dimension{ Name = "ga:pagepath" } },
                            Metrics = new[] { new Metric{ Expression = "ga:pageviews", Alias = "Path Views"}},
                            DimensionFilterClauses = new List<DimensionFilterClause> { filterClause },

                            ViewId = ""
                        }
                    }
                });
                try
                {

                    var response = request.Execute();

                    foreach (var row in response.Reports[0].Data.Rows)
                    {
                         foreach (var metric in row.Metrics)
                        {
                             metricVC = string.Join("", metric.Values);
                        }



                    }
                }
                catch (HttpRequestException ex)
                {
                    return ex.Message;
                }

                return metricVC;

            }
			
         }
    }

}