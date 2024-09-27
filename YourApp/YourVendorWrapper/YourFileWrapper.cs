using Vendor.DocReader;
using YourFeatureInterface;

namespace YourVendorWrapper
{
  public class YourFileWrapper : IFeatureFile
  {
    public string FileName => this.doc.FilePath;
    public byte[] Content => this.doc.Content;

    protected VendorDoc doc;
    public YourFileWrapper(VendorDoc doc)
    {
      this.doc = doc;
    }
  }
}
