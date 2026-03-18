using AutoMapper;
using DematecStock.Communication.Responses;
using DematecStock.Domain.Repositories.ProductAddress;

namespace DematecStock.Application.UseCases.ProductsAddress.GetStorageProductsByLocationQuery
{
    public class GetStorageProductsByLocationQueryUseCase : IGetStorageProductsByLocationQueryUseCase
    {
        private readonly IProductAddressReadOnlyRepository _productAddressReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetStorageProductsByLocationQueryUseCase(
            IProductAddressReadOnlyRepository productAddressReadOnlyRepository,
            IMapper mapper)
        {
            _productAddressReadOnlyRepository = productAddressReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<ResponseLocationProduct>> Execute(string query, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation, string? isProductInactive)
        {
            var locations = await _productAddressReadOnlyRepository.GetStoredItemsByLocationQuery(query, isActive, isMovementAllowed, isAllowReplenishment, isPickingLocation, isProductInactive);
            return _mapper.Map<List<ResponseLocationProduct>>(locations);
        }
    }
}
