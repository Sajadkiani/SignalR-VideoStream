using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace VideoStreamServer.Hubs
{
    public class VideoStreamHub : Hub
    {
        private string path = Directory.GetCurrentDirectory() + "/Videos/Window.mp4";
        private static int counter = 0;
        // public async IAsyncEnumerable<int> Counter(
        //         int count,
        //         int delay,
        //         [EnumeratorCancellation]
        //          CancellationToken cancellationToken)
        // {
        //     // //   var zipBytes = await File.ReadAllBytesAsync(zipPath);
        //     // int sectionSize = 5000;
        //     // var fileStream = File.Open(path, FileMode.Open);
        //     // long lenght = fileStream.Length / (1024 * 1024);
        //     // for (int i = 0; i < lenght; i = i + sectionSize)
        //     // {
        //     //     // Check the cancellation token regularly so that the server will stop
        //     //     // producing items if the client disconnects.
        //     //     cancellationToken.ThrowIfCancellationRequested();

        //     //     var remainSize =lenght - i;
        //     //     if (remainSize < sectionSize)
        //     //         sectionSize = (int)remainSize;
        //     //     byte[] buffer = new byte[sectionSize];
        //     //     _ = await fileStream.ReadAsync(buffer, i, sectionSize);
        //     //     yield return buffer;

        //     //     // Use the cancellationToken in other APIs that accept cancellation
        //     //     // tokens so the cancellation can flow down to them.
        //     //      await Task.Delay(delay, cancellationToken);
        //     // }
        // }


        public ChannelReader<byte[]> Counter(
    int count,
    int delay,
    CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<byte[]>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = WriteItemsAsync(channel.Writer, count, delay, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItemsAsync(
            ChannelWriter<byte[]> writer,
            int count,
            int delay,
            CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                int sectionSize = 5000;
                // var fileStream = File.Open(path, FileMode.Open);
                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                // long lenght = fileStream.Length / (1024 * 1024);
                long lenght = fileStream.Length;
                for (int i = 0; i < lenght; i = i + sectionSize)
                {
                    // Check the cancellation token regularly so that the server will stop
                    // producing items if the client disconnects.
                    cancellationToken.ThrowIfCancellationRequested();

                    var remainSize = lenght - i;
                    if (remainSize < sectionSize)
                    {
                        sectionSize = (int)remainSize;
                        byte[] buffer = new byte[sectionSize];
                        _ = await fileStream.ReadAsync(buffer, 0, sectionSize);

                        await writer.WriteAsync(buffer, cancellationToken);
                    }
                    else
                    {
                        //  fileStream.Seek(i, SeekOrigin.Begin);
                        byte[] buffer = new byte[sectionSize];
                        _ = await fileStream.ReadAsync(buffer, 0, sectionSize);

                        await writer.WriteAsync(buffer, cancellationToken);
                    }
                    // yield return buffer;

                    // Use the cancellationToken in other APIs that accept cancellation
                    // tokens so the cancellation can flow down to them.
                    await Task.Delay(1, cancellationToken);
                }


                // for (var i = 0; i < count; i++)
                // {
                //     await writer.WriteAsync(i, cancellationToken);

                //     // Use the cancellationToken in other APIs that accept cancellation
                //     // tokens so the cancellation can flow down to them.
                //     await Task.Delay(delay, cancellationToken);
                // }
            }
            catch (Exception ex)
            {
                localException = ex;
            }
            finally
            {
                writer.Complete(localException);
            }
        }
    }
}