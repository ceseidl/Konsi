using Konsi.API.Models;
using Elastic.Clients.Elasticsearch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;


namespace Konsi.Shared
{
    public class ElasticsearchService
    {
        private readonly IElasticClient _elasticClient;

        public ElasticsearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task IndexBenefitsAsync(string cpf, List<Beneficio> beneficios)
        {
            foreach (var beneficio in beneficios)
            {
                var document = new { Cpf = cpf, Beneficio = beneficio };
                await _elasticClient.IndexDocumentAsync(document);
            }
        }
    }

}
