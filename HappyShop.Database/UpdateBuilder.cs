using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HappyShop.Database
{
  public class UpdateBuilder
  {
    private string _name;
    private string _where;
    private readonly Dictionary<string, object> _columns = new Dictionary<string, object>();

    public UpdateBuilder OnTable(string name)
    {
      _name = name;
      return this;
    }

    public UpdateBuilder Column(string name, object value)
    {
      _columns.Add(name, value);
      return this;
    }

    public UpdateBuilder Where(string where)
    {
      _where = where;
      return this;
    }

    public string Build()
    {
      //string _columns.Select(column => column.Name)
      StringBuilder builder = new StringBuilder();
      builder.AppendFormat("UPDATE {0} SET ", _name);
      builder.Append(string.Join(",", _columns.Select(pair => string.Format("`{0}`={1}", pair.Key, pair.Value))));
      builder.AppendFormat(" WHERE {0}", _where);
      return builder.ToString();
    }
  }
}
