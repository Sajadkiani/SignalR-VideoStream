using System;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace VideoStreamServer.Hubs
{
    public class VideoStreamHub : Hub
    {
        private string path = Directory.GetCurrentDirectory() + "/Videos/Window.mp4";
        private string pathout = Directory.GetCurrentDirectory() + "/Videos/out.mp4";
        private static Channel<byte[]> channel;
        private int sectionSize = 10000;
        private static FileStream fileStream;
        private bool starter = false;

        public VideoStreamHub()
        {
            fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            channel = Channel.CreateUnbounded<byte[]>();
        }

        public void CallCounter()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            _ = Counter(cts.Token);

            starter = true;
        }

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
            CancellationToken cancellationToken
         )
        {
            // var channel = Channel.CreateUnbounded<byte[]>();
            if (starter) return channel.Reader;
            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = WriteItemsAsync(channel.Writer, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItemsAsync(
            ChannelWriter<byte[]> writer,
            CancellationToken cancellationToken
            )
        {
            Exception localException = null;
            try
            {
                // long lenght = fileStream.Length / (1024 * 1024);
                // var fs=new FileStream(pathout, FileMode.CreateNew, FileAccess.Write);
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
                        // await fs.WriteAsync(buffer,0,sectionSize);
                    }
                    // yield return buffer;

                    // Use the cancellationToken in other APIs that accept cancellation
                    // tokens so the cancellation can flow down to them.
                    await Task.Delay(1, cancellationToken);
                    Console.WriteLine("doing..");
                }


                // for (var i = 0; i < count; i++)
                // {
                //     await writer.WriteAsync(i, cancellationToken);

                //     // Use the cancellationToken in other APIs that accept cancellation
                //     // tokens so the cancellation can flow down to them.
                //     await Task.Delay(delay, cancellationToken);
                // }
                Console.WriteLine("Send complete.");
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