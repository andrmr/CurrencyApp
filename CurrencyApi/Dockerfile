FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5165
EXPOSE 7165

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CurrencyApi/CurrencyApi.csproj", "CurrencyApi/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "CurrencyApi/CurrencyApi.csproj"
COPY . .
WORKDIR "/src/CurrencyApi"
RUN dotnet build "CurrencyApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CurrencyApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CurrencyApi.dll"]