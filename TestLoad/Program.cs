using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NBomber.Configuration;
using NBomber.Contracts;
using NBomber.CSharp;
using Newtonsoft.Json;

namespace TestLoad
{
   
        class Program
        {
            private static int ii = 1;
            static void Main(string[] args)
            {



                var callDateNowStep = Step.Create("call datenow", async (context) =>
                {

                    Message body = new Message
                    {
                        hotelId =100//GetHotel()
                    };
                    var b = JsonConvert.SerializeObject(body);
                    using HttpClient client = new HttpClient();
                    var url = $"http://{Auth.Host}:8080/apicore";
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var data = new StringContent(b, Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = await client.PostAsync(url, data);
                    if (response.IsSuccessStatusCode)
                        return Response.Ok(statusCode: (int)response.StatusCode);
                    else
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        return Response.Fail(statusCode: (int)response.StatusCode);
                    }


                }, TimeSpan.FromSeconds(10));

                var scenario = ScenarioBuilder.CreateScenario("Call DateNow Api", callDateNowStep)
                    .WithWarmUpDuration(TimeSpan.FromSeconds(10))
                    .WithLoadSimulations(
                        LoadSimulation.NewInjectPerSec(_rate: 190, _during: TimeSpan.FromMinutes(1))
                    );

                NBomberRunner
                    .RegisterScenarios(scenario)
                    .WithReportFormats(ReportFormat.Html, ReportFormat.Md)
                    .Run();

                Console.WriteLine("Press any key ...");
                Console.ReadKey();
            }

            static int GetHotel()
            {
                if (ii < 100)
                {
                    Interlocked.Increment(ref ii);
                    return ii * 100;

                }
                else
                {
                    ii = 1;
                    return GetHotel();
                }
            }



        }

        class Message
        {

            public int messageId { get; set; } = 102034;
            public int hotelId { get; set; }
            public int posId { get; set; } = 1;
            public int formCode { get; set; }
            public int operationType { get; set; } = 1;
            public string totalSum { get; set; } = "444230,00";
            public int paymentMode { get; set; } = 1;

        }

        class MessageR
        {

            public string errorText { get; set; }
        }
}

