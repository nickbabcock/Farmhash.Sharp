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
                    PrintUnitsInHeader = true,
                    TimeUnit = BenchmarkDotNet.Horology.TimeUnit.Nanosecond
                }));

            Add(StatisticColumn.Mean);
            Add(StatisticColumn.StdErr);
            Add(StatisticColumn.StdDev);
            Add(StatisticColumn.Median);
#if CORE
            // dotnet cli toolchain supports only x64 compilation
            Add(new Job("core-64bit")
            {
                Env = { Runtime = Runtime.Core, Jit = Jit.RyuJit, Platform = Platform.X64 }
            });
#endif
#if CLASSIC
            Add(new Job("net-legacy-32bit")
            {
                Env = { Runtime = Runtime.Clr, Jit = Jit.LegacyJit, Platform = Platform.X86 }
            });

            Add(new Job("net-legacy-64bit")
            {
                Env = { Runtime = Runtime.Clr, Jit = Jit.LegacyJit, Platform = Platform.X64 }
            });

            // Ryu is only for 64bit jobs
            Add(new Job("net-ryu-64bit")
            {
                Env = { Runtime = Runtime.Clr, Jit = Jit.RyuJit, Platform = Platform.X64 }
            });
#endif
#if MONO
            Add(new Job("mono-32bit")
            {
                Env = { Runtime = Runtime.Mono, Jit = Jit.Llvm, Platform = Platform.X86 }
            });

            Add(new Job("mono-64bit")
            {
                Env = { Runtime = Runtime.Mono, Jit = Jit.Llvm, Platform = Platform.X64 }
            });
#endif
        }
    }
}
