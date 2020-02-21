using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace TempusHubBlazor.Services
{
    public class YoutubeAPIService
    {
        private YouTubeService _youtubeService;
        public YoutubeAPIService()
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
            var channelResponse = await channelRequest.ExecuteAsync();
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

                var uploadListResponse = await uploadListRequest.ExecuteAsync();
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
    }
}
