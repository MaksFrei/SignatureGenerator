# SignatureGenerator

A console program for generating the signature of the specified file. The signature is generated as follows: the source file is divided into blocks of a given length (except the last block), the value of the SHA256 hash function is calculated for each block, and together with its number is output to the console(The bottleneck of the application, I tried to optimize, but still not enough. Output to a file muuuch faster, especially on SSD).

The program is able to process files whose size exceeds the amount of RAM, and at the same time effectively use the computing power of a multiprocessor system. When working with threads, only standard classes and libraries from .Net were used (excluding ThreadPool, BackgroundWorker, TPL)


The path to the input file and the block size are set on the command line. If an error occurs during the execution of the program, its text and Stack Trace are output to the console.