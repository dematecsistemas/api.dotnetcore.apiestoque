using AutoMapper;
using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageLocationsByProduct;
using DematecStock.Communication.Responses;
using DematecStock.Domain.Repositories.ProductAddress;

namespace DematecStock.Application.UseCases.ProductsAddress.GetAllLocationsByProducts
{
    public class GetAllLocationsByProductUseCase : IGetAllLocationsByProductUseCase
    {
        private readonly IProductAddressReadOnlyRepository _addressProductReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetAllLocationsByProductUseCase(IProductAddressReadOnlyRepository addressProductReadOnlyRepository, IMapper mapper)
        {
            _addressProductReadOnlyRepository = addressProductReadOnlyRepository;
            _mapper = mapper;
        }
        public async Task<ResponseProductLocations> Execute(int? idProduct, string? reference, string? ean13Code, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation, string? isProductInactive)
        {
            var locationsProduct = await _addressProductReadOnlyRepository.GetStoredItems(idProduct, reference, ean13Code, isActive, isMovementAllowed, isAllowReplenishment, isPickingLocation, isProductInactive);

            var response = _mapper.Map<ResponseProductLocations>(locationsProduct);
            return response;
        }
    }


}
