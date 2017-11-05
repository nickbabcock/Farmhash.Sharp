library(tidyverse)
library(readr)

df <- read_csv("./benchmarks.csv")

# Calculate the throughput by computing MB/s. There are 10^6 bytes in a
# MB and 10^9 nanoseconds in a second.
df <- mutate(df, Throughput = PayloadLength * 10^9 / (Mean * 10^6))

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

# Moving on to comparing relative throughput of every hashing library, on every
# platform, for all payload sizes, for 32bit and 64bit hashes. Careful, this
# heatmap contains a lot of good information!
df2 <- df %>% group_by(Job, Kind, PayloadLength) %>%
  mutate(Relative = Throughput / max(Throughput)) %>%
  ungroup() %>%
  select(Job, Kind, Method, PayloadLength, Relative, Throughput) %>%
  complete(Job, Kind, Method, PayloadLength)

df2 %>% ggplot(aes(Method, as.factor(PayloadLength))) +
  geom_tile(aes(fill = Relative), color = "white") +
  facet_grid(Job ~ Kind) +
  scale_x_discrete(position = "top") +
  scale_fill_gradient(name = "", low = "white", high = "steelblue", na.value = "#D8D8D8", labels = c("lowest", "highest"), breaks = c(0,1)) +
  xlab("Hash Library") +
  ylab("Payload Size (bytes)") +
  geom_text(size=2.5, aes(label = ifelse(is.na(Relative), "NA", format(round(Relative, 2), digits = 3)))) +
  theme(legend.position="bottom") +
  theme(axis.text.x.top=element_text(angle=45, hjust=0, vjust=0)) +
  theme(plot.caption = element_text(hjust=0)) +
  ggtitle("Non-cryptographic Hash Functions with Relative Throughput",
          subtitle = "32bit and 64bit hash functions on Mono, Ryu, Core, and Legacy Jits") +
  labs(caption = "Shaded by payload and facet. For instance, SparrowXXHash has 70% of the throughput of Farmhash.Sharp\nwhen calculating the 64bit hash and both given a 4 byte payload on the .NET Ryu platform (64bits)")

# Previous heatmap detailed relative throughput, but that was for each facet's
# payload size. How can one tell if in terms of absolute throughput what
# configuration yields the highest throughput at a given payload size. Welcome to
# the next heatmap.
df3 <- df %>% mutate(Throughput = Throughput / 1000)
  group_by(PayloadLength) %>%
  mutate(Relative = Throughput / max(Throughput)) %>%
  ungroup() %>%
  select(Job, Kind, Method, PayloadLength, Relative, Throughput) %>%
  complete(Job, Kind, Method, PayloadLength)

df3 %>% ggplot(aes(Method, as.factor(PayloadLength))) +
  geom_tile(aes(fill = Relative), color = "white") +
  facet_grid(Job ~ Kind) +
  scale_x_discrete(position = "top") +
  scale_fill_gradient(name = "", low = "white", high = "steelblue", na.value = "#D8D8D8", labels = c("lowest", "highest"), breaks = c(0,1)) +
  xlab("Hash Library") +
  ylab("Payload Size (bytes)") +
  geom_text(size=2.5, aes(label = ifelse(is.na(Relative), "NA", format(round(Throughput, 1), digits = 3)))) +
  theme(axis.text.x.top=element_text(angle=45, hjust=0, vjust=0)) +
  theme(legend.position="bottom") +
  theme(plot.caption = element_text(hjust=0)) +
  ggtitle("Non-cryptographic Hash Functions with Throughput (GB/s)",
          subtitle = "32bit and 64bit hash functions on Mono, Ryu, Core, and Legacy Jits") +
  labs(caption = "Shaded by payload. For instance, for payloads of 4 bytes, the fastest is\nFarmhash.Sharp on .NET Ryu 64bit calculating 64bit hashes (1.2 GB/s)")

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
