using App.Models.Producao; // alterar mais tarde
using App.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using VendaERP.Core;
using VendaERP.Core.Models; // alterar mais tarde
using X.PagedList;

namespace App.Controllers
{
    public class ProducaoController : Controller
    {
        private readonly DBAccess _db;
        private ProducaoModel _model; // alterar mais tarde

        public ProducaoController(DBAccess db)
        {
            this._db = db;
            this._model = new ProducaoModel(); // alterar mais tarde
        }

        public IActionResult Index(string? item, string? status, DateTime? dataInicio, DateTime? dataFim, int pageNumber = 1, int pageSize = 15)
        {
            Autocompletar();
            if (this._model.filtros == null)
                this._model.filtros = new FiltrosProducao(); // alterar mais tarde

            #region Filtros
            var filters = Builders<DtoOrdemProducao>.Filter.Where(x => true); // alterar mais tarde

            _model.filtros.item = item ?? "";
            if (!string.IsNullOrEmpty(_model.filtros.item))
                filters &= Builders<DtoOrdemProducao>.Filter.Where(x => x.Item == _model.filtros.item);

            _model.filtros.status = status ?? "Todos";
            if (_model.filtros.status != "Todos")
                filters &= Builders<DtoOrdemProducao>.Filter.Where(x => x.Status == _model.filtros.status);

            _model.filtros.dataInicio = dataInicio;
            if (dataInicio.HasValue)
                filters &= Builders<DtoOrdemProducao>.Filter.Where(x => x.Data >= dataInicio.Value);

            _model.filtros.dataFim = dataFim;
            if (dataFim.HasValue)
                filters &= Builders<DtoOrdemProducao>.Filter.Where(x => x.Data <= dataFim.Value);
            #endregion

            var producaoList = _db._repositoryProducao.Collection.Aggregate().Match(filters).ToList().OrderByDescending(x => x.Data).ToList();

            #region Paginação
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageSize < 15)
                pageSize = 15;
            else if (pageSize > 100)
                pageSize = 100;
            #endregion

            var pager = new Pager(producaoList.Count(), pageNumber, pageSize);
            _model.listaProducao = producaoList.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).ToList();
            this.ViewBag.pager = pager;
            return View(_model);
        }

        public IActionResult NovaProducao(string? item, int? quantidade, DateTime? data, string? status, string? observacoes)
        {
            Autocompletar();
            if (item != null && quantidade.HasValue)
            {
                var novoProducao = new DtoOrdemProducao // alterar mais tarde com os campos que estão no mongo
                {
                    Item = item,
                    Quantidade = quantidade.Value,
                    Data = data ?? DateTime.Now,
                    Status = status ?? "Pendente",
                    Observacoes = observacoes,
                    DataModificacao = DateTime.Now
                };

                _db._repositoryProducao.Collection.InsertOne(novoProducao); // alterar mais tarde
                _model.novaProducao = "true";
            }
            else
            {
                _model.novaProducao = "false";
            }

            return View(_model);
        }

        private void Autocompletar()
        {
            if (this._model.autocompletar == null)
                this._model.autocompletar = new App.Models.Producao.Autocompletar(); // alterar mais tarde

            if (this._model.autocompletar.itens == null)
                this._model.autocompletar.itens = _db._repositoryItens.Collection.Find<DtoItem>(_ => true) // alterar mais tarde
                    .ToList().OrderBy(x => x.Nome).Select(x => x.Nome).Distinct().ToList();

            if (this._model.autocompletar.status == null)
                this._model.autocompletar.status = new List<string> { "Pendente", "Em Produção", "Concluído", "Cancelado" }; //lembrar de separar as filas por tanques
        }
    }
}
