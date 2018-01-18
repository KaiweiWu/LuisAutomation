using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LuisAutomation
{
    class Program
    {
        
        // NOTE: Replace this example LUIS application ID with the ID of your LUIS application.
        static string appID = "3d31a77f-4049-48d9-80b3-f6c8692554f4";

        // NOTE: Replace this example LUIS application version number with the version number of your LUIS application.
        static string appVersion = "0.2.7.9";

        // NOTE: Replace this example LUIS programmatic key with a valid key.
        static string key = "3680509bd75b497188662f3a4f98ff55";

        static string host = "	https://westus.api.cognitive.microsoft.com";
        static string path = "/luis/api/v2.0/apps/" + appID + "/versions/" + appVersion + "/";

        static string usage = @"Usage: program <input file>, program -train <input file>, program -status The contents of <input file> must be in the format described at: https://aka.ms/add-utterance-json-format";

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine(usage);
                Console.ReadLine();
            }
            else
            {
                if (true == String.Equals(args[0], "-format", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length > 1)
                    {
                        Format(args[1]);
                    }
                    else
                    {
                        Console.WriteLine(usage);
                    }
                }

                if (true == String.Equals(args[0], "-train", StringComparison.OrdinalIgnoreCase))
                {
                    if (args.Length > 1)
                    {
                        Train(args[1]).Wait();
                    }
                    else
                    {
                        Console.WriteLine(usage);
                    }
                }

                else if (true == String.Equals(args[0], "-status", StringComparison.OrdinalIgnoreCase))
                {
                    Status().Wait();
                }
                else
                {
                    AddUtterances(args[0]).Wait();
                }
            }

        }

        async static Task<HttpResponseMessage> SendGet(string uri)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(uri);
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                return await client.SendAsync(request);
            }
        }

        async static Task<HttpResponseMessage> SendPost(string uri, string requestBody)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);  
                request.Content = new StringContent(requestBody, Encoding.UTF8, "text/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                return await client.SendAsync(request);
            }
        }

        async static Task AddUtterances(string input_file)
        {
            string uri = host + path + "examples";
            string requestBody = File.ReadAllText("../../Files/" + input_file);

            var response = await SendPost(uri, requestBody);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Added utterances.");
            Console.WriteLine(ProgramHelper.JsonPrettyPrint(result));
            Console.ReadLine();
        }

        private static void Format(string input_file)
        {
            string requestBody = File.ReadAllText("../../Files/" + input_file);
        }

        async static Task Train(string input_file)
        {
            string uri = host + path + "train";
            string requestBody = File.ReadAllText("../../Files/" + input_file);

            var response = await SendPost(uri, requestBody);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Sent training request.");
            Console.WriteLine(ProgramHelper.JsonPrettyPrint(result));
            await Status();
        }

        async static Task Status()
        {
            var response = await SendGet(host + path + "train");
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Requested training status.");
            Console.WriteLine(ProgramHelper.JsonPrettyPrint(result));
            Console.ReadLine();
        }
    }
}
