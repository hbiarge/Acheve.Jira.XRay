using System.Collections.Generic;
using CommandLine;

namespace Acheve.Jira.XRay.Options
{
    [Verb("upload-results", HelpText = "Upload returls and executions Jira XRay.")]
    public class ResultsUploaderOptions
    {
        [Option('h', "host", Required = true, HelpText = "Jira host.")]
        public string Host { get; set; }

        [Option('u', "username", Required = true, HelpText = "User name.")]
        public string UserName { get; set; }

        [Option('p', "password", Required = true, HelpText = "Password.")]
        public string Password { get; set; }

        [Option('t', "test-plan", Required = true, HelpText = "Jira Test plan id to bind the results.")]
        public string TestPlan { get; set; }

        [Option('f', "file", Required = true, HelpText = "The XML report path to upload.")]
        public string File { get; set; }
    }
}