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
COPY ["Services/Notification/src/Notification.Contracts/Notification.Contracts.csproj", "Services/Notification/src/Notification.Contracts/"]
COPY ["Services/Notification/src/Notification.Domain/Notification.Domain.csproj", "Services/Notification/src/Notification.Domain/"]
COPY ["Services/Notification/src/Notification.Application/Notification.Application.csproj", "Services/Notification/src/Notification.Application/"]
COPY ["Services/Notification/src/Notification.Infrastructure/Notification.Infrastructure.csproj", "Services/Notification/src/Notification.Infrastructure/"]
COPY ["Services/Notification/src/Notification.Api/Notification.Api.csproj", "Services/Notification/src/Notification.Api/"]

RUN dotnet restore "Services/Notification/src/Notification.Api/Notification.Api.csproj"

COPY ["BuildingBlocks/", "BuildingBlocks/"]
COPY ["Shared/", "Shared/"]
COPY ["Services/Notification/src/", "Services/Notification/src/"]

RUN dotnet publish "Services/Notification/src/Notification.Api/Notification.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Notification.Api.dll"]
