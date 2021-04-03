FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Src/Contract/Contract.csproj", "Src/Contract/"]
COPY ["Src/Application/Application.csproj", "Src/Application/"]
COPY ["Src/WebApi/WebApi.csproj", "Src/WebApi/"]
RUN dotnet restore "Src/WebApi/WebApi.csproj"
COPY . .
WORKDIR "/src/Src/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Isitar.FileStorage.WebApi.dll"]
