# API — Acervo Imobiliário (.NET 9)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY AcervoImobiliario.sln ./
COPY AcervoImobiliario.Api/AcervoImobiliario.Api.csproj AcervoImobiliario.Api/
COPY AcervoImobiliario.Application/AcervoImobiliario.Application.csproj AcervoImobiliario.Application/
COPY AcervoImobiliario.Domain/AcervoImobiliario.Domain.csproj AcervoImobiliario.Domain/
COPY AcervoImobiliario.Infrastructure/AcervoImobiliario.Infrastructure.csproj AcervoImobiliario.Infrastructure/

RUN dotnet restore AcervoImobiliario.Api/AcervoImobiliario.Api.csproj

COPY AcervoImobiliario.Api/ AcervoImobiliario.Api/
COPY AcervoImobiliario.Application/ AcervoImobiliario.Application/
COPY AcervoImobiliario.Domain/ AcervoImobiliario.Domain/
COPY AcervoImobiliario.Infrastructure/ AcervoImobiliario.Infrastructure/

RUN dotnet publish AcervoImobiliario.Api/AcervoImobiliario.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AcervoImobiliario.Api.dll"]
