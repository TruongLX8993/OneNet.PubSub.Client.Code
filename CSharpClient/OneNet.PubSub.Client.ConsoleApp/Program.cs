using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OneNet.PubSub.Client.Models;

namespace OneNet.PubSub.Client.ConsoleApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var task = Task.Run(NewMethod);
            task.Wait();
        }

        private static async Task NewMethod()
        {
            var topicHandler = new TopicHandler(OnNewMessage, OnAbortTopic);
            var baseUrl = ConfigurationManager.AppSettings["baseUrl"];
            var pubSubConnection = new PubSubConnection(baseUrl, "truonglx");
            pubSubConnection.SetNewTopicHandler(OnNewTopic);
            pubSubConnection.SetCloseHandler(OnClose);

            await pubSubConnection.Start();

            await pubSubConnection.CreateTopic("topic-test", new TopicConfig()
            {
                IsKeepTopicWhenOwnerDisconnect = false,
                IsUpdateOwnerConnection = true,
            });

            var topics = await pubSubConnection.SearchTopic("topic-test");
            Console.WriteLine(JsonConvert.SerializeObject(topics));

            await pubSubConnection.SubscribeTopic("topic-test", topicHandler);
            while (true)
            {
                var data = Console.ReadLine();
                if (data == "exit")
                    break;
                await pubSubConnection.Publish("topic-test", data);
            }
        }

        private static void OnNewMessage(string topic, object data)
        {
            Console.WriteLine(data.ToString());
        }

        private static void OnNewTopic(Topic topic)
        {
            Console.WriteLine($"{nameof(OnNewTopic)}:{JsonConvert.SerializeObject(topic)}");
        }

        private static void OnAbortTopic(Topic topic)
        {
            Console.WriteLine($"Abort topic:{JsonConvert.SerializeObject(topic)}");
        }


        private static Task OnClose(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Task.CompletedTask;
        }
    }
}