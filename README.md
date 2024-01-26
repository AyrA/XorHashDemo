# XorHashDemo

A quick and dirty implementation of the algorithm described in [this reddit comment](https://www.reddit.com/r/cryptography/comments/19dlhih/_/kj6tx4r/)

The algorithm works as follows:

1. Start with a xorsum of 32 nullbytes
2. Begin enumerating files in a given directory
2. Open the file and hash `<filename>\0<contents>` using SHA256
3. `xorsum=xorsum XOR hash`
4. If the directory contains more files, go to step 2
5. Done. `xorsum` contains a sort order independent, combined hash of all files.

