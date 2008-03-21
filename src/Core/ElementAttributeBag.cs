#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System;
using System.Globalization;
using mshtml;
using WatiN.Core.Constraints;
using WatiN.Core.InternetExplorer;
using StringComparer = WatiN.Core.Comparers.StringComparer;
using WatiN.Core.Interfaces;

namespace WatiN.Core
{
	/// <summary>
	/// Wrapper around the <see cref="mshtml.IHTMLElement"/> object. Used by <see cref="BaseConstraint.Compare"/>.
	/// </summary>
	public class ElementAttributeBag : IAttributeBag
	{
		private IHTMLElement _htmlElement = null;
		private Element _element = null;
		private Element _elementTyped;
	    private DomContainer _domContainer;

	    public ElementAttributeBag() {}

        public ElementAttributeBag(DomContainer domContainer)
        {
            _domContainer = domContainer;
        }

		public ElementAttributeBag(IHTMLElement element)
		{
			IHTMLElement = element;
		}

		/// <summary>
		/// Gets or sets the IHTMLelement from which the attribute values are read.
		/// </summary>
		/// <value>The IHTMLelement.</value>
		public IHTMLElement IHTMLElement
		{
			get { return _htmlElement; }
			set { _htmlElement = value; }
		}

        public DomContainer DomContainer
        {
            get { return _domContainer; }
            set { _domContainer = value; }
        }
        
        /// <summary>
		/// Returns a typed Element instance that can be casted to an ElementsContainer.
		/// </summary>
		/// <value>The element.</value>
		public Element Element
		{
			get
			{
				if (_element == null)
				{
					_element = new ElementsContainer(DomContainer, IHTMLElement);
				}

				return _element;
			}
		}

		/// <summary>
		/// Returns a typed Element instance that can be casted to the specific WatiN type.
		/// </summary>
		/// <value>The element typed.</value>
		public Element ElementTyped
		{
			get
			{
				if (_elementTyped == null)
				{
					_elementTyped = Element.GetTypedElement(DomContainer, new IEElement(IHTMLElement));
				}

				return _elementTyped;
			}
		}

	    /// <summary>
		/// Gets the value for the given <paramref name="attributename" />
		/// </summary>
		/// <param name="attributename">The attributename.</param>
		/// <returns>The value of the attribute</returns>
		public string GetValue(string attributename)
		{
			if (StringComparer.AreEqual(attributename, "style", true))
			{
				return _htmlElement.style.cssText;
			}

			object attributeValue;

			if (attributename.ToLower(CultureInfo.InvariantCulture).StartsWith("style."))
			{
				attributeValue = Style.GetAttributeValue(attributename.Substring(6), _htmlElement.style);
			}
			else
			{
				attributeValue = _htmlElement.getAttribute(attributename, 0);
			}

			if (attributeValue == DBNull.Value)
			{
				return null;
			}

			if (attributeValue == null)
			{
				return null;
			}

			return attributeValue.ToString();
		}
	}
}
