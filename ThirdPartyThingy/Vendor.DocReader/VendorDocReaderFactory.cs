using System.IO;

namespace Vendor.DocReader
{
  public static class VendorDocReaderFactory
  {
    // Load the file and return a VendorDoc object
    public static VendorDoc Load(string fileName)
    {
      if(!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
      {
        byte[] content = File.ReadAllBytes(fileName);
        return new VendorDoc(fileName, content);
      }
      return null;
    }
  }
}
