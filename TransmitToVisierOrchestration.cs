using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace Degreed.Azure.Functions.Visier
{

    public static class TransmitToVisierOrchestration
    {
        // setup for using Azurite storage emulator:
        public const string _ConnectionString_ = "UseDevelopmentStorage=true";
        //   public const string ConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;";

        static TransmitToVisierOrchestration()
        {
            _storageAccount = CloudStorageAccount.Parse(_ConnectionString_);
            
        }
        private static CloudStorageAccount _storageAccount;
        

        [FunctionName("TransmitToVisierOrchestration")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            var source = context.GetInput<string>();

            var packageName = await context.CallActivityAsync<string>("TransmitToVisier_Package", source);
            var encryptedName = await context.CallActivityAsync<string>("TransmitToVisier_Encrypt", packageName);
            var result = await context.CallActivityAsync<string>("TransmitToVisier_Transmit", encryptedName);
            var final = await context.CallActivityAsync<string>("TransmitToVisier_Handshake", result);
            //outputs.Add(await context.CallActivityAsync<string>("TransmitToVisierOrchestration_Hello", "Tokyo"));
            //outputs.Add(await context.CallActivityAsync<string>("TransmitToVisierOrchestration_Hello", "Seattle"));
            //outputs.Add(await context.CallActivityAsync<string>("TransmitToVisierOrchestration_Hello", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

       /* [FunctionName("TransmitToVisierOrchestration_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }*/

        [FunctionName("TransmitToVisierOrchestration_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            var source = req.Content.ReadAsStringAsync().Result;
            
            string instanceId = await starter.StartNewAsync("TransmitToVisierOrchestration", source);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}