:: Example: .\packnpush SharpIgnite 0.1.1

nuget pack %1.csproj
nuget push %1.%2.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey %NUGET_API_KEY%
