using System;
using System.Diagnostics;

namespace HappyShop.SystemTools
{
  public class KioskUtils
  {
    public static bool IsLinux
    {
      get
      {
        int p = (int)Environment.OSVersion.Platform;
        return (p == 4) || (p == 6) || (p == 128);
      }
    }

    public static string HeadCommitId
    {
      get
      {
        return Process.Start("git log -n 1").StandardOutput.ReadToEnd();
      }
    }
  }
}
