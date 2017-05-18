namespace HappyShop.Database
{
  public class Column
  {
    public Column(string name, string parameterName)
    {
      Name = name;
      ParameterName = parameterName;
    }

    public string EscapedName
    {
      get { return string.Format("`{0}`", Name); }
    }

    public string PreparedParameterName
    {
      get { return string.Format("@{0}", string.IsNullOrEmpty(ParameterName) ? Name : ParameterName); }
    }

    public string Name { get; set; }
    public string ParameterName { get; set; }
  }
}
