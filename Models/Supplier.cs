using System;
using System.Collections.Generic;
namespace Inventory.Api.Models;

public class Supplier
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string RNC { get; set; }
    
    public ICollection<Product> Products { get; set; }
}