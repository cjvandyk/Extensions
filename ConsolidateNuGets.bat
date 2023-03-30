@echo off
D:
cd "\SOURCE\Extensions"
move /Y bin\Debug\*.nupkg NuGet
move /Y bin\Debug\*.snupkg NuGet
move /Y Extensions.Azure\bin\Debug\*.nupkg NuGet
move /Y Extensions.Azure\bin\Debug\*.snupkg NuGet
move /Y Extensions.Core\bin\Debug\*.nupkg NuGet
move /Y Extensions.Core\bin\Debug\*.snupkg NuGet
move /Y Extensions.Graph\bin\Debug\*.nupkg NuGet
move /Y Extensions.Graph\bin\Debug\*.snupkg NuGet
move /Y Extensions.Identity\bin\Debug\*.nupkg NuGet
move /Y Extensions.Identity\bin\Debug\*.snupkg NuGet
move /Y Extensions.Logit\bin\Debug\*.nupkg NuGet
move /Y Extensions.Logit\bin\Debug\*.snupkg NuGet
move /Y Extensions.State\bin\Debug\*.nupkg NuGet
move /Y Extensions.State\bin\Debug\*.snupkg NuGet
move /Y Extensions.String\bin\Debug\*.nupkg NuGet
move /Y Extensions.String\bin\Debug\*.snupkg NuGet
move /Y Extensions.Telemetry\bin\Debug\*.nupkg NuGet
move /Y Extensions.Telemetry\bin\Debug\*.snupkg NuGet
@echo NuGets consolidated.
