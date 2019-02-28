dotnet publish -c Release -o bin/publish "example/Xunit.Fixture.Api.Example"
docker-compose down --remove-orphans --volumes
docker-compose up --build --remove-orphans --force-recreate --abort-on-container-exit --exit-code-from integration-tests