using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using RunMode = BenchmarkDotNet.Jobs.RunMode;

Job job = new Job(InfrastructureMode.InProcess, RunMode.Default);

ManualConfig config = ManualConfig
                      .Create(DefaultConfig.Instance)
                      .AddJob(job)
                      .WithOptions(ConfigOptions.DisableLogFile)
                      .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig()));

Console.WriteLine("1. Run all");
Console.WriteLine("2. Pick specific benchmark");

char k = Console.ReadKey(true).KeyChar;

while (k < '0' && k > '2')
{
    Console.WriteLine("Invalid choice. Try again.");
    k = Console.ReadKey(true).KeyChar;
}

if (k == '1')
{
    config.AddColumn(new CategoriesColumn())
          .WithOptions(ConfigOptions.JoinSummary)
          .AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByCategory);

    BenchmarkRunner.Run(typeof(Program).Assembly, config, args);
}
else
{
    BenchmarkSwitcher
        .FromAssembly(typeof(Program).Assembly)
        .Run(args, config);
}