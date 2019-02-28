FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY ./bin/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Xunit.Fixture.Api.Example.dll"]