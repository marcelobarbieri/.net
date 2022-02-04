# ASP.NET Core Empty

```ps
dotnet new --list
dotnet new web
```

|Nome do modelo|Nome curto|Idioma|Tags|
|:---|:---|:---|:---|
|ASP.NET Core Empty|web|[C#],F#|Web/Empty|

appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

appsettingsDevelopment.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}

```

# Global.json

```ps
dotnet --list-sdks
dotnet new globaljson --sdk-version 6.0.101
```

global.json
```json
{
  "sdk": {
    "version": "6.0.101"
  }
}
```

# ASPNETCORE_ENVIRONMENT

```
properties/
    launchSettings.json
```

launchSettings.json
```json
{
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:30257",
      "sslPort": 44310
    }
  },
  "profiles": {
    "configuration_manager": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:7020;http://localhost:5285",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### Criar chave **Env** e adicionar seu respectivo valor

appsettings.json
```json
{
  "Env": "production",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

appsettingsDevelopment.json
```json
{
  "Env": "development",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}

```

# GetValue

Podemos ler as configurações em dois momentos distintos na aplicação, durante a construção do App (build) e após o App ser construído. Em ambos o formato é sempre o mesmo, conforme mostrado abaixo.

```c#
// App está na fase de build
builder.Configuration.GetValue<string>("Env");

// App já está pronto para ser executado
app.Configuration.GetValue<string>("Env");
```

*IMPORTANTE: Não confunda o build do App com a compilação da aplicação. Isto já ocorreu, aqui é a execução do código.*

Note que usamos o **Configuration.GetValue** que permite definirmos um tipo, ou usar algo dinâmico. Neste caso, vamos tentar fazer um parse do valor para string, por isto utilizamos **Configuration.GetValue<string>("Env")**.

Para exemplificar, vamos exibir o valor na tela do Browser usando o exemplo padrão do projeto Web que o ASP.NET traz.

Program.cs
```c#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.MapGet("/",() => app.Configuration.GetValue<string>("Env"));

app.Run();
```

### Development
  
```ps
dotnet run  
```

```
Compilando...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7020
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5285
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\DEV\.NET\ASP\configuration-manager\  
```

### Production
  
```ps
dotnet build -c Release  
```

```
Microsoft(R) Build Engine versão 17.0.0+c9eb9dd64 para .NET 
Copyright (C) Microsoft Corporation. Todos os direitos reservados.

  Determinando os projetos a serem restaurados...
  Todos os projetos estão atualizados para restauração.
  configuration-manager -> C:\DEV\.NET\ASP\configuration-manager\bin\Release\net6.0\configuration-manager.dll

Compilação com êxito.
    0 Aviso(s)
    0 Erro(s)

Tempo Decorrido 00:00:02.37  
```
  
```ps
dotnet publish -c Release  
```
  
```
Microsoft(R) Build Engine versão 17.0.0+c9eb9dd64 para .NET
Copyright (C) Microsoft Corporation. Todos os direitos reservados.

  Determinando os projetos a serem restaurados...
  Todos os projetos estão atualizados para restauração.
  configuration-manager -> C:\DEV\.NET\ASP\configuration-manager\bin\Release\net6.0\configuration-manager.dll
  configuration-manager -> C:\DEV\.NET\ASP\configuration-manager\bin\Release\net6.0\publish\  
```

```ps
cd .\bin\Release\net6.0\publish\
```
  
```ps 
.\configuration-manager.exe
```
  
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\DEV\.NET\ASP\configuration-manager\bin\Release\net6.0\publish\  
```
  
# GetSection
  
Criar nova pasta e arquivo: Configurations/SmtpConfiguration.cs
  
SmtpConfiguration.cs    
```c#
public class SmtpConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; } = 25;
    public string UserName { get; set; }
    public string Password { get; set; }
}
```

appsettings.Development.json  
```json
{
  "SmtpConfiguration": {
    "Host": "smtp.sendgrid.net",
    "Port": "587",
    "UserName": "apikey",
    "Password": "suasenha"
  },  
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
  
```  
  
program.cs
```c#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.MapGet("/", () => app.Configuration.GetValue<string>("Env"));

var smtp = new Configuration.SmtpConfiguration();
app.Configuration.GetSection("SmtpConfiguration").Bind(smtp);

Console.WriteLine(smtp);

app.Run();  
```
  
```ps
dotnet run  
```  
  
```ps
Compilando...
Host: smtp.sendgrid.net Port: 587 Username: apikey Password: suasenha
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7020
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5285
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\DEV\.NET\ASP\configuration-manager\  
```  
  
  
