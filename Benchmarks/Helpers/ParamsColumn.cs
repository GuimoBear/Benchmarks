using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmarks.Helpers;

public class ParamColumn : IColumn
{
    public string Id => nameof(ParamColumn);
    public string ColumnName { get; } = "Params";
    public string Legend => "The parameters used in benchmarks";

    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;
    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
    {
        var type = benchmarkCase.Descriptor.WorkloadMethod.DeclaringType;
        if (!benchmarkCase.HasParameters)
            return "no parameters";
        var parameters = new string[benchmarkCase.Parameters.Count];
        for(int i = 0; i < benchmarkCase.Parameters.Count; i++)
        {
            var parameter = benchmarkCase.Parameters[i];
            parameters[i] = parameter.ToString();
        }
        return string.Join(", ", parameters);
    }

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) => GetValue(summary, benchmarkCase);

    public bool IsAvailable(Summary summary) => summary.BenchmarksCases.Any(bc => bc.HasParameters);
    public bool AlwaysShow => true;
    public ColumnCategory Category => ColumnCategory.Job;
    public int PriorityInCategory => -10;
    public bool IsNumeric => false;
    public UnitType UnitType => UnitType.Dimensionless;
    public override string ToString() => ColumnName;
}
