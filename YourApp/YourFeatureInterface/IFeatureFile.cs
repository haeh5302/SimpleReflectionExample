namespace YourFeatureInterface
{
  public interface IFeatureFile
  {
    string FileName { get; }
    byte[] Content { get; }
  }
}
