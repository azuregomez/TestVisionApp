using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestVisionApp
{
    public class VisionAgent
    {
        ComputerVisionClient _client;
        public VisionAgent(string endpoint, string subscriptionKey)
        {
            // Authenticate
            _client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey)) { Endpoint = endpoint };
        }

        public async IAsyncEnumerable<string> Read(string urlFile)
        {
            // Read text from URL
            var textHeaders = await _client.ReadAsync(urlFile);
            // After the request, get the operation operation ID
            string operationLocation = textHeaders.OperationLocation;
            //Thread.Sleep(2000);

            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);
            // Extract the text
            ReadOperationResult results;            
            do
            {
                results = await _client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted));
            var textUrlFileResults = results.AnalyzeResult.ReadResults;            
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    yield return line.Text;
                }
            }           
        }

    }
}
