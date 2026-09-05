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
COPY ["Services/Cart/src/Cart.Contracts/Cart.Contracts.csproj", "Services/Cart/src/Cart.Contracts/"]
COPY ["Services/Cart/src/Cart.Domain/Cart.Domain.csproj", "Services/Cart/src/Cart.Domain/"]
COPY ["Services/Cart/src/Cart.Application/Cart.Application.csproj", "Services/Cart/src/Cart.Application/"]
COPY ["Services/Cart/src/Cart.Infrastructure/Cart.Infrastructure.csproj", "Services/Cart/src/Cart.Infrastructure/"]
COPY ["Services/Cart/src/Cart.Api/Cart.Api.csproj", "Services/Cart/src/Cart.Api/"]

RUN dotnet restore "Services/Cart/src/Cart.Api/Cart.Api.csproj"

COPY ["BuildingBlocks/", "BuildingBlocks/"]
COPY ["Shared/", "Shared/"]
COPY ["Services/Cart/src/", "Services/Cart/src/"]

RUN dotnet publish "Services/Cart/src/Cart.Api/Cart.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cart.Api.dll"]
