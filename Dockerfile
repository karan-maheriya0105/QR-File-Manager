# Stage 1: Build using .NET 9.0 SDK Preview
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copy everything and publish
COPY . .
RUN dotnet publish BrochureAPI.csproj -c Release -o /app/publish

# Stage 2: Runtime using .NET 9.0 ASP.NET Preview
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Set hosting port
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "BrochureAPI.dll"]
