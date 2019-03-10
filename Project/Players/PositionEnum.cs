using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Players
{
    public enum PositionEnum
    {
        center = 1,
        [Description("power forward")]
        powerForward,
        [Description("small forward")]
        smallForward,
        [Description("shooting guard")]
        shootingGuard,
        [Description("point guard")]
        pointGuard
    }

    public static class ErrorLevelExtensions
    {
        public static string ToFriendlyString(this PositionEnum me)
        {
            switch (me)
            {
                case PositionEnum.center:
                    return "center";
                case PositionEnum.powerForward:
                    return "power forward";
                case PositionEnum.smallForward:
                    return "small forward";
                case PositionEnum.shootingGuard:
                    return "shooting guard";
                case PositionEnum.pointGuard:
                    return "point guard";
                default:
                    return string.Empty;
            }
        }
    }
}
