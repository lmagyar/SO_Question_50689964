using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Hosting;

using GrainInterfaces;

namespace DevClusterClient
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            IClusterClient client = null;
            try
            {
                client = await StartClusterClientAsync();
                await DoClientWorkAsync(client);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
            finally
            {
                Console.WriteLine("Press Enter to terminate...");
                Console.ReadKey();
                if (client != null)
                    await client.Close();
            }
        }

        private static async Task<IClusterClient> StartClusterClientAsync(int initializeAttemptsBeforeFailing = 5)
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .ConfigureLogging(logging => logging.AddConsole())
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IExampleGrain).Assembly).WithReferences())
                .Build();
            var attempt = 1;
            await client.Connect(async exception =>
            {
                if (exception is SiloUnavailableException)
                {
                    Console.WriteLine($"\nAttempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.\n");
                    if (attempt++ < initializeAttemptsBeforeFailing)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(4));
                        return true;
                    }
                }
                return false;
            });
            Console.WriteLine("Client successfully connected to silo host.");
            return client;
        }

        private static async Task DoClientWorkAsync(IClusterClient client)
        {
            var grain = client.GetGrain<IExampleGrain>(Guid.NewGuid());
            Console.WriteLine(await grain.Ping("test"));
        }
    }
}
