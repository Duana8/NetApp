FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build
WORKDIR /App
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/framework/sdk:4.8
WORKDIR /App
COPY --from=build-env /App/out .
EXPOSE 8888
ENTRYPOINT ["dotnet", "net-project.dll"]