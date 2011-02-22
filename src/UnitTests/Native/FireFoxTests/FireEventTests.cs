﻿using System;
using System.Collections.Specialized;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using WatiN.Core.Native;

namespace WatiN.Core.UnitTests.Native.FireFoxTests
{
    [TestFixture]
    public class FireEventTests
    {
        [Test]
        public void Should_create_javascript_code_to_call_mouse_event_with_the_defaults()
        {
            // GIVEN
            var creator = new JSEventCreator("test", new ClientPortMock());

            // WHEN
            var command = creator.CreateEvent("onmousedown", new NameValueCollection{},true );

            // THEN
            Assert.That(command, Is.Not.Null, "Expected code");
            Assert.That(command, Is.EqualTo("var event = test.ownerDocument.createEvent(\"MouseEvents\");event.initMouseEvent('mousedown',true,true,null,0,0,0,0,0,false,false,false,false,0,null);var res = test.dispatchEvent(event); if(res){true;}else{false;};"), "Unexpected method signature");
        }

        [Test]
        public void Should_create_javascript_code_to_call_click_event_with_the_defaults()
        {
            // GIVEN
            var creator = new JSEventCreator("test", new ClientPortMock());

            // WHEN
            var command = creator.CreateEvent("click", new NameValueCollection{},true );

            // THEN
            Assert.That(command, Is.Not.Null, "Expected code");
            Assert.That(command, Is.EqualTo("var event = test.ownerDocument.createEvent(\"MouseEvents\");event.initMouseEvent('click',true,true,null,0,0,0,0,0,false,false,false,false,0,null);var res = test.dispatchEvent(event); if(res){true;}else{false;};"), "Unexpected method signature");
        }

        [Test]
        public void Should_create_mouse_event_with_params_set()
        {
            // GIVEN
            var eventParams = new NameValueCollection
                {
                    {"type", "type"},
                    {"bubbles", "bubbles"},
                    {"cancelable", "cancelable"},
                    {"windowObject", "windowObject"},
                    {"detail", "detail"},
                    {"screenX", "screenX"},
                    {"screenY", "screenY"},
                    {"clientX", "clientX"},
                    {"clientY", "clientY"},
                    {"ctrlKey", "ctrlKey"},
                    {"altKey", "altKey"},
                    {"shiftKey", "shiftKey"},
                    {"metaKey", "metaKey"},
                    {"button", "button"},
                    {"relatedTarget", "relatedTarget"}
                };

            var creator = new JSEventCreator("test", new ClientPortMock());

            // WHEN
            var command = creator.CreateMouseEventCommand("mousedown", eventParams);

            // THEN
            Assert.That(command, Is.Not.Null, "Expected code");
            Assert.That(command, Is.EqualTo("var event = test.ownerDocument.createEvent(\"MouseEvents\");event.initMouseEvent('mousedown',bubbles,cancelable,windowObject,detail,screenX,screenY,clientX,clientY,ctrlKey,altKey,shiftKey,metaKey,button,relatedTarget);"), "Unexpected method signature");
        }

        [Test]
        public void Should_create_javascript_code_to_call_key_event_with_the_defaults()
        {
            // GIVEN
            var creator = new JSEventCreator("test", new ClientPortMock(JavaScriptEngineType.Mozilla));

            // WHEN
            var command = creator.CreateEvent("onkeydown", new NameValueCollection { }, true);

            // THEN
            Assert.That(command, Is.Not.Null, "Expected code");
            Assert.That(command, Is.EqualTo("var event = test.ownerDocument.createEvent(\"KeyboardEvent\");event.initKeyEvent('keydown',true,true,null,false,false,false,false,0,0);var res = test.dispatchEvent(event); if(res){true;}else{false;};"), "Unexpected method signature");
        }

        [Test]
        public void Should_create_key_event_with_params_set()
        {
            // GIVEN
            var eventParams = new NameValueCollection
                {
                    {"type", "type"},
                    {"bubbles", "bubbles"},
                    {"cancelable", "cancelable"},
                    {"windowObject", "windowObject"},
                    {"ctrlKey", "ctrlKey"},
                    {"altKey", "altKey"},
                    {"shiftKey", "shiftKey"},
                    {"metaKey", "metaKey"},
                    {"keyCode", "keyCode"},
                    {"charCode", "charCode"}
                };

            var creator = new JSEventCreator("test", new ClientPortMock());

            // WHEN
            var command = creator.CreateKeyboardEventForMozilla("keydown", eventParams);

            // THEN
            Assert.That(command, Is.Not.Null, "Expected code");
            Assert.That(command, Is.EqualTo("var event = test.ownerDocument.createEvent(\"KeyboardEvent\");event.initKeyEvent('keydown',bubbles,cancelable,windowObject,ctrlKey,altKey,shiftKey,metaKey,keyCode,charCode);"), "Unexpected method signature");
        }
    }

    public class ClientPortMock : ClientPortBase
    {
        private readonly JavaScriptEngineType _javaScriptEngineType;

        public ClientPortMock(){}

        public ClientPortMock(JavaScriptEngineType javaScriptEngineType)
        {
            _javaScriptEngineType = javaScriptEngineType;
        }

        #region Overrides of ClientPortBase

        public override string DocumentVariableName
        {
            get { throw new NotImplementedException(); }
        }

        public override JavaScriptEngineType JavaScriptEngine
        {
            get { return _javaScriptEngineType; }
        }

        public override string BrowserVariableName
        {
            get { throw new NotImplementedException(); }
        }

        public override void InitializeDocument()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void Connect(string url)
        {
            throw new NotImplementedException();
        }

        protected override void SendAndRead(string data, bool resultExpected, bool checkForErrors, params object[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
