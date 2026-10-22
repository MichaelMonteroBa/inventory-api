using System;
using System.Collections.Generic;
namespace Inventory.Api.Models;

public class StockMovement
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public string type { get; set; } // Entry, exit, adjustment
    public int Quantity { get; set; }
    public string Reason { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}