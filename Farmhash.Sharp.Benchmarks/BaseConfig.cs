using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;

namespace Farmhash.Sharp.Benchmarks
{
    public class BaseConfig : ManualConfig
    {
        public BaseConfig()
        {
            Add(new CsvExporter(CsvSeparator.CurrentCulture,
                new BenchmarkDotNet.Reports.SummaryStyle
                {
                    PrintUnitsInContent = false,
                    TimeUnit = BenchmarkDotNet.Horology.TimeUnit.Nanosecond
                }));

            Add(StatisticColumn.Mean);
            Add(StatisticColumn.StdErr);
            Add(StatisticColumn.StdDev);
            Add(StatisticColumn.Median);

            // dotnet cli toolchain supports only x64 compilation
            Add(new Job("core-64bit", EnvironmentMode.Core));

            Add(new Job("net-legacy-32bit", EnvironmentMode.LegacyJitX86));
            Add(new Job("net-legacy-64bit", EnvironmentMode.LegacyJitX64));

            Add(new Job("net-ryu-64bit", EnvironmentMode.RyuJitX64));

            Add(new Job("mono-64bit", EnvironmentMode.Mono).With(Platform.X64));
            Add(new Job("mono-32bit", EnvironmentMode.Mono).With(Platform.X86));
        }
    }
}
