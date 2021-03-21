using DepsTestApp.Contracts;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using static DepsTestApp.Rates.RatesStorageInMemory;

namespace DepsTestApp
{
    static class DepsTests
    {
        public static async Task RegisterTest(HttpClient client)
        {
            Console.WriteLine("Test: RegisterTest\n");

            var uri = client.BaseAddress + "register";

            Console.WriteLine("[ValidModelRegisterCase]");

            var loginData = new LoginDataDto
            {
                Login = "string1",
                Password = "string"
            };

            await Register(client, loginData, "register", 200);

            Console.WriteLine("[InvalidModelRegisterCase]");
            await Register(client, new LoginDataDto(), "register", 400);

            Console.WriteLine("[RegisterAlreadyExistedAccountCase]");
            await Register(client, loginData, "register", 409);
        }

        public static async Task GetConvertedCurrencyTest(HttpClient client)
        {
            Console.WriteLine("Test: GetConvertedCurrency\n");

            Console.WriteLine("{When unauthorized}\n");

            Console.WriteLine("[SameCurrencyCase(WithDefaultAmount)]");
            await CurrencyConvertTest(client, "Rates/EUR/EUR", "1",401);

            Console.WriteLine("[DifferentCurrencyCase(WithDefaultAmount)]");
            await CurrencyConvertTest(client, "Rates/EUR/UAH", "",401);

            Console.WriteLine("[DifferentCurrencyCase(WithSettedAmount)]");
            await CurrencyConvertTest(client, "Rates/EUR/UAH?amount=1000", "",401);

            Console.WriteLine("[NonExistingCurrencyCase(WithDefaultAmount)]");
            await CurrencyConvertTest(client, "Rates/asR/lll", "Invalid currency code", 401);

            Console.WriteLine("{When authorized}\n");

            string expectedContent;
            client.DefaultRequestHeaders.Add("Authorization", "Basic c3RyaW5nMTpzdHJpbmc=");

            Console.WriteLine("[SameCurrencyCase(WithDefaultAmount)]");
            await CurrencyConvertTest(client, "Rates/EUR/EUR", "1");

            Console.WriteLine("[DifferentCurrencyCase(WithDefaultAmount)]");
            expectedContent = GetExpectedRate("EUR", "UAH");
            await CurrencyConvertTest(client, "Rates/EUR/UAH", expectedContent);

            Console.WriteLine("[DifferentCurrencyCase(WithSettedAmount)]");
            expectedContent = GetExpectedRate("EUR", "UAH", 1000);
            await CurrencyConvertTest(client, "Rates/EUR/UAH?amount=1000", expectedContent);

            Console.WriteLine("[NonExistingCurrencyCase(WithDefaultAmount)]");
            await CurrencyConvertTest(client, "Rates/asR/lll", "Invalid currency code", 400);

            Console.WriteLine();
        }

        private static async Task CurrencyConvertTest(
            HttpClient client, 
            string requestUri, 
            string expectedContent, 
            int expectedStatusCode = 200)
        {
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, client.BaseAddress + requestUri));

            var actualContent = await response.Content.ReadAsStringAsync();

            expectedContent = expectedContent.Replace(',', '.');

            if (actualContent.Length == 0)
            {
                Console.WriteLine($"  Request: {{/{requestUri}}}\n  Expected-> Status code: {{{expectedStatusCode}}}             \n  Actual-> Status code: {{{(int)response.StatusCode}}}\n  Success: {(int)response.StatusCode == expectedStatusCode}");

            }
            else
            {
                Console.WriteLine($"  Request: {{/{requestUri}}}\n  Expected-> Status code: {{{expectedStatusCode}}}\n             Content:{expectedContent}\n  Actual-> Status code: {{{(int)response.StatusCode}}}\n           Content: {actualContent}\n  Success: {(int)response.StatusCode == expectedStatusCode && expectedContent == actualContent}");
            }

            Console.WriteLine();
        }

        private static async Task Register(
            HttpClient client, 
            LoginDataDto loginData, 
            string uri, 
            int expectedStatusCode)
        {
            var response = await client.PostAsync(uri, loginData, new JsonMediaTypeFormatter());

            Console.WriteLine($"  Request: {{/{uri}}}\n  Expected-> Status code: {{{expectedStatusCode}}}           \n  Actual-> Status code: {{{(int)response.StatusCode}}}           \n  Success: {(int)response.StatusCode == expectedStatusCode}");

            Console.WriteLine();
        }

        private static string GetExpectedRate(
            string srcCurrency, 
            string destCurrency,
            decimal amount = 1)
        {
           return ConvertCurrency(srcCurrency, destCurrency, amount).ToString();
        }
    }
}
