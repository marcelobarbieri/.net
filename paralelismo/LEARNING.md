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
    Console.WriteLine("Processo 1 finalizado");
}

void Processo2()
{
    Console.WriteLine("Processo 2 finalizado");
}

void Processo3()
{
    Console.WriteLine("Processo 3 finalizado");
}
```

```ps
dotnet run
```
