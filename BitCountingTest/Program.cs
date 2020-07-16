using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections;

namespace BitCountingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //var summary = BenchmarkRunner.Run<test>();
            var random = new Random();
            for (var j = 0; j < 1000; j++)
            {
                var data = new BitArray(random.Next(900, 1100));
                data.SetAll(true);
                for (var i = 0; i < data.Length; i++)
                {
                    if (random.Next(0, 2) == 1)
                    {
                        data.Set(i, false);
                    }
                }
                var a1 = test.GetCardinality(data);
                var a2 = test.GetCardinality2(data);
                var a3 = test.GetCardinality3(data);
                var a4 = test.GetCardinality4(data);

                if (a1 != a2)
                {
                    ;
                }
                if (a3 != a2)
                {
                    ;
                }
                if (a4 != a2)
                {
                    ;
                }
            }

            var ndata = new BitArray(1000);
            ndata.SetAll(true);
            ndata.LeftShift(20);
            var ncount=test.GetCardinality4(ndata);
        }
    }
    public class test
    {
        private BitArray data;
        static Random random = new Random();
        [GlobalSetup]
        public void Setup()
        {
            var array = new int[(N >> 5) + 1];
            for(var i=0;i<array.Length;i++)
            {
                array[i] = random.Next();
            }
            data = new BitArray(array);
        }
        [Params(100, 10000)]
        public int N;
        [Benchmark]
        public int test1()
        {
            return GetCardinality(data);
        }
        [Benchmark]
        public int test2()
        {
            return GetCardinality2(data);

        }
        [Benchmark]
        public int test3()
        {
            return GetCardinality3(data);

        }
        [Benchmark]
        public int test4()
        {
            return GetCardinality4(data);

        }
        public static Int32 GetCardinality(BitArray bitArray)
        {

            Int32[] ints = new Int32[(bitArray.Count >> 5) + 1];

            bitArray.CopyTo(ints, 0);

            Int32 count = 0;

            // fix for not truncated bits in last integer that may have been set to true with SetAll()
            ints[ints.Length - 1] &= ~(-1 << (bitArray.Count % 32));

            for (Int32 i = 0; i < ints.Length; i++)
            {

                Int32 c = ints[i];

                // magic (http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel)
                unchecked
                {
                    c = c - ((c >> 1) & 0x55555555);
                    c = (c & 0x33333333) + ((c >> 2) & 0x33333333);
                    c = ((c + (c >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
                }

                count += c;

            }

            return count;

        }
        public static Int32 GetCardinality2(BitArray bitArray)
        {
            int i = 0;
            foreach (bool one in bitArray)
            {
                if (one)
                    i++;
            }
            return i;
        }
        public static Int32 GetCardinality3(BitArray bitArray)
        {

            Int32[] ints = new Int32[(bitArray.Count >> 5) + 1];

            bitArray.CopyTo(ints, 0);

            Int32 count = 0;

            // fix for not truncated bits in last integer that may have been set to true with SetAll()
            ints[ints.Length - 1] &= ~(-1 << (bitArray.Count % 32));

            for (Int32 i = 0; i < ints.Length; i++)
            {

                Int32 c = ints[i];

                count += System.Numerics.BitOperations.PopCount((uint)c); ;

            }

            return count;
        }

        public unsafe static Int32 GetCardinality4(BitArray bitArray)
        {

            int[] ints = new int[(bitArray.Count >> 5) + 2];

            bitArray.CopyTo(ints, 0);

            int count = 0;

            // fix for not truncated bits in last integer that may have been set to true with SetAll()
            ints[^2] &= ~(-1 << (bitArray.Count % 32));
            fixed (int* ptr = ints)
            {
                var llptr = (long*)ptr;
                var nl = ints.Length / 2;
                for (int i = 0; i < nl; i ++)
                {
                    var ll = llptr[i];
                    count += System.Numerics.BitOperations.PopCount((ulong)ll);
                }
            }


            return count;
        }


//        Mean = 167.786 ns, StdErr = 0.343 ns (0.20%), N = 15, StdDev = 1.327 ns
//Min = 164.945 ns, Q1 = 167.022 ns, Median = 167.609 ns, Q3 = 168.473 ns, Max = 170.338 ns
//IQR = 1.451 ns, LowerFence = 164.845 ns, UpperFence = 170.649 ns
//ConfidenceInterval = [166.368 ns; 169.205 ns] (CI 99.9%), Margin = 1.418 ns (0.85% of Mean)
//Skewness = 0.01, Kurtosis = 2.77, MValue = 2

//// ***** BenchmarkRunner: Finish  *****

//// * Export *
//  BenchmarkDotNet.Artifacts\results\BitCountingTest.test-report.csv
//  BenchmarkDotNet.Artifacts\results\BitCountingTest.test-report-github.md
//  BenchmarkDotNet.Artifacts\results\BitCountingTest.test-report.html

//// * Detailed results *
//test.test1: DefaultJob [N=100]
//Runtime = .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT; GC = Concurrent Workstation
//Mean = 27.606 ns, StdErr = 0.134 ns (0.49%), N = 16, StdDev = 0.537 ns
//Min = 26.653 ns, Q1 = 27.250 ns, Median = 27.568 ns, Q3 = 28.020 ns, Max = 28.639 ns
//IQR = 0.770 ns, LowerFence = 26.096 ns, UpperFence = 29.175 ns
//ConfidenceInterval = [27.060 ns; 28.153 ns] (CI 99.9%), Margin = 0.547 ns (1.98% of Mean)
//Skewness = 0.04, Kurtosis = 2, MValue = 2
//-------------------- Histogram --------------------
//[26.547 ns ; 27.111 ns) | @@@
//[27.111 ns ; 27.736 ns) | @@@@@@
//[27.736 ns ; 28.919 ns) | @@@@@@@
//---------------------------------------------------

//test.test2: DefaultJob [N=100]
//Runtime = .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT; GC = Concurrent Workstation
//Mean = 872.572 ns, StdErr = 2.465 ns (0.28%), N = 14, StdDev = 9.222 ns
//Min = 859.619 ns, Q1 = 865.793 ns, Median = 870.880 ns, Q3 = 876.724 ns, Max = 893.719 ns
//IQR = 10.931 ns, LowerFence = 849.397 ns, UpperFence = 893.120 ns
//ConfidenceInterval = [862.169 ns; 882.975 ns] (CI 99.9%), Margin = 10.403 ns (1.19% of Mean)
//Skewness = 0.6, Kurtosis = 2.64, MValue = 2
//-------------------- Histogram --------------------
//[854.597 ns ; 898.741 ns) | @@@@@@@@@@@@@@
//---------------------------------------------------

//test.test3: DefaultJob [N=100]
//Runtime = .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT; GC = Concurrent Workstation
//Mean = 25.021 ns, StdErr = 0.189 ns (0.75%), N = 97, StdDev = 1.860 ns
//Min = 22.443 ns, Q1 = 23.506 ns, Median = 24.588 ns, Q3 = 26.241 ns, Max = 29.206 ns
//IQR = 2.735 ns, LowerFence = 19.405 ns, UpperFence = 30.343 ns
//ConfidenceInterval = [24.379 ns; 25.662 ns] (CI 99.9%), Margin = 0.641 ns (2.56% of Mean)
//Skewness = 0.52, Kurtosis = 2.26, MValue = 2.38
//-------------------- Histogram --------------------
//[22.423 ns ; 23.574 ns) | @@@@@@@@@@@@@@@@@@@@@@@@@
//[23.574 ns ; 24.637 ns) | @@@@@@@@@@@@@@@@@@@@@@@@@@
//[24.637 ns ; 26.137 ns) | @@@@@@@@@@@@@@@@@@@@
//[26.137 ns ; 26.850 ns) | @@@@@@@
//[26.850 ns ; 27.913 ns) | @@@@@@@@@@@@
//[27.913 ns ; 29.247 ns) | @@@@@@@
//---------------------------------------------------

//test.test4: DefaultJob [N=100]
//Runtime = .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT; GC = Concurrent Workstation
//Mean = 23.036 ns, StdErr = 0.114 ns (0.49%), N = 15, StdDev = 0.441 ns
//Min = 22.340 ns, Q1 = 22.773 ns, Median = 23.034 ns, Q3 = 23.308 ns, Max = 23.837 ns
//IQR = 0.535 ns, LowerFence = 21.970 ns, UpperFence = 24.112 ns
//ConfidenceInterval = [22.565 ns; 23.507 ns] (CI 99.9%), Margin = 0.471 ns (2.05% of Mean)
//Skewness = 0.02, Kurtosis = 1.86, MValue = 2
//-------------------- Histogram --------------------
//[22.170 ns ; 22.639 ns) | @@@
//[22.639 ns ; 23.225 ns) | @@@@@@@
//[23.225 ns ; 24.072 ns) | @@@@@
//---------------------------------------------------

//test.test1: DefaultJob [N=10000]
//Runtime = .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT; GC = Concurrent Workstation
//Mean = 496.369 ns, StdErr = 1.758 ns (0.35%), N = 12, StdDev = 6.091 ns
//Min = 486.713 ns, Q1 = 492.761 ns, Median = 496.894 ns, Q3 = 498.892 ns, Max = 508.879 ns
//IQR = 6.131 ns, LowerFence = 483.563 ns, UpperFence = 508.089 ns
//ConfidenceInterval = [488.567 ns; 504.171 ns] (CI 99.9%), Margin = 7.802 ns (1.57% of Mean)
//Skewness = 0.33, Kurtosis = 2.32, MValue = 2
//-------------------- Histogram --------------------
//[483.221 ns ; 510.998 ns) | @@@@@@@@@@@@
//---------------------------------------------------

//test.test2: DefaultJob [N=10000]
//Runtime = .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT; GC = Concurrent Workstation
//Mean = 112.924 us, StdErr = 0.262 us (0.23%), N = 13, StdDev = 0.944 us
//Min = 111.499 us, Q1 = 112.092 us, Median = 113.022 us, Q3 = 113.260 us, Max = 114.589 us
//IQR = 1.169 us, LowerFence = 110.338 us, UpperFence = 115.014 us
//ConfidenceInterval = [111.794 us; 114.054 us] (CI 99.9%), Margin = 1.130 us (1.00% of Mean)
//Skewness = 0.26, Kurtosis = 1.8, MValue = 2
//-------------------- Histogram --------------------
//[110.972 us ; 115.116 us) | @@@@@@@@@@@@@
//---------------------------------------------------

//test.test3: DefaultJob [N=10000]
//Runtime = .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT; GC = Concurrent Workstation
//Mean = 244.094 ns, StdErr = 0.990 ns (0.41%), N = 13, StdDev = 3.570 ns
//Min = 239.053 ns, Q1 = 242.575 ns, Median = 243.989 ns, Q3 = 245.681 ns, Max = 251.938 ns
//IQR = 3.106 ns, LowerFence = 237.916 ns, UpperFence = 250.341 ns
//ConfidenceInterval = [239.819 ns; 248.370 ns] (CI 99.9%), Margin = 4.276 ns (1.75% of Mean)
//Skewness = 0.42, Kurtosis = 2.57, MValue = 2
//-------------------- Histogram --------------------
//[237.060 ns ; 242.094 ns) | @@@
//[242.094 ns ; 253.931 ns) | @@@@@@@@@@
//---------------------------------------------------

//test.test4: DefaultJob [N=10000]
//Runtime = .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT; GC = Concurrent Workstation
//Mean = 167.786 ns, StdErr = 0.343 ns (0.20%), N = 15, StdDev = 1.327 ns
//Min = 164.945 ns, Q1 = 167.022 ns, Median = 167.609 ns, Q3 = 168.473 ns, Max = 170.338 ns
//IQR = 1.451 ns, LowerFence = 164.845 ns, UpperFence = 170.649 ns
//ConfidenceInterval = [166.368 ns; 169.205 ns] (CI 99.9%), Margin = 1.418 ns (0.85% of Mean)
//Skewness = 0.01, Kurtosis = 2.77, MValue = 2
//-------------------- Histogram --------------------
//[164.239 ns ; 171.044 ns) | @@@@@@@@@@@@@@@
//---------------------------------------------------

//// * Summary *

//BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.388 (2004/?/20H1)
//Intel Core i7-8086K CPU 4.00GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
//.NET Core SDK=3.1.302
//  [Host]     : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT  [AttachedDebugger]
//  DefaultJob : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT


//| Method |     N |          Mean |        Error |     StdDev |
//|------- |------ |--------------:|-------------:|-----------:|
//|  test1 |   100 |      27.61 ns |     0.547 ns |   0.537 ns |
//|  test2 |   100 |     872.57 ns |    10.403 ns |   9.222 ns |
//|  test3 |   100 |      25.02 ns |     0.641 ns |   1.860 ns |
//|  test4 |   100 |      23.04 ns |     0.471 ns |   0.441 ns |
//|  test1 | 10000 |     496.37 ns |     7.802 ns |   6.091 ns |
//|  test2 | 10000 | 112,924.16 ns | 1,130.291 ns | 943.844 ns |
//|  test3 | 10000 |     244.09 ns |     4.276 ns |   3.570 ns |
//|  test4 | 10000 |     167.79 ns |     1.418 ns |   1.327 ns |

//// * Warnings *
//Environment
//  Summary -> Benchmark was executed with attached debugger

//// * Hints *
//Outliers
//  test.test2: Default -> 1 outlier  was  removed (903.38 ns)
//  test.test3: Default -> 3 outliers were removed (32.36 ns..34.22 ns)
//  test.test1: Default -> 3 outliers were removed (535.40 ns..664.84 ns)
//  test.test2: Default -> 2 outliers were removed (117.01 us, 117.49 us)
//  test.test3: Default -> 2 outliers were removed (354.34 ns, 355.71 ns)

//// * Legends *
//  N      : Value of the 'N' parameter
//  Mean   : Arithmetic mean of all measurements
//  Error  : Half of 99.9% confidence interval
//  StdDev : Standard deviation of all measurements
//  1 ns   : 1 Nanosecond (0.000000001 sec)

//// ***** BenchmarkRunner: End *****
//// ** Remained 0 benchmark(s) to run **
//Run time: 00:03:57 (237.76 sec), executed benchmarks: 8

//Global total time: 00:04:01 (241.71 sec), executed benchmarks: 8
//// * Artifacts cleanup *
    }
}
