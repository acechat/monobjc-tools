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
using Monobjc.Tools.Utilities;
using NUnit.Framework;

namespace Monobjc.Tools
{
    [TestFixture]
    [Category("Signing")]
    public class NativeVersionExtractorTests
    {
        [Test]
        public void TestNativeVersionExtractor()
        {
            Version version1 = NativeVersionExtractor.GetVersion("/Developer/Applications/Xcode");
            Assert.IsNotNull(version1, "Version should be retrurned");
            Console.WriteLine("version=" + version1);

            Version version2 = NativeVersionExtractor.GetVersion("/Developer/Applications/Xcode.app");
            Assert.IsNotNull(version2, "Version should be retrurned");
            Console.WriteLine("version=" + version1);
            Assert.AreEqual(version1, version2);
        }
    }
}