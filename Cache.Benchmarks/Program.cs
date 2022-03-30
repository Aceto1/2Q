using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Cache._2Q;
using Cache.Benchmarks.Enum;
using Cache.Benchmarks.Model;
using Cache.Experiments.Utility;
using Cache.LRU;
using Cache.LRUK;

namespace Cache.Experiments
{
    internal class Program
    {
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            string outputDirectory;

            if (args.Length != 1 ||
               string.IsNullOrEmpty(outputDirectory = args[0]))
            {
                Console.WriteLine("Usage: Cache.Benchmarks.exe {outputDirectory}");
                return;
            }

            var pool1 = new List<string>();
            var pool2 = new List<string>();

            for (int i = 0; i < 100; i++)
            {
                pool1.Add("I " + (i + 1).ToString());
            }

            for (int i = 0; i < 10000; i++)
            {
                pool2.Add("R " + (i + 1).ToString());
            }

            var twoPoolDistribution = BuildTwoPoolDistribution(pool1, pool2, 1_000_000);
            var zipfDistribution1 = BuildZipfian(50_000, 0.86f, 1_000_000);
            var zipfDistribution2 = BuildZipfian(50_000, 0.5f, 1_000_000);
            var zipfDistributionWithScans1 = BuildZipfianWithScans(50_000, 0.86f, 1_000_000, 0.333, new int[] { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000 });
            var zipfDistributionWithScans2 = BuildZipfianWithScans(50_000, 0.5f,  1_000_000, 0.333, new int[] { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000 });

            var configurations = new List<Configuration>
            {
                // Zipf distribution with alpha value of 0.86, buffer sizes ranging from 5% to 40%
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 5% buffer size",
                    PageCount = 50_000,
                    PageSlots = 2_500,
                    SampleSize = 1_000_000,
                    References = zipfDistribution1
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 10% buffer size",
                    PageCount = 50_000,
                    PageSlots = 5_000,
                    SampleSize = 1_000_000,
                    References = zipfDistribution1
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 15% buffer size",
                    PageCount = 50_000,
                    PageSlots = 7_500,
                    SampleSize = 1_000_000,
                    References = zipfDistribution1
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 20% buffer size",
                    PageCount = 50_000,
                    PageSlots = 10_000,
                    SampleSize = 1_000_000,
                    References = zipfDistribution1
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 25% buffer size",
                    PageCount = 50_000,
                    PageSlots = 12_500,
                    SampleSize = 1_000_000,
                    References = zipfDistribution1
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 30% buffer size",
                    PageCount = 50_000,
                    PageSlots = 15_000,
                    SampleSize = 1_000_000,
                    References = zipfDistribution1
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 35% buffer size",
                    PageCount = 50_000,
                    PageSlots = 17_500,
                    SampleSize = 1_000_000,
                    References = zipfDistribution1
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 40% buffer size",
                    PageCount = 50_000,
                    PageSlots = 20_000,
                    SampleSize = 1_000_000,
                    References = zipfDistribution1
                },

                // Zipf Distribution with alpha value of 0.5, buffer sizes ranging from 5% to 40%
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 5% buffer size",
                    PageCount = 50_000,
                    PageSlots = 2_500,
                    SampleSize = 1_000_000,
                    References = zipfDistribution2
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 10% buffer size",
                    PageCount = 50_000,
                    PageSlots = 5_000,
                    SampleSize = 1_000_000,
                    References = zipfDistribution2
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 15% buffer size",
                    PageCount = 50_000,
                    PageSlots = 7_500,
                    SampleSize = 1_000_000,
                    References = zipfDistribution2
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 20% buffer size",
                    PageCount = 50_000,
                    PageSlots = 10_000,
                    SampleSize = 1_000_000,
                    References = zipfDistribution2
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 25% buffer size",
                    PageCount = 50_000,
                    PageSlots = 12_500,
                    SampleSize = 1_000_000,
                    References = zipfDistribution2
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 30% buffer size",
                    PageCount = 50_000,
                    PageSlots = 15_000,
                    SampleSize = 1_000_000,
                    References = zipfDistribution2
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 35% buffer size",
                    PageCount = 50_000,
                    PageSlots = 17_500,
                    SampleSize = 1_000_000,
                    References = zipfDistribution2
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 40% buffer size",
                    PageCount = 50_000,
                    PageSlots = 20_000,
                    SampleSize = 1_000_000,
                    References = zipfDistribution2
                },

                // Zipf Distribution with alpha value of 0.86, scan length of 1000-8000, 10% of pages can fit in buffer
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 10% buffer size, scan length 1000",
                    PageCount = 50_000,
                    PageSlots = 5_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans1[0]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 10% buffer size, scan length 2000",
                    PageCount = 50_000,
                    PageSlots = 5_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans1[1]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 10% buffer size, scan length 3000",
                    PageCount = 50_000,
                    PageSlots = 5_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans1[2]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 10% buffer size, scan length 4000",
                    PageCount = 50_000,
                    PageSlots = 5_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans1[3]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 10% buffer size, scan length 5000",
                    PageCount = 50_000,
                    PageSlots = 5_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans1[4]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 10% buffer size, scan length 6000",
                    PageCount = 50_000,
                    PageSlots = 5_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans1[5]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 10% buffer size, scan length 7000",
                    PageCount = 50_000,
                    PageSlots = 5_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans1[6]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.86, 10% buffer size, scan length 8000",
                    PageCount = 50_000,
                    PageSlots = 5_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans1[7]
                },

                // Zipf Distribution with alpha value of 0.5, scan length of 1000-8000, 20% of pages can fit in buffer#
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 20% buffer size, scan length 1000",
                    PageCount = 50_000,
                    PageSlots = 10_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans2[0]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 20% buffer size, scan length 2000",
                    PageCount = 50_000,
                    PageSlots = 10_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans2[1]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 20% buffer size, scan length 3000",
                    PageCount = 50_000,
                    PageSlots = 10_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans2[2]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 20% buffer size, scan length 4000",
                    PageCount = 50_000,
                    PageSlots = 10_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans2[3]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 20% buffer size, scan length 5000",
                    PageCount = 50_000,
                    PageSlots = 10_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans2[4]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 20% buffer size, scan length 6000",
                    PageCount = 50_000,
                    PageSlots = 10_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans2[5]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 20% buffer size, scan length 7000",
                    PageCount = 50_000,
                    PageSlots = 10_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans2[6]
                },
                new Configuration
                {
                    Description = "Zipf distribution, alpha = 0.5, 20% buffer size, scan length 8000",
                    PageCount = 50_000,
                    PageSlots = 10_000,
                    SampleSize = 1_000_000,
                    References = zipfDistributionWithScans2[7]
                },

                // Two Pool experiment, buffer sizes ranging from 60 to 160
                new Configuration
                {
                    Description = "Two Pool experiment, 100 index pages, 10.000 reference pages, 60 buffer size",
                    PageCount = 10_100,
                    PageSlots = 60,
                    SampleSize = 1_000_000,
                    References = twoPoolDistribution
                },
                new Configuration
                {
                    Description = "Two Pool experiment, 100 index pages, 10.000 reference pages, 70 buffer size",
                    PageCount = 10_100,
                    PageSlots = 70,
                    SampleSize = 1_000_000,
                    References = twoPoolDistribution
                },
                new Configuration
                {
                    Description = "Two Pool experiment, 100 index pages, 10.000 reference pages, 80 buffer size",
                    PageCount = 10_100,
                    PageSlots = 80,
                    SampleSize = 1_000_000,
                    References = twoPoolDistribution
                },
                new Configuration
                {
                    Description = "Two Pool experiment, 100 index pages, 10.000 reference pages, 90 buffer size",
                    PageCount = 10_100,
                    PageSlots = 90,
                    SampleSize = 1_000_000,
                    References = twoPoolDistribution
                },
                new Configuration
                {
                    Description = "Two Pool experiment, 100 index pages, 10.000 reference pages, 100 buffer size",
                    PageCount = 10_100,
                    PageSlots = 100,
                    SampleSize = 1_000_000,
                    References = twoPoolDistribution
                },
                new Configuration
                {
                    Description = "Two Pool experiment, 100 index pages, 10.000 reference pages, 110 buffer size",
                    PageCount = 10_100,
                    PageSlots = 110,
                    SampleSize = 1_000_000,
                    References = twoPoolDistribution
                },
                new Configuration
                {
                    Description = "Two Pool experiment, 100 index pages, 10.000 reference pages, 120 buffer size",
                    PageCount = 10_100,
                    PageSlots = 120,
                    SampleSize = 1_000_000,
                    References = twoPoolDistribution
                },
                new Configuration
                {
                    Description = "Two Pool experiment, 100 index pages, 10.000 reference pages, 130 buffer size",
                    PageCount = 10_100,
                    PageSlots = 130,
                    SampleSize = 1_000_000,
                    References = twoPoolDistribution
                },
                new Configuration
                {
                    Description = "Two Pool experiment, 100 index pages, 10.000 reference pages, 140 buffer size",
                    PageCount = 10_100,
                    PageSlots = 140,
                    SampleSize = 1_000_000,
                    References = twoPoolDistribution
                },
                new Configuration
                {
                    Description = "Two Pool experiment, 100 index pages, 10.000 reference pages, 150 buffer size",
                    PageCount = 10_100,
                    PageSlots = 150,
                    SampleSize = 1_000_000,
                    References = twoPoolDistribution
                },
                new Configuration
                {
                    Description = "Two Pool experiment, 100 index pages, 10.000 reference pages, 160 buffer size",
                    PageCount = 10_100,
                    PageSlots = 160,
                    SampleSize = 1_000_000,
                    References = twoPoolDistribution
                }
            };

            for (int i = 0; i < configurations.Count; i++)
            {
                var configuration = configurations[i];

                var twoQ = new _2Q<string>(configuration.PageSlots, 1, configuration.PageSlots / 2);
                var lru = new LRU<string>(configuration.PageSlots);
                // crp = 0, since there are no correlated references for these benchmarks
                var lruk = new LRUK<string>(configuration.PageSlots, 2, 0);
                var result = new Result()
                {
                    PageCount = configuration.PageCount,
                    PageSlots = configuration.PageSlots,
                    Stats = new Dictionary<Algorithm, CacheStatistic>()
                    {
                        { Algorithm._2Q,  new CacheStatistic() },
                        { Algorithm.LRU,  new CacheStatistic() },
                        { Algorithm.LRUK, new CacheStatistic() },
                    }
                };

                foreach (var reference in configuration.References)
                {
                    if (twoQ.Access(reference))
                        result.Stats[Algorithm._2Q].Hits++;
                    else
                        result.Stats[Algorithm._2Q].Misses++;

                    if (lru.Access(reference))
                        result.Stats[Algorithm.LRU].Hits++;
                    else
                        result.Stats[Algorithm.LRU].Misses++;

                    if (lruk.Access(reference))
                        result.Stats[Algorithm.LRUK].Hits++;
                    else
                        result.Stats[Algorithm.LRUK].Misses++;
                }

                var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });

                Console.WriteLine();
                Console.WriteLine($"{configuration.Description}:");
                Console.WriteLine(json);

                var dir = Directory.CreateDirectory(outputDirectory);
                File.WriteAllText($"{dir.FullName}\\{i}.json", json);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Takes a list of available pages and returns a list of randomly generated accesses to them using a zipfian input distribution.
        /// </summary>
        /// <param name="availablePages">The "keys" of the available pages. In a real world example this could be the addresses on disk.</param>
        /// <param name="alpha">Value of the alpha parameter</param>
        /// <param name="length">Desired length of the reference list</param>
        /// <returns>A list of page accesses</returns>
        private static List<string> BuildZipfian(int pageCount, float alpha, int length)
        {
            var samples = new int[length];
            Zipf.Samples(rnd, samples, alpha, pageCount);

            var result = new List<string>();
            foreach (var sample in samples)
            {
                result.Add(sample.ToString());
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageCount"></param>
        /// <param name="alpha"></param>
        /// <param name="length"></param>
        /// <param name="scanRatio"></param>
        /// <returns></returns>
        private static List<string>[] BuildZipfianWithScans(int pageCount, float alpha, int length, double scanRatio, int[] scanLengths)
        {
            var result = new List<List<string>>();

            var samples = new int[length];
            Zipf.Samples(rnd, samples, alpha, pageCount);

            var targetScanCount = (int)Math.Floor(length * scanRatio);

            foreach (var scanLength in scanLengths)
            {
                var scanCount = 0;
                var usedIndices = new HashSet<int>();
                var resultSample = (int[])samples.Clone();

                while (scanCount < targetScanCount)
                {
                    // Random start index for the scan with max value of the last index that would fit the whole scan
                    var scanStartIndex = rnd.Next(length - scanLength - 1);

                    var usedIndex = false;
                    for (int i = 0; i < scanLength; i++)
                    {
                        // Index was used in a scan before, start new scan somewhere else
                        if (usedIndices.Contains(i + scanStartIndex))
                        {
                            usedIndex = true;
                            break;
                        }
                    }

                    if (usedIndex)
                        continue;

                    for (int i = 0; i < scanLength; i++)
                    {
                        var element = resultSample[i + scanStartIndex];

                        // Scan has reached the last page, start from the beginning
                        if (element == pageCount)
                            element = 0;

                        resultSample[i + scanStartIndex + 1] = element + 1;

                        usedIndices.Add(i + scanStartIndex);
                        scanCount++;

                        if (scanCount >= targetScanCount)
                            break;
                    }
                }

                var resultSampleList = new List<string>();

                foreach (var sample in resultSample)
                {
                    resultSampleList.Add(sample.ToString());
                }

                result.Add(resultSampleList);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Takes two lists of pages and returns a list with alternating random accesses to them.
        /// </summary>
        /// <param name="pool1">List that contains the index pages</param>
        /// <param name="pool2">List that contains the reference pages</param>
        /// <param name="length">Desired length of the reference list</param>
        /// <returns></returns>
        public static List<string> BuildTwoPoolDistribution(List<string> pool1, List<string> pool2, int length)
        {
            var result = new List<string>();

            for (int i = 0; i < length / 2; i++)
            {
                var pool1Sample = rnd.Next(pool1.Count);
                var pool2Sample = rnd.Next(pool2.Count);

                result.Add(pool1[pool1Sample]);
                result.Add(pool2[pool2Sample]);
            }

            return result;
        }
    }
}