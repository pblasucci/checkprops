Imports FsCheck.PropertyExtensions
Imports CheckProperty = FsCheck.NUnit.PropertyAttribute

Namespace fszmq.props

  <Extension()>
  Public Module Classifiers
  
    <Extension()>
    Function Within (ByVal item  As Integer, 
                     ByVal min   As Integer, 
                     ByVal max   As Integer) As Boolean
      Return (item >= min) AndAlso (item <= max)
    End Function
    
    <Extension()>
    Function IsTiny(ByVal input As Mod4Binary) As Boolean
      Return input.Data.Length.Within(Integer.MinValue,4)
    End Function
    
    <Extension()>
    Function IsSmall(ByVal input As Mod4Binary) As Boolean
      Return input.Data.Length.Within(8,20)
    End Function
    
    <Extension()>
    Function IsMedium(ByVal input As Mod4Binary) As Boolean
      Return input.Data.Length.Within(24,60)
    End Function

    <Extension()>
    Function IsLarge(ByVal input As Mod4Binary) As Boolean
      Return input.Data.Length.Within(64,Integer.MaxValue)
    End Function

  End Module

  <TestFixture()> _
  Public Class Observations

    <CheckProperty(Arbitrary := { GetType(Generators) })>
    Public Function EncodeDecodeAreDuals_Trivial (ByVal input As Mod4Binary) As [Property]
      Dim processed = Z85.Decode(Z85.Encode(input.Data))
      ' identify a special case
      Return processed.SequenceEqual(input.Data) _
                      .Trivial(input.Data.Any(Function (b) b = 70))
    End Function

    <CheckProperty(Arbitrary := { GetType(Generators) })>
    Public Function EncodeDecodeAreDuals_Classified (ByVal input As Mod4Binary) As [Property]
      Dim processed = Z85.Decode(Z85.Encode(input.Data))
      ' bucket by common sizes
      Return processed.SequenceEqual(input.Data)                              _
                      .Classify(input.IsTiny  (), "Tiny (0 .. 4 bytes)"    )  _
                      .Classify(input.IsSmall (), "Small (8 .. 20 bytes)"  )  _
                      .Classify(input.IsMedium(), "Medium (24 .. 60 bytes)")  _
                      .Classify(input.IsLarge (), "Large (64 .. ∞ bytes)"  )
    End Function

    <CheckProperty(Arbitrary := { GetType(Generators) })>
    Public Function EncodeDecodeAreDuals_Collected (ByVal input As Mod4Binary) As [Property]
      Dim processed = Z85.Decode(Z85.Encode(input.Data))
      ' gather individual sizes
      Return processed.SequenceEqual(input.Data) _
                      .Collect(input.Data.Length)
    End Function

    <CheckProperty(Arbitrary := { GetType(Generators) })>
    Public Function EncodeDecodeAreDuals_Combined (ByVal input As Mod4Binary) As [Property]
      Dim processed = Z85.Decode(Z85.Encode(input.Data))
      ' identify a special case, bucket by common sizes, and gather individual sizes
      Return processed.SequenceEqual(input.Data)                              _
                      .Trivial  (input.Data.Any(Function (b) b = 70))         _
                      .Classify (input.IsTiny  (), "Tiny (0 .. 4 bytes)"    ) _
                      .Classify (input.IsSmall (), "Small (8 .. 20 bytes)"  ) _
                      .Classify (input.IsMedium(), "Medium (24 .. 60 bytes)") _
                      .Classify (input.IsLarge (), "Large (64 .. ∞ bytes)"  ) _
                      .Collect  (input.Data.Length)  
    End Function

  End Class

End Namespace
