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
COPY ["Services/Identity/src/Identity.Contracts/Identity.Contracts.csproj", "Services/Identity/src/Identity.Contracts/"]
COPY ["Services/Identity/src/Identity.Domain/Identity.Domain.csproj", "Services/Identity/src/Identity.Domain/"]
COPY ["Services/Identity/src/Identity.Application/Identity.Application.csproj", "Services/Identity/src/Identity.Application/"]
COPY ["Services/Identity/src/Identity.Infrastructure/Identity.Infrastructure.csproj", "Services/Identity/src/Identity.Infrastructure/"]
COPY ["Services/Identity/src/Identity.Api/Identity.Api.csproj", "Services/Identity/src/Identity.Api/"]

RUN dotnet restore "Services/Identity/src/Identity.Api/Identity.Api.csproj"

COPY ["BuildingBlocks/", "BuildingBlocks/"]
COPY ["Shared/", "Shared/"]
COPY ["Services/Identity/src/", "Services/Identity/src/"]

RUN dotnet publish "Services/Identity/src/Identity.Api/Identity.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]
