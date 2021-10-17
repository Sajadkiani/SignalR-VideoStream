using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace VideoStreamClient.SignalReader
{
    public class VideoStreamReader
    {
        private readonly HubConnection hubConnection;
        public VideoStreamReader()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/streamHub")
                .Build();
        }

        public async Task ReadAndWriteInConsoleAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            await hubConnection.StartAsync();
            var stream = hubConnection.StreamAsync<int>(
                "Counter", 10, 500, cancellationTokenSource.Token);

            await WriteInConsoleAsync(stream);
            Console.WriteLine("Streaming completed");
        }

        private async Task WriteInConsoleAsync(IAsyncEnumerable<int> stream)
        {
            await foreach (var count in stream)
            {
                Console.WriteLine($"{count}");
            }
        }
    }
}