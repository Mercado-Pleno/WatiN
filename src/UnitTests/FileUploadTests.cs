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
using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace WatiN.Core.UnitTests
{
	[TestFixture]
	public class FileUploadTests : BaseWithBrowserTests
	{
		[Test]
		public void FileUploadElementTags()
		{
			Assert.AreEqual(1, FileUpload.ElementTags.Count, "1 elementtags expected");
			Assert.AreEqual("input", ((ElementTag) FileUpload.ElementTags[0]).TagName);
			Assert.AreEqual("file", ((ElementTag) FileUpload.ElementTags[0]).InputTypes);
		}

		[Test]
		public void CreateFileUploadFromElement()
		{
			var element = Ie.Element("upload");
			var fileUpload = new FileUpload(element);
			Assert.AreEqual("upload", fileUpload.Id);
		}

		[Test]
		public void FileUploadExists()
		{
			Assert.IsTrue(Ie.FileUpload("upload").Exists);
			Assert.IsTrue(Ie.FileUpload(new Regex("upload")).Exists);
			Assert.IsFalse(Ie.FileUpload("noneexistingupload").Exists);
		}

		[Test]
		public void FileUploadTest()
		{
			var fileUpload = Ie.FileUpload("upload");

			Assert.That(fileUpload.Exists);
			Assert.IsNull(fileUpload.FileName);

			fileUpload.Set(MainURI.LocalPath);

			Assert.AreEqual(MainURI.LocalPath, fileUpload.FileName);
		}

		[Test, ExpectedException(typeof (System.IO.FileNotFoundException))]
		public void FileUploadFileNotFoundException()
		{
			var fileUpload = Ie.FileUpload("upload");
			fileUpload.Set("nonexistingfile.nef");
		}

		[Test]
		public void FileUploads()
		{
			const int expectedFileUploadsCount = 1;
			Assert.AreEqual(expectedFileUploadsCount, Ie.FileUploads.Length, "Unexpected number of FileUploads");

			// Collection.Length
			var formFileUploads = Ie.FileUploads;

			// Collection items by index
			Assert.AreEqual("upload", Ie.FileUploads[0].Id);

			IEnumerable FileUploadEnumerable = formFileUploads;
			var FileUploadEnumerator = FileUploadEnumerable.GetEnumerator();

			// Collection iteration and comparing the result with Enumerator
			var count = 0;
			foreach (FileUpload inputFileUpload in formFileUploads)
			{
				FileUploadEnumerator.MoveNext();
				var enumFileUpload = FileUploadEnumerator.Current;

				Assert.IsInstanceOfType(inputFileUpload.GetType(), enumFileUpload, "Types are not the same");
				Assert.AreEqual(inputFileUpload.OuterHtml, ((FileUpload) enumFileUpload).OuterHtml, "foreach and IEnumator don't act the same.");
				++count;
			}

			Assert.IsFalse(FileUploadEnumerator.MoveNext(), "Expected last item");
			Assert.AreEqual(expectedFileUploadsCount, count);
		}

		[Test]
		public void FileUploadOfFileWithSpecialCharactersInFilenameShouldBeHandledOK()
		{
            // GIVEN
			Ie.Refresh();

			var fileUpload = Ie.FileUpload("upload");

			Assert.That(fileUpload.Exists, "Pre-Condition: Expected file upload element");
			Assert.That(fileUpload.FileName, Is.Null, "pre-Condition: Expected empty filename");

			var file = new Uri(HtmlTestBaseURI, @"~^+{}[].txt").LocalPath;

            // WHEN
            fileUpload.Set(file);

            // THEN
			Assert.That(fileUpload.FileName, Is.EqualTo(file), "Unexpected filename");
		}

		public override Uri TestPageUri
		{
			get { return MainURI; }
		}
	}
}