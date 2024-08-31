using System.Collections.Generic;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using ProcurandoApartamento.Domain;
using ProcurandoApartamento.Domain.Entities;

namespace ProcurandoApartamento.Domain.Services.Interfaces
{
    public interface IApartamentoService
    {
        Task<Apartamento> Save(Apartamento apartamento);

        Task<IPage<Apartamento>> FindAll(IPageable pageable);

        Task<List<Apartamento>> GetAll();

        Task<Apartamento> FindOne(long id);
        Task<List<Apartamento>> FindQuadra(int Quadra);

        Task Delete(long id);

        Task<Apartamento> MelhorApartamento(Busca busca);
    }
}
