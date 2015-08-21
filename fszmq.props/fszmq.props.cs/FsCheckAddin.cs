namespace fszmq.props
{
  using NUnit.Core.Extensibility;

  using FsCheck.NUnit;
  using FsCheck.NUnit.Addin;

  /// <summary>
  /// A simpler helper which allows the NUnit runner to find tests tagged with FsCheck.NUnit.PropertyAttribute
  /// </summary>
  [NUnitAddin (Description = "FsCheck addin")]
  public class FsCheckNunitAddin : IAddin 
  {
    public bool Install(IExtensionHost host) 
    {
      var tcBuilder = new FsCheckTestCaseBuilder();
      host.GetExtensionPoint("TestCaseBuilders").Install(tcBuilder);
      return true;
    }
  }
}
