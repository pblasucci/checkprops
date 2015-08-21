Imports FsCheck.GenExtensions

Imports GenMessage = FsCheck.Gen(Of fszmq.Message)
Imports GenVersion = FsCheck.Gen(Of fszmq.Version)

Namespace fszmq.props

  ''' <summary>
  ''' A byte[] whose length is evenly divisible by 4
  ''' </summary>
  Public NotInheritable Class Mod4Binary
    Private ReadOnly data_() As Byte

    Public Sub New (ByVal data() As Byte)
      ' Mod4Binary encapsulates the following domain rules
      If data Is Nothing OrElse data.Length Mod 4 <> 0 Then 
        Throw New ArgumentException("Not evenly divisble by 4", "data")
      End If
      Me.data_ = data
    End Sub

    Public ReadOnly Property Data As Byte()
      Get
        Return Me.data_
      End Get
    End Property

    Public Overrides Function Equals(obj As Object) As Boolean
      Dim that = TryCast(obj, Mod4Binary)
      If that Is Nothing Then Return False

      Return Me.Data.SequenceEqual(that.Data)
    End Function

    Public Overrides Function GetHashCode() As Integer
      Const seed = -1640531527
      Return seed XOr Me.Data.GetHashCode()
    End Function
   
    Public Overrides Function ToString() As String
      Dim buffer As New StringBuilder("Mod4Binary ")
      buffer.Append("[| ")
      Me.Data.Aggregate(buffer, Function (a,d) a.AppendFormat("{0:X2}; ", d))
      buffer.Append(" |]")
      Return buffer.ToString()
    End Function

  End Class

  Module Generators

    ''' <summary>
    ''' Allows FsCheck to randomly generate fszmq.Message instances
    ''' </summary>
    Function Message () As Arbitrary(Of Message)
      ' Generates an empty fszmq.Message
      Dim empty = Gen.Constant(new Message())

      ' Generates a fszmq.Message with random content
      Dim random = From data In Arb.Generate(Of NonNull(Of Byte()))()
                   Select New Message(data.Get)
      
      ' Creates a random distribution weighted in favor of messages with random content
      Dim distribution = Gen.Frequency(
          new WeightAndValue(Of GenMessage)(2, random),
          new WeightAndValue(Of GenMessage)(1, empty )
      )
      
      Return Arb.From(distribution)
    End Function

    ''' <summary>
    ''' Allows FsCheck to randomly generate fszmq.Version instances
    ''' </summary>
    Function Version () As Arbitrary(Of Version)
      ' Generates a version when the actual values are unknown
      Dim unknown = Gen.Constant(Of Version)(Global.fszmq.Version.Unknown)

      ' Generates a version from random known values
      Dim random = From data In Arb.Generate(Of NonNegativeInt).Three()
                   Let major    = data.Item1.Item
                   Let minor    = data.Item2.Item
                   Let revision = data.Item3.Item
                   Select Global.fszmq.Version.NewVersion(major,minor,revision)

      ' Creates a random distribution weighted in favor of version with random values
      Dim distribution = Gen.Frequency(
          new WeightAndValue(Of GenVersion)(2, random),
          new WeightAndValue(Of GenVersion)(1, unknown)
      )

      Return Arb.From(distribution)
    End Function

    ''' <summary>
    ''' Allows FsCheck to randomly generate byte[] instances divisible by 4
    ''' </summary>
    Function Mod4Binary () As Arbitrary(Of Mod4Binary)
      ' Mod4Binary encapsulates the following domain rules
      Dim isValid = Function (ByVal data() As Byte) 
                      Return  (data IsNot Nothing)  AndAlso
                              (data.Length > 0)     AndAlso 
                              (data.Length Mod 4 = 0)
                    End Function

      Dim generator = From data In Arb.Generate(Of Byte())()
                      Where isValid(data)
                      Select New Mod4Binary(data)

      Dim shrinker =  Function (ByVal input As Mod4Binary)
                        Return  From data in Arb.Shrink(input.Data)
                                Where isValid(data)
                                Select New Mod4Binary(data)
                      End Function

      Return Arb.From(generator,shrinker)
    End Function
   
  End Module

End Namespace
