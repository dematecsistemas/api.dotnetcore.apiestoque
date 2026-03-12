using AutoMapper;
using DematecStock.Communication.Responses;
using DematecStock.Domain.Repositories.ProductAddress;

namespace DematecStock.Application.UseCases.ProductsAddress.GetAllStorageProductsByLocation
{
    public class GetAllStorageProductsByLocationUseCase : IGetAllStorageProductsByLocationUseCase
    {
        private readonly IProductAddressReadOnlyRepository _productAddressReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetAllStorageProductsByLocationUseCase(IProductAddressReadOnlyRepository productAddressReadOnlyRepository, IMapper mapper)
        {
            _productAddressReadOnlyRepository = productAddressReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<ResponseLocationProduct> Execute(int idLocation)
        {
            var productsLocation = await _productAddressReadOnlyRepository.GetStoredItemsByLocation(idLocation);

            var response = _mapper.Map<ResponseLocationProduct>(productsLocation);
            return response;
        }
    }
}
