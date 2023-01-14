D:
cd "\SOURCE\Extensions"
rem md "bin\Debug\net6.0"
rem copy Extensions6\bin\Debug\net6.0\Extensions6.dll bin\Debug\net6.0\Extensions6.dll /Y
rem copy Projects\Properties\AssemblyInfo480.cs Properties\AssemblyInfo.cs /Y
nuget pack Extensions.csproj
rem move /Y Extensions*.nupkg NuGet
rem cd "Projects"
rem copy Properties\AssemblyInfo472.cs ..\Properties\AssemblyInfo.cs /Y
rem nuget pack Extensions472.csproj
rem move /Y Extensions*.nupkg ..\NuGet
rem cd..
