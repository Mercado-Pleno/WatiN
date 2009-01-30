#region WatiN Copyright (C) 2006-2009 Jeroen van Menen

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

using System.Collections.Generic;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core
{
	/// <summary>
	/// This class provides specialized functionality for a HTML Form element.
	/// </summary>
	public class Form : ElementsContainer<Form>
	{
        private static List<ElementTag> elementTags;

        public static List<ElementTag> ElementTags
		{
			get
			{
				if (elementTags == null)
				{
                    elementTags = new List<ElementTag> { new ElementTag("form") };
				}

				return elementTags;
			}
		}

		public Form(DomContainer domContainer, INativeElement htmlFormElement) : base(domContainer, htmlFormElement) {}

		public Form(DomContainer domContainer, INativeElementFinder finder) : base(domContainer, finder) {}

		/// <summary>
		/// Initialises a new instance of the <see cref="Form"/> class based on <paramref name="element"/>.
		/// </summary>
		/// <param name="element">The element.</param>
		public Form(Element element) : base(element, ElementTags) {}

		public void Submit()
		{
			Logger.LogAction("Submitting " + GetType().Name + " '" + ToString() + "'");

		    NativeElement.SubmitForm();
			WaitForComplete();
		}

		public override string ToString()
		{
			if (UtilityClass.IsNotNullOrEmpty(Title))
			{
				return Title;
			}
			if (UtilityClass.IsNotNullOrEmpty(Id))
			{
				return Id;
			}

			return UtilityClass.IsNotNullOrEmpty(Name) ? Name : base.ToString();
		}

		public string Name
		{
			get { return GetAttributeValue("name"); }
		}

		internal new static Element New(DomContainer domContainer, INativeElement element)
		{
			return new Form(domContainer, element);
		}
	}
}
