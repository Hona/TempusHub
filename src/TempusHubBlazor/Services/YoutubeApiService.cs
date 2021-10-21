using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace TempusHubBlazor.Services
{
    public sealed class YoutubeApiService : IDisposable
    {
        private readonly YouTubeService _youtubeService;
        public YoutubeApiService()
        {
            var baseGoogleService = new BaseClientService.Initializer()
            {
                ApiKey = Environment.GetEnvironmentVariable("YOUTUBE_API_KEY")
            };
            _youtubeService = new YouTubeService(baseGoogleService);
        }
        public async Task<List<PlaylistItem>> GetAllVideosAsync(string channelParam, bool isId = false, int pages = -1)
        {
            var output = new List<PlaylistItem>();

            var channelRequest = _youtubeService.Channels.List("contentDetails");

            // Specify the channel
            if (isId)
            {
                channelRequest.Id = channelParam;
            }
            else
            {
                channelRequest.ForUsername = channelParam;
            }

            // Get the response
            var channelResponse = await channelRequest.ExecuteAsync().ConfigureAwait(false);
            var channel = channelResponse.Items.First();
            var uploadPlaylistId = channel.ContentDetails.RelatedPlaylists.Uploads;

            var pagesRead = 0;
            var nextUploadPageToken = "";
            while (nextUploadPageToken != null)
            {
                var uploadListRequest = _youtubeService.PlaylistItems.List("snippet");
                uploadListRequest.PlaylistId = uploadPlaylistId;
                uploadListRequest.MaxResults = 50;
                uploadListRequest.PageToken = nextUploadPageToken;

                var uploadListResponse = await uploadListRequest.ExecuteAsync().ConfigureAwait(false);
                var uploads = uploadListResponse.Items;

                output.AddRange(uploads);

                // Get the next page
                nextUploadPageToken = uploadListResponse.NextPageToken;
                pagesRead++;
                if (pages != -1 && pages <= pagesRead)
                {
                    return output;
                }
            }

            return output;
        }

        public void Dispose()
        {
            _youtubeService?.Dispose();
        }
    }
}
