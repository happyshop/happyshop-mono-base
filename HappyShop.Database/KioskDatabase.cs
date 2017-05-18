using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Dapper;
using HappyShop.Configuration;
using HappyShop.Model;

namespace HappyShop.Database
{
  public class KioskDatabase : IDisposable
  {
    public KioskDatabase(Func<IDbConnection> connectionFunc, bool withTransaction = false)
    {
      _connectionFunc = connectionFunc;
      if (withTransaction)
      {
        Connection.BeginTransaction();
      }
      if( !float.TryParse((string)Static.Merged.PriceCalculationExtraCharge, out _priceCalculationExtraCharge) )
      {
        _priceCalculationExtraCharge = 0.02f;
      }
    }

    private readonly float _priceCalculationExtraCharge;
    private readonly Func<IDbConnection> _connectionFunc;
    private IDbConnection _connection;

    private IDbConnection Connection
    {
      get { return _connection ?? (_connection = _connectionFunc()); }
    }

    public User SelectUserByShortname(string shortname)
    {
      string query = new SelectBuilder()
        .ColumnAs("short", "Short")
        .ColumnAs("firstname", "Name")
        .ColumnAs("lastname", "Lastname")
        .ColumnAs("mail", "Mail")
        .ColumnAs("balance", "Balance")
        .From("users")
        .Where(string.Format("`short`='{0}'", shortname))
        .Limit(1).Build();
      return Connection.Query<User>(query).FirstOrDefault();
    }

    public Item SelectItemByBarcode(string barcode)
    {
      string query = new SelectBuilder()
        .ColumnAs("barcode", "Barcode")
        .ColumnAs("description", "Description")
        .ColumnAs("price", "Price")
        .ColumnAs("count", "Count")
        .From("items")
        .Where(string.Format("barcode={0}", barcode))
        .Limit(1).Build();
      return Connection.Query<Item>(query).FirstOrDefault();
    }

    public bool AddItem(string barcode, int count, float fullPrice)
    {
      bool success = false;

      Item item = SelectItemByBarcode(barcode);
      if (item != null)
      {
        int oldCount = item.Count;
        double oldFullPrice = item.Price * item.Count;
        item.Count += count;
        double newPriceFrac = fullPrice/count;
        double newPrice = Math.Round(newPriceFrac, 2, MidpointRounding.AwayFromZero) + _priceCalculationExtraCharge;
        double sumFullPrice = (newPrice*count) + oldFullPrice;
        double effectivePriceFrac = sumFullPrice/(count + oldCount);
        double effectivePrice = Math.Round(effectivePriceFrac, 2);
        item.Price = (float)effectivePrice;
        success = UpdateItem(item);
      }
    
      return success;
    }

    private bool UpdateItem(Item item)
    {
      bool success = false;
      
      if (item != null)
      {
        string queryUpdateBalance = new UpdateBuilder()
          .OnTable("items")
          .Column("price", item.Price.ToString("0.00", CultureInfo.InvariantCulture))
          .Column("count", item.Count)
          .Where(string.Format("`barcode`='{0}'", item.Barcode))
          .Build();
        success = (Connection.Execute(queryUpdateBalance) == 1);
      }

      return success;
    }
    
    public bool PayIn(string shortname, float amount)
    {
      bool success = false;

      if (!string.IsNullOrEmpty(shortname))
      {
        string queryPayIn = new UpdateBuilder()
          .OnTable("users")
          .Column("balance", "`balance`+(" + amount.ToString("0.00", CultureInfo.InvariantCulture) + ")")
          .Where(String.Format("`short`='{0}'", shortname))
          .Build();

        success = (Connection.Execute(queryPayIn) == 1);
      }

      return success;
    }

    public bool CancelLastOrder(string shortname, string origin)
    {
      string queryFindLastOrder = new SelectBuilder()
        .From("purchases")
        .Limit(1)
        .Where("`short`='" + shortname + "'")
        .ColumnAs("barcode", "Barcode")
        .ColumnAs("short", "Short")
        .ColumnAs("sum", "Sum")
        .ColumnAs("when", "When")
        .OrderBy("when", SelectBuilder.SelectOrder.Descending)
        .Build();

      Purchase purchase = Connection.Query<Purchase>(queryFindLastOrder).FirstOrDefault();

      if (purchase == null) return false;

      if (purchase.Sum <= 0) return false;

      Item item = SelectItemByBarcode(purchase.Barcode);

      if (item == null) return false;

      return RemovePurchase(shortname, item, origin);
    }

    public bool AddPurchase(string shortname, Item item, string origin)
    {
      Console.WriteLine("AddPurchase({0}, {1})", shortname, item.Barcode);
      return DoPurchase(shortname, item, origin, "-");
    }

    public bool RemovePurchase(string shortname, Item item, string origin)
    {
      return DoPurchase(shortname, item, origin, "+");
    }

    private bool DoPurchase(string shortname, Item item, string origin, string direction)
    {
      string queryAddPurchase = new InsertBuilder()
        .OnTable("purchases")
        .Column("barcode", "Barcode")
        .Column("short", "Short")
        .Column("sum", "Sum")
        .Column("when", "When")
        .Column("origin", "Origin")
        .Build();

      var purchase = new Purchase
      {
        Barcode = item.Barcode,
        Short = shortname,
        When = DateTime.Now,
        Sum = item.Price,
        Origin = origin
      };

      if (direction == "+")
      {
        purchase.Sum *= -1;
      }

      int rowsAffected = Connection.Execute(queryAddPurchase, purchase);

      string queryUpdateBalance = new UpdateBuilder()
        .OnTable("users")
        .Column("balance", "`balance`" + direction + item.Price.ToString("0.00", CultureInfo.InvariantCulture))
        .Where(String.Format("`short`='{0}'", shortname))
        .Build();

      rowsAffected += Connection.Execute(queryUpdateBalance);

      string queryUpdateItem = new UpdateBuilder()
        .OnTable("items")
        .Column("count", "`count`" + direction + "1")
        .Where(String.Format("`barcode`='{0}'", item.Barcode))
        .Build();

      rowsAffected += Connection.Execute(queryUpdateItem);

      return (rowsAffected == 3);
    }

    public bool UpdateStock(string barcode, int iCount, string origin)
    {
      Console.WriteLine("UpdateStock({0}, {1})", barcode, iCount);
      Item item = SelectItemByBarcode(barcode);
      if (item == null) return false;

      if (item.Count > iCount)
      {
        int loss = item.Count - iCount;

        for (var i = 0; i < loss; i++)
        {
          AddPurchase("XXX", item, origin); 
        }
      }

      item.Count = iCount;
      if (!UpdateItem(item)) return false;

      return true;
    }

    public void Commit()
    {
      var kioskConnection = Connection as KioskConnection;
      if (kioskConnection != null && kioskConnection.Transaction != null)
      {
        kioskConnection.Commit();
      }
    }

    public void Dispose()
    {
      _connection.Dispose();
      _connection = null;
    }

    public IEnumerable<User> Users(string filter)
    {
      string query = new SelectBuilder()
        .ColumnAs("short", "Short")
        .ColumnAs("firstname", "Name")
        .ColumnAs("lastname", "Lastname")
        .ColumnAs("mail", "Mail")
        .ColumnAs("balance", "Balance")
        .From("users")
        .Build();
      var users = Connection.Query<User>(query);

      if( filter != null )
      {
        string[] shorts = filter.Split(',');
        return users.Where(user => shorts.Contains(user.Short)).ToList();
      }
      return users.ToList();
    }

    public IEnumerable<Item> Items(string filter)
    {
      string query = new SelectBuilder()
        .ColumnAs("barcode", "Barcode")
        .ColumnAs("description", "Description")
        .ColumnAs("price", "Price")
        .ColumnAs("count", "Count")
        .From("items")
        .Build();
      var items = Connection.Query<Item>(query);

      if (filter != null)
      {
        string[] barcodes = filter.Split(',');
        return items.Where(item => barcodes.Contains(item.Barcode)).ToList();
      }
      return items.ToList();
    }

    public Stream GetItemImageStream(string barcode)
    {
      string commandText = new SelectBuilder()
        .ColumnAs("image", "Image")
        .From("items")
        .Where(string.Format("barcode={0}", barcode))
        .Build();
      byte[] bytes = Connection.Query<byte[]>(commandText).FirstOrDefault();

      return bytes != null ? new MemoryStream(bytes) : null;
    }
  }
}
