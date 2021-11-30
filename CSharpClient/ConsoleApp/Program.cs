using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ConsoleApp
{
    class Program
    {
        private readonly HubConnection _hubConnection;

        public Program()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/pub-sub")
                .Build();
        }

        public async Task Start()
        {
            _hubConnection.On<string, object>("receiveMessage", (topic, data) => { });
            await _hubConnection.StartAsync();
        }


        static async Task Main(string[] args)
        {
            var program = new Program();
            await program.Start();
        }
    }
}