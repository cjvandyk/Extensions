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
            List<KeyValuePair<string, string>> files = new List<KeyValuePair<string, string>>();
            files.Add(new KeyValuePair<string, string>("Extensions.Azure.Blob.cs", "Extensions.Azure"));
            files.Add(new KeyValuePair<string, string>("Extensions.Constants.cs", "Extensions.Core"));
            files.Add(new KeyValuePair<string, string>("Extensions.Core.cs", "Extensions.Core"));
            files.Add(new KeyValuePair<string, string>("Extensions.TenantConfig.cs", "Extensions.Core"));
            files.Add(new KeyValuePair<string, string>("Extensions.Graph.cs", "Extensions.Graph"));
            files.Add(new KeyValuePair<string, string>("Microsoft.Graph.ListItem.cs", "Extensions.Graph"));
            files.Add(new KeyValuePair<string, string>("Extensions.Identity.App.cs", "Extensions.Identity"));
            files.Add(new KeyValuePair<string, string>("Extensions.Identity.Auth.ClientApplicationType.cs", "Extensions.Identity"));
            files.Add(new KeyValuePair<string, string>("Extensions.Identity.Auth.cs", "Extensions.Identity"));
            files.Add(new KeyValuePair<string, string>("Extensions.Identity.AuthMan.cs", "Extensions.Identity"));
            files.Add(new KeyValuePair<string, string>("Extensions.Identity.Cert.cs", "Extensions.Identity"));
            files.Add(new KeyValuePair<string, string>("Extensions.Identity.Scopes.cs", "Extensions.Identity"));
            files.Add(new KeyValuePair<string, string>("System.Logit.cs", "Extensions.Logit"));
            files.Add(new KeyValuePair<string, string>("Extensions.State.cs", "Extensions.State"));
            files.Add(new KeyValuePair<string, string>("System.Object.cs", "Extensions.State"));
            files.Add(new KeyValuePair<string, string>("System.String.cs", "Extensions.String"));
            files.Add(new KeyValuePair<string, string>("System.Text.StringBuilder.cs", "Extensions.String"));
            files.Add(new KeyValuePair<string, string>("Extensions.Telemetry.cs", "Extensions.Telemetry"));
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
                File.Copy(
                    $"{root}\\{project}\\bin\\debug\\{project}.{version}.nupkg", 
                    $"{root}\\NuGet\\{project}.{version}.nupkg", true);
                File.Copy(
                    $"{root}\\{project}\\bin\\debug\\{project}.{version}.snupkg",
                    $"{root}\\NuGet\\{project}.{version}.snupkg", true);
            }
            foreach (var file in files)
            {
                File.Copy($"{root}\\Classes\\{file.Key}", $"{root}\\{file.Value}\\{file.Key}", true);
            }
            Console.WriteLine("Done!");
        }
    }
}

#pragma warning restore CS8602