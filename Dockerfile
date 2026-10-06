# Stage 1: Build and Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the solution file and project files
COPY ["SEP490_SPORTNEXUS_BE.slnx", "./"]
COPY ["SEP490_SPORTNEXUS_BE/SEP490_SPORTNEXUS_BE.API.csproj", "SEP490_SPORTNEXUS_BE/"]
COPY ["SEP490_SPORTNEXUS.Repositories/SEP490_SPORTNEXUS_BE.Repositories.csproj", "SEP490_SPORTNEXUS.Repositories/"]
COPY ["SEP490_SPORTNEXUS.Services/SEP490_SPORTNEXUS_BE.Services.csproj", "SEP490_SPORTNEXUS.Services/"]

# Restore dependencies
RUN dotnet restore "SEP490_SPORTNEXUS_BE/SEP490_SPORTNEXUS_BE.API.csproj"

# Copy the rest of the source code
COPY . .

# Build and publish
WORKDIR "/src/SEP490_SPORTNEXUS_BE"
RUN dotnet publish "SEP490_SPORTNEXUS_BE.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Tell .NET to listen on port 8080 (Render's default)
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "SEP490_SPORTNEXUS_BE.API.dll"]

