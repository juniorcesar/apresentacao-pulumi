o# Comandos 

## Cria nova stack
pulumi stack init dev

## Cria config 
pulumi config set environment dev

## Cria config Secret
pulumi config set --secret vm-password sdjaiUJ#dIKL33

## Obtém valor do Output 
pulumi stack output sshPrivateKeyPem --show-secrets

## Adiciona pacotes no Projeto
dotnet add package Pulumi.TLS
dotnet add package Pulumi.Azure