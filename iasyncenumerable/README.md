# ASP.NET Async Streaming

# Referências

Stream de dados no ASP.NET 6

## IAsyncEnumerable

Desde o ASP.NET Core 3, podemos utilizar **IAsyncEnumerable<T>** como retorno das **Actions** em nossos **Controllers**, porém sua efetividade não era tão completa.

Ele basicamente aguardava a execução do método da mesma forma como aguardaria por um **IList**, pois não havia o conceito de **AsyncStreaming** ainda presente no ASP.NET.

Na versão 6 do ASP.NET, este conceito foi introduzido e agora podemos "stremar" (enviar por partes) dados para as interfaces. O melhor de tudo é que esta mudança ocorreu por baixo dos panos, nenhuma mudança de código é necessária.

### Yield

Você provavelmente já viu a palavra **yeld** em algum código C#, e sua tradução é render, produzir. Desta forma podemos **retornar informações de um método sem necessariamente sair dele**.

```c#
yield return 1000;
```

## Utilizando IAsyncEnumerable

Sabendo que com o **yield** podemos retornar uma informação sem necessariamente finalizar a execução do método, podemos usar o **IAsyncEnumerable** para criar uma lista assíncrona.

Isto significa que não vamos aguardar toda a lista ser carregada, vamos receber os valores dela assim que os mesmos forem adicionados (retornados com **yield**).

```c#
private static async IAsyncEnumerable<int> GetData()
{
    for (var i = 1; i <= 1000; i++)
    {
        await Task.Delay(1000);
        yield return i;
    }
}
```

Para este exemplo, utilizamos um **IAsyncEnumerable<int>**, seguido por um **for** que irá iterar por 1000 repetições.

Como estamos utilizando **IAsyncEnumerable**, precisamos do **yield return** e adicionamos um **Task.Delay(1000)** para demorar um segundo entre um retorno e outro.

Para finalizar, não podemos esquecer de marcar o método como **async** já que estamos trabalhando com **IAsyncEnumerable** aqui.

## Streaming de dados

Tendo um método que produz dados de forma assíncrona com **IAsyncEnumerable** já podemos fazer a mágica acontecer. Na verdade este mesmo código se rodado no ASP.NET 5 e ASP.NET 6 produz efeitos diferentes.

No ASP.NET 5, este mesmo código aguardaria todos os dados serem retornados da lista (1000 segundos) para depois enviar tudo para tela, enquando no ASP.NET 6, isto é feito item a item.

```c#
[HttpGet("/")]
public IAsyncEnumerable<int> Get()
{
    IAsyncEnumerable<int> value = GetData();
    return value;
}
```

Note que temos apenas um retorno do tipo **IAsyncEnumerable<int>**, em seguida recebemos os dados do método **GetData()** que criamos anteriormente e fazemos o envio para tela.

Não há necessidade de configurar nenhuma informação adicional aqui, apenas retornar um **IAsyncEnumerable** já basta.

## Exemplo

Rotinas inseridas no controlador **Data**:

```c#
using Exemplo;
using Microsoft.AspNetCore.Mvc;

namespace Exemplo.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class DataController : Controller
    {
        [HttpGet("/")]
        public IAsyncEnumerable<int> Get()
        {
            IAsyncEnumerable<int> value = GetData();
            return value;
        }

        private static async IAsyncEnumerable<int> GetData()
        {
            for (var i = 1; i <= 1000; i++)
            {
                await Task.Delay(1000);
                yield return i;
            }
        }
    }
}
```

## Conclusão

Podemos utilizar o **IAsyncEnumerable** para enviar dados para tela ou outras interfaces no formato de Stream de forma simples e fácil no ASP.NET 6.

## Referências

> [Balta.io](https://balta.io/blog/aspnet-async-streaming)
