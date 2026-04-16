# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PersonalFinanceManager/PersonalFinanceManager.csproj", "PersonalFinanceManager/"]
RUN dotnet restore "PersonalFinanceManager/PersonalFinanceManager.csproj"

COPY . .
WORKDIR "/src/PersonalFinanceManager"
RUN dotnet publish -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:$PORT

ENTRYPOINT ["dotnet", "PersonalFinanceManager.dll"]