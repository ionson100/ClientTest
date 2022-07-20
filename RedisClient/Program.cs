using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Oriflame.RedisMessaging.ReliableDelivery;
using StackExchange.Redis;

namespace RedisClient
{
    class Program
    {
        static async Task Main(string[] args)
        {


            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
                new ConfigurationOptions//45.8.145.232
                {
#if DEBUG
                    EndPoints = { "localhost:6379" },
#else
                    Password = Auth.Pwd,
                    EndPoints = { $"{Auth.Host}:6379" },
#endif

                    AbortOnConnectFail = false,
                });

            ISubscriber sub = redis.GetSubscriber();
            await sub.SubscribeAsync($"canel:{100}:{1}", async (channel, message) =>
            {
                _ = Task.Run(async () =>
                  {
                      var m = JsonConvert.DeserializeObject<WrapperMessage>(message);
                      await sub.PublishAsync($"canel:{100}:{1}:{m.Key}", "Ответ получен");
                      Console.WriteLine(m.Data);
                  });
                
            });
            Console.Read();
             await sub.UnsubscribeAsync($"canel:{100}:{1}");
            //redis.Dispose();

        }
        public class WrapperMessage
        {
            public string Key { get; set; }
            public string Data { get; set; }

        }
    }
}
