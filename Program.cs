using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace TestVisionApp
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/quickstarts-sdk/client-library?tabs=visual-studio&pivots=programming-language-csharp#read-printed-and-handwritten-text
    /// </summary>
    class Program
    {
        private static IConfiguration _iconfiguration;        
        static void Main(string[] args)
        {
            string imageUrl = args[0];
            GetAppSettings();
            Task t = ReadTextFromImage(imageUrl);
            t.Wait();
        }

        public static async Task ReadTextFromImage(string imageUrl)
        {
            Console.WriteLine("Reading " + imageUrl);
            string endpoint = _iconfiguration.GetSection("EndPoint").Value;
            string subscriptionKey = _iconfiguration.GetSection("SubscriptionKey").Value;
            var agent = new VisionAgent(endpoint, subscriptionKey);
            var enumerable = agent.Read(imageUrl);
            var enumerator = enumerable.GetAsyncEnumerator();
            while (await enumerator.MoveNextAsync())
            {
                var item = enumerator.Current;
                Console.WriteLine(item);            
            }
        }

        static void GetAppSettings()
        {            
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }


    }
}
