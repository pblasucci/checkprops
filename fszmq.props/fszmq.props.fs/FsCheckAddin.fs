namespace FsCheck.NUnit.Examples

open NUnit.Core.Extensibility

open FsCheck.NUnit
open FsCheck.NUnit.Addin

/// A simpler helper which allows the NUnit runner to find tests tagged with FsCheck.NUnit.PropertyAttribute
[<NUnitAddin(Description = "FsCheck addin")>]
type FsCheckAddin() =
  interface IAddin with
    override __.Install host =
      let tcBuilder = new FsCheckTestCaseBuilder()
      host.GetExtensionPoint("TestCaseBuilders").Install(tcBuilder)
      true
