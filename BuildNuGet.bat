D:
cd "\SOURCE\Extensions"
rem copy Projects\Properties\AssemblyInfo480.cs Properties\AssemblyInfo.cs /Y
nuget pack Extensions.csproj
move /Y Extensions*.nupkg NuGet
rem cd "Projects"
rem copy Properties\AssemblyInfo472.cs ..\Properties\AssemblyInfo.cs /Y
rem nuget pack Extensions472.csproj
rem move /Y Extensions*.nupkg ..\NuGet
rem cd..
