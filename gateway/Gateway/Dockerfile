FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src
COPY ["gateway/Gateway/Gateway.csproj", "gateway/Gateway/"]
RUN dotnet restore "gateway/Gateway/Gateway.csproj"

FROM restore AS publish
WORKDIR /src
COPY . .
WORKDIR "/src/gateway/Gateway"
RUN dotnet publish "Gateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.dll"]

