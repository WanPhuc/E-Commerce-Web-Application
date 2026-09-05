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
COPY ["Services/Seller/src/Seller.Contracts/Seller.Contracts.csproj", "Services/Seller/src/Seller.Contracts/"]
COPY ["Services/Seller/src/Seller.Domain/Seller.Domain.csproj", "Services/Seller/src/Seller.Domain/"]
COPY ["Services/Seller/src/Seller.Application/Seller.Application.csproj", "Services/Seller/src/Seller.Application/"]
COPY ["Services/Seller/src/Seller.Infrastructure/Seller.Infrastructure.csproj", "Services/Seller/src/Seller.Infrastructure/"]
COPY ["Services/Seller/src/Seller.Api/Seller.Api.csproj", "Services/Seller/src/Seller.Api/"]

RUN dotnet restore "Services/Seller/src/Seller.Api/Seller.Api.csproj"

COPY ["BuildingBlocks/", "BuildingBlocks/"]
COPY ["Shared/", "Shared/"]
COPY ["Services/Seller/src/", "Services/Seller/src/"]

RUN dotnet publish "Services/Seller/src/Seller.Api/Seller.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Seller.Api.dll"]
