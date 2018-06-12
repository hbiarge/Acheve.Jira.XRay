using System;
using System.Collections.Generic;
using CommandLine;

namespace Acheve.Jira.XRay.Options
{
    [Verb("get-features", HelpText = "Download and extract features from Jira XRay.")]
    public class FeaturesDownloaderOptions
    {
        [Option('h', "host", Required = true, HelpText = "Jira host.")]
        public string Host { get; set; }

        [Option('u', "username", Required = true, HelpText = "User name.")]
        public string UserName { get; set; }

        [Option('p', "password", Required = true, HelpText = "Password.")]
        public string Password { get; set; }

        [Option('t', "test-plan", Required = true, HelpText = "Jira Test plan ids wich contains the features.")]
        public string TestPlan { get; set; }

        [Option('d', "directory", Required = true, HelpText = "Directory where downloaded features will be extracted.")]
        public string TargetRelativeDirectory { get; set; }
    }
}