# ---------------- FRONTEND (Node + Tailwind) ----------------
FROM node:20-alpine AS frontend

WORKDIR /src/frontend

COPY HiTechPay/frontend/package*.json ./
RUN npm install

COPY HiTechPay/frontend ./
COPY HiTechPay/Views ../Views

RUN npm run build


# ---------------- BACKEND (.NET BUILD) ----------------
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

WORKDIR /src

COPY ["HiTechPay/HiTechPay.csproj", "HiTechPay/"]
COPY ["HiTechPay.Sdk/HiTechPay.Sdk.csproj", "HiTechPay.Sdk/"]

RUN dotnet restore "./HiTechPay/HiTechPay.csproj"

COPY . .

# inject frontend build into wwwroot
COPY --from=frontend /src/frontend/../wwwroot ./HiTechPay/wwwroot

WORKDIR "/src/HiTechPay"
RUN dotnet publish -c Release -o /app/publish


# ---------------- RUNTIME ----------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "HiTechPay.dll"]