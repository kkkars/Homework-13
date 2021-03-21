using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DepsTestApp.Rates
{
    public static class RatesStorageInMemory
    {
        private static List<CurrencyData> _currencyData = new List<CurrencyData>();

        public static decimal ConvertCurrency(string srcCurrency, string destCurrency, decimal amount = 1)
        {

            if (string.IsNullOrWhiteSpace(srcCurrency))
            {
                throw new ArgumentNullException(nameof(srcCurrency));
            }
            if (string.IsNullOrWhiteSpace(destCurrency))
            {
                throw new ArgumentNullException(nameof(srcCurrency));
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Amount can't be less or equals zero");
            }

            srcCurrency = srcCurrency.ToUpperInvariant();
            destCurrency = destCurrency.ToUpperInvariant();

            if (!(IsExist(srcCurrency) && IsExist(destCurrency)))
            {
                throw new InvalidOperationException("Such a currency code does not exist");
            }

            if (string.Equals(srcCurrency, destCurrency))
            {
                return 1;
            }

            var uah = new CurrencyData
            {
                CC = "UAH",
                Rate = 0
            };

            _currencyData.Add(uah);

            var srcCurrencyRate = _currencyData.FirstOrDefault(currency => currency.CC == srcCurrency).Rate;
            var destCurrencyRate = _currencyData.FirstOrDefault(currency => currency.CC == destCurrency).Rate;

            if (srcCurrency == "UAH")
            {
                return (amount / destCurrencyRate);
            }

            if (destCurrency == "UAH")
            {
                return (amount * srcCurrencyRate);
            }

            return (amount * (srcCurrencyRate / destCurrencyRate));
        }

        public static async Task GetCurrencyData()
        {
            var client = new HttpClient();

            var response = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, @"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json")).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            _currencyData = JsonSerializer.Deserialize<List<CurrencyData>>(json);
        }

        private static bool IsExist(string currencyCode)
        {
            if (currencyCode == "UAH")
            {
                return true;
            }

            return _currencyData.Any(currency => currency.CC == currencyCode);
        }
    }
}
