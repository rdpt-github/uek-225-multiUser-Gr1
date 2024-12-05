using System;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FSharp.Json;
using L_Bank_W_Backend.Core.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace LBank.Tests.Loadtest.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var jwt = await Login("admin", "adminpass");
            List<Ledger> ledgers = await GetAllLedgers(jwt);
            foreach (var ledger in ledgers)
            {
                Console.WriteLine(ledger.Name);
            }

            var beforeTotal = GetTotal(ledgers);
            NbomberTest(jwt);
            CheckTotals(beforeTotal, GetTotal(await GetAllLedgers(jwt)));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static void CheckTotals(decimal beforeTotal, decimal getTotal)
        {
            Console.WriteLine("Starting money:" + beforeTotal + " ending money:" + getTotal + " difference:" +
                              (getTotal - beforeTotal));
        }

        private static decimal GetTotal(List<Ledger> getAllLedgers)
        {
            decimal result = 0;
            getAllLedgers.ForEach(ledger => result += ledger.Balance);
            return result;
        }

        private static void NbomberTest(AuthDto token)
        {
            var scenario = Scenario.Create("http_scenario", async context =>
                {
                    var ledgers = await GetAllLedgers(token);
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.token);
                    var random = new Random();
                    var first = ledgers[random.Next(ledgers.Count - 1)];
                    var second = ledgers[random.Next(ledgers.Count - 1)];
                    while (first.Equals(second))
                    {
                        second = ledgers[random.Next(ledgers.Count - 1)];
                    }

                    decimal amount = 0;
                    if (first.Balance > 0)
                    {
                        amount = random.Next((int)Math.Floor(first.Balance));
                    }
                    var booking = new Booking();
                    booking.SourceId = first.Id;
                    booking.DestinationId = second.Id;
                    booking.Amount = amount;
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    var jsonContent = JsonSerializer.Serialize(booking, options);
                    var request = Http.CreateRequest("POST", "http://localhost:5000/api/v1/bookings")
                        .WithBody(new StringContent(jsonContent, Encoding.UTF8, "application/json")); 

                    var response = await Http.Send(httpClient, request);

                    return response;
                })
                .WithoutWarmUp();

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithReportFileName("fetch_users_report")
                .WithReportFolder("fetch_users_reports")
                .WithReportFormats(ReportFormat.Html)
                .Run();
        }

        private static async Task<List<Ledger>> GetAllLedgers(AuthDto jwt)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.token);
            var response = httpClient.GetAsync("http://localhost:5000/api/v1/ledgers");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<Ledger>>(await response.Result.Content.ReadAsStringAsync(), options);
        }

        private static async Task<AuthDto> Login(string admin, string adminpass)
        {
            using var httpClient = new HttpClient();
            var response = httpClient.PostAsync("http://localhost:5000/api/v1/login", new StringContent(
                JsonSerializer.Serialize(new { username = admin, password = adminpass }),
                Encoding.UTF8,
                "application/json"
            ));
            return JsonSerializer.Deserialize<AuthDto>(await response.Result.Content.ReadAsStringAsync());
        }
    }

    internal class AuthDto
    {
        public string token { get; set; }
    }
}