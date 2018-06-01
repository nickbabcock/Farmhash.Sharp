### 0.6 - June 1st 2018

* Fix bug affecting Hash32 users who used the `byte*` signature that would calculate the wrong hash for inputs with length of four or less
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
