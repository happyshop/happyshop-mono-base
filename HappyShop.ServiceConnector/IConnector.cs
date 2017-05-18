using System.Collections.Generic;
using System.IO;
using HappyShop.Model;

namespace HappyShop.ServiceConnector
{
  public interface IConnector
  {
    User GetUser(string shortName);
    Response OrderItem(string shortName, string barcode);
    Item GetItem(string barcode);
    IEnumerable<string> GetItemIds();
    Stream GetImage(string barcode);
  }
}