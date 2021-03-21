using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static DepsTestApp.DepsTests;
using static DepsTestApp.Rates.RatesStorageInMemory;

namespace DepsTestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var client = new HttpClient();

                var data = GetData();

                client.BaseAddress = new Uri(data.BaseAddress);

                await GetCurrencyData();

                await RegisterTest(client);
                await GetConvertedCurrencyTest(client);
            }
            catch (Exception e)
            {
                Console.WriteLine("Critical unhandled exception");
                Console.WriteLine(e.Message);
            }
        }

        static public Data GetData()
        {
            if (!File.Exists("data.json"))
            {
                throw new FileNotFoundException();
            }

            var json = File.ReadAllText("data.json");
            var data = JsonSerializer.Deserialize<Data>(json);

            if (data == null)
            {
                throw new ArgumentNullException();
            }

            return data;
        }
    }
}
