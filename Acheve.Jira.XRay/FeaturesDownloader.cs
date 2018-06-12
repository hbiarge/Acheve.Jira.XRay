using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acheve.Jira.XRay.Options;

namespace Acheve.Jira.XRay
{
    public class FeaturesDownloader
    {
        private const string RelativePath = "rest/raven/1.0/export/test?fz=true&keys={0}";

        private readonly FeaturesDownloaderOptions _options;

        public FeaturesDownloader(FeaturesDownloaderOptions options)
        {
            _options = options;
        }

        public async Task DownloadAndExtractAsync()
        {
            var client = new HttpClient();

            var relativePath = string.Format(RelativePath, _options.TestPlan);

            var path = _options.Host.EndsWith("/")
                ? _options.Host + relativePath
                : _options.Host + "/" + relativePath;

            var request = new HttpRequestMessage(
                method: HttpMethod.Get,
                requestUri: path);

            Console.WriteLine($"Downloading features from [{_options.Host}]");
            Console.WriteLine($"  Jira Test Plan Id: {_options.TestPlan}");

            request.Headers.Authorization = new AuthenticationHeaderValue(
                scheme: "Basic",
                parameter: Helper.BuildBasicAuthentication(_options.UserName, _options.Password));

            try
            {
                Console.WriteLine($"  Calling: {path}");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode == false)
                {
                    Console.WriteLine($"Response is not success: {response.StatusCode} - {response.ReasonPhrase}");
                    return;
                }

                var zippedStream = await response.Content.ReadAsStreamAsync();

                Helper.ExtractZipStream(zippedStream, _options.TargetRelativeDirectory);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
