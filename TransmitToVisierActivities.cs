using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Degreed.Azure.Functions.Visier
{
    public static class transmitToVisierActivities
    {
        [FunctionName("TransmitToVisier_Zip")]
        public static TransmitPartial Package([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var containerInfo = context.GetInput<string>();
            var status = $"Packaging {containerInfo}";
            log.LogInformation(status + ".");
            
            var packageInfo = "Package Info";

            return new TransmitPartial(packageInfo, status + " completed.");
        }
        [FunctionName("TransmitToVisier_Encrypt")]
        public static TransmitPartial Encrypt([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var packageInfo = context.GetInput<string>();
            var status = $"Encrypting {packageInfo}.";
            log.LogInformation(status + ".");
            //:




            string encryptedInfo = "encryptedBlob";
            return new TransmitPartial(encryptedInfo, status);
        }
        [FunctionName("TransmitToVisier_Transmit")]
        public static TransmitPartial Transmit([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var encryptedInfo = context.GetInput<string>();
            var status = $"Encrypting {encryptedInfo}.";
            log.LogInformation(status + ".");
            //:




            string transmittedInfo = "completed";
            return new TransmitPartial(transmittedInfo, status);
        }
        [FunctionName("TransmitToVisier_Notify")]
        public static TransmitPartial Handshake([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var transmittedInfo = context.GetInput<string>();
            var status = $"Handshaking {transmittedInfo}.";
            log.LogInformation(status + ".");
            //:



            string handshakeInfo = "acknowledged";
            return new TransmitPartial(handshakeInfo, status);
        }
    }
}