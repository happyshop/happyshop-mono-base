using System;

namespace HappyShop.Model
{
  public class Purchase
  {
    public int Id { get; set; }
    public string Barcode { get; set; }
    public string Short { get; set; }
    public double Sum { get; set; }
    public DateTime When { get; set; }
    public string Origin { get; set; }
  }
}
