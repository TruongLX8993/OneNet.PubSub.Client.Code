using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using OneNet.PubSub.Client.DTOs;
using OneNet.PubSub.Client.Exceptions;
using OneNet.PubSub.Client.Models;
using RestSharp;

namespace OneNet.PubSub.Client
{
    public class PubSubConnection : IAsyncDisposable
    {
        private readonly HubConnection _hub;
        private readonly IDictionary<string, TopicHandler> _handlersByTopicName;
        private readonly string _url;
        private Action<Topic> _newTopicHandler;
        private Func<Exception, Task> _closeHandler;


        public PubSubConnection(
            string url,
            string username)
        {
            url = url?.Trim();
            username = username?.Trim();

            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("Url is empty");
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username is not empty");

            url = url.EndsWith("/") ? url.Remove(url.Length - 1) : url;
            _url = url;

            _handlersByTopicName = new Dictionary<string, TopicHandler>();
            _hub = new HubConnectionBuilder()
                .WithUrl($"{url}/{Constance.HubName}?username={username}")
                // .WithAutomaticReconnect()
                .Build();

            _hub.Closed += async ex => { _closeHandler?.Invoke(ex); };

            _hub.On<string, object>(Constance.OnNewMessageMethod, (topicName, data) =>
            {
                if (!_handlersByTopicName.ContainsKey(topicName))
                    if (!_handlersByTopicName.ContainsKey(topicName))
                        return;
                _handlersByTopicName[topicName]
                    .OnNewMessage(topicName, data: data);
            });

            _hub.On<Topic>(Constance.OnAbortTopic, (topic) =>
            {
                if (!_handlersByTopicName.ContainsKey(topic.Name))
                    return;
                _handlersByTopicName[topic.Name]
                    .OnAbortTopic(topic);

                _handlersByTopicName.Remove(topic.Name);
            });


            _hub.On<Topic>(Constance.OnNewTopic, topic => { _newTopicHandler?.Invoke(topic); });
        }

        public void SetCloseHandler(Func<Exception, Task> closeHandler)
        {
            _closeHandler = closeHandler;
        }


        public async Task Start()
        {
            await _hub.StartAsync();
        }

        public async Task Stop()
        {
            await _hub.StopAsync();
        }


        public ValueTask DisposeAsync()
        {
            return _hub.DisposeAsync();
        }

        public async Task CreateTopic(string topicName, TopicConfig topicConfig)
        {
            try
            {
                await _hub.InvokeAsync(Constance.CreateTopic, topicName, topicConfig);
            }
            catch (HubException e) when (e.Message.Contains("not_exist"))
            {
                throw new NotExistTopicException();
            }
            catch (HubException e) when (e.Message.Contains("existed_topic"))
            {
                throw new ExistTopicException();
            }
            catch (Exception e)
            {
                throw new PubSubException("Server has error", e);
            }
        }

        public async Task AbortTopic(string topicName)
        {
            await _hub.InvokeAsync(Constance.AbortTopic, topicName);
        }

        public async Task SubscribeTopic(
            string topicName,
            TopicHandler topicHandler)
        {
            try
            {
                await _hub.InvokeAsync(Constance.SubscribeMethod, topicName);
                if (_handlersByTopicName.ContainsKey(topicName))
                    _handlersByTopicName.Remove(topicName);
                _handlersByTopicName.Add(topicName, topicHandler);
            }
            catch (HubException e) when (e.Message.Contains("not_exist"))
            {
                throw new NotExistTopicException();
            }
            catch (Exception e)
            {
                throw new PubSubException("Server has error", e);
            }
        }

        public async Task Publish(string topicName, object data)
        {
            await _hub.InvokeAsync(Constance.PublishMethod, topicName, data);
        }

        public async Task UnSubscribeTopic(string topicName)
        {
            await _hub.InvokeAsync(Constance.UnSubscribeMethod, topicName);
            if (_handlersByTopicName.ContainsKey(topicName))
            {
                _handlersByTopicName.Remove(topicName);
            }
        }

        public void SetNewTopicHandler(Action<Topic> handler)
        {
            _newTopicHandler = handler;
        }

        public async Task<IList<Topic>> SearchTopic(string topicName)
        {
            var restClient = new RestClient(_url);
            var request = new RestRequest(Constance.FindingTopicResource, Method.GET);
            request.AddQueryParameter("name", topicName);
            var res = await restClient.ExecuteAsync<ApiResponse<IList<Topic>>>(request);
            if (res.IsSuccessful)
                return res.Data.Data;
            throw new Exception(res.ErrorMessage);
        }
    }
}