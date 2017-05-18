using System.IO;
using System.Text;

namespace HappyShop.ServiceConnector
{
  static public class StreamExtensions
  {
    static public string AsString(this Stream @this)
    {
      using (StreamReader reader = new StreamReader(@this, Encoding.UTF8))
      {
        return reader.ReadToEnd();
      }    
    }

    static public Stream Rewind(this Stream @this)
    {
      if (@this.CanSeek && @this.Position != 0)
      {
        @this.Position = 0;
      }
      return @this;
    }

    static public byte[] ToArray(this Stream @this)
    {
      using ( MemoryStream stream = new MemoryStream() )
      {
        @this.CopyTo(stream);
        return stream.ToArray();
      }
    }
  }
}