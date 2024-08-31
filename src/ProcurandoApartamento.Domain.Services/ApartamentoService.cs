using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using ProcurandoApartamento.Domain.Services.Interfaces;
using ProcurandoApartamento.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using ProcurandoApartamento.Domain.Entities;
using LanguageExt;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ProcurandoApartamento.Domain.Services
{
    public class ApartamentoService : IApartamentoService
    {
        protected readonly IApartamentoRepository _apartamentoRepository;
        public Apartamento melhorApartamento;

        public ApartamentoService(IApartamentoRepository apartamentoRepository)
        {
            _apartamentoRepository = apartamentoRepository;
        }

        public virtual async Task<Apartamento> Save(Apartamento apartamento)
        {
            await _apartamentoRepository.CreateOrUpdateAsync(apartamento);
            await _apartamentoRepository.SaveChangesAsync();
            return apartamento;
        }

        public virtual async Task<IPage<Apartamento>> FindAll(IPageable pageable)
        {
            var page = await _apartamentoRepository.QueryHelper().GetPageAsync(pageable);
            return page;
        }

        public virtual async Task<Apartamento> FindOne(long id)
        {
            var result = await _apartamentoRepository.QueryHelper()
                .GetOneAsync(apartamento => apartamento.Id == id);
            return result;
        }

        public virtual async Task<List<Apartamento>> FindQuadra(int Quadra)
        {
            var allItens = await _apartamentoRepository.FindQuadra(Quadra);
            return allItens;
        }

        public virtual async Task Delete(long id)
        {
            await _apartamentoRepository.DeleteByIdAsync(id);
            await _apartamentoRepository.SaveChangesAsync();
        }

        public async Task<Apartamento> MelhorApartamento(Busca busca)
        {
            var todosApartementos = await GetAll();
            

            var ListaMelhoresApartamentos = BuscarMelhorApartamento(busca, todosApartementos).OrderByDescending(i => i.Quadra);
            foreach (var item in ListaMelhoresApartamentos)
            {
                var Quadra = ListaMelhoresApartamentos.Where(r => r.Quadra == item.Quadra).ToList(); 
                if(Quadra.Count() == busca.Estabelecimento.Count())
                {
                    melhorApartamento = await FindOne(item.Id);
                    return  melhorApartamento;
                    break;
                }
                else
                {
                    melhorApartamento = await FindOne(item.Id);
                }
            }

            return melhorApartamento;
        }



        private List<Apartamento> BuscarMelhorApartamento(Busca busca, List<Apartamento> todosApartementos)
        {
            var melhoresApartamentos = new List<Apartamento>();
            BuscaRecursiva(busca, new List<string>(), todosApartementos, ref melhoresApartamentos);
            return melhoresApartamentos;
        }

        private void BuscaRecursiva(Busca busca, List<string> ApartamentoAtual, List<Apartamento> todosApartementos, ref List<Apartamento> melhoresApartamentos)
        {
            foreach (var item in busca.Estabelecimento)
            {
                foreach (var x in todosApartementos)
                {
                    if (x.Estabelecimento.ToUpper() == item.ToUpper() && x.EstabelecimentoExiste)
                    {
                        var ap = new Apartamento
                        {
                            Id = x.Id,
                            Quadra = x.Quadra,
                            ApartamentoDisponivel = x.ApartamentoDisponivel,
                            EstabelecimentoExiste = x.EstabelecimentoExiste,
                            Estabelecimento = x.Estabelecimento
                        };
                        melhoresApartamentos.Add(ap);
                    }
                    melhoresApartamentos.OrderBy(i => i.Quadra);
                }
            }

            melhoresApartamentos.OrderBy(i => i.Quadra);
        }

        public async Task<List<Apartamento>> GetAll()
        {
            try
            {
                var allItens = await _apartamentoRepository.GetAllAsync();
                return allItens;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
