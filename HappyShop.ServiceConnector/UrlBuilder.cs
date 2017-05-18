using System.Linq;
using System.Text;

namespace HappyShop.ServiceConnector
{
  public class UrlBuilder
  {
    static private readonly string _rootUrl;
    private readonly StringBuilder _builder = new StringBuilder(_rootUrl);

    static UrlBuilder()
    {
      _rootUrl = Configuration.Static.Merged.ServiceConnectorUrl;
      if (_rootUrl.Last() == '/')
      {
        _rootUrl = _rootUrl.TrimEnd('/');
      }
    }

    public UrlBuilder ForUser(string shortName)
    {
      _builder.AppendFormat("/user/{0}", shortName);
      return this;
    }

    public UrlBuilder ForItem(string barcode)
    {
      _builder.AppendFormat("/item/{0}", barcode);
      return this;
    }

    public UrlBuilder ItemImage(string barcode)
    {
      _builder.AppendFormat("/item/{0}/image", barcode);
      return this;
    }

    public UrlBuilder ItemIds()
    {
      _builder.AppendFormat("/itemids");
      return this;
    }

    public UrlBuilder Order(string barcode)
    {
      _builder.AppendFormat("/order/{0}", barcode);
      return this;
    }

    public string Build()
    {
      return _builder.ToString();
    }
  }
}