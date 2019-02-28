FROM microsoft/dotnet:2.2-sdk
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "test", "-c", "Release", "example/Xunit.Fixture.Api.Example.Tests"]