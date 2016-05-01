# Benchmarking

Using [BenchmarkDotNet](https://github.com/PerfDotNet/BenchmarkDotNet), the
FarmHash.Sharp benchmarking code pits several non-cryptographic hash functions
against each other in terms of throughput.

Benchmarking was done on the following machine:

```ini
BenchmarkDotNet=v0.9.1.0
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

## C# vs. C++

A good question would be how much efficiency is lost because we're using
C# and not C++, as the original farmhash algorithm uses C++. You can find the
benchmark code [here](https://github.com/nickbabcock/Farmhash.Sharp/tree/5ef3ffc22a1b70b7875dc0b5ae73be496a45fb28/src/Farmhash.Benchmarks).
It uses two versions of the algorithm, one that uses hardware acceleration
([SIMD](https://en.wikipedia.org/wiki/SIMD) instructions), denoted by `-ha`
in the graph, and another compilation that does not use hardware acceleration.

![Farmhash-benchmark3](/Farmhash.Sharp/img/c-sharp-vs-cpp.png)

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

data <- read.csv("farmhash-report.csv", sep=";") %>%
  select(Type, Method, Size=PayloadLength, Median)

breaks <- data %>% select(Size) %>% distinct() %>% arrange(Size) %>% t %>% c

ggplot(data, aes(as.factor(Size), Median, group=Method, color=Method)) +
  labs(title='Median Time to Hash', x='Data size (bytes)',
       y='Time to hash (ns)') +
  geom_line(size=1.5) +
  scale_y_log10() +
  facet_grid(Type ~ .)

relative <- data %>%
  mutate(Name = paste(Method, Type, sep="-")) %>%
  select(-Method, -Type) %>%
  group_by(Size) %>%
  arrange(Median) %>%
  mutate(value = first(Median) / Median) %>%
  top_n(5, wt = value)

ggplot(relative, aes(as.factor(Size), value)) +
  geom_bar(aes(fill=Name), stat='identity', position='dodge') +
  labs(title='Relative Throughput across Resulting Hash Size', 
       x='Data size (bytes)', y="Relative Throughput")
```

And the code for the C# vs C++ graph:

```R
library(ggplot2)
library(dplyr)

benchmark <- rep(c("farmhash-ha", "farmhash", "Farmhash.Sharp"), each = 6)
payload <- rep(c(4, 11, 25, 100, 1000, 10000), 3)

# Throughput is measured in how many GB/s can be hashed
throughput <- c(1.03503, 2.50114, 5.68304, 5.82953, 13.0187, 23.7148, 1.3749, 3.04061, 6.6442,
                5.79228, 14.2568, 16.2982, 1.1241, 2.5563, 5.171, 2.80498, 5.86157, 6.528081)

data <- data.frame(benchmark, payload, throughput)

relative <- data %>%
  group_by(payload) %>%
  arrange(throughput) %>%
  mutate(value = throughput / last(throughput))

ggplot(relative, aes(as.factor(payload), value)) +
  geom_bar(aes(fill=benchmark), stat='identity', position='dodge') +
  labs(title='Relative Throughput of Farmhash: C# vs C++',
       x='Data size (bytes)', y="Relative Throughput")
```
