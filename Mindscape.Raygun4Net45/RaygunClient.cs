using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace Mindscape.Raygun4Net
{
  public class RaygunClient : RaygunClientBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RaygunClient" /> class.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    public RaygunClient(string apiKey) : base(apiKey)
    {
      AddWrapperExceptions(typeof(HttpUnhandledException));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RaygunClient" /> class.
    /// Uses the ApiKey specified in the config file.
    /// </summary>
    public RaygunClient()
      : this(RaygunSettings.Settings.ApiKey)
    {
    }

    public void Send(AggregateException aggregateException)
    {
      Send(aggregateException, null, null);
    }

    public void Send(AggregateException aggregateException, IList<string> tags)
    {
      Send(aggregateException, tags, null);
    }

    public void Send(AggregateException aggregateException, IList<string> tags, IDictionary userCustomData)
    {
      foreach (var exception in aggregateException.InnerExceptions)
      {
        Send(exception, tags, userCustomData);
      }
    }

    public void SendInBackground(AggregateException aggregateException)
    {
      SendInBackground(aggregateException, null, null);
    }

    public void SendInBackground(AggregateException aggregateException, IList<string> tags)
    {
      SendInBackground(aggregateException, tags, null);
    }

    public void SendInBackground(AggregateException aggregateException, IList<string> tags, IDictionary userCustomData)
    {
      foreach (var exception in aggregateException.InnerExceptions)
      {
        SendInBackground(exception, tags, userCustomData);
      }
    }

    protected override IRaygunMessageBuilder BuildMessageCore()
    {
      return RaygunMessageBuilder.New
        .SetHttpDetails(HttpContext.Current, _requestMessageOptions);
    }
  }
}
