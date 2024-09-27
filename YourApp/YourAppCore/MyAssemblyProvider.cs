using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using YourFeatureInterface;

namespace YourAppCore
{
  public class MyAssemblyProvider
  {
    protected ILogger<MyAssemblyProvider> logger;

    protected string assemblyPath;

    protected Assembly assemblyRef;
    public bool IsLoaded { get; protected set; } = false;
    public MyAssemblyProvider(ILogger<MyAssemblyProvider> logger, string assemblyPath)
    {
      this.logger = logger;
      this.IsLoaded = Init(assemblyPath);
    }
    private bool Init(string assemblyPath)
    {
      if (string.IsNullOrWhiteSpace(assemblyPath))
      {
        this.logger.LogError("No path provied.");
        return false;
      }

      string normalizedPath = Path.GetFullPath(assemblyPath);

      if (string.IsNullOrWhiteSpace(normalizedPath) || !File.Exists(normalizedPath))
      {
        this.logger.LogError("File not found: {0}", normalizedPath);
        return false;
      }

      try
      {
        string normalizedDir = Path.GetDirectoryName(normalizedPath);

        // Must define this before attempting to load the assembly
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
          // Construct the path to the dependency
          string fileName = Path.GetFileName(args.Name);
          string dependencyPath = Path.Combine(normalizedDir, fileName);

          // Load the dependency if it exists
          if (File.Exists(dependencyPath))
          {
            return Assembly.LoadFrom(dependencyPath);
          }

          return null;
        };

        Assembly assembly = Assembly.Load(normalizedPath);
        this.assemblyPath = normalizedPath;
        this.assemblyRef = assembly;
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Cannot load assembly {0}.", normalizedPath);
      }
      return true;
    }

    public T CreateInstance<T>(bool withLogging)
    {
      Type requestedType = typeof(T);
      T retVal = default(T);

      if (!IsLoaded || !requestedType.IsInterface)
        return retVal;

      try
      {

        Type loadedType = null;

        foreach (Type type in assemblyRef.GetTypes())
        {
          if (typeof(IFeatureFileLoader).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
          {
            loadedType = type;
            break;
          }
        }

        if (loadedType == null)
        {
          this.logger.LogError("No class implementing requested {0} found in the assembly.", requestedType.Name);
          return retVal;

        }
        ConstructorInfo constructor = null;
        object[] parameters = null;

        if (withLogging)
        {
          var loggerFactory = LoggerFactory.Create(builder => { });
          var logger = loggerFactory.CreateLogger<T>();

          constructor = loadedType.GetConstructor(new Type[] { typeof(ILogger<T>) });
          parameters = new object[] { logger };
        }
        else
        {
          constructor = loadedType.GetConstructor(new Type[] { });
          parameters = new object[] { };
        }

        retVal = (T)constructor.Invoke(parameters);

        if (retVal == null)
          this.logger.LogError("Unexpected issue loading {0} found in the assembly.", loadedType.Name);

        return retVal;
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Unexpected issue requesting {0} from the assembly.", requestedType.Name);
        return retVal;
      }
    }
  }
}
