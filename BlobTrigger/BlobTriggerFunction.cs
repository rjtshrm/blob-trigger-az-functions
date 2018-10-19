using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace BlobTrigger
{
    public static class BlobTriggerFunction
    {
        /// <summary>
        /// Returns an authenticated Azure context using the credentials in the
        /// specified auth file.
        /// </summary>
        /// <param name="authFilePath">The full path to a credentials file on the local filesystem.</param>
        /// <returns>Authenticated IAzure context.</returns>
        private static IAzure GetAzureContext(string authFilePath)
        {
            IAzure azure;
            ISubscription sub;

            try
            {
                Console.WriteLine($"Authenticating with Azure using credentials in file at {authFilePath}");

                azure = Azure.Authenticate(authFilePath).WithDefaultSubscription();
                sub = azure.GetCurrentSubscription();

                Console.WriteLine($"Authenticated with subscription '{sub.DisplayName}' (ID: {sub.SubscriptionId})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFailed to authenticate:\n{ex.Message}");

                if (String.IsNullOrEmpty(authFilePath))
                {
                    Console.WriteLine("Have you set the AZURE_AUTH_LOCATION environment variable?");
                }

                throw;
            }

            return azure;
        }

        [FunctionName("BlobTrigger")]
        public static void Run([BlobTrigger("blob-store/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, TraceWriter log)
        {
            string authFilePath = "C:\\Users\\ZORSHARM\\Desktop\\BlobTrigger\\BlobTrigger\\my.azureauth";

            IAzure azure = GetAzureContext(authFilePath);

            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
