using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ICSharpCode.SharpZipLib.Zip;
using System.Text.RegularExpressions;

namespace Degreed.Azure.Functions.Visier
{










    /*SAS/*
    /*http://127.0.0.1:10000/devstoreaccount1/nix01?sv=2018-03-28&si=nix01-1789424F0D1&sr=c&sig=8GGj42Ysn%2Flr8ODFgZ%2BdYtfaATSmhcv8p8JWyaeKoWg%3D*/
    /*?sv=2018-03-28&si=nix01-1789424F0D1&sr=c&sig=8GGj42Ysn%2Flr8ODFgZ%2BdYtfaATSmhcv8p8JWyaeKoWg%3D*/
    public static class ActivityHelper
    {

        private static string _connection;
        private static CloudStorageAccount _account;
        private static CloudBlobClient _client;
        private static CloudBlobContainer _container;
        private static CloudBlobDirectory _directory;
        internal static void SetConnection(string connection)
        {
            _connection = connection;
            _account = CloudStorageAccount.Parse(_connection);
            _client = new CloudBlobClient(_account.BlobStorageUri, _account.Credentials);
            _container = _client.GetContainerReference("nix01"); //!
            _directory = _container.GetDirectoryReference("batch01"); //!
        }
          //internal static void SetAccount(CloudStorageAccount account)
         // {
        //     _account = account;
       // }

        private const int _BufferSize_ = 4096;

        public async static Task<string> ZipFromBatch(string batchPath, string outputName, ILogger log)
        {
            var zipPath = Path.Combine(batchPath, outputName);

            BlobContinuationToken blobContinuationToken = null;
            List<CloudBlob> blobs = new List<CloudBlob>();
            do
            { // we expect less that 5000 files so this should not loop!
                var sourceFiles = await _directory.ListBlobsSegmentedAsync(blobContinuationToken);
                blobContinuationToken = sourceFiles.ContinuationToken;
                foreach (var item in sourceFiles.Results)
                {
                    var blob = (CloudBlockBlob)item;
                    await blob.FetchAttributesAsync();
                    blobs.Add(blob);
                }
            } while (blobContinuationToken != null);

            string current = "opening target zip file";
            // Add the target zip file...
            var outputBlob = _container.GetBlockBlobReference(zipPath);

            try
            {
                Stream output = await outputBlob.OpenWriteAsync();
                {
                    current = "output stream open";
                    using (ZipOutputStream outputStream = new ZipOutputStream(output))
                    {
                        outputStream.SetLevel(9); // highest level
                        outputStream.UseZip64 = UseZip64.On;
                        byte[] buffer = new byte[_BufferSize_];
                        int bufferCount = 0;
                        
                        foreach (var sourceBlob in blobs)
                        {
                            current = "have blob reference";
                            var inputStream = await sourceBlob.OpenReadAsync();

                            // Make an entry for this stream
                            ZipEntry entry = new ZipEntry(sourceBlob.Name);
                            // timestamp
                            var t = sourceBlob.Properties.LastModified ?? DateTimeOffset.Now;
                            entry.DateTime = t.DateTime;
                            // add to archive
                            outputStream.PutNextEntry(entry);
                            // read through the buffer
                            current = "saving zip entry";
                            do
                            {
                                bufferCount = inputStream.Read(buffer, 0, buffer.Length);
                                outputStream.Write(buffer, 0, bufferCount);
                            } while (bufferCount > 0);

                            current = "written through buffer";
                        }
                        current = "finishing zip stream";
                        outputStream.Finish();
                    }
                }
                output.Close();
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Stage: {current}");
            }

            return zipPath;
        }
        public static string EncryptedFromZip(string zipPath)
        {
            return "";
        }
        public static string Transmitted(string encryptedPath)
        {
            return "";
        }
        public static string Notified(string transmissionPath)
        {
            return "";
        }
    }
}