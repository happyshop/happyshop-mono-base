using System.Diagnostics;

namespace HappyShop.SystemTools
{
    public class WatterottDisplay : IDisplaySwitch
    {
        private const string _commandTemplate = "sh -c \"echo {0} > /sys/class/backlight/fb_ili9341/bl_power\"";

        private void Run(int value)
        {
            if (KioskUtils.IsLinux)
            {
                Process.Start("sudo", string.Format(_commandTemplate, value));
            }
        }

        public void Off()
        {
            Run(1);
        }

        public void On()
        {
            Run(0);
        }
    }
}