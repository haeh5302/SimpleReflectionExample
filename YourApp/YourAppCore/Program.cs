using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourFeatureInterface;

namespace YourAppCore
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
      var logger = loggerFactory.CreateLogger<MyAssemblyProvider>();

      string relativePath = "../../../../YourVendorWrapper/bin/Debug/net480/YourVendorWrapper.dll";
      string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath); // INI SETTING ME
      MyAssemblyProvider myAssemblyProvider = new MyAssemblyProvider(logger, path);


      IFeatureFileLoader vendorImplementation = myAssemblyProvider.CreateInstance<IFeatureFileLoader>(true);

      IFeatureFile file = null;
      if (myAssemblyProvider.IsLoaded)
      {
        string name = "myrandomdoc.txt"; // PASS ME IN
        string nameAbs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);

        if (!File.Exists(nameAbs))
        {
          logger.LogError("File not found: {0}", nameAbs);
          return;
        }

        vendorImplementation.TryGetDoc(nameAbs, out file);
      }

      if (file != null)
      {
        Console.WriteLine("File name: {0}", file.FileName);
        Console.WriteLine("File content: {0}", Encoding.UTF8.GetString(file.Content));
      }

    }
  }
}
