FROM mcr.microsoft.com/dotnet/nightly/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["./MSA.SagaOrchestrationStateMachine/MSA.SagaOrchestrationStateMachine.csproj","src/MSA.SagaOrchestrationStateMachine/"]
COPY ["./MSA.Common.Contracts/MSA.Common.Contracts.csproj","src/MSA.Common.Contracts/"]
COPY ["./MSA.Common.PostgresMassTransit/MSA.Common.PostgresMassTransit.csproj","src/MSA.Common.PostgresMassTransit/"]

RUN dotnet restore "src/MSA.SagaOrchestrationStateMachine/MSA.SagaOrchestrationStateMachine.csproj"
COPY . .
WORKDIR "/src/MSA.SagaOrchestrationStateMachine"
RUN dotnet build "MSA.SagaOrchestrationStateMachine.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MSA.SagaOrchestrationStateMachine.csproj" -c Release -o /app/publish --self-contained false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "MSA.SagaOrchestrationStateMachine.dll" ]