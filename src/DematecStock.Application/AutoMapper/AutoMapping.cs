using AutoMapper;
using DematecStock.Communication.Requests.InventoryLocation;
using DematecStock.Communication.Requests.InventoryMovement;
using DematecStock.Communication.Requests.WarehouseLocations;
using DematecStock.Communication.Responses;
using DematecStock.Domain.DTOs;
using DematecStock.Domain.Entities;

namespace DematecStock.Application.AutoMapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            RequestToEntity();
            EntityToResponse();
            ResponseToDTO();
            DTOToResponse();
        }

        private void RequestToEntity()
        {
            // Exemplo: CreateMap<RequestRegisterExpenseJson, Expense>();
            CreateMap<RequestWriteWarehouseLocationJson, WarehouseLocations>();
            CreateMap<RequestUpdateWarehouseLocationJson, WarehouseLocations>();
            CreateMap<RequestAddInventoryLocationJson, InventoryLocation>();
            CreateMap<RequestAddInventoryMovementJsons, InventoryMovements>()
                .ForMember(dest => dest.MovementDate, opt => opt.Ignore());
        }

        private void EntityToResponse()
        {
            CreateMap<WarehouseLocations, ResponseLocationsJson>();
        }

        private void ResponseToDTO()
        {
            // CreateMap<ResponseProductByCode128, ProductionMovementQueryResult>();
            
        }

        private void DTOToResponse()
        {
            //CreateMap<OrderQueryResult, ResponseOrderJson>();
            CreateMap<LocationQueryResult, ResponseLocationProduct>();
            CreateMap<LocationWithProductsQueryResult, ResponseLocationWithProductsJson>();
            CreateMap<ProductLocationsQueryResult, ResponseProductLocations>();
            CreateMap<ProductWithLocationsQueryResult, ResponseProductWithLocations>();
        }
    }
}
