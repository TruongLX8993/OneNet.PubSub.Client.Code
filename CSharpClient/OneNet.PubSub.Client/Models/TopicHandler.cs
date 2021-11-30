using System;

namespace OneNet.PubSub.Client.Models
{
    public class TopicHandler
    {
        private readonly Action<string, object> _newMessageHandler;
        private readonly Action<Topic> _abortTopicHandler;

        public TopicHandler(Action<string, object> newMessageHandler, Action<Topic> abortTopicHandler)
        {
            _newMessageHandler = newMessageHandler;
            _abortTopicHandler = abortTopicHandler;
        }

        internal void OnNewMessage(string topicName, object data)
        {
            _newMessageHandler?.Invoke(topicName, data);
        }

        internal void OnAbortTopic(Topic topic)
        {
            _abortTopicHandler?.Invoke(topic);
        }
    }
}