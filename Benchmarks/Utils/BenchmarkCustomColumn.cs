using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks.Utils
{
    public class BenchmarkCustomColumn : IColumn
    {
        public string Id => $"{nameof(BenchmarkCustomColumn)}-{this.ColumnName}";
        public string ColumnName { get; private set; }
        public bool AlwaysShow => true;
        public ColumnCategory Category => ColumnCategory.Custom;
        public int PriorityInCategory => 1;
        public bool IsNumeric => true;
        public UnitType UnitType { get; private set; }
        public string Legend => "Custom Column";
        public bool IsAvailable(Summary summary) => true;
        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => true;

        public BenchmarkCustomColumn(string columnName, UnitType unitType)
        {
            ColumnName = columnName;
            UnitType = unitType;
        }

        public string? GetValue(Summary summary, BenchmarkCase benchmarkCase)
        {
            string benchmarkClassName = benchmarkCase.Descriptor.WorkloadMethod.DeclaringType.Name;
            string benchmarkMethodName = benchmarkCase.Descriptor.WorkloadMethod.Name;

            BenchmarkMetadata.Instance.Load(benchmarkClassName);
            var value = BenchmarkMetadata.Instance.GetMetdata(ColumnName, benchmarkMethodName, benchmarkCase.Parameters.PrintInfo);
            if (value is not null)
            {
                return value!.ToString();
            }
            else
                return "N/A";
        }

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
        {
            return GetValue(summary, benchmarkCase);
        }
    }
}
