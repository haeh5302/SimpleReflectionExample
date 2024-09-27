using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using YourFeatureInterface;

namespace YourAppCore
{
  public class MyAssemblyProvider
  {
    protected ILogger<MyAssemblyProvider> Logger;
    public MyAssemblyProvider(ILogger<MyAssemblyProvider> logger)
    {
      this.Logger = logger;
    }

    public static bool IsLoaded { get; protected set; } = false;
    public static IFeatureFileLoader FileLoader { get; protected set; } = null!;
    public IFeatureFileLoader GetLoader(string assemblyPath)
    {
      if (IsLoaded)
        return FileLoader;

      try
      {
        string normalized = Path.GetFullPath(assemblyPath);
        if(string.IsNullOrWhiteSpace(normalized) || !File.Exists(normalized))
        {
          Logger.LogError("File not found: {0}", normalized);
          return null!;
        }

        Assembly assembly = Assembly.Load(normalized);
        Type loaderType = null!;
        foreach (Type type in assembly.GetTypes())
        {
          if (typeof(IFeatureFileLoader).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
          {
            loaderType = type;
            break;
          }
        }
        if (loaderType != null)
          FileLoader = (IFeatureFileLoader)Activator.CreateInstance(loaderType)!;


        if (FileLoader == null)
        {
          Logger.LogError("No class implementing IFooBar found in the assembly.");
          return null!;
        }

        IsLoaded = true;
        return FileLoader;

      }
      catch (Exception ex)
      {
        Logger.LogError(ex, "No class implementing IFooBar found in the assembly.");
        return null!;
      }
    }
  }
}
