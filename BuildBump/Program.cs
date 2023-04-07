#pragma warning disable CS8602

using System.Xml;

namespace BuildBump
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string version = "5.0.700";
            string targetframeworks = "net7.0;net6.0;netstandard2.1;netstandard2.0;net48;net472;net471;net47";
            string root = @"..\..\..\..";
            string[] projects = { "Extensions.Azure", "Extensions.Core", "Extensions.Graph", "Extensions.Identity", 
                "Extensions.Logit", "Extensions.State", "Extensions.String", "Extensions.Telemetry" };
            XmlDocument doc = new XmlDocument();
            doc.Load($"{root}\\Directory.Build.props");
            doc.FirstChild.FirstChild.FirstChild.InnerText = version;
            doc.Save($"{root}\\Directory.Build.props");
            doc.Load($"{root}\\Extensions.csproj");
            doc.FirstChild.FirstChild.FirstChild.InnerText = targetframeworks;
            doc.Save($"{root}\\Extensions.csproj");
            foreach (string project in projects)
            {
                doc.Load($"{root}\\{project}\\Directory.Build.props");
                doc.FirstChild.FirstChild.FirstChild.InnerText = version;
                doc.Save($"{root}\\{project}\\Directory.Build.props");
                doc.Load($"{root}\\{project}\\{project}.csproj");
                doc.FirstChild.FirstChild.FirstChild.InnerText = targetframeworks;
                doc.Save($"{root}\\{project}\\{project}.csproj");
            }
            Console.WriteLine("Done!");
        }
    }
}

#pragma warning restore CS8602