# Projeto Apresentação Pulumi 

## Requisitos
##  - Pulumi SDK
##  - Azure Cli
##  - SDK .NET 5.0


## Realiza Login
$ az login
$ az account show
$ pulumi login
$ pulumi stack ls

## Cria um novo projeto e stack utilizando template
$  pulumi new azure-csharp -s juniorcesar/apresentacao-pulumi/dev

## Adiciona pacotes no Projeto
$ dotnet add package Pulumi.TLS
$ dotnet add package Pulumi.Azure

## Cria nova stack
$ pulumi stack init hml

## Cria config 
$ pulumi config set environment hml

## Cria config Secret
$ pulumi config set --secret vm-password sdjaiUJ#dIKL33

## Exibe valor do secret
$ pulumi config --show-secrets

## Exibe valor do Output 
$ pulumi stack output sshPrivateKeyPem --show-secrets

## Altera Stack
$ pulumi stack select prd

