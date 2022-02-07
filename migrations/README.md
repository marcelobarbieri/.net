# Migrations : Ferramentas do CLI

[Macoratti.net](http://www.macoratti.net/21/03/efc_migra1.htm)

Hoje vamos apresentar os conceitos relacionados ao Migrations do EF Core e a utilização dos comandos da ferramenta da linha de comando NET CLI para aplicar as migrações.

O recurso Migrations do EF Core oferece uma maneira de atualizar de forma incremental o esquema de banco de dados para mantê-lo em sincronia com o modelo de dados do aplicativo, preservando os dados existentes no banco de dados.

O funcionamento resumido pode ser expresso da seguinte forma :

Quando uma alteração de modelo de dados é introduzida, o desenvolvedor usa as ferramentas do EF Core para adicionar uma migração correspondente que descreve as atualizações necessárias para manter o esquema de banco de dados em sincronia. O EF Core compara o modelo atual com um instantâneo do modelo antigo para determinar as diferenças e gera arquivos de origem de migração; os arquivos podem ser acompanhados no controle do código-fonte do projeto, como qualquer outro arquivo de origem.

Depois que uma nova migração é gerada, é possível aplicá-la a um banco de dados de várias maneiras. O EF Core registra todas as migrações aplicadas em uma tabela de histórico especial, permitindo que ela saiba quais migrações foram ou não aplicadas.
O comando **Add-Migration <MigrationName>** do console do gerenciador de pacotes gerará um arquivo chamado **“<DateTimeStamp>\_<MigrationName>”** geralmente em uma pasta chamada Migrations dentro do seu projeto onde seu DBContext reside.

Essas migrações são registradas e rastreadas em uma tabela de banco de dados chamada **\_\_EFMigrationsHistory** que contém informações sobre todas as migrações que foram executadas em seu banco de dados.

Cada arquivo de migração possui dois métodos: um método **Up** e um método **Down**.

O método **Up** possui um código que é executado para atualizar o banco de dados e o método **Down** possui um código que é executado para reverter as alterações.

As migrações do Entity Framework Core são ótimas para atualizar o banco de dados quando seus modelos mudam. No entanto, rollbacks ou reversão de esquema são recursos pouco usados, e, nunca devem ser usados em um ambiente de produção. Isso ocorre porque o rollback tem um código que geralmente está eliminando uma coluna ou uma tabela e no ambiente de produção isso pode acarretar perda de dados.

Além disso, as migrações EF Core não suportam a execução de uma migração específica. Uma vez que a migração é executada, ela permanece em seu projeto sem função. Na verdade, é extremamente arriscado escolher uma migração específica e executá-la porque existe a capacidade de gerenciar as repercussões dessa execução.

Você pode excluir um arquivo de código de migração específico, mas isso não exclui o registro da tabela **\_\_EFMigrationHistory**. Portanto, você precisará dele para se referir a como o histórico do esquema do banco de dados evoluiu ou para um cenário muito raro de reversão.

Além disso, não há nenhum mecanismo para executar algum código ou antes da execução da migração e após a execução da migração.

Outro detalhe é que o código de migração gerado não verifica a existência antes de adicionar ou descartar. Então, se você excluir um registro de
**\_\_EFMigrationHistory//, para permitir que uma migração específica seja executada, é mais provável que isso cause uma falha, a menos que você tenha cuidado de limpar.

Por exemplo, se você removeu um registro que cria uma tabela Funcionario. A migração de criação de funcionários agora será executada, mas falhará, porque essa tabela existe no banco de dados.

Portanto, você deve garantir manualmente que a tabela seja eliminada antes de executá-la. Se você estiver lidando com um grande esquema de banco de dados, isso pode ser uma tarefa muito arriscada.

## Instalando a ferramenta do EF Core

As ferramentas da CLI (interface de linha de comando) para Entity Framework Core permite executar tarefas de desenvolvimento em tempo de design.

Por exemplo, elas criam migrações, aplicam migrações e geram código para um modelo com base em um banco de dados existente. Os comandos são uma extensão para o comando dotnet que faz parte do SDK do .NET Core. Essas ferramentas funcionam com projetos do .NET Core.

Para usar as ferramentas do EF Core e aplicar as migrações usando comandos CLI você tem que instalar globalmente a ferramenta no seu ambiente usando o seguinte comando:

### dotnet tool install --global dotnet-ef

Após isso você estar apto a usar os comandos CLI no seu ambiente.

Nota: Para poder usar as ferramentas em um projeto específico, você precisará adicionar o Microsoft.EntityFrameworkCore.Design pacote a ele.

Para verificar a instalação basta digitar o comando : 
  
### dotnet ef

Para atualizar a ferramenta para a versão mais recente use o comando : 
  
### dotnet tool update --global dotnet-ef

Se você tiver as ferramentas instaladas localmente no seu projeto, use : 
  
### dotnet tool update dotnet-ef .
