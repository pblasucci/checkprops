Imports CheckProperty = FsCheck.NUnit.PropertyAttribute

Namespace fszmq.props

  <TestFixture()> _
  Public Class Generation

    <Test()>
    Public Sub PrintVersionDistribution ()
      For Each pair in Distributions.Versions(25)
        Dim version = pair.Key
        Dim count   = pair.Value
        Console.WriteLine("{0:00},{1}", count, version)
      Next
    End Sub

    <CheckProperty()>
    Public Function EncodeDecodeAreDuals_Failing (ByVal data() As Byte) As Boolean
      Dim processed = Z85.Decode(Z85.Encode(data))
      Return processed.SequenceEqual(data)
    End Function

     <CheckProperty(Arbitrary := { GetType(Generators) })>
    Public Function EncodeDecodeAreDuals_Passing (ByVal input As Mod4Binary) As Boolean
      Dim processed = Z85.Decode(Z85.Encode(input.Data))
      Return processed.SequenceEqual(input.Data)
    End Function

  End Class

End Namespace
