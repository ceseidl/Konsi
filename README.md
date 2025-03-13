# Konsi
# Solução do Desafio Konsi

### Descrição
Esta solução atende ao desafio técnico da Konsi, implementando uma API para buscar benefícios de CPFs em uma API externa, processando dados com RabbitMQ, Redis e Elasticsearch.

### Tecnologias Utilizadas
- .NET Core
- RabbitMQ
- Redis
- Elasticsearch
- Docker

### Rodando o Projeto
1. Certifique-se de que o Docker está instalado e rodando.
2. Execute o seguinte comando para subir os serviços:
   ```bash
   docker-compose up --build
Organização do Projeto
Konsi.API: Controladores e endpoints principais.

Konsi.Worker: Processamento assíncrono de CPFs.

Konsi.Shared: Serviços compartilhados (Redis, Elasticsearch).

Konsi.Web: Interface web para consulta de CPFs.

Melhorias Futuras
Adicionar logs estruturados com Serilog.

Melhorar a interface com design responsivo.
