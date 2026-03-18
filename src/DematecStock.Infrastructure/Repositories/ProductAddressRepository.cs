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

		private static IQueryable<FlatLocationWithProductsQueryResult> ApplyFilters(
			IQueryable<FlatLocationWithProductsQueryResult> query,
			string? isActive, string? isMovementAllowed, string? isAllowReplenishment,
			string? isPickingLocation, string? isProductInactive)
		{
			if (!string.IsNullOrWhiteSpace(isActive)) query = query.Where(x => x.IsActive == isActive);
			if (!string.IsNullOrWhiteSpace(isMovementAllowed)) query = query.Where(x => x.AllowsStockMovement == isMovementAllowed);
			if (!string.IsNullOrWhiteSpace(isAllowReplenishment)) query = query.Where(x => x.AllowsReplenishment == isAllowReplenishment);
			if (!string.IsNullOrWhiteSpace(isPickingLocation)) query = query.Where(x => x.IsPickingLocation == isPickingLocation);
			if (!string.IsNullOrWhiteSpace(isProductInactive)) query = query.Where(x => x.IsProductInactive == (isProductInactive == "S"));
			return query;
		}

		public async Task<LocationQueryResult> GetStoredItemsByLocation(int idLocation, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation, string? isProductInactive)
		{            
			var productsStored = await ApplyFilters(BaseQuery().Where(x => x.IdLocation == idLocation), isActive, isMovementAllowed, isAllowReplenishment, isPickingLocation, isProductInactive)
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

		public async Task<List<LocationQueryResult>> GetStoredItemsByLocationQuery(string query, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation, string? isProductInactive)
		{
			var baseQuery = BaseQuery();

			if (int.TryParse(query, out int id))
				baseQuery = baseQuery.Where(x => x.IdLocation == id);
			else
				baseQuery = baseQuery.Where(x => x.LocationName != null && x.LocationName.Contains(query));

			baseQuery = ApplyFilters(baseQuery, isActive, isMovementAllowed, isAllowReplenishment, isPickingLocation, isProductInactive);

			var rows = await baseQuery.ToListAsync();

			if (!rows.Any())
				throw new NotFoundException("Localização não encontrada.");

			var result = rows
				.GroupBy(x => x.IdLocation)
				.Select(g =>
				{
					var header = g.First();
					return new LocationQueryResult
					{
						IdLocation = header.IdLocation ?? 0,
						LocationName = header.LocationName ?? string.Empty,
						Aisle = header.Aisle ?? 0,
						Building = header.Building ?? 0,
						Level = header.Level ?? 0,
						Bin = header.Bin ?? 0,
						IsActive = header.IsActive ?? string.Empty,
						AllowsReplenishment = header.AllowsReplenishment ?? string.Empty,
						AllowsStockMovement = header.AllowsStockMovement ?? string.Empty,
						IsPickingLocation = header.IsPickingLocation ?? string.Empty,
						StoreProducts = g.Select(product => new LocationWithProductsQueryResult
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
				})
				.ToList();

			return result;
		}

		public async Task<ProductLocationsQueryResult> GetStoredItems(int? idProduct, string? reference, string? ean13, string? isActive, string? isMovementAllowed, string? isAllowReplenishment, string? isPickingLocation, string? isProductInactive)
		{
			var query = BaseQuery();

			if (idProduct.HasValue)
				query = query.Where(x => x.IdProduct == idProduct.Value);

			if (!string.IsNullOrWhiteSpace(reference))
				query = query.Where(x => x.Reference == reference);

			if (!string.IsNullOrWhiteSpace(ean13))
				query = query.Where(x => x.Ean13Code == ean13);

			query = ApplyFilters(query, isActive, isMovementAllowed, isAllowReplenishment, isPickingLocation, isProductInactive);

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
