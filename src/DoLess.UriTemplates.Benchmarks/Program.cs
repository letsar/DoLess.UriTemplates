using System;
using System.Collections.Generic;
using System.Diagnostics;
using DoLess.UriTemplates.Benchmarks.Adapters;

namespace DoLess.UriTemplates.Benchmarks
{
    class MainClass
    {
        private const double Iterations = 100000;

        public static void Main(string[] args)
        {
            Benchmark01();
        }

        private static void Benchmark01()
        {
            var template = "http://{host,hostNumber}.org{/path01,path02,path03}{?query,filters*}{#fragment}";
            var variables = new Dictionary<string, object>
            {
                ["host"] = "www.example",
                ["hostNumber"] = "01",
                ["path02"] = "resources",
                ["path03"] = "books",
                ["query"] = "foo",
                ["filters"] = new[] { "genre", "author" },
                ["fragment"] = "frag"
            };

            Stopwatch swatch = new Stopwatch();
            Dictionary<string, double> results = new Dictionary<string, double>();
            var adapters = CreateAdapters();
            var count = adapters.Count;

            for (int i = 0; i < count; i++)
            {
                var adapter = adapters[i];
                adapter.Template = template;
                foreach (var key in variables.Keys)
                {
                    adapter.AddParameter(key, variables[key]);
                }
            }

            for (int i = 0; i < count; i++)
            {
                var adapter = adapters[i];
                swatch.Restart();
                for (long j = 0; j < Iterations; j++)
                {
                    adapter.Expand();
                }
                swatch.Stop();
                results[adapter.Name] = swatch.ElapsedMilliseconds / Iterations;
            }

            foreach (var key in results.Keys)
            {
                Console.WriteLine($"{key}: {results[key]}ms");
            }

            Console.Read();
        }

        private static IReadOnlyList<UriTemplateAdapter> CreateAdapters()
        {
            return new List<UriTemplateAdapter>
            {
                new DoLessAdapter(),
                new RestaAdapter(),
                new TavisAdapter()
            };
        }
    }
}
