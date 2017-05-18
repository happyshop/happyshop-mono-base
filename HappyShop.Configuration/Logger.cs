using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace HappyShop.Configuration
{
  public static class Logger
  {
    public static void Setup()
    {
      var hierarchy = (Hierarchy) LogManager.GetRepository();

      var patternLayout = new PatternLayout
      {
        ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
      };
      patternLayout.ActivateOptions();

      var consoleAppender = new ConsoleAppender {Layout = patternLayout};
      consoleAppender.ActivateOptions();
      hierarchy.Root.AddAppender(consoleAppender);

      hierarchy.Root.Level = Level.Info;
      hierarchy.Configured = true;
    }
  }
}