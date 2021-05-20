using System;
using System.IO;

namespace FaceDetection
{
    class Program
    {
        private static string msg = "Please provide the API key as the first CLI parameter, followed by the filename of the image";
        static void Main(string[] args)
        {
            //cmd line args

            var apiKey = !string.IsNullOrWhiteSpace(args[0]) ? args[0] : throw new ArgumentException(msg, args[0]);
            var filename = File.Exists(args[1]) ? args[1] : throw new FileNotFoundException(msg, args[1]);

            //http request

            var region = "westus";
            var target = new Uri($"https//{region}.api.cognitive.microsoft.com/face/1.0/detect/?subscription-key={apiKey}");
            var httpPost = CreateHttpRequest(target, "POST", "application/octet-stream");

            //load image



            //submit image to API endpoint

            //inspect JSON

            //draw rectangles on the image (copy)
        }
    }
}


// https://csharpdetection.cognitiveservices.azure.com/