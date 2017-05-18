using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HappyShop.Database
{
  public class SelectBuilder
  {
    private string _from;
    private string _where;
    private string _orderByColumn;
    private SelectOrder _selectOrder = SelectOrder.Ascending;
    private int _limit = -1;
    private readonly Dictionary<string, string> _columns = new Dictionary<string, string>();

    public SelectBuilder From(string from)
    {
      string mySqlDatabase = Configuration.Static.Merged.MySqlDatabase;
      Console.WriteLine("Using database name " + mySqlDatabase);
      _from = string.Format(mySqlDatabase + ".{0}", from);
      return this;
    }

    public SelectBuilder Limit(int limit)
    {
      _limit = limit;
      return this;
    }

    public SelectBuilder ColumnAs(string name, string parameterName)
    {
      _columns.Add(name, parameterName);
      return this;
    }

    public SelectBuilder Where(string where)
    {
      _where = where;
      return this;
    }

    public string Build()
    {
      //string _columns.Select(column => column.Name)
      StringBuilder builder = new StringBuilder();
      builder.Append("SELECT ");
      builder.Append(string.Join(",", _columns.Select(pair => string.Format("`{0}` AS `{1}`", pair.Key, pair.Value))));
      builder.AppendFormat(" FROM {0}", _from);

      if( !string.IsNullOrEmpty(_where) )
      {
        builder.AppendFormat(" WHERE {0}", _where);
      }
      
      if (!string.IsNullOrEmpty(_orderByColumn))
      {
        builder.AppendFormat(" ORDER BY `{0}`", _orderByColumn);
        builder.Append(_selectOrder == SelectOrder.Ascending ? " ASC" : " DESC");
      }

      if (_limit != -1)
      {
        builder.AppendFormat(" LIMIT {0}", _limit);
      }
      return builder.ToString();
    }

    public enum SelectOrder
    {
      Ascending,
      Descending,
    }

    public SelectBuilder OrderBy(string column, SelectOrder selectOrder)
    {
      _orderByColumn = column;
      _selectOrder = selectOrder;
      return this;
    }
  }
}
