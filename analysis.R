library(tidyverse)
library(readr)

df <- read_csv("./benchmarks.csv")

# First we need to scrub the thousand separator and the unit
# suffix in the numeric columns. This may not be a problem in 10.4.
# Calculate the payload by taking computing MB/s. There are 10^6
# bytes in a MB and 10^9 nanoseconds in a second
df <- df %>%
  mutate(Mean = as.numeric(gsub(',', '', gsub('.{3}$', '', Mean))),
         StdErr = as.numeric(gsub(',', '', gsub('.{3}$', '', StdErr))),
         StdDev = as.numeric(gsub(',', '', gsub('.{3}$', '', StdErr))),
         Median = as.numeric(gsub(',', '', gsub('.{3}$', '', Median))),
         Throughput = PayloadLength * 10^9 / (Mean * 10^6))

# The first thing we're going to do is show the difference between
# a cryptographic function (albeit a bad one) and a non-cryptographic
# function. We're going to see MD5 vs Farmhash
fm <- df %>% 
  filter((Method == 'FarmHash' | Method == 'Md5') &
           Job == 'net-legacy-32bit' & Kind == '32bit hash') %>%
  select(Method, PayloadLength, Median, StdErr, StdDev, Throughput)

ggplot(fm, aes(as.factor(PayloadLength), Throughput)) +
  geom_bar(aes(fill=Method), stat='identity', position='dodge') +
  labs(x='Payload (bytes)', y='Throughput MB/s') +
  ggtitle('Cryptographic vs Non-Cryptographic Hash Function',
          subtitle = "Using 32bit CLR with 32bit Farmhash as a baseline")

# Next we're going to see the performance difference when the 64bit Farmhash
# function is executed in a 64bit Clr Runtime
clr_run <- df %>%
  filter((Method == 'FarmHash') & Runtime == 'Clr' & Kind == '64bit hash') %>%
  select(Method, Job, Runtime, PayloadLength, Median, StdErr, StdDev, Throughput)

ggplot(clr_run, aes(as.factor(PayloadLength), Throughput)) +
  geom_bar(aes(fill=Job), stat='identity', position='dodge') +
  labs(x='Payload (bytes)', y='Throughput MB/s') + 
  ggtitle("Throughput by Jit Runtime",
          subtitle = "64bit Farmhash Performance on the Clr")

# Does the same hold true for Mono?
mono_run <- df %>%
  filter((Method == 'FarmHash') & Runtime == 'Mono' & Kind == '64bit hash') %>%
  select(Method, Job, Runtime, PayloadLength, Median, StdErr, StdDev, Throughput)

ggplot(mono_run, aes(as.factor(PayloadLength), Throughput)) +
  geom_bar(aes(fill=Job), stat='identity', position='dodge') +
  labs(x='Payload (bytes)', y='Throughput MB/s') + 
  ggtitle("Throughput by Jit Runtime",
          subtitle = "64bit Farmhash Performance on the Mono")

# Now let's compare core, mono, and clr
runtime_df <- df %>%
  filter((Method == 'FarmHash')) %>%
  select(Method, Job, Runtime, Kind, PayloadLength, Median, StdErr, StdDev, Throughput)

ggplot(runtime_df, aes(as.factor(PayloadLength), Throughput)) +
  geom_bar(aes(fill=Job), stat='identity', position='dodge') +
  facet_grid(Kind ~ .) +
  labs(x='Payload (bytes)', y='Throughput MB/s') + 
  ggtitle("Throughput by Runtime",
          subtitle = "32 and 64bit Farmhash Performance across Core, Mono, and Clr")

# Let's limit our dealings with the top three runtimes:
# - core-64bit
# - mono-64bit
# - net-ryu-64bit

hash_plot <- function(job_name, hash_kind) {
  # Eliminate md5 methods from the graph as we know it's a cryptographic
  # hash function and it's not really comparing apples to apples
  n_df <- df %>% filter(Job == job_name & Kind == hash_kind & Method != 'Md5') %>%
    group_by(PayloadLength) %>%
    mutate(Relative = Throughput / max(Throughput))
  title <- paste("Highest Throughput for each Payload for", hash_kind, "on", job_name)
  ggplot(n_df, aes(as.factor(PayloadLength), Relative)) +
    geom_bar(aes(fill=Method), stat='identity', position='dodge') +
    labs(x='Payload (bytes)', y='Relative Throughput (1.0 is highest throughput)') + 
    ggtitle(title)
}

hash_plot('core-64bit', '32bit hash')
hash_plot('core-64bit', '64bit hash')
hash_plot('mono-64bit', '32bit hash')
hash_plot('mono-64bit', '64bit hash')
hash_plot('net-ryu-64bit', '32bit hash')
hash_plot('net-ryu-64bit', '64bit hash')

# This is the C++ data. Since the benchmark doesn't output a csv these
# numbers are handcoded from a C++ benchmark run
cpp <- rep(c("farmhash-ha", "farmhash"), each = 6)
cpp_loads <- rep(c(4, 11, 25, 100, 1000, 10000), 2)
cpp_through <- c(911, 2037, 4862, 5460, 12500, 23520,
                 1379, 3240, 6706, 5941, 14147, 16077)
cpp_df <- data.frame(cpp, cpp_loads, cpp_through)
colnames(cpp_df) <- c("Job", "PayloadLength", "Throughput")

net_df <- df %>%
  filter(Job == 'net-ryu-64bit' & Method == 'FarmHash' & Kind == '64bit hash') %>%
  select(Job, PayloadLength, Throughput)

new_df <- rbind(net_df, cpp_df) %>%
  group_by(PayloadLength) %>%
  mutate(Relative = Throughput / max(Throughput))

ggplot(new_df, aes(as.factor(PayloadLength), Relative)) +
  geom_bar(aes(fill=Job), stat='identity', position='dodge') +
  labs(x='Payload (bytes)', y='Relative Throughput (1.0 is highest throughput)') +
  ggtitle("Throughput of C++ Farmhash vs Farmhash.Sharp",
          subtitle = "Where farmhash-ha uses hardware acceleration")