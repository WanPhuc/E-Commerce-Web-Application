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
COPY ["Services/Ordering/src/Ordering.Contracts/Ordering.Contracts.csproj", "Services/Ordering/src/Ordering.Contracts/"]
COPY ["Services/Ordering/src/Ordering.Domain/Ordering.Domain.csproj", "Services/Ordering/src/Ordering.Domain/"]
COPY ["Services/Ordering/src/Ordering.Application/Ordering.Application.csproj", "Services/Ordering/src/Ordering.Application/"]
COPY ["Services/Ordering/src/Ordering.Infrastructure/Ordering.Infrastructure.csproj", "Services/Ordering/src/Ordering.Infrastructure/"]
COPY ["Services/Ordering/src/Ordering.Api/Ordering.Api.csproj", "Services/Ordering/src/Ordering.Api/"]

RUN dotnet restore "Services/Ordering/src/Ordering.Api/Ordering.Api.csproj"

COPY ["BuildingBlocks/", "BuildingBlocks/"]
COPY ["Shared/", "Shared/"]
COPY ["Services/Ordering/src/", "Services/Ordering/src/"]

RUN dotnet publish "Services/Ordering/src/Ordering.Api/Ordering.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ordering.Api.dll"]
