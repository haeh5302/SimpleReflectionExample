using System;
using Vendor.DocReader;
using Microsoft.Extensions.Logging;
using YourFeatureInterface;

namespace YourVendorWrapper
{
  public class YourFileLoader : IFeatureFileLoader
  {
    protected ILogger<YourFileLoader> Logger;
    public YourFileLoader(ILogger<YourFileLoader> logger)
    {
      this.Logger = logger;
    }

    public bool TryGetDoc(string path, out IFeatureFile doc)
    {
      try
      {
        VendorDoc vendorDoc = VendorDocReaderFactory.Load(path);
        YourFileWrapper wrapper = new YourFileWrapper(vendorDoc);
        doc = wrapper;
        return true;
      }
      catch (Exception ex)
      {
        this.Logger.LogError(ex, "Failed to load document from path: {0}", path);
        doc = null;
        return false;
      }
    }
  }
}
