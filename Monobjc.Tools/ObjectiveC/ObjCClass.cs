﻿//
// This file is part of Monobjc, a .NET/Objective-C bridge
// Copyright (C) 2007-2014 - Laurent Etiemble
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

namespace Monobjc.Tools.ObjectiveC
{
    /// <summary>
    ///   Simple wrapper for an Objective-C class.
    /// </summary>
    public class ObjCClass : ObjCId
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "ObjCClass" /> class.
        /// </summary>
        /// <param name = "pointer">The pointer.</param>
        public ObjCClass(IntPtr pointer) : base(pointer) {}

        /// <summary>
        ///   Gets the specified name.
        /// </summary>
        /// <param name = "name">The name.</param>
        /// <returns></returns>
        public static ObjCClass Get(String name)
        {
            return new ObjCClass(NativeMethods.objc_getClass(name));
        }
    }
}