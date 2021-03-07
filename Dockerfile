FROM mcr.microsoft.com/dotnet/sdk:5.0
LABEL project="VK_Bot"
LABEL maintainer="Stranik"
WORKDIR VK_LongPull_Bot
COPY . .
RUN dotnet restore
ENTRYPOINT dotnet run