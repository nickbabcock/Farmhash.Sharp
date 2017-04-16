#include <string>
#include <benchmark/benchmark.h>
#include "farmhash.h"

static void BM_Farmhash(benchmark::State& state) {
  auto payload = std::string(state.range(0), '.');
  while (state.KeepRunning())
    util::Hash64(payload);
  state.SetBytesProcessed(int64_t(state.iterations()) * int64_t(state.range(0)));
}

// Register the function as a benchmark
BENCHMARK(BM_Farmhash)->Arg(4)->Arg(11)->Arg(25)->Arg(100)->Arg(1000)->Arg(10000);

BENCHMARK_MAIN();
