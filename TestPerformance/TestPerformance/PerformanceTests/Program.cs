// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<Tests.Tests>(DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator));

