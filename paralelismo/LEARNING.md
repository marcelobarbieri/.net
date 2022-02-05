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























