﻿using Mindscape.Raygun4Net.Messages;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mindscape.Raygun4Net.WindowsStore.Tests
{
  [TestFixture]
  public class RaygunClientTests
  {
    private FakeRaygunClient _client;
    private Exception _exception = new NullReferenceException("The thing is null");

    [SetUp]
    public void SetUp()
    {
      _client = new FakeRaygunClient();
    }

    // Cancel send tests

    [Test]
    public void NoHandlerSendsAll()
    {
      Assert.IsTrue(_client.ExposeOnSendingMessage(_client.ExposeBuildMessage(_exception)));
    }

    [Test]
    public void HandlerIsChecked()
    {
      bool filterCalled = false;
      _client.SendingMessage += (object o, RaygunSendingMessageEventArgs e) =>
      {
        Assert.AreEqual("NullReferenceException: The thing is null", e.Message.Details.Error.Message);
        filterCalled = true;
        e.Cancel = true;
      };
      Assert.IsFalse(_client.ExposeOnSendingMessage(_client.ExposeBuildMessage(_exception)));
      Assert.IsTrue(filterCalled);
    }

    [Test]
    public void HandlerCanAllowSend()
    {
      _client.SendingMessage += (object o, RaygunSendingMessageEventArgs e) =>
      {
        // Allow send by not setting e.Cancel
      };
      Assert.IsTrue(_client.ExposeOnSendingMessage(_client.ExposeBuildMessage(_exception)));
    }

    [Test]
    public void AllHandlersAreChecked()
    {
      bool filter1Called = false;
      bool filter2Called = false;
      _client.SendingMessage += (object o, RaygunSendingMessageEventArgs e) =>
      {
        Assert.AreEqual("NullReferenceException: The thing is null", e.Message.Details.Error.Message);
        filter1Called = true;
        e.Cancel = true;
      };
      _client.SendingMessage += (object o, RaygunSendingMessageEventArgs e) =>
      {
        Assert.AreEqual("NullReferenceException: The thing is null", e.Message.Details.Error.Message);
        filter2Called = true;
        e.Cancel = true;
      };
      Assert.IsFalse(_client.ExposeOnSendingMessage(_client.ExposeBuildMessage(_exception)));
      Assert.IsTrue(filter1Called);
      Assert.IsTrue(filter2Called);
    }

    [Test]
    public void DontSendIfFirstHandlerCancels()
    {
      _client.SendingMessage += (object o, RaygunSendingMessageEventArgs e) =>
      {
        e.Cancel = true;
      };
      _client.SendingMessage += (object o, RaygunSendingMessageEventArgs e) =>
      {
        // Allow send by not setting e.Cancel
      };
      Assert.IsFalse(_client.ExposeOnSendingMessage(_client.ExposeBuildMessage(_exception)));
    }

    [Test]
    public void DontSendIfSecondHandlerCancels()
    {
      _client.SendingMessage += (object o, RaygunSendingMessageEventArgs e) =>
      {
        // Allow send by not setting e.Cancel
      };
      _client.SendingMessage += (object o, RaygunSendingMessageEventArgs e) =>
      {
        e.Cancel = true;
      };
      Assert.IsFalse(_client.ExposeOnSendingMessage(_client.ExposeBuildMessage(_exception)));
    }

    [Test]
    public void AllowSendIfNoHandlerCancels()
    {
      _client.SendingMessage += (object o, RaygunSendingMessageEventArgs e) =>
      {
        // Allow send by not setting e.Cancel
      };
      _client.SendingMessage += (object o, RaygunSendingMessageEventArgs e) =>
      {
        // Allow send by not setting e.Cancel
      };
      Assert.IsTrue(_client.ExposeOnSendingMessage(_client.ExposeBuildMessage(_exception)));
    }

    [Test]
    public void HandlerCanModifyMessage()
    {
      RaygunMessage message = _client.ExposeBuildMessage(_exception);
      Assert.AreEqual("NullReferenceException: The thing is null", message.Details.Error.Message);

      _client.SendingMessage += (object o, RaygunSendingMessageEventArgs e) =>
      {
        e.Message.Details.Error.Message = "Custom error message";
      };

      Assert.IsTrue(_client.ExposeOnSendingMessage(message));
      Assert.AreEqual("Custom error message", message.Details.Error.Message);
    }
  }
}
