# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project file
COPY ["SDTechnicalAssessment.csproj", "./"]

# Restore NuGet packages
RUN dotnet restore "SDTechnicalAssessment.csproj"

# Copy source code
COPY . .

# Build the application
RUN dotnet build "SDTechnicalAssessment.csproj" \
    -c Release \
    -o /app/build


# Stage 2: Publish
FROM build AS publish

RUN dotnet publish "SDTechnicalAssessment.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# Stage 3: Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Container port
EXPOSE 8080

# Start the API
ENTRYPOINT ["dotnet", "SDTechnicalAssessment.dll"]