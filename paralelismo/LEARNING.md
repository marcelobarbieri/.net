# Programação Paralela

C# Avançado
Classe Parallel

## O que é paralelismo?

- Quando dois ou mais processos são realizados de forma simultânea
- O processamento é gerenciado pelo programador
- Não confundir programação paralela com concorrência
- As tarefas são manipuladas pelos processadores lógicos (*Threads*) da máquina que está rodando a aplicação em si
- Nasceu na versão 4.0 do .NET Framework

![Distribuição de Núcleos](https://github.com/marcelobarbieri/.net/blob/main/paralelismo/assets/nucleos.png)

```ps
dotnet new console
```

## Sequencial 

program.cs
```c#
using System.Diagnostics;

var stopWatch = new Stopwatch();

stopWatch.Start();
Processo1();
Processo2();
Processo3();
stopWatch.Stop();

Console.WriteLine($"O tempo de processamento total foi de {stopWatch.ElapsedMilliseconds} ms");

void Processo1()
{
    Console.WriteLine($"Processo 1 finalizado. Thread {Thread.CurrentThread.ManagedThreadId}");
    Thread.Sleep(1000);
}

void Processo2()
{
    Console.WriteLine($"Processo 2 finalizado. Thread {Thread.CurrentThread.ManagedThreadId}");
    Thread.Sleep(1000);
}

void Processo3()
{
    Console.WriteLine($"Processo 3 finalizado. Thread {Thread.CurrentThread.ManagedThreadId}");
    Thread.Sleep(1000);
}
```

```ps
dotnet run

Processo 1 finalizado. Thread 1.
Processo 2 finalizado. Thread 1.
Processo 3 finalizado. Thread 1.
O tempo de processamento total foi de 3043 ms
```

### Parallel

```programa.cs
using System.Diagnostics;

// o método Parallel.Invoke espera receber parâmetros Action 
// para as respectivas ações a serem executadas
var acao1 = new Action(Processo1);
var acao2 = new Action(Processo2);
var acao3 = new Action(Processo3);

var stopWatch = new Stopwatch();

stopWatch.Start();

// utilização do método Invoke() ao invés de executar os processos
Parallel.Invoke(acao1, acao2, acao3);

stopWatch.Stop();

Console.WriteLine($"O tempo de processamento total foi de {stopWatch.ElapsedMilliseconds} ms");

void Processo1()
{
    Console.WriteLine($"Processo 1 finalizado. Thread {Thread.CurrentThread.ManagedThreadId}.");
    Thread.Sleep(1000);
}

void Processo2()
{
    Console.WriteLine($"Processo 2 finalizado. Thread {Thread.CurrentThread.ManagedThreadId}.");
    Thread.Sleep(1000);
}

void Processo3()
{
    Console.WriteLine($"Processo 3 finalizado. Thread {Thread.CurrentThread.ManagedThreadId}.");
    Thread.Sleep(1000);
}
```

```ps
dotnet run

Processo 2 finalizado. Thread 4.
Processo 3 finalizado. Thread 6.
Processo 1 finalizado. Thread 1.
O tempo de processamento total foi de 1016 ms
```

### Exemplo com uso de API

[ViaCEP](http://viacep.com.br/)

[Exemplo de pesquisa](http://viacep.com.br/ws/01001000/json/)

```json
{
  "cep": "01001-000",
  "logradouro": "Praça da Sé",
  "complemento": "lado ímpar",
  "bairro": "Sé",
  "localidade": "São Paulo",
  "uf": "SP",
  "ibge": "3550308",
  "gia": "1004",
  "ddd": "11",
  "siafi": "7107"
}
```

Criar nova pasta e arquivo: Models/CepModel.cs

CepModel.cs
```c#
using System.Text.Json.Serialization;

namespace Paralelismo.Models
{
    public class CepModel
    {
        [JsonPropertyName("cep")]
        public string Cep { get; set; }
        [JsonPropertyName("logradouro")]
        public string Logradouro { get; set; }
        [JsonPropertyName("complemento")]
        public string Complemento { get; set; }
        [JsonPropertyName("bairro")]
        public string Bairro { get; set; }
        [JsonPropertyName("localidade")]        
        public string Localidade { get; set; }
        [JsonPropertyName("uf")]
        public string Uf { get; set; }
        [JsonPropertyName("ibge")]
        public string Ibge { get; set; }
        [JsonPropertyName("gia")]
        public string Gia { get; set; }
        [JsonPropertyName("ddd")]
        public string Ddd { get; set; }
        [JsonPropertyName("siafi")]
        public string Siafi { get; set; }

        public override string ToString()
        {
            return $"{this.Logradouro} - {this.Bairro}, {this.Localidade}/{this.Uf} - {this.Cep}";
        }
    }
}
```

Criar nova pasta e arquivo: Services/ViaCepService.cs

ViaCepService.cs
```c#
using System.Text.Json;
using Paralelismo.Models;

namespace Paralelismo
{
    public class ViaCepService
    {
        public CepModel GetCep(string cep)
        {
            var client = new HttpClient();
            var response = client.GetAsync($"https://viacep.com.br/ws/{cep}/json/").Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var cepResult = JsonSerializer.Deserialize<CepModel>(content);

            return cepResult;
        }
    }
}
```

> [Gerador de CEP](https://www.4devs.com.br/gerador_de_cep)

#### Programa sem utilizar *Parallel*

Program.cs
```c#
using System.Diagnostics;
using Paralelismo;
using Paralelismo.Models;

string[] ceps = new string[5];
ceps[0] = "07155081";
ceps[1] = "15800100";
ceps[2] = "38407369";
ceps[3] = "77445100";
ceps[4] = "78015818";

var stopWatch = new Stopwatch();

stopWatch.Start();

foreach (var cep in ceps)
{
    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} -  {new ViaCepService().GetCep(cep)}");
}

stopWatch.Stop();

Console.WriteLine($"O Tempo de processamento total é de {stopWatch.ElapsedMilliseconds} ms");
```

```ps
dotnet run

Thread 1 -  Viela Eça - Conjunto Residencial Haroldo Veloso, Guarulhos/SP - 07155-081
Thread 1 -  Rua Sergipe - Centro, Catanduva/SP - 15800-100
Thread 1 -  Rua Araticum - Morumbi, Uberlândia/MG - 38407-369
Thread 1 -  Avenida José do Patrocínio - Cidade Industrial, Gurupi/TO - 77445-100
Thread 1 -  Rua Dinamarca - Terceiro, Cuiabá/MT - 78015-818
O Tempo de processamento total é de 2164 ms
```

#### Programa com a utilização de *Parallel*

Program.cs
```c#
using System.Diagnostics;
using Paralelismo;
using Paralelismo.Models;

string[] ceps = new string[5];
ceps[0] = "07155081";
ceps[1] = "15800100";
ceps[2] = "38407369";
ceps[3] = "77445100";
ceps[4] = "78015818";

var stopWatch = new Stopwatch();

stopWatch.Start();

Parallel.ForEach(ceps, cep =>
{
    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} -  {new ViaCepService().GetCep(cep)}");
});

stopWatch.Stop();

Console.WriteLine($"O Tempo de processamento total é de {stopWatch.ElapsedMilliseconds} ms");
```

```ps
dotnet run

Thread 4 -  Rua Sergipe - Centro, Catanduva/SP - 15800-100
Thread 8 -  Rua Dinamarca - Terceiro, Cuiabá/MT - 78015-818
Thread 6 -  Avenida José do Patrocínio - Cidade Industrial, Gurupi/TO - 77445-100
Thread 7 -  Rua Araticum - Morumbi, Uberlândia/MG - 38407-369
Thread 1 -  Viela Eça - Conjunto Residencial Haroldo Veloso, Guarulhos/SP - 07155-081
O Tempo de processamento total é de 699 ms
```

#### ParallelOptions

Program.cs
```c#
using System.Diagnostics;
using Paralelismo;
using Paralelismo.Models;

string[] ceps = new string[5];
ceps[0] = "07155081";
ceps[1] = "15800100";
ceps[2] = "38407369";
ceps[3] = "77445100";
ceps[4] = "78015818";

var parallelOptions = new ParallelOptions();
parallelOptions.MaxDegreeOfParallelism = 8;

var stopWatch = new Stopwatch();

stopWatch.Start();

Parallel.ForEach(ceps, parallelOptions, cep =>
{
    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} -  {new ViaCepService().GetCep(cep)}");
});

stopWatch.Stop();

Console.WriteLine($"O Tempo de processamento total é de {stopWatch.ElapsedMilliseconds} ms");
```

```ps
dotnet run

Thread 8 -  Rua Araticum - Morumbi, Uberlândia/MG - 38407-369
Thread 1 -  Viela Eça - Conjunto Residencial Haroldo Veloso, Guarulhos/SP - 07155-081
Thread 4 -  Avenida José do Patrocínio - Cidade Industrial, Gurupi/TO - 77445-100
Thread 6 -  Rua Sergipe - Centro, Catanduva/SP - 15800-100
Thread 7 -  Rua Dinamarca - Terceiro, Cuiabá/MT - 78015-818
O Tempo de processamento total é de 681 ms
```

#### List<CepModel>
    
Program.cs    
```c#
using System.Diagnostics;
using Paralelismo;
using Paralelismo.Models;

string[] ceps = new string[5];
ceps[0] = "07155081";
ceps[1] = "15800100";
ceps[2] = "38407369";
ceps[3] = "77445100";
ceps[4] = "78015818";

var parallelOptions = new ParallelOptions();
parallelOptions.MaxDegreeOfParallelism = 8;

var stopWatch = new Stopwatch();

stopWatch.Start();

var listaCep = new List<CepModel>();

Parallel.ForEach(ceps, parallelOptions, cep =>
{
    listaCep.Add(new ViaCepService().GetCep(cep));
});

stopWatch.Stop();

Console.WriteLine($"O Tempo de processamento total é de {stopWatch.ElapsedMilliseconds} ms");    
```    









