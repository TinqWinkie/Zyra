using VendaERP.Core.Models; //

namespace App.Models.Producao
{
    public class ProducaoModel
    {
        public ProducaoModel()
        {
            autocompletar = new Autocompletar();
            filtros = new FiltrosProducao();
            listaProducao = new List<DtoOrdemProducao>();
        }

        public Autocompletar autocompletar { get; set; }
        public FiltrosProducao filtros { get; set; }
        public List<DtoOrdemProducao> listaProducao { get; set; }
        public string novaProducao { get; set; }
    }

    public class Autocompletar
    {
        /// <summary>
        /// Lista de itens disponíveis para produção
        /// </summary>
        public List<string> itens { get; set; }

        /// <summary>
        /// Lista de status possíveis
        /// </summary>
        public List<string> status { get; set; }
    }

    public class FiltrosProducao
    {
        /// <summary>
        /// Filtro pelo nome do item
        /// </summary>
        public string item { get; set; }

        /// <summary>
        /// Filtro pelo status da produção
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// Data inicial para filtrar
        /// </summary>
        public DateTime? dataInicio { get; set; }

        /// <summary>
        /// Data final para filtrar
        /// </summary>
        public DateTime? dataFim { get; set; }
    }
}
