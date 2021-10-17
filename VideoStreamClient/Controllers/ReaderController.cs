using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VideoStreamClient.SignalReader;

namespace VideoStreamClient.Controller
{
    [Route("api/reader")]
    public class ReaderController : ControllerBase
    {
        private readonly VideoStreamReader _reader;
        public ReaderController()
        {
            _reader = new VideoStreamReader();
        }

        [HttpGet]
        public Task Get()
        {
           return _reader.ReadAndWriteInConsoleAsync();
        }
    }
}