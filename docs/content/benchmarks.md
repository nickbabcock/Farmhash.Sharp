# Benchmarking

Using [BenchmarkDotNet](https://github.com/PerfDotNet/BenchmarkDotNet), the
FarmHash.Sharp benchmarking code pits several non-cryptographic hash functions
against each other in terms of throughput.

Benchmarking was done on the following machine:

```ini
BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-6700K CPU @ 4.00GHz, ProcessorCount=8
Frequency=3914059 ticks, Resolution=255.4893 ns
HostCLR=MS.NET 4.0.30319.42000, Arch=64-bit RELEASE [RyuJIT]
```

To run the benchmarks:

```
build.cmd Benchmark
./build.sh Benchmark
```

## Median Time to Hash

![Farmhash-benchmark1](/Farmhash.Sharp/img/farmhash-benchmark1.png)

The graph depicts the median time in nanoseconds to hash data of a certain
size grouped by resulting hash size (eg. 32 or 64bit, which is not underlying
machine architecture). A logarithmic scale had to be applied because other
hash functions are so slow, they'd heavily skew the graph.

We can see for 64bit, that when hashing data that is 1000 bytes (~1KB), that
FarmHash is about 10x faster than the closest competitor.

For 32bit hash functions, the built in `GetHashCode` on strings has a tight
grip on smaller data, but falls off for larget data.

## Relative Throughput

![Farmhash-benchmark2](/Farmhash.Sharp/img/farmhash-benchmark2.png)

The bar shows the relative throughput of each hash function relative to the
fastest hash function in that category. So the higher the bar chart, the
better.

Here we combine resulting 32 and 64 bit hash functions for an interesting
result. Across all data sizes, the FarmHash-64bit version is fastest, though
for short string lengths, `GetHashCode` gives FarmHash a run for its money.

![Farmhash-benchmark3](/Farmhash.Sharp/img/farmhash-benchmark3.png)

## C# vs. C++

A good question would be how much efficiency is lost because we're using
C# and not C++, as the original farmhash algorithm uses C++. You can find the
benchmark code [here](https://github.com/nickbabcock/Farmhash.Sharp/tree/5ef3ffc22a1b70b7875dc0b5ae73be496a45fb28/src/Farmhash.Benchmarks).
It uses two versions of the algorithm, one that uses hardware acceleration
([SIMD](https://en.wikipedia.org/wiki/SIMD) instructions), denoted by `-ha`
in the graph, and another compilation that does not use hardware acceleration.

![Farmhash-benchmark4](/Farmhash.Sharp/img/c-sharp-vs-cpp.png)

I'm pleased to report that for small payloads (<= 25 bytes), Farmhash.Sharp
is around 75% as fast as the fastest configuration. It's only at larger payloads
do we see C++'s lead extend as hardware acceleration becomes more effective.

## Conclusion

When deploying on a 64bit application, always choose the 64bit Farmhash
version. If, for whatever reason, Farmhash isn't for you, choose xxHash for
data of lengths less than 1000 bytes and HashFunction CityHash for lengths
greater than 1000.

And I'm just going to squirrel away the code used to generate the graph.

```R
library(ggplot2)
library(dplyr)
library(stringr)
library(readr)

# Converts "2.313 ns" to 2.313
toNs <- function(x) {
    split <- str_split(x, " ")
    num <- as.numeric(gsub(",", "", split[[1]][[1]]))
    factor <- switch(split[[1]][[2]], ns=1, us=1000, ms=1000*1000)
    num * factor
}
vtoNs <- Vectorize(toNs)

core32 <- read_csv2("core-HashBenchmark32-report.csv") %>%
    mutate(Bit='32bit', Jit='.NET Core')
core64 <- read_csv2("core-HashBenchmark64-report.csv") %>%
    mutate(Bit='64bit', Jit='.NET Core')
net32 <- read_csv2("HashBenchmark32-report.csv") %>%
    mutate(Bit='32bit', Jit='.NET 4.5')
net64 <- read_csv2("HashBenchmark64-report.csv") %>%
    mutate(Bit='64bit', Jit='.NET 4.5')

# Combine all four sheets into one with parsed properties
combined <- bind_rows(core32, core64, net32, net64) %>%
    select(Method, PayloadLength, Median, StdDev, Bit, Jit) %>%
    mutate(Median = vtoNs(Median), StdDev = vtoNs(StdDev))

ggplot(combined, mapping = aes(as.factor(PayloadLength), Median, group = Method, color = Method)) +
    geom_line(size=1.5) +
    scale_y_log10() +
    facet_grid(Bit ~ Jit) +
    xlab("Data size (bytes)") + ylab("Median (ns)") +
    ggtitle("Comparison of Median Time To Hash Data Across Jit and Return Size with Logarithmic Scale")

relative <- combined %>%    
  # Throughput converts bytes/ns to MB/s
  mutate(Name = paste(Method, Bit, Jit, sep="-"),
         Throughput = (PayloadLength / (1024 * 1024)) /
            (Median / (1000*1000*1000))) %>%
  select(-Method, -Bit, -Jit) %>%
  group_by(PayloadLength) %>%
  mutate(value = min(Median) / Median) %>%
  top_n(3, wt = value)

ggplot(relative, aes(as.factor(PayloadLength), value)) +
  geom_bar(aes(fill=Name), stat='identity', position='dodge') +
  labs(title='Relative Throughput of Top 3 Performers Per Data Size', 
       x='Data size (bytes)', y="Relative Throughput to Top Perfomer")

ggplot(relative, aes(as.factor(PayloadLength), Throughput)) +
  geom_bar(aes(fill=Name), stat='identity', position='dodge') +
  labs(title='Absolute Throughput of Top 3 Perfomers Per Data Size', 
       x='Data size (bytes)', y="MB/s")
```

And the code for the C# vs C++ graph:

```R
library(ggplot2)
library(dplyr)

benchmark <- rep(c("farmhash-ha", "farmhash", "Farmhash.Sharp"), each = 6)
payload <- rep(c(4, 11, 25, 100, 1000, 10000), 3)

# Throughput is measured in how many GB/s can be hashed
throughput <- c(1.03503, 2.50114, 5.68304, 5.82953, 13.0187, 23.7148, 1.3749, 3.04061, 6.6442,
                5.79228, 14.2568, 16.2982, 1.359, 2.3488, 6.752, 3.058, 11.008, 13.637)

data <- data.frame(benchmark, payload, throughput)

relative <- data %>%
  group_by(payload) %>%
  mutate(value = throughput / max(throughput))

ggplot(relative, aes(as.factor(payload), value)) +
  geom_bar(aes(fill=benchmark), stat='identity', position='dodge') +
  labs(title='Relative Throughput of Farmhash: C# vs C++',
       x='Data size (bytes)', y="Relative Throughput")
```
