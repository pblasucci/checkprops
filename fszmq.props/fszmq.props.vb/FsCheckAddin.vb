Namespace fszmq.props

  ''' <summary>
  ''' A simpler helper which allows the NUnit runner to find tests tagged with FsCheck.NUnit.PropertyAttribute
  ''' </summary>
  <NUnitAddin (Description := "FsCheck addin")>
  Public Class FsCheckAddin
    Implements IAddin
    
    Public Function Install (ByVal host As IExtensionHost) As Boolean Implements IAddin.Install
    
      Dim tcBuilder As New FsCheckTestCaseBuilder()
      host.GetExtensionPoint("TestCaseBuilders").Install(tcBuilder)
      Return True
    
    End Function

  End Class

End Namespace
