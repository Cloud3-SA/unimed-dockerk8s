FROM alpine:latest

RUN apk add --no-cache dotnet8-sdk

COPY . /opt/app

WORKDIR /opt/app

RUN dotnet build

CMD dotnet watch
