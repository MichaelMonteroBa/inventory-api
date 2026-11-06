using System;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Api.Models;

public class Product
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    public string SKU { get; set; }
    public string Description { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public int minStock { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    
    public Guid? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    
    public ICollection<StockMovement> StockMovements { get; set; }
    
}