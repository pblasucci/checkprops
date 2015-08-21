Imports CheckProperty = FsCheck.NUnit.PropertyAttribute

Namespace fszmq.props

  <TestFixture()> _
  Public Class Introduction

    ''' <summary>
    ''' Runs 1 test against 1 fixed input
    ''' </summary>
    <Test()> _
    Sub CloneReturnsNewInstance ()
      Dim test() As Byte = { &H74, &H65, &H73, &H74 }
      Using msg1 As New Message(test),
            msg2 As Message = msg1.Clone()

        Assert.That(msg2, [Is].Not.EqualTo(msg1))
      End Using
    End Sub

    ''' <summary>
    ''' Runs 100 tests against 100 random inputs
    ''' </summary>
    <CheckProperty()> _
    Public Function NotEqualToOriginal(ByVal data() As Byte) As Boolean
      Using msg1 As New Message(data),
            msg2 As Message = msg1.Clone()

        Return msg2 IsNot msg1
      End Using
    End Function

    ' Since properties are just functions of type (Func<T,Boolean>), we can
    ' work with them just like any other functions. This includes using them 
    ' with higher-order functions or partially applied functions.

    ''' <summary>
    ''' Creates a Message from the given data, clones it, and passes both
    ''' Message instances to the given compare function.
    ''' </summary>
    Function withClone (ByVal compare As Func(Of Message,Message,Boolean), ByVal data() As Byte) As Boolean
      Using msg1 As New Message(data),
            msg2 As Message = msg1.Clone()

        Return compare(msg1, msg2)
      End Using
    End Function

    ''' <summary>
    ''' Compares the size of 2 Message instances
    ''' </summary>
    <CheckProperty()> _
    Public Function HasSameSizeAsOriginal (ByVal data() As Byte) As Boolean
      Return withClone(Function (ByVal msg1, ByVal msg2) msg1.Size() = msg2.Size(), data)
    End Function

    ''' <summary>
    ''' Compares the data of 2 Message instances
    ''' </summary>
    <CheckProperty()> _
    Public Function HasSameDataAsOriginal (ByVal data() As Byte) As Boolean
      Return withClone(Function (ByVal msg1, ByVal msg2) msg1.Data().SequenceEqual(msg2.Data()), data)
    End Function

  End Class

End Namespace
