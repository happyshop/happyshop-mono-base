namespace HappyShop.Model
{
  public class Response
  {
    public ResponseReturnCode ReturnCode;
    public string Message;
    public Item Item;
    public User User;
    public Purchase Purchase;
  }

  public enum ResponseReturnCode
  {
    Ok = 0,
    ItemNotFound,
    UserNotFound,
    FailedToAddPurchase,
    FailedToParseCount,
    FailedToCancelLastOrder,
    FailedToParseAmount,
    FailedToPayIn,
    FailedToParseFullPrice,
    FailedToAddItems,
    NotImplemented,
  }
}
