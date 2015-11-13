# Benchmarking

The following benchmarks pit Farmhash against various hashing algorithms. The
numbers represent the number of  hash calculations per milliseconds. The
numbers may be hard to look at, but the conclusion that should be drawn is
that one should use 64bit hash on 64bit architecture for the fastest possible
hash function.

To run the benchmarks:

```
build.cmd Benchmark
./build.sh Benchmark
```

## Benchmark Cases

- Smallest: Four bytes "doge"
- Smaller: eleven bytes "hello world"
- Small: twenty-four bytes "Hello I'm Farmhash.Sharp"
- Medium: hundred bytes of periods
- Large: thousand bytes of periods
- Largest: ten-thousand bytes of periods

## 32bit Benchmarks

Name                   | Smallest   | Smaller    | Small     | Medium    | Large    | Largest
-----------------------|------------|------------|-----------|-----------|----------|--------
Farmhash               | 90464.99   | 79222.56   | 74943.79  | 26819.72  | 3588.73  | 376.77
xxHashSharp            | 111300.73  | 61394.89   | 21485.66  | 3497.22   | 593.65   | 59.33
CityHash.Net           | 7193.21    | 5460.50    | 3976.99   | 1498.16   | 171.48   | 17.68
String-hashCode        | 171174.26  | 120685.49  | 71547.82  | 17821.50  | 2182.77  | 219.60
HashFunction-CityHash  | 31679.66   | 19870.44   | 13399.80  | 4743.14   | 565.86   | 57.60
HashFunction-Spooky    | 1934.78    | 1960.25    | 1922.31   | 1404.34   | 434.73   | 54.06
MD5sum                 | 788.60     | 810.59     | 795.88    | 748.13    | 348.11   | 54.34

While 32bit Farmhash is the fastest for byte arrays twenty-four bytes or
larger, for smaller byte arrays xxHashSharp and the native hash code function
for strings take the cake.

## 64bit Benchmarks

For 64bit benchmarking, I have removed the native string hash function because
it returns only 32bit hashes. Also for 64bit benchmarking, the code was
executed on a 64bit platform.

Name                   | Smallest   | Smaller    | Small      | Medium    | Large    | Largest
-----------------------|------------|------------|------------|-----------|----------|--------
Farmhash               | 174114.92  | 118586.45  | 146527.30  | 26649.61  | 5763.67  | 668.69
xxHashSharp            | 112841.35  | 58424.87   | 22075.38   | 5654.04   | 538.75   | 59.14
CityHash.Net           | 7095.22    | 5598.48    | 4376.62    | 1432.02   | 190.35   | 18.98
HashFunction-CityHash  | 29379.50   | 24479.40   | 21544.60   | 5049.50   | 930.30   | 96.79
HashFunction-Spooky    | 1884.97    | 1936.78    | 1895.61    | 1422.37   | 440.12   | 50.74
MD5sum                 | 811.07     | 719.08     | 776.32     | 645.47    | 291.14   | 52.59

64bit Farmhash is the fastest algorithm.