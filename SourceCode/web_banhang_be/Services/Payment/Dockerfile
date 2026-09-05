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
COPY ["Services/Payment/src/Payment.Contracts/Payment.Contracts.csproj", "Services/Payment/src/Payment.Contracts/"]
COPY ["Services/Payment/src/Payment.Domain/Payment.Domain.csproj", "Services/Payment/src/Payment.Domain/"]
COPY ["Services/Payment/src/Payment.Application/Payment.Application.csproj", "Services/Payment/src/Payment.Application/"]
COPY ["Services/Payment/src/Payment.Infrastructure/Payment.Infrastructure.csproj", "Services/Payment/src/Payment.Infrastructure/"]
COPY ["Services/Payment/src/Payment.Api/Payment.Api.csproj", "Services/Payment/src/Payment.Api/"]

RUN dotnet restore "Services/Payment/src/Payment.Api/Payment.Api.csproj"

COPY ["BuildingBlocks/", "BuildingBlocks/"]
COPY ["Shared/", "Shared/"]
COPY ["Services/Payment/src/", "Services/Payment/src/"]

RUN dotnet publish "Services/Payment/src/Payment.Api/Payment.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Payment.Api.dll"]
