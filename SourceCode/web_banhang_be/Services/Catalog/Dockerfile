FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy BuildingBlocks & Shared
COPY ["BuildingBlocks/VanFucVN.Core.Common/VanFucVN.Core.Common.csproj", "BuildingBlocks/VanFucVN.Core.Common/"]
COPY ["BuildingBlocks/VanFucVN.Core.Persistence/VanFucVN.Core.Persistence.csproj", "BuildingBlocks/VanFucVN.Core.Persistence/"]
COPY ["BuildingBlocks/VanFucVN.Core.Messaging/VanFucVN.Core.Messaging.csproj", "BuildingBlocks/VanFucVN.Core.Messaging/"]
COPY ["BuildingBlocks/VanFucVN.Core.Web/VanFucVN.Core.Web.csproj", "BuildingBlocks/VanFucVN.Core.Web/"]
COPY ["Shared/AuraMart.Shared/AuraMart.Shared.csproj", "Shared/AuraMart.Shared/"]

# Copy Service Projects
COPY ["Services/Catalog/src/Catalog.Contracts/Catalog.Contracts.csproj", "Services/Catalog/src/Catalog.Contracts/"]
COPY ["Services/Catalog/src/Catalog.Domain/Catalog.Domain.csproj", "Services/Catalog/src/Catalog.Domain/"]
COPY ["Services/Catalog/src/Catalog.Application/Catalog.Application.csproj", "Services/Catalog/src/Catalog.Application/"]
COPY ["Services/Catalog/src/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "Services/Catalog/src/Catalog.Infrastructure/"]
COPY ["Services/Catalog/src/Catalog.Api/Catalog.Api.csproj", "Services/Catalog/src/Catalog.Api/"]

RUN dotnet restore "Services/Catalog/src/Catalog.Api/Catalog.Api.csproj"

COPY ["BuildingBlocks/", "BuildingBlocks/"]
COPY ["Shared/", "Shared/"]
COPY ["Services/Catalog/src/", "Services/Catalog/src/"]

RUN dotnet publish "Services/Catalog/src/Catalog.Api/Catalog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Api.dll"]
