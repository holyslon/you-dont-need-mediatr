FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Example.App/Example.App.csproj ./Example.App/
COPY Benchmark/Benchmark.csproj ./Benchmark/
COPY Example.App.F/Example.App.F.fsproj ./Example.App.F/
COPY Example.Web/Example.Web.csproj ./Example.Web/
COPY YouDontNeedMediatR.sln ./

RUN dotnet restore YouDontNeedMediatR.sln

# Copy everything else and build
COPY ./ ./
RUN dotnet publish Example.Web/Example.Web.csproj -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:6.0 as web
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Example.Web.dll"]