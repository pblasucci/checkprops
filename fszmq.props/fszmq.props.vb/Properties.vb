Imports FsCheck.PropertyExtensions
Imports CheckProperty = FsCheck.NUnit.PropertyAttribute

Namespace fszmq.props

  <TestFixture()>
  Public Class Properties

    ''' <summary>
    ''' Tests is 2 Message instances have the same size and data
    ''' </summary>
    Function HasEqualContent (ByVal msg1 As Message, 
                              ByVal msg2 As Message) As Boolean
      Return  msg1.Size() = msg2.Size() AndAlso 
              msg1.Data().SequenceEqual(msg2.Data())
    End Function

    ''' <summary>
    ''' Successive clones should not change the content of a Message
    ''' </summary>
    <CheckProperty(Arbitrary := { GetType(Generators) })>
    Function CloneIsIdempotent (ByVal msg As Message) As Boolean
      Using once  = msg.Clone(), 
            twice = msg.Clone().Clone()

        Return HasEqualContent(once, twice)
      End Using
    End Function
    
    ''' <summary>
    ''' Compare content before and after cloning
    ''' </summary>
    <CheckProperty(Arbitrary := { GetType(Generators) })>
    Function CopyAltersTargetNotSource_Naive (ByVal msg1 As Message,
                                              ByVal msg2 As Message) As Boolean
      Dim source = msg1.Data()
      Dim before = msg2.Data()
    
      msg1.Copy(msg2)
      Dim after = msg2.Data()
    
      Return after.SequenceEqual(source) AndAlso Not after.SequenceEqual(before)
    End Function

    ''' <summary>
    ''' Compare content before and after cloning
    ''' </summary>
    <CheckProperty(Arbitrary := { GetType(Generators) })>
    Function CopyAltersTargetNotSource_Labels (ByVal msg1 As Message,
                                               ByVal msg2 As Message) As [Property]
      Dim source = msg1.Data()
      Dim before = msg2.Data()
    
      msg1.Copy(msg2)
      Dim after = msg2.Data()
      
      Return after.SequenceEqual(source)                  _
                  .Label("Source")                        _
                  .And  (Not after.SequenceEqual(before)) _
                  .Label("Target")
    End Function
  
    ''' <summary>
    ''' Compare dissimilar content before and after cloning
    ''' </summary>
    <CheckProperty(Arbitrary := { GetType(Generators) })>
    Public Function CopyAltersTargetNotSource_Filtered () As [Property]
      Return Prop.ForAll(New Action(Of Message,Message)(Sub (ByVal msg1, ByVal msg2)
        Dim source = msg1.Data()
        Dim before = msg2.Data()
        
        Dim act As Func(Of Boolean) = Function ()
          msg1.Copy(msg2)
          Return msg2.Data().SequenceEqual(source)
        End Function
                                                            
        act.When  (Not before.SequenceEqual(source))      _
           .Label ("Source")                              _
           .And   (Not msg2.Data().SequenceEqual(before)) _
           .Label ("Target")
      End Sub))
      
    End Function

  End Class

End Namespace
