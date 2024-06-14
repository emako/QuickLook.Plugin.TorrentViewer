// Copyright © 2024 ema
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
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Windows;
using System.Windows.Resources;

namespace QuickLook.Common.Helpers;

public static class ResourceHelper
{
    static ResourceHelper()
    {
        if (!UriParser.IsKnownScheme("pack"))
            _ = PackUriHelper.UriSchemePack;
    }

    public static bool HasResource(string uriString)
    {
        try
        {
            using Stream stream = GetStream(uriString);
            _ = stream;
            return true;
        }
        catch
        {
        }
        return false;
    }

    public static byte[] GetBytes(string uriString)
    {
        Uri uri = new(uriString);
        StreamResourceInfo info = Application.GetResourceStream(uri);
        using BinaryReader stream = new(info.Stream);
        return stream.ReadBytes((int)info.Stream.Length);
    }

    public static Stream GetStream(string uriString)
    {
        Uri uri = new(uriString);
        StreamResourceInfo info = Application.GetResourceStream(uri);
        return info?.Stream!;
    }

    public static string GetString(string uriString, Encoding encoding = null!)
    {
        Uri uri = new(uriString);
        StreamResourceInfo info = Application.GetResourceStream(uri);
        using StreamReader stream = new(info.Stream, encoding ?? Encoding.UTF8);
        return stream.ReadToEnd();
    }
}
