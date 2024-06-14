// Copyright © 2017 Paddy Xu
//
// This file is part of QuickLook program.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using QuickLook.Common.NativeMethods;
using static QuickLook.Common.NativeMethods.User32;
using static QuickLook.Common.NativeMethods.MsCms;
using System.Text;

namespace QuickLook.Common.Helpers;

public static class DisplayDeviceHelper
{
    public const int DefaultDpi = 96;

    public static ScaleFactor GetScaleFactorFromWindow(Window window)
    {
        return GetScaleFactorFromWindow(new WindowInteropHelper(window).EnsureHandle());
    }

    public static ScaleFactor GetCurrentScaleFactor()
    {
        return GetScaleFactorFromWindow(User32.GetForegroundWindow());
    }

    public static ScaleFactor GetScaleFactorFromWindow(IntPtr hwnd)
    {
        var dpiX = DefaultDpi;
        var dpiY = DefaultDpi;

        try
        {
            if (Environment.OSVersion.Version > new Version(6, 2)) // Windows 8.1 = 6.3.9200
            {
                var hMonitor = MonitorFromWindow(hwnd, MonitorDefaults.TOPRIMARY);
                GetDpiForMonitor(hMonitor, MonitorDpiType.EFFECTIVE_DPI, out dpiX, out dpiY);
            }
            else
            {
                var g = Graphics.FromHwnd(IntPtr.Zero);
                var desktop = g.GetHdc();

                dpiX = GetDeviceCaps(desktop, DeviceCap.LOGPIXELSX);
                dpiY = GetDeviceCaps(desktop, DeviceCap.LOGPIXELSY);
            }
        }
        catch (Exception e)
        {
            ProcessHelper.WriteLog(e.ToString());
        }

        return new ScaleFactor { Horizontal = (float)dpiX / DefaultDpi, Vertical = (float)dpiY / DefaultDpi };
    }

    public static string GetMonitorColorProfileFromWindow(Window window)
    {
        var hMonitor = MonitorFromWindow(new WindowInteropHelper(window).EnsureHandle(), MonitorDefaults.TONEAREST);
        return GetMonitorColorProfile(hMonitor);
    }

    public static string GetMonitorColorProfile(IntPtr hMonitor)
    {
        var profileDir = new StringBuilder(255);
        var pDirSize = (uint)profileDir.Capacity;
        GetColorDirectory(null, profileDir, ref pDirSize);

        var mInfo = new MONITORINFOEX();
        mInfo.cbSize = (uint)Marshal.SizeOf(mInfo);
        if (!GetMonitorInfo(hMonitor, ref mInfo))
            return null;

        var dd = new DISPLAYDEVICE();
        dd.cb = (uint)Marshal.SizeOf(dd);
        if (!EnumDisplayDevices(mInfo.szDevice, 0, ref dd, 0))
            return null;

        WcsGetUsePerUserProfiles(dd.DeviceKey, CLASS_MONITOR, out bool usePerUserProfiles);
        var scope = usePerUserProfiles ? WcsProfileManagementScope.CURRENT_USER : WcsProfileManagementScope.SYSTEM_WIDE;

        if (!WcsGetDefaultColorProfileSize(scope, dd.DeviceKey, ColorProfileType.ICC, ColorProfileSubtype.NONE, 0, out uint size))
            return null;

        var profileName = new StringBuilder((int)size);
        if (!WcsGetDefaultColorProfile(scope, dd.DeviceKey, ColorProfileType.ICC, ColorProfileSubtype.NONE, 0, size, profileName))
            return null;
        return System.IO.Path.Combine(profileDir.ToString(), profileName.ToString());
    }

    [DllImport("shcore.dll")]
    private static extern uint
        GetDpiForMonitor(IntPtr hMonitor, MonitorDpiType dpiType, out int dpiX, out int dpiY);

    private enum MonitorDpiType
    {
        EFFECTIVE_DPI = 0,
        ANGULAR_DPI = 1,
        RAW_DPI = 2
    }

    public struct ScaleFactor
    {
        public float Horizontal;
        public float Vertical;
    }
}
