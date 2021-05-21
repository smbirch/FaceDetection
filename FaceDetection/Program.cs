﻿using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

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
            var target = new Uri($"https://{region}.api.cognitive.microsoft.com/face/v1.0/detect/?subscription-key={apiKey}");
            var httpPost = CreateHttpRequest(target, "POST", "application/octet-stream");

            //load image

            using (var fs = File.OpenRead(filename))
            {
                fs.CopyTo(httpPost.GetRequestStream());
            }

            //submit image to API endpoint

            string data = getResponse(httpPost);

            //inspect JSON

            var faces = JArray.Parse(data);
            foreach (var face in faces)
            {
                var id = (string)face["faceId"];

                var top = (int)face["faceRectangle"]["top"];
                var left = (int)face["faceRectangle"]["left"];
                var width = (int)face["faceRectangle"]["width"];
                var height = (int)face["faceRectangle"]["height"];
            }

            //draw rectangles on the image (copy)
        }

        private static string getResponse(HttpWebRequest httpPost)
        {
            using (var response = httpPost.GetResponse())
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

        private static HttpWebRequest CreateHttpRequest(Uri target, string method, string contentType)
        {
            var request = WebRequest.CreateHttp(target);
            request.Method = method;
            request.ContentType = contentType;
            return request;
        }
    }
}

