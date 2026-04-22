FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Raycynix.Services.AuthService.csproj", "./"]
RUN dotnet restore "Raycynix.Services.AuthService.csproj"

COPY . .
RUN dotnet publish "Raycynix.Services.AuthService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Raycynix.Services.AuthService.dll"]
