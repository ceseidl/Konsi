using Konsi.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nest;

namespace Konsi.Web.Pages
{
    public class SearchModel : PageModel
    {
        private readonly IElasticClient _elasticClient;

        public SearchModel(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        [BindProperty(SupportsGet = true)]
        public string Cpf { get; set; }

        public List<Beneficio> Beneficios { get; set; }

        public async Task OnGetAsync()
        {
            if (string.IsNullOrEmpty(Cpf))
            {
                Beneficios = new List<Beneficio>();
                return;
            }

            var response = await _elasticClient.SearchAsync<Beneficio>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Cpf)
                        .Query(Cpf)
                    )
                )
            );

            Beneficios = response.Documents.ToList();
        }
    }

    

}