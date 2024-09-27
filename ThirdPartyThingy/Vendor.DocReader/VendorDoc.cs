namespace Vendor.DocReader
{
  public class VendorDoc
  {
    public string FilePath { get; set; }
    public byte[] Content { get; set; }

    internal VendorDoc(string filePath, byte[] content)
    {
      this.FilePath = filePath;
      this.Content = content;
    }
  }
}
