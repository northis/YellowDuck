using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using YellowDuck.LearnChinese.Interfaces;

namespace YellowDuck.LearnChineseBotService.WebHook
{
    public class FlashCardController : ApiController
    {
        private readonly IWordRepository _repository;

        public FlashCardController(IWordRepository repository)
        {
            _repository = repository;
        }

        public IHttpActionResult Get(string fileId)
        {
            var id = fileId.Replace(".jpg", "");

            var longId = long.Parse(id);

            var file = _repository.GetWordFlashCard(longId);
            if (file == null)
                return new NotFoundResult(new HttpRequestMessage());

            return new ImageResult(file);
        }
    }

    public class ImageResult : IHttpActionResult
    {
        private readonly byte[] _imageBytes;

        public ImageResult(byte[] imageBytes)
        {
            _imageBytes = imageBytes;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(_imageBytes)
                };

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                return response;
            }, cancellationToken);
        }
    }
}