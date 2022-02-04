# ASP.NET Configuration Manager

[Balta.io](https://balta.io/blog/aspnet-configuration-manager?utm_source=Discord&utm_campaign=social-to-blog&utm_content=aspnet-configuration-manager&utm_medium=social)

Neste artigo vamos conferir as mudanças na leitura e carregamento das configurações que tivemos no ASP.NET 6.

## Configuration Manager

O .NET possui alguns recursos de configurações bem interessantes, 
que nos permitem carregar informações definidas em um arquivo JSON em nossas aplicações.

Ao criar uma aplicação Web por exemplo, 
nós temos a adição de dois arquivos de configuração em na raiz da aplicação, 
o **appsettings.json** e o **appsettings.Development.json**, que normalmente vem com o seguinte conteúdo.

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

*IMPORTANTE: É possível (porém não recomendado) alterar os nomes destes arquivos. Para este exemplo, vamos manter os padrões.*

### Development VS Production

Como pudemos notar, temos dois arquivos diferentes, um com a palavra **Development** e outro sem. 
O ASP.NET trabalha com variáveis de ambiente, e dentre elas temos uma muito importante, 
chamada **ASPNETCORE_ENVIRONMENT** que fica no arquivo **launchSettings.json** na pasta **Properties**.

É importante frisar que este arquivo (**launchSettings.json**) é o responsável pelas configurações de execução da aplicação. 
Tanto o Visual Studio, Visual Studio Code ou Rider, utilizam ele para depurar nossos Apps.

Vamos começar nossa brincadeira editando os arquivos **appsettings.json** e **appsettings.Development.json** 
adicionando os respectivos valores para a chave **Env** que vamos criar.

appsettings.Development.json
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

### Lendo Configurações

Podemos ler as configurações em dois momentos distintos na aplicação, 
durante a construção do App (build) e após o App ser construído. 
Em ambos o formato é sempre o mesmo, conforme mostrado abaixo.

```c#
// App está na fase de build
builder.Configuration.GetValue<string>("Env");

// App já está pronto para ser executado
app.Configuration.GetValue<string>("Env");
```

*IMPORTANTE: Não confunda o build do App com a compilação da aplicação. 
Isto já ocorreu, aqui é a execução do código.*

Note que usamos o **Configuration.GetValue** que permite definirmos um tipo, ou usar algo dinâmico. 
Neste caso, vamos tentar fazer um parse do valor para string, por isto utilizamos **Configuration.GetValue<string>("Env")**.
Para exemplificar, vamos exibir o valor na tela do Browser usando o exemplo padrão do projeto Web que o ASP.NET traz.

```c#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => app.Configuration.GetValue<string>("Env"));

app.Run();
```

### Executando a aplicação

Para executar a aplicação em modo de desenvolvimento, você pode apertar o play na sua IDE favorita ou rodar o comando **dotnet run**. 
Tanto faz, o Run ou Debug vão passar pelo **launchSettings.json** 
que vai definir a variável de ambiente **ASPNETCORE_ENVIRONMENT** como **Development**.

Para rodar a aplicação no modo Release, podemos utilizar a sequência abaixo, para compilar em modo Release e executar o exe gerado.

```ps
dotnet build -c Release
dotnet publish -c Release
cd /bin/Release/net6.0/publish
./NomeDoSeuProjeto.exe
```

Neste momento, rodando a aplicação em modo Development você deve ter visto 
um valor diferente do exibido ao executar a aplicação em modo Release, 
cada um lido do seu respectivo arquivo de configuração.

### Lendo blocos de configurações

Na sessão anterior, vimos como ler o valor de uma chave no arquivo de configuração, 
mas existe uma forma mais simples de ler diversos valores de uma única vez, utilizando o **GetSection**.

O **GetSection** permite ler um bloco inteiro do arquivo de configuração para dentro de um objeto. 
Ele faz isto comparando as chaves com os nomes das propriedades existentes na sessão do arquivo.

Então, vamos tomar como base a classe **Configuration** que usamos no Curso de Fundamentos do ASP.NET 6. 
Nesta classe, temos um trecho de código pertinente as configurações do SMTP, para envio de E-mails.

```c#
public class SmtpConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; } = 25;
    public string UserName { get; set; }
    public string Password { get; set; }
}
```

Como o nome sugere, o **GetSection** obtém uma sessão, um conjunto de informações, do arquivo de configuração, 
então precisamos estruturar nosso **appsettings.Development.json** para passar estas informações corretamente.

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
  },
  "AllowedHosts": "*"
}
```

Note que utilizamos os mesmos nomes nas chaves e nas propriedades da classe **SmtpConfiguration**. 
Desta forma o **Configuration** já consegue inferir os valores e popular nosso objeto. 
Abaixo uma demonstração de como podemos fazer isto.

```c#
var smtp = new Configuration.SmtpConfiguration();
app.Configuration.GetSection("SmtpConfiguration").Bind(smtp);
```

### Connection Strings

O arquivo de configuração possui uma sessão especial dedicada as **Connection Strings**, 
podendo ser definidas várias delas com uma leitura facilitada posteriormente. 
Abaixo um exemplo de como podemos armazenar uma **Connection String** no arquivo de configuração.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=balta;User ID=sa;Password=1q2w3e4r@#$"
  },
  "Env": "development",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

Com a **Connection String** devidamente armazenada, podemos utilizar o método **Configuration.GetConnectionString**
para procurá-la diretamente na sessão **ConnectionStrings** do arquivo de configuração, como mostrado abaixo.

```c#
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

IMPORTANTE: Há maneiras melhores de armazenar dados sensíveis como cadeias de conexão sem ser no arquivo de configuração, 
como o **dotnet user secrets**. Tenha em mente que este arquivo será versionado junto ao projeto e 
ficará disponível no versionador de código da sua empresa.
