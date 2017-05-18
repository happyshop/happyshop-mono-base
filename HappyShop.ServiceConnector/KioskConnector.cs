using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HappyShop.Model;
using System.Web.Script.Serialization;

namespace HappyShop.ServiceConnector
{
  public class KioskConnector : IConnector
  {
    private WebRequest CreateRequest(string url)
    {
      WebRequest request = WebRequest.Create(url);
      request.Credentials = new NetworkCredential("root", "root");
      Console.WriteLine("Get({0}) called.", url);
      return request;
    }

    public Response Get(UrlBuilder builder)
    {
      WebRequest request = CreateRequest(builder.Build());
      return new JavaScriptSerializer().Deserialize<Response>(request.GetResponse().GetResponseStream().AsString());
    }

    public User GetUser(string shortName)
    {
      return Get(new UrlBuilder().ForUser(shortName)).User;
    }

    public Response OrderItem(string shortName, string barcode)
    {
      return Get(new UrlBuilder().ForUser(shortName).Order(barcode));
    }

    public Item GetItem(string barcode)
    {
      return Get(new UrlBuilder().ForItem(barcode)).Item;
    }

    public IEnumerable<string> GetItemIds()
    {
      WebRequest request = CreateRequest(new UrlBuilder().ItemIds().Build());
      string input = request.GetResponse().GetResponseStream().AsString();
      return new JavaScriptSerializer().Deserialize<IEnumerable<string>>(input);
    }

    public Stream GetImage(string barcode)
    {
      WebRequest request = CreateRequest(new UrlBuilder().ItemImage(barcode).Build());
      return request.GetResponse().GetResponseStream();
    }
  }
}
