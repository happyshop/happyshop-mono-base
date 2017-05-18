using System;
using System.IO;

namespace HappyShop.ServiceConnector
{
  public static class Extensions
  {
    public static string AppendTimeStamp(this string fileName)
    {
      return string.Concat(
          Path.GetFileNameWithoutExtension(fileName),
          DateTime.Now.ToString("yyyyMMddHHmmssfff"),
          Path.GetExtension(fileName)
          );
    }
  }
}
