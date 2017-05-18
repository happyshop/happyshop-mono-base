using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HappyShop.Database
{
  public class InsertBuilder
  {
    private string _name;
    private readonly List<Column> _columns = new List<Column>();

    public InsertBuilder OnTable(string name)
    {
      _name = name;
      return this;
    }

    public InsertBuilder Column(string name)
    {
      _columns.Add(new Column(name, string.Empty));
      return this;
    }

    public InsertBuilder Column(string name, string parameterName)
    {
      _columns.Add(new Column(name, parameterName));
      return this;
    }

    public string Build()
    {
      //string _columns.Select(column => column.Name)
      StringBuilder builder = new StringBuilder();
      builder.AppendFormat("insert {0}(", _name);
      builder.Append(string.Join(",", _columns.Select(column => column.EscapedName)));
      builder.Append(") VALUES (");
      builder.Append(string.Join(",", _columns.Select(column => column.PreparedParameterName)));
      builder.Append(")");
      return builder.ToString();
    }
  }
}
