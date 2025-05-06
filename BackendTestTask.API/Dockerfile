FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY *.sln ./
COPY BackendTestTask.API/*.csproj ./BackendTestTask.API/
COPY BackendTestTask.Common/*.csproj ./BackendTestTask.Common/
COPY BackendTestTask.Services/*.csproj ./BackendTestTask.Services/
COPY BackendTestTask.Database/*.csproj ./BackendTestTask.Database/
COPY BackendTestTask.AspNetExtensions/*.csproj ./BackendTestTask.AspNetExtensions/
RUN dotnet restore

COPY . .
WORKDIR /app/BackendTestTask.API
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/BackendTestTask.API/out ./
ENTRYPOINT ["dotnet", "BackendTestTask.API.dll"]
