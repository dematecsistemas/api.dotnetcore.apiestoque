using DematecStock.Domain.DTOs;
using DematecStock.Domain.Repositories.ProductAddress;
using DematecStock.Exception.ExceptionsBase;
using DematecStock.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DematecStock.Infrastructure.Repositories
{
	public class ProductAddressRepository : IProductAddressReadOnlyRepository
	{
		private readonly DematecStockDbContext _dbContext;

		public ProductAddressRepository(DematecStockDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		private IQueryable<FlatLocationWithProductsQueryResult> BaseQuery()
		{
			return _dbContext.Set<FlatLocationWithProductsQueryResult>()
				.AsNoTracking();
		}

		public async Task<LocationQueryResult> GetStoredItemsByLocation(int idLocation)
		{			
			var productsStored = await BaseQuery()
				.Where(x => x.IdLocation == idLocation)
				.ToListAsync();

			if (!productsStored.Any())
			{
				throw new NotFoundException("Localização não encontrada.");
			}

			var productsStoredHeader = productsStored.First();

			var locationQueryResult = new LocationQueryResult
			{
				IdLocation = productsStoredHeader.IdLocation ?? 0,
				LocationName = productsStoredHeader.LocationName ?? string.Empty,
				Aisle = productsStoredHeader.Aisle ?? 0,
				Building = productsStoredHeader.Building ?? 0,
				Level = productsStoredHeader.Level ?? 0,
				Bin = productsStoredHeader.Bin ?? 0,
				IsActive = productsStoredHeader.IsActive ?? string.Empty,
				AllowsReplenishment = productsStoredHeader.AllowsReplenishment ?? string.Empty,
				AllowsStockMovement = productsStoredHeader.AllowsStockMovement ?? string.Empty,
				IsPickingLocation = productsStoredHeader.IsPickingLocation ?? string.Empty,

				StoreProducts = productsStored.Select(product => new LocationWithProductsQueryResult
				{
					IdProduct = product.IdProduct,
					ProductDescription = product.ProductDescription,
					Reference = product.Reference,
					CreatedDate = product.CreatedDate ?? default,
					IdProductGroup = product.IdProductGroup,
					ProductGroupDescription = product.ProductGroupDescription,
					IdProductSubgroup = product.IdProductSubgroup,
					ProductSubgroupDescription = product.ProductSubgroupDescription,
					IdSupplier = product.IdSupplier,
					SupplierName = product.SupplierName,
					IdNcm = product.IdNcm,
					NcmNumber = product.NcmNumber,
					IsProductInactive = product.IsProductInactive,
					ProductType = product.ProductType,
					Ean13Code = product.Ean13Code,
					MaxQuantity = product.MaxQuantity,
					MinQuantity = product.MinQuantity,
					PackQuantity = product.PackQuantity,
					GrossWeight = product.GrossWeight,
					NetWeight = product.NetWeight,
					OnHandQuantity = product.OnHandQuantity
				}).ToList()
			};
			return locationQueryResult;

		}

		public async Task<ProductLocationsQueryResult> GetStoredItems(int? idProduct, string? reference, string? ean13)
		{
			var query = BaseQuery();

			if (idProduct.HasValue)
				query = query.Where(x => x.IdProduct == idProduct.Value);

			if (!string.IsNullOrWhiteSpace(reference))
				query = query.Where(x => x.Reference == reference);

			if (!string.IsNullOrWhiteSpace(ean13))
				query = query.Where(x => x.Ean13Code == ean13);

			var rows = await query.ToListAsync();

			if (!rows.Any())
			{
				throw new NotFoundException("Produto não encontrado em nenhuma localização.");
			}

			var productsStoredHeader = rows.First();



			var productsQueryResult = new ProductLocationsQueryResult
			{
				IdProduct = productsStoredHeader.IdProduct,
				ProductDescription = productsStoredHeader.ProductDescription,
				Reference = productsStoredHeader.Reference,
				IdProductGroup = productsStoredHeader.IdProductGroup,
				ProductGroupDescription = productsStoredHeader.ProductGroupDescription,
				IdProductSubgroup = productsStoredHeader.IdProductSubgroup,
				ProductSubgroupDescription = productsStoredHeader.ProductSubgroupDescription,
				IdSupplier = productsStoredHeader.IdSupplier,
				SupplierName = productsStoredHeader.SupplierName,
				IdNcm = productsStoredHeader.IdNcm,
				NcmNumber = productsStoredHeader.NcmNumber,
				IsProductInactive = productsStoredHeader.IsProductInactive,
				ProductType = productsStoredHeader.ProductType,
				Ean13Code = productsStoredHeader.Ean13Code,
				MaxQuantity = productsStoredHeader.MaxQuantity,
				MinQuantity = productsStoredHeader.MinQuantity,
				PackQuantity = productsStoredHeader.PackQuantity,
				GrossWeight = productsStoredHeader.GrossWeight,
				NetWeight = productsStoredHeader.NetWeight,

				StorageBin = rows
					.Where(productStored => productStored.IdLocation.HasValue)
					.Select(productStored => new ProductWithLocationsQueryResult
					{
						IdLocation = productStored.IdLocation!.Value,
						LocationName = productStored.LocationName ?? string.Empty,
						Aisle = productStored.Aisle ?? 0,
						Building = productStored.Building ?? 0,
						Level = productStored.Level ?? 0,
						Bin = productStored.Bin ?? 0,
						IsActive = productStored.IsActive ?? string.Empty,
						AllowsReplenishment = productStored.AllowsReplenishment ?? string.Empty,
						AllowsStockMovement = productStored.AllowsStockMovement ?? string.Empty,
						IsPickingLocation = productStored.IsPickingLocation ?? string.Empty,
						CreatedDate = productStored.CreatedDate ?? default,
						OnHandQuantity = productStored.OnHandQuantity
					}).ToList()
			};
			return productsQueryResult;
		}

	}
}
