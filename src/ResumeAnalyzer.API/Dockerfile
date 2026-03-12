FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/ResumeAnalyzer.API/ResumeAnalyzer.API.csproj", "ResumeAnalyzer.API/"]
COPY ["src/ResumeAnalyzer.Application/ResumeAnalyzer.Application.csproj", "ResumeAnalyzer.Application/"]
COPY ["src/ResumeAnalyzer.Domain/ResumeAnalyzer.Domain.csproj", "ResumeAnalyzer.Domain/"]
COPY ["src/ResumeAnalyzer.Infrastructure/ResumeAnalyzer.Infrastructure.csproj", "ResumeAnalyzer.Infrastructure/"]

RUN dotnet restore "ResumeAnalyzer.API/ResumeAnalyzer.API.csproj"

COPY src/ .

RUN dotnet publish "ResumeAnalyzer.API/ResumeAnalyzer.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ResumeAnalyzer.API.dll"]