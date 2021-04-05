using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            ActivityHelper.SetConnection(_ConnectionString_);
            ActivityHelper.SetAccount(_storageAccount);
        }
        private static CloudStorageAccount _storageAccount;

        [FunctionName("TransmitToVisierOrchestration")]
        public static async Task<List<TransmitPartial>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<TransmitPartial>();
            // This is reading from the query string.  This will probably change to jsondata in the body.
            var x = context.GetInput<TransmitRequest>();
            TransmitPartial zippedPartial, encryptedPartial, transmittedPartial, notifiedPartial;
            //! more likely we will receive the data in the body.
            outputs.Add(zippedPartial = await context.CallActivityAsync<TransmitPartial>("TransmitToVisier_Zip", x.Source));
            outputs.Add(encryptedPartial = await context.CallActivityAsync<TransmitPartial>("TransmitToVisier_Encrypt", zippedPartial.Partial));
            outputs.Add(transmittedPartial = await context.CallActivityAsync<TransmitPartial>("TransmitToVisier_Transmit", encryptedPartial));
            outputs.Add(notifiedPartial = await context.CallActivityAsync<TransmitPartial>("TransmitToVisier_Notify", transmittedPartial));

            return outputs;
        }

        [FunctionName("TransmitToVisierOrchestration_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var x = req.RequestUri.ParseQueryString();
            var sourceInfo = new TransmitRequest(x["Source"]);

            string instanceId = await starter.StartNewAsync("TransmitToVisierOrchestration", sourceInfo);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}