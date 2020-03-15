using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Farmhash.Sharp.Benchmarks
{
    public class BaseConfig : ManualConfig
    {
        public BaseConfig()
        {
            Add(new CsvExporter(
                CsvSeparator.CurrentCulture,
                new BenchmarkDotNet.Reports.SummaryStyle(
                    false,
                    null,
                    BenchmarkDotNet.Horology.TimeUnit.Nanosecond,
                    false)
            ));

            Add(StatisticColumn.Mean);
            Add(StatisticColumn.StdErr);
            Add(StatisticColumn.StdDev);
            Add(StatisticColumn.Median);

            // dotnet cli toolchain supports only x64 compilation
            Add(new Job("core-64bit").With(CoreRuntime.Core21).WithId("Core")
                .With(CsProjCoreToolchain.NetCoreApp21));

            Add(new Job("net-legacy-32bit", EnvironmentMode.LegacyJitX86).With(CsProjClassicNetToolchain.Net48));
            Add(new Job("net-legacy-64bit", EnvironmentMode.LegacyJitX64).With(CsProjClassicNetToolchain.Net48));

            Add(new Job("net-ryu-64bit", EnvironmentMode.RyuJitX64));

            Add(new Job("mono-64bit").With(MonoRuntime.Default).With(Platform.X64));
            Add(new Job("mono-32bit").With(MonoRuntime.Default).With(Platform.X86));
        }
    }
}