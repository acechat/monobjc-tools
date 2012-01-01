﻿//
// This file is part of Monobjc, a .NET/Objective-C bridge
// Copyright (C) 2007-2012 - Laurent Etiemble
//
// Monobjc is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// Monobjc is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Monobjc.  If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Globalization;
using Monobjc.Tools.Utilities;

namespace Monobjc.Tools.External
{
    /// <summary>
    ///   Wrapper class around the <c>chmod</c> command line tool.
    /// </summary>
    public static class Chmod
    {
        /// <summary>
        ///   Applies new permissions to a file.
        /// </summary>
        /// <param name = "mask">The permission mask.</param>
        /// <param name = "file">The file.</param>
        /// <returns>The result of the command.</returns>
        public static String ApplyTo(String mask, String file)
        {
            String arguments = String.Format(CultureInfo.InvariantCulture, "{0} \"{1}\"", mask, file);
            ProcessHelper helper = new ProcessHelper(Executable, arguments);
            String output = helper.Execute();
            return output;
        }

        private static string Executable
        {
            get { return "chmod"; }
        }
    }
}