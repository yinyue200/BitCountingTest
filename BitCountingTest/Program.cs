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
            var summary = BenchmarkRunner.Run<test>();
            //var random = new Random();
            //for (var j = 0; j < 1000; j++)
            //{
            //    var data = new BitArray(random.Next(900,1100));
            //    data.SetAll(true);
            //    for(var i = 0; i < data.Length; i++)
            //    {
            //        if(random.Next(0, 2) == 1)
            //        {
            //            data.Set(i, false);
            //        }
            //    }
            //    var a1 = test.GetCardinality(data);
            //    var a2 = test.GetCardinality2(data);
            //    var a3 = test.GetCardinality3(data);
            //    if (a1 != a2)
            //    {
            //        ;
            //    }
            //    if (a3 != a2)
            //    {
            //        ;
            //    }
            //}
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
    }
}
