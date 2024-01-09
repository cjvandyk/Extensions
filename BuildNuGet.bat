C:
cd "\SOURCE\Extensions"
nuget pack Extensions.csproj
move /Y Extensions*.nupkg ..\..\NuGet
move /Y Extensions*.snupkg ..\..\NuGet