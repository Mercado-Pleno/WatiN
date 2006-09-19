#region WatiN Copyright (C) 2006 Jeroen van Menen

// WatiN (Web Application Testing In dotNet)
// Copyright (C) 2006 Jeroen van Menen
//
// This library is free software; you can redistribute it and/or modify it under the terms of the GNU 
// Lesser General Public License as published by the Free Software Foundation; either version 2.1 of 
// the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this library; 
// if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 
// 02111-1307 USA 

#endregion Copyright

using System;
using NUnit.Framework;

using WatiN.Core;

namespace WatiN.UnitTests
{
  [TestFixture]
  public class Utils : WatiNTest
  {
    [Test]
    public void DumpElements()
    {
      using (IE ie = new IE(HtmlTestBaseURI + "main.html"))
      {
        UtilityClass.DumpElements(ie);
      }
    }

    [Test]
    public void DumpElementsElab()
    {
      using (IE ie = new IE(HtmlTestBaseURI + "Frameset.html"))
      {
        UtilityClass.DumpElementsWithHtmlSource(ie);
      }
    }
    
    [Test]
    public void IsNullOrEmpty()
    {
      Assert.IsTrue(UtilityClass.IsNullOrEmpty(null), "null should return true");
      Assert.IsTrue(UtilityClass.IsNullOrEmpty(String.Empty), "Empty should return true");
      Assert.IsTrue(UtilityClass.IsNullOrEmpty(""), "zero length string should return true");
      Assert.IsFalse(UtilityClass.IsNullOrEmpty("test"), "string 'test' should return false");
    }
    
    [Test]
    public void IsNotNullOrEmpty()
    {
      Assert.IsFalse(UtilityClass.IsNotNullOrEmpty(null), "null should return false");
      Assert.IsFalse(UtilityClass.IsNotNullOrEmpty(String.Empty), "Empty should return false");
      Assert.IsFalse(UtilityClass.IsNotNullOrEmpty(""), "zero length string should return false");
      Assert.IsTrue(UtilityClass.IsNotNullOrEmpty("test"), "string 'test' should return true");
    }
  }
}