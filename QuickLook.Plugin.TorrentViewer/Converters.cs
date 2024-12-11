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

using QuickLook.Common.ExtensionMethods;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QuickLook.Plugin.TorrentViewer;

public class SizePrettyPrintConverter : DependencyObject, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var size = (long)value;

        return size.ToPrettySize(2);
    }

    public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class FileExtToIconConverter : DependencyObject, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var name = (string)value;

        return IconManager.FindIconForFilename(name, false);
    }

    public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class AddConverter : DependencyObject, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double p1 && double.TryParse(parameter?.ToString(), out double p2))
        {
            return p1 + p2;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
