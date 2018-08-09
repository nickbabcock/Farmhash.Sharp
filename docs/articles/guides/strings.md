# String Hashing

It is very easy to hash a string with Farmhash.Sharp with performance equivalent to hashing bytes directly

```csharp
ulong hash64 = Farmhash.Hash64("Hello");
uint hash32 = Farmhash.Hash32("Hello");
```

At this point all the public APIs with strings have been covered, but there is some additional information that may help one understand the implementation and use cases.

## Behind the Scenes

The Farmhash algorithm operates on a sequence of bytes. A string is a sequence of characters. These two seem fundamentally at odds. Googling "C# string to bytes" will yield the [top Stackoverflow question](https://stackoverflow.com/q/16072709/433785) and [dot net perls](https://www.dotnetperls.com/convert-string-byte-array) as the second result. Both of these pages instruct one to decode the string according to a [character encoding](https://en.wikipedia.org/wiki/Character_encoding). This would be bad advice for Farmhash.Sharp for two reasons:

- Would incur a performance penalty as a intermediate byte array would be allocated
- Would complicate the API by forcing the user to pass in an [`Encoding`](https://msdn.microsoft.com/en-us/library/system.text.encoding(v=vs.110).aspx) class else risk increased hash collisions due to decoding issues.

To illustrate the hash collision:

```csharp
var payload = "ø";

var data = Encoding.ASCII.GetBytes(payload);
// byte[1] { 63 }

Encoding.ASCII.GetString(data)
// "?"
```

Thus if we ASCII decoded "ø", we'd get the same hash as "?". Collision via replacement characters would be a terrible property of any hash function.

Encodings are incredibly important, as the choice can affect the byte representation. The Hindi character for you (यू) decoded in UTF8, UTF16, and UTF32 will yield 3 different results

```csharp
Encoding.UTF8.GetBytes("यू")
// byte[6] { 224, 164, 175, 224, 165, 130 }

Encoding.Unicode.GetBytes("यू")
// byte[4] { 47, 9, 66, 9 }

Encoding.UTF32.GetBytes("यू")
// byte[8] { 47, 9, 0, 0, 66, 9, 0, 0 }
```

All three byte arrays are valid, but are near useless as one can't recover the original string without keeping track of the encoding. While recognizing the encoding of the data you are working with is always good idea, chaining users to an `Encoding` before they can use Farmhash.Sharp would be an ergonomic hurdle. The way I think of it, hashing a string should be akin to `String::GetHashCode` in simplicity.

## Performance

We can avoid any performance penalties of converting a string to bytes without any downsides with the following approach:

```csharp
public static unsafe ulong Hash64(string s)
{
    fixed (char* buffer = s)
    {
        return Hash64((byte*)buffer, (s.Length * sizeof(char)));
    }
}
```

* A string is UTF16 encoded
* We get the string's raw character buffer
* Cast it to a byte array
* Since a byte is 8 bits, a [string's length is the number of characters in the string](https://msdn.microsoft.com/en-us/library/system.string.length(v=vs.110).aspx), and a char is 16 bits representing a UTF16 code point ([C# Language Specification: 9.3.6 Integral types](https://www.ecma-international.org/publications/files/ECMA-ST/Ecma-334.pdf)), we can arrive at the total length of the byte array by essentially multiplying the string's length by 2 (number of bytes in a `char`).

Some may state that this will fail for characters that fall into the surrogate pair range. When it's necessary to join two 16 bit characters to form single character, it's called a surrogate pair. Surrogate pairs are necessary to encode less commonly used symbols, as there are more than 65536 (16 bits) symbols across all languages, emojis, etc. I'll address the two failure points that one might think could occur: buffer overflow causing undefined behavior, and not consuming the entire byte buffer, resulting in an increase of hash collisions.

Let's take the Mandarin compound "to shatter": 𤭢

```csharp
"𤭢".Length
// 2

"𤭢".ToCharArray()
// char[2] { '\ud852', '\udf62' }

var data = Encoding.Unicode.GetBytes("𤭢")
// byte[4] { 82, 216, 98, 223 }

Encoding.Unicode.GetString(data)
// "𤭢"
```

While "𤭢" may be a single symbol, it is composed of multiple characters, so our implementation earlier will handle it smoothly and accurately interpret the string as composed of four bytes. For a bit of context on why the string has a length of 2 instead of 1, [Wikipedia has the explanation](https://en.wikipedia.org/wiki/UTF-16#Usage):

> String implementations based on UTF-16 typically return lengths and allow
indexing in terms of code units, not code points. Neither code points nor code
units correspond to anything an end user might recognize as a "character"; the
things users identify as characters may in general consist of a base code
point and a sequence of combining characters (or be a sequence of code points
of other kind, for example Hangul conjoining jamos) – Unicode refers to this
as a grapheme cluster – and as such, applications dealing with Unicode
strings, whatever the encoding, have to cope with the fact that they cannot
arbitrarily split and combine strings.

Finally for proof that that hashing "𤭢" and the UTF-16 bytes of "𤭢" are the same:

```csharp
var d = Encoding.Unicode.GetBytes("𤭢");
Farmhash.Hash64(d, d.Length).ToString("X2")
// 7D9D6CEA9FCF031D

Farmhash.Hash64("𤭢").ToString("X2")
// 7D9D6CEA9FCF031D
```

## Conclusion

A single sentence can sum up the Farmhash.Sharp string implementation:

> Farmhash.Sharp hashes strings in a zero allocation implementation by interpreting the UTF-16 characters as bytes.

Sounds too simplistic, right? Like we're doing something wrong, as the internet is rife to suggest, but sometimes the best answer is the simplest one.