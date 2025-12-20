![scribbly_banner.png](./docs/scribbly_banner.png)

![GitHub](https://img.shields.io/github/license/Scribbly-Fun/Scribbly.Cubby) 
![GitHub all releases](https://img.shields.io/github/downloads/Scribbly-Fun/Scribbly.Cubby/total) 
![Nuget](https://img.shields.io/nuget/dt/Scribbly.Cubby)
[![GitHub issues](https://img.shields.io/github/issues/Scribbly-Fun/Scribbly.Cubby)](https://github.com/Scribbly-Fun/Scribbly.Cubby/issues)
![GitHub Repo stars](https://img.shields.io/github/stars/Scribbly-Fun/Scribbly.Cubby?style=social)
![GitHub forks](https://img.shields.io/github/forks/Scribbly-Fun/Scribbly.Cubby?style=social)
[![Tests](https://github.com/Scribbly-Fun/Scribbly.Cubby/actions/workflows/dotnet-test.yml/badge.svg?branch=main)](https://github.com/Scribbly-Fun/Scribbly.Cubby/actions/workflows/dotnet-test.yml)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/Scribbly-Fun/Scribbly.Cubby/main)

# Scribbly Cubby

A distributed caching service built on dotnet.  You know take stuff out and put stuff away.

![cubby.png](./docs/cubby.png)

![Static Badge](https://img.shields.io/badge/CACHE-blue)

## Table of Contents
1. [Example](#Example)
2. [Benchmarks](#Benchmarks)

## Example

Below is a brief snip of code to get you started before reading more.

# Benchmarks

### 2025.12.20 Store Comparisons

The initial benchmarks to help select a default starting point for a in-memory caching mechanism

| Method                          | Threads | EntryCount | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Completed Work Items | Lock Contentions | Allocated | Alloc Ratio |
|-------------------------------- |-------- |----------- |----------:|----------:|----------:|----------:|------:|--------:|---------------------:|-----------------:|----------:|------------:|
| ConcurrentDictionary_Get        | 1       | 100000     |  7.297 ms | 0.7886 ms | 2.3004 ms |  6.601 ms |  1.08 |    0.45 |                    - |                - |   1.47 KB |        1.00 |
| ShardedConcurrentDictionary_Get | 1       | 100000     |  5.278 ms | 0.3443 ms | 0.9989 ms |  5.069 ms |  0.78 |    0.25 |                    - |                - |   1.47 KB |        1.00 |
| LockFreeHashTable_Get           | 1       | 100000     | 11.693 ms | 0.2281 ms | 0.4054 ms | 11.511 ms |  1.74 |    0.45 |                    - |                - |   1.47 KB |        1.00 |
|                                 |         |            |           |           |           |           |       |         |                      |                  |           |             |
| ConcurrentDictionary_Get        | 4       | 100000     |  2.206 ms | 0.1414 ms | 0.4170 ms |  2.053 ms |  1.03 |    0.26 |               3.0000 |                - |   2.05 KB |        1.00 |
| ShardedConcurrentDictionary_Get | 4       | 100000     |  1.415 ms | 0.0310 ms | 0.0895 ms |  1.389 ms |  0.66 |    0.12 |               3.0000 |                - |   2.05 KB |        1.00 |
| LockFreeHashTable_Get           | 4       | 100000     |  3.362 ms | 0.0420 ms | 0.0351 ms |  3.353 ms |  1.57 |    0.25 |               3.0000 |                - |   2.05 KB |        1.00 |
|                                 |         |            |           |           |           |           |       |         |                      |                  |           |             |
| ConcurrentDictionary_Get        | 8       | 100000     |  1.408 ms | 0.0275 ms | 0.0282 ms |  1.408 ms |  1.00 |    0.03 |               6.9277 |                - |   2.89 KB |        1.00 |
| ShardedConcurrentDictionary_Get | 8       | 100000     |  1.162 ms | 0.0232 ms | 0.0523 ms |  1.156 ms |  0.83 |    0.04 |               6.9922 |                - |    2.9 KB |        1.00 |
| LockFreeHashTable_Get           | 8       | 100000     |  1.608 ms | 0.0300 ms | 0.0368 ms |  1.608 ms |  1.14 |    0.03 |               6.9902 |                - |    2.9 KB |        1.00 |
|                                 |         |            |           |           |           |           |       |         |                      |                  |           |             |
| ConcurrentDictionary_Get        | 16      | 100000     |  1.501 ms | 0.0297 ms | 0.0542 ms |  1.491 ms |  1.00 |    0.05 |               7.8340 |                - |   3.82 KB |        1.00 |
| ShardedConcurrentDictionary_Get | 16      | 100000     |  1.302 ms | 0.0242 ms | 0.0526 ms |  1.290 ms |  0.87 |    0.05 |               7.9316 |                - |   3.72 KB |        0.97 |
| LockFreeHashTable_Get           | 16      | 100000     |  1.678 ms | 0.0327 ms | 0.0350 ms |  1.679 ms |  1.12 |    0.05 |               7.8438 |                - |   3.62 KB |        0.95 |

