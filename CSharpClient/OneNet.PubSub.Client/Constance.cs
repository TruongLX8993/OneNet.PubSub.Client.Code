namespace OneNet.PubSub.Client
{
    internal static class Constance
    {
        public const string HubName = "pub-sub";
        public const string SubscribeMethod = "subscribe";
        public const string UnSubscribeMethod = "un-subscribe";
        public const string PublishMethod = "publish";
        public const string OnNewMessageMethod = "onNewMessage";
        public const string OnNewTopic = "onNewTopic";
        public const string OnAbortTopic = "onAbortTopic";
        public const string AbortTopic = "abort-topic";
        public const string CreateTopic = "create-topic";
        public const string FindingTopicResource = "/api/topic/search";
    }
}