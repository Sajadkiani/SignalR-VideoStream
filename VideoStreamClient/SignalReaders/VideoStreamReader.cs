using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace VideoStreamClient.SignalReader
{
    public class VideoStreamReader
    {
        private readonly HubConnection hubConnection;
        private string path = Directory.GetCurrentDirectory() + "/ReciveFiles/";
        public VideoStreamReader()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/streamHub")
                .Build();
        }

        // public async Task ReadAndWriteInConsoleAsync()
        // {

        //     // Call "Cancel" on this CancellationTokenSource to send a cancellation message to
        //     // the server, which will trigger the corresponding token in the hub method.
        //     var cancellationTokenSource = new CancellationTokenSource();
        //     await hubConnection.StartAsync();
        //     var channel = await hubConnection.StreamAsChannelAsync<int>(
        //         "Counter", 3000, 4000, cancellationTokenSource.Token);

        //     // Wait asynchronously for data to become available
        //     while (await channel.WaitToReadAsync())
        //     {
        //         // Read all currently available data synchronously, before waiting for more data
        //         while (channel.TryRead(out var count))
        //         {
        //             Console.WriteLine($"{count}");
        //         }
        //     }

        //     Console.WriteLine("Streaming completed");
        // }

        public async Task ReadAndWriteInConsoleAsync()
        {
            // Call "Cancel" on this CancellationTokenSource to send a cancellation message to
            // the server, which will trigger the corresponding token in the hub method.
            // var cancellationTokenSource = new CancellationTokenSource();
            // await hubConnection.StartAsync();
            // var channel = await hubConnection.StreamAsChannelAsync<string>(
            //     "Counter", 3000, 4000, cancellationTokenSource.Token);

            // // Wait asynchronously for data to become available
            // while (await channel.WaitToReadAsync())
            // {
            //     // Read all currently available data synchronously, before waiting for more data
            //     while (channel.TryRead(out var count))
            //     {
            //         Console.WriteLine($"{count}");
            //     }
            // }


            // var cancellationTokenSource = new CancellationTokenSource();
            // await hubConnection.StartAsync();
            // var connectionStream = hubConnection.StreamAsync<byte[]>(
            //     "Counter", 3000, 500, cancellationTokenSource.Token);

            var cancellationTokenSource = new CancellationTokenSource();
            await hubConnection.StartAsync();
            var channel = await hubConnection.StreamAsChannelAsync<byte[]>(
                "Counter", 3000, 4000, cancellationTokenSource.Token);

            if(!Directory.Exists(path))
             Directory.CreateDirectory(path);

            string fullPath = path + "recived.mp4";

            //if it doesnt exist will be made
            //otherwise will be open
            FileStream fileStream = new FileStream(fullPath, FileMode.Append, FileAccess.Write);

            await WriteInConsoleAsync(fileStream, channel);

            fileStream.Close();
            Console.WriteLine("Streaming completed");
        }

        private async Task WriteInConsoleAsync(FileStream fileStream,ChannelReader<byte[]> channel)
        {
            try
            {
                while (await channel.WaitToReadAsync())
                {
                    // Read all currently available data synchronously, before waiting for more data
                    while (channel.TryRead(out var bytes))
                    {
                        await fileStream.WriteAsync(bytes);
                        Console.WriteLine(new Random().Next());
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}