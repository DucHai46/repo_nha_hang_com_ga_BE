# Sử dụng image .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy project files và restore dependencies
COPY . .
RUN dotnet restore

# Build ứng dụng
RUN dotnet publish -c Release -o out

# Sử dụng image runtime cho ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port
EXPOSE 80

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "repo_nha_hang_com_ga_BE.dll"]