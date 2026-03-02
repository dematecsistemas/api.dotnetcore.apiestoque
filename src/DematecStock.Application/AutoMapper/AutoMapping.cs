using AutoMapper;
using DematecStock.Communication.Requests.WarehouseLocations;
using DematecStock.Communication.Responses;
using DematecStock.Domain.Entities;

namespace DematecStock.Application.AutoMapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            RequestToEntity();
            EntityToResponse();
        }

        private void RequestToEntity()
        {
            // Exemplo: CreateMap<RequestRegisterExpenseJson, Expense>();
            CreateMap<RequestWriteWarehouseLocationJson, WarehouseLocations>();
        }

        private void EntityToResponse()
        {
            CreateMap<WarehouseLocations, ResponseLocationsJson>();
        }
    }
}
