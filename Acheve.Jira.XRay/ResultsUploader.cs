using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Acheve.Jira.XRay.Options;
using Newtonsoft.Json.Linq;

namespace Acheve.Jira.XRay
{
    public class ResultsUploader
    {
        private const string TestResultsRelativePath = "rest/raven/1.0/import/execution/cucumber";
        private const string TestExecutionRelativePath = "rest/raven/1.0/api/testplan/{0}/testexecution";

        private readonly ResultsUploaderOptions _options;

        public ResultsUploader(ResultsUploaderOptions options)
        {
            _options = options;
        }

        public async Task UpdateResultsAsync()
        {
            var client = new HttpClient();

            var (success, testExecution) = await UploadResults(client);

            if (success)
            {
                await CreateTestExecution(client, testExecution);
            }
        }

        private async Task<(bool success, string testExecution)> UploadResults(HttpClient client)
        {
            var testResultsPath = Helper.BuildUri(_options.Host, TestResultsRelativePath);

            var testResultsRequest = new HttpRequestMessage(
                method: HttpMethod.Post,
                requestUri: testResultsPath);

            var resultFileFullPath = Path.GetFullPath(_options.File);

            Console.WriteLine($"Uploading result to [{_options.Host}]");
            Console.WriteLine($"  File: {resultFileFullPath}");

            testResultsRequest.Headers.Authorization = new AuthenticationHeaderValue(
                scheme: "Basic",
                parameter: Helper.BuildBasicAuthentication(_options.UserName, _options.Password));

            testResultsRequest.Content = new StringContent(
                content: File.ReadAllText(resultFileFullPath),
                encoding: Encoding.UTF8,
                mediaType: "application/json");

            try
            {
                Console.WriteLine($"  Calling: {testResultsPath}");

                var response = await client.SendAsync(testResultsRequest);

                if (response.IsSuccessStatusCode == false)
                {
                    Console.WriteLine($"Response is not success: {response.StatusCode} - {response.ReasonPhrase}");
                    return (success: false, testExecution: null);
                }

                Console.WriteLine($"Results successfully uploaded to {_options.Host}");

                var content = JObject.Parse(await response.Content.ReadAsStringAsync());

                return (success: true, testExecution: content["testExecIssue"].Value<string>("key"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (success: false, testExecution: null);
            }
        }

        private async Task CreateTestExecution(HttpClient client, string testExecutionId)
        {
            var relativePath = string.Format(TestExecutionRelativePath, _options.TestPlan);
            var path = Helper.BuildUri(_options.Host, relativePath);

            var request = new HttpRequestMessage(
                method: HttpMethod.Post,
                requestUri: path);

            Console.WriteLine($"Adding test execution {testExecutionId} to test plan [{_options.TestPlan}]");

            request.Headers.Authorization = new AuthenticationHeaderValue(
                scheme: "Basic",
                parameter: Helper.BuildBasicAuthentication(_options.UserName, _options.Password));

            request.Content = new StringContent(
                content: "{\"add\": [\"" + testExecutionId + "\"]}",
                encoding: Encoding.UTF8,
                mediaType: "application/json");

            try
            {
                Console.WriteLine($"  Calling: {path}");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode == false)
                {
                    Console.WriteLine($"Response is not success: {response.StatusCode} - {response.ReasonPhrase}");
                    return;
                }

                Console.WriteLine($"Test execution successfully added");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}