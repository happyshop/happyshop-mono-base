using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HappyShop.Configuration
{
  public static class Static
  {
    public static dynamic Merged
    {
      get { return _merged ?? (_merged = Merge(Default, Local)); }
    }

    public static dynamic Default
    {
      get { return _default ?? (_default = LoadJson("config.default.json")); }
    }

    public static dynamic Local
    {
      get { return _local ?? (_local = LoadJson("config.local.json")); }
    }

    private static dynamic Merge(object item1, object item2)
    {
      IDictionary<string, object> result = new ExpandoObject();

      var jObj1 = (JObject) item1;

      foreach (JToken token in jObj1.Children())
      {
        if (token is JProperty)
        {
          var prop = token as JProperty;
          result[prop.Name] = prop.Value;
        }
      }

      var jObj2 = (JObject) item2;

      foreach (JToken token in jObj2.Children())
      {
        if (token is JProperty)
        {
          var prop = token as JProperty;
          result[prop.Name] = prop.Value;
        }
      }

      return result;
    }

    private static dynamic LoadJson(string file)
    {
      var json = ReadFileToStringOrEmptyJson(file);
      return JsonConvert.DeserializeObject(json);
    }

    private static string ReadFileToStringOrEmptyJson(string file)
    {
      var content = "{}";
      if (File.Exists(file))
      {
        using (var r = new StreamReader(file))
        {
          content = r.ReadToEnd();
        }
      }
      return content;
    }

    private static dynamic _default;
    private static dynamic _local;
    private static dynamic _merged;
  }
}