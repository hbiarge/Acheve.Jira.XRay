using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acheve.Jira.XRay.Options;

namespace Acheve.Jira.XRay
{
    public class ResultsUploader
    {
        private const string RelativePath = "rest/raven/1.0/import/execution/nunit?testPlanKey={0}";

        private readonly ResultsUploaderOptions _options;

        public ResultsUploader(ResultsUploaderOptions options)
        {
            _options = options;
        }

        public async Task UpdateResultsAsync()
        {
            var client = new HttpClient();

            var relativePath = string.Format(RelativePath, _options.TestPlan);
            var path = Helper.BuildUri(_options.Host, relativePath);

            var request = new HttpRequestMessage(
                method: HttpMethod.Post,
                requestUri: path);

            request.Headers.Authorization = new AuthenticationHeaderValue(
                scheme: "Basic",
                parameter: Helper.BuildBasicAuthentication(_options.UserName, _options.Password));
            var content = new MultipartContent
            {
                new ByteArrayContent(File.ReadAllBytes(_options.File))
            };

            request.Content = content;

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode == false)
            {
                // Error
            }
            else
            {
                Console.WriteLine($"Results successfully uploaded to {_options.Host}");
            }
        }
    }
}