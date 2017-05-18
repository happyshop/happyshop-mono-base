using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace HappyShop.Database
{
  public class KioskConnection : IDbConnection
  {
    private readonly IDbConnection _connection = null;
    private IDbTransaction _transaction = null;

    public IDbConnection Connection
    {
      // ReSharper disable once ConvertPropertyToExpressionBody
      get { return _connection; }
    }

    public IDbTransaction Transaction
    {
      // ReSharper disable once ConvertPropertyToExpressionBody
      get { return _transaction; }
    }

    public KioskConnection()
    {
      string hostname = (string)Configuration.Static.Merged.MySqlHostname;
      string database = (string)Configuration.Static.Merged.MySqlDatabase;
      string username = (string)Configuration.Static.Merged.MySqlUsername;
      string password = (string)Configuration.Static.Merged.MySqlPassword;
      string defaultCommandTimeout = Configuration.Static.Merged.MySqlDefaultCommandTimeout;
      var mySqlConnectionString = BuildMySqlConnectionString(hostname, database, username, password, defaultCommandTimeout);
      Console.WriteLine("Opening MySQL with " + mySqlConnectionString);
      _connection = new MySqlConnection(mySqlConnectionString);
      _connection.Open();
    }

    private static string BuildMySqlConnectionString(string hostname, string database, string username, string password, string defaultCommandTimeout)
    {
      // ReSharper disable once UseStringInterpolation
      return string.Format("server={0};Database={1};Uid={2};Pwd={3};default command timeout={4};", hostname, database, username, password, defaultCommandTimeout);
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
      _transaction = _connection.BeginTransaction(il);
      return _transaction;
    }

    public IDbTransaction BeginTransaction()
    {
      _transaction = _connection.BeginTransaction();
      return _transaction;
    }

    public void ChangeDatabase(string databaseName)
    {
      _connection.ChangeDatabase(databaseName);
    }

    public void Close()
    {
      _connection.Close();
    }

    // ReSharper disable once ConvertPropertyToExpressionBody
    ConnectionState IDbConnection.State { get { return _connection.State; } }

    public void Open()
    {
      _connection.Open();
    }

    public string ConnectionString
    {
      get
      {
        return _connection.ConnectionString;
      }
      set
      {
        _connection.ConnectionString = value;
      }
    }

    public int ConnectionTimeout
    {
      get { return _connection.ConnectionTimeout; }
    }

    public void Rollback()
    {
      if (!IsCommitted && _transaction != null)
      {
        _transaction.Rollback();
      }
    }

    public void Commit()
    {
      if (_transaction != null)
      {
        _transaction.Commit();
        IsCommitted = true;
      }
    }

    public bool IsCommitted
    {
      get;
      private set;
    }
    public string Database { get; private set; }
    public ConnectionState State { get; private set; }

    public IDbCommand CreateCommand()
    {
      IDbCommand command = _connection.CreateCommand();
      command.Transaction = _transaction;
      return command;
    }

    public void Dispose()
    {
      Rollback();
      _connection.Dispose();
    }
  }
}