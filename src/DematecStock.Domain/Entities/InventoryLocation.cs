using System.ComponentModel.DataAnnotations.Schema;

namespace DematecStock.Domain.Entities
{
    [Table("ProdEstocagem")]
    public class InventoryLocation
    {
        [Column("CodEst")]
        public int IdLocation { get; set; }

        [Column("CodProduto")]
        public int IdProduct { get; set; }

        [Column("Referencia")]
        public string? Reference { get; set; }

        [Column("DataCadastro")]
        public DateTime? CreatedDate { get; set; }

        [Column("SaldoEstoque")]
        public decimal OnHandQuantity { get; private set; }

        public void UpdateOnHandQuantity(decimal quantity) => OnHandQuantity = quantity;
    }
}
