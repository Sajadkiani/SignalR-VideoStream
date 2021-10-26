using System.Text;
using System;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using FFmpeg.NET;

namespace VideoStreamServer.Hubs
{
    public class VideoStreamHub : Hub
    {
        private string path = Directory.GetCurrentDirectory() + "/Videos/Window.mp4";
        private string pathout = Directory.GetCurrentDirectory() + "/Videos/out.mp4";
        private static Channel<string> channel;
        private int sectionSize = 10000;
        private static FileStream fileStream;
        private bool starter = false;

        public VideoStreamHub()
        {
            fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            channel = Channel.CreateUnbounded<string>();
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


        public ChannelReader<string> Counter(
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

        // private async Task WriteItemsAsync(
        //     ChannelWriter<byte[]> writer,
        //     CancellationToken cancellationToken
        //     )
        // {
        //     Exception localException = null;
        //     string strBytes = string.Empty;
        //     try
        //     // long lenght = fileStream.Length / (1024 * 1024);
        //     {
        //         if (File.Exists(pathout))
        //             File.Delete(pathout);
        //         var fs = new FileStream(pathout, FileMode.CreateNew, FileAccess.Write);
        //         long lenght = fileStream.Length;
        //         for (int i = 0; i < lenght; i = i + sectionSize)
        //         {
        //             // Check the cancellation token regularly so that the server will stop
        //             // producing items if the client disconnects.
        //             cancellationToken.ThrowIfCancellationRequested();

        //             var remainSize = lenght - i;
        //             if (remainSize < sectionSize)
        //             {
        //                 sectionSize = (int)remainSize;
        //                 byte[] buffer = new byte[sectionSize];
        //                 _ = await fileStream.ReadAsync(buffer, 0, sectionSize);
        //                 await writer.WriteAsync(buffer, cancellationToken);
        //             }
        //             else
        //             {
        //                 //  fileStream.Seek(i, SeekOrigin.Begin);
        //                 byte[] buffer = new byte[sectionSize];
        //                 _ = await fileStream.ReadAsync(buffer, 0, sectionSize);
        //                 await writer.WriteAsync(buffer, cancellationToken);

        //                 // await fs.WriteAsync(buffer,0,sectionSize);
        //             }
        //             // yield return buffer;

        //             // Use the cancellationToken in other APIs that accept cancellation
        //             // tokens so the cancellation can flow down to them.
        //             await Task.Delay(1, cancellationToken);
        //             Console.WriteLine("doing..");
        //         }


        //         // for (var i = 0; i < count; i++)
        //         // {
        //         //     await writer.WriteAsync(i, cancellationToken);

        //         //     // Use the cancellationToken in other APIs that accept cancellation
        //         //     // tokens so the cancellation can flow down to them.
        //         //     await Task.Delay(delay, cancellationToken);
        //         // }
        //         // strBytes = strBytes.Replace('-', '+');
        //         // strBytes = strBytes.Replace('_', '/');
        //         // strBytes = strBytes.Replace('=', '/');
        //         // strBytes = strBytes.Replace(':', '/');
        //         var t = Convert.FromBase64String(strBytes);
        //         await fs.WriteAsync(t, 0, t.Length);
        //         Console.WriteLine("Send complete.");
        //     }
        //     catch (Exception ex)
        //     {
        //         localException = ex;
        //     }
        //     finally
        //     {
        //         writer.Complete(localException);
        //     }
        // }

         private async Task WriteItemsAsync(
            ChannelWriter<string> writer,
            CancellationToken cancellationToken
            )
        {
            Exception localException = null;
            // string strBytes = string.Empty;
            try
            // long lenght = fileStream.Length / (1024 * 1024);
            {
                if (File.Exists(pathout))
                    File.Delete(pathout);
                // var fs = new FileStream(pathout, FileMode.CreateNew, FileAccess.Write);
                  await ReadChunkss(path,writer,cancellationToken);
                // for (var i = 0; i < count; i++)
                // {
                //     await writer.WriteAsync(i, cancellationToken);

                //     // Use the cancellationToken in other APIs that accept cancellation
                //     // tokens so the cancellation can flow down to them.
                //     await Task.Delay(delay, cancellationToken);
                // }
                // strBytes = strBytes.Replace('-', '+');
                // strBytes = strBytes.Replace('_', '/');
                // strBytes = strBytes.Replace('=', '/');
                // strBytes = strBytes.Replace(':', '/');
                // var t = Convert.FromBase64String(strBytes);
                // await fs.WriteAsync(t, 0, t.Length);
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

        public async Task ReadChunkss(string path, ChannelWriter<string> writer, CancellationToken cancellationToken)
        {
            var inputFile = new MediaFile(path);
            var outputFile = new MediaFile(pathout);
            int chunkSize=300; //5 minute
            int startPosition=0;
            var ffmpeg = new Engine("D:/ffmpeg/bin/ffmpeg.exe");
            var options = new ConversionOptions();
            var metadata = await ffmpeg.GetMetaDataAsync(inputFile);
            var duration=metadata.Duration.TotalSeconds;
            RemoveOutPutFile();
            
            while (startPosition < duration)
            {
                options.CutMedia(TimeSpan.FromSeconds(startPosition), TimeSpan.FromSeconds(chunkSize));
                await ffmpeg.ConvertAsync(inputFile, outputFile, options);

                using(var file = File.OpenRead(pathout))
                {
                    var buffer = new byte[chunkSize];
                    int readed = file.Read(buffer, 0, buffer.Length);
                    var base64File = Convert.ToBase64String(buffer);
                    await writer.WriteAsync(base64File);
                    // RemoveOutPutFile();
                }

                startPosition = startPosition + chunkSize;
                // await Task.Delay(2000, cancellationToken);
            }
            // This example will create a 25 second video, starting from the 
            // 30th second of the original video.
            //// First parameter requests the starting frame to cut the media from.
            //// Second parameter requests how long to cut the video.
        }

        private void RemoveOutPutFile()
        {
            if (File.Exists(pathout))
                File.Delete(pathout);
        }

        public async Task ReadChunks(string path, ChannelWriter<string> writer)
        {
            // var size = 10000;      
            string strBytes = string.Empty;
            const int chunkSize = 30000; // read the file by chunks of 1KB
            // var fs = new FileStream(pathout, FileMode.CreateNew, FileAccess.Write);
            using (var file = File.OpenRead(path))
            {
                var buffer1 = new byte[file.Length];
                var file1 = File.OpenRead(path);
                int readed = file1.Read(buffer1, 0, buffer1.Length);
                var str = Convert.ToBase64String(buffer1);
                await writer.WriteAsync(str);
                // return;
                
                string str1="";
                int bytesRead;
                var count=Math.Ceiling((decimal)file.Length/chunkSize); 
                for (int i = 1; i <= count; i++)
                {
                    if (i < count)
                    {
                        var buffer = new byte[chunkSize];
                        bytesRead = file.Read(buffer, 0, buffer.Length);
                        strBytes = Convert.ToBase64String(buffer);
                        str1=str1+strBytes;
                        await writer.WriteAsync(strBytes);
                        // var t = Convert.FromBase64String(strBytes);
                        // fs.Write(t, 0, bytesRead);
                    }
                    else
                    {
                        var remain = file.Length - ((i - 1) * chunkSize)+1;
                        var buffer = new byte[remain];
                        bytesRead = file.Read(buffer, 0, buffer.Length);
                        strBytes = Convert.ToBase64String(buffer);
                        str1=str1+strBytes;
                        await writer.WriteAsync((strBytes+""));
                        // var t = Convert.FromBase64String(strBytes);
                        // fs.Write(t, 0, bytesRead);
                    }
                }

                if(str==(str1+"="))
                {
                    int x=0;
                }

                // while ((bytesRead = file.Read(buffer, 0, buffer.Length)) > 0)
                // {
                //       strBytes=Convert.ToBase64String(buffer);
                //       var t = Convert.FromBase64String(strBytes);
                //       fs.Write(buffer, 0, bytesRead);
                // }
            }
        }
    }
}