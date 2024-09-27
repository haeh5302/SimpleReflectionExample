namespace YourFeatureInterface
{
  public interface IFeatureFileLoader
  {
    bool TryGetDoc(string path, out IFeatureFile doc);
  }
}
