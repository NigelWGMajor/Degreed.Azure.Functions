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

namespace Degreed.Azure.Functions.Visier
{
    /*SAS/*
    /*http://127.0.0.1:10000/devstoreaccount1/nix01?sv=2018-03-28&si=nix01-1789424F0D1&sr=c&sig=8GGj42Ysn%2Flr8ODFgZ%2BdYtfaATSmhcv8p8JWyaeKoWg%3D*/
    /*?sv=2018-03-28&si=nix01-1789424F0D1&sr=c&sig=8GGj42Ysn%2Flr8ODFgZ%2BdYtfaATSmhcv8p8JWyaeKoWg%3D*/
    public static class ActivityHelper
    {
        private static string _connection;
        private static CloudStorageAccount _account;
        internal static void SetConnection(string connection)
        {
            _connection = connection;
        }
        internal static void SetAccount(CloudStorageAccount account)
        {
            _account = account;
        }

        private const int _BufferSize_ = 4096;

        public async static Task<string> ZipFromBatch(string batchPath, string outputName)
        {
            CloudBlobClient client = _account.CreateCloudBlobClient();
            var container = client.GetContainerReference("nix01");

            BlobContinuationToken blobContinuationToken = null;
            List<IListBlobItem> blobs = new List<IListBlobItem>();
            do
            { // we expect less that 5000 files so this should loop!
                var sourceFiles = await container.ListBlobsSegmentedAsync(null, blobContinuationToken);
                blobContinuationToken = sourceFiles.ContinuationToken;
                foreach (var blob in sourceFiles.Results)
                {
                    blobs.Add(blob);
                }
            } while (blobContinuationToken != null)

            // Add the target zip file...
            var output = container.GetBlockBlobReference(outputName);
            using (Stream output = new FileStream(outputPath, FileMode.Create))
	    	{
		    	using (zip.ZipOutputStream outputStream = new zip.ZipOutputStream(output, _BufferSize_))
			    {
				    outputStream.SetLevel(9); // highest level
			    	byte[] buffer = new byte[_BufferSize_];
			    	int bufferCount = 0;

			     	foreach (var sourceFile in sourceFiles)
				    {
					    // upload the string for the virtual file name 
				    	sourceFile.SetOrgAndTimeStamp(_orgId, _timeStamp);
				    	// make a stream, as that's what we would normally be working with...
				    	using (FileStream inputStream = new FileStream(sourceFile.SourcePath, FileMode.Open))
				    	{
				    		// Make an entry for this stream
				    		zip.ZipEntry entry = new zip.ZipEntry(sourceFile.FullPath);
				    		// timestamp
				    		entry.DateTime = _packageDateTime;
				    		// add to archive
				    		outputStream.PutNextEntry(entry);
				    		// read through the buffer
				    		do
				    		{
					    		bufferCount = inputStream.Read(buffer, 0, buffer.Length);
						    	outputStream.Write(buffer, 0, bufferCount);
				    		} while (bufferCount > 0);
					    } // memory stream closed
				    }
				    outputStream.Finish();
                }
			}
            return "hljuhlkihj.zip";
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