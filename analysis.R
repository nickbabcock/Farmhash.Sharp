library(tidyverse)
library(readr)

df <- read_csv("./benchmarks.csv")

df <- df %>%
  mutate(Mean = as.numeric(gsub(',', '', gsub('.{3}$', '', Mean))),
         StdErr = as.numeric(gsub(',', '', gsub('.{3}$', '', StdErr))),
         StdDev = as.numeric(gsub(',', '', gsub('.{3}$', '', StdErr))),
         Median = as.numeric(gsub(',', '', gsub('.{3}$', '', Median))),
         Throughput = PayloadLength * 10^9 / (Mean * 10^6))

fm <- df %>% 
  filter((Method == 'FarmHash' | Method == 'Md5') &
          Job == 'net-legacy-32bit' & Kind == '32bit hash') %>%
  select(Method, PayloadLength, Median, StdErr, StdDev, Throughput)

ggplot(fm, aes(as.factor(PayloadLength), Throughput)) +
  geom_bar(aes(fill=Method), stat='identity', position='dodge') +
  labs(x='Payload (bytes)', y='Throughput MB/s') +
  ggtitle('Cryptographic vs Non-Cryptographic Hash Function',
          subtitle = "Using 32bit CLR with 32bit Farmhash as a baseline")
