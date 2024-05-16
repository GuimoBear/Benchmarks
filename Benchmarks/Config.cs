using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using Benchmarks.Helpers;
using Benchmarks.Utils;

namespace Benchmarks;

public class Config : ManualConfig
{
    public const int Iterations = 200;

    public Config()
    {
        AddLogger(ConsoleLogger.Default);

        AddExporter(CsvExporter.Default);
        AddExporter(MarkdownExporter.GitHub);
        AddExporter(HtmlExporter.Default);

        var md = MemoryDiagnoser.Default;
        AddDiagnoser(md);
        AddColumn(new DescriptionColumn());
        AddColumn(TargetMethodColumn.Method);
        AddColumn(new Helpers.ParamColumn());
        //AddColumn(new ReturnColumn());
        AddColumn(StatisticColumn.Mean);
        AddColumn(new BenchmarkCustomColumn("Iterations count", UnitType.Size)); //  Your custom column
        AddColumn(new BenchmarkCustomColumn("Maybe exists", UnitType.Size)); //  Your custom column
        AddColumn(new BenchmarkCustomColumn("Not exists", UnitType.Size)); //  Your custom column
        AddColumn(StatisticColumn.StdDev);
        AddColumn(StatisticColumn.Error);
        AddColumn(BaselineRatioColumn.RatioMean);
        AddColumnProvider(DefaultColumnProviders.Metrics);

        AddJob(Job.Default
               .WithLaunchCount(1)
               .WithWarmupCount(0)
               .WithUnrollFactor(Iterations)
               .WithIterationCount(5)
        ); 
        Orderer = new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest);
        Options |= ConfigOptions.JoinSummary;
    }
}
