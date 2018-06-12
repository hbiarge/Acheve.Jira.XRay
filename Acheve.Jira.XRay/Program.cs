using System.Threading.Tasks;
using Acheve.Jira.XRay.Options;
using CommandLine;

namespace Acheve.Jira.XRay
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<FeaturesDownloaderOptions, ResultsUploaderOptions>(args)
                .WithParsed<FeaturesDownloaderOptions>(featuresDownloaderOptions =>
                {
                    var downloader = new FeaturesDownloader(featuresDownloaderOptions);

                    downloader.DownloadAndExtractAsync().GetAwaiter().GetResult();
                })
                .WithParsed<ResultsUploaderOptions>(resultsUploaderOptions =>
                {
                    var uploader = new ResultsUploader(resultsUploaderOptions);

                    uploader.UpdateResultsAsync().GetAwaiter().GetResult();
                });
        }
    }
}
