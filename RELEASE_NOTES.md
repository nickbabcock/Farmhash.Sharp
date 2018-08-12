## Release Notes

### 0.7 - August 11th 2018

* Fixed the return type for 32bit hashes taking a span as an argument, so that it is consistent with the return types of the other 32bit hash functions.

```diff
- ulong Hash32(ReadOnlySpan<byte> span)
+ uint Hash32(ReadOnlySpan<byte> span)
```

* Hash functions receiving a byte pointer and length has had the length parameter changed from `uint` to `int` to match the other public hash functions (which match return type of `Array::length`)

```diff
- uint Hash32(byte* s, uint len)
+ uint Hash32(byte* s, int len)

- ulong Hash64(byte* s, uint len)
+ ulong Hash64(byte* s, int len)
```
### 0.6 - June 1st 2018

* Fix bug affecting Hash32 users who used the `byte*` signature that would calculate the wrong hash for inputs with length of four or less bytes
* Introduce `ReadOnlySpan<byte>` methods for Hash64 and Hash32

### 0.5 - April 10th 2018

* Strong sign assemblies so that other strong signed assemblies can use Farmhash.Sharp

### 0.4 - November 13th 2017
* Expose internal methods using byte pointers as part of the public interface (#12)

### 0.3 - April 25th 2017
* Release Farmhash.Sharp under netstandard 1.0
* Switch to new MSBuild project files

### 0.2 - June 25th 2016
* String helper functions
* 40% faster processing large (> 1KB) inputs

### 0.1 - November 12th 2015
* 32bit Farmhash implemented
* 64bit Farmhash implemented
