FROM microsoft/dotnet:2.1.3-runtime
ARG publish_dir

WORKDIR /app
COPY $publish_dir .

ENTRYPOINT ["dotnet", "Dmarc.AggregateReport.Api.dll"]