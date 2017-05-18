namespace HappyShop.Model
{
  public class Item
  {
    public int Id { get; set; }
    public string Barcode { get; set; }
    public string Description { get; set; }
    public float Price { get; set; }
    public int Count { get; set; }
  }
}