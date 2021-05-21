using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

//using SixLabors.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Diagnostics;

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

            var rectangles = GetRectangles(data);

            //draw rectangles on the image (copy)

            var img = Image.Load(filename);

            var count = 0;
            foreach (var rectangle in GetRectangles(data))
            {
                img.Mutate(a => a.DrawPolygon(Color.HotPink, 20, rectangle));
                count++;
            }
            Console.WriteLine($"Number of faces detected: {count}");

            var outputfilename = $"{Environment.CurrentDirectory}\\{Path.GetFileNameWithoutExtension(filename)}-2{Path.GetExtension(filename)}";
            SaveImage(img, outputfilename);

            OpenWithDefaultApp(filename);
        }

        private static void OpenWithDefaultApp(string filename)
        {
            var si = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = filename,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process.Start(si);
        }

        private static void SaveImage(Image img, string outputfilename)
        {
            using (var fs = File.Create(outputfilename))
            {
                img.SaveAsJpeg(fs);
            }
        }

        private static IEnumerable<PointF[]> GetRectangles(string data)
        {
            var faces = JArray.Parse(data);
            foreach (var face in faces)
            {
                var id = (string)face["faceId"];

                var top = (int)face["faceRectangle"]["top"];
                var left = (int)face["faceRectangle"]["left"];
                var width = (int)face["faceRectangle"]["width"];
                var height = (int)face["faceRectangle"]["height"];

                var rectangle = new PointF[]
                {
                    new PointF(left, top),
                    new PointF(left + width, top),
                    new PointF(left + width, top + height),
                    new PointF(left, top + height),
                };

                yield return rectangle;
            }
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

