Imports VersionDistribution = System.Collections.Generic.IEnumerable(Of System.Collections.Generic.KeyValuePair(Of fszmq.Version,System.Int32))

Namespace fszmq.props

  Module Distributions

    ''' <summary>
    ''' Generates a random distribution of `count` items for the given generator
    ''' </summary>
    <Extension()>
    Function Distribution(Of T) (ByVal arb   As Arbitrary(Of T), 
                                        ByVal size  As Int32, 
                                        ByVal total As Int32) As IEnumerable(Of KeyValuePair(Of T,Int32))
      Dim sampled = GenExtensions.Sample(arb.Generator,size,total) _
                                  .AsEnumerable() 
      'NOTE: converted to IEnumerable(Of T) because working in FSharpList(Of T) would be awkward... but possible
      Return sampled.GroupBy(Of T,T) (Function (datum) datum,
                                      Function (datum) datum) _
                    .OrderBy(Function (data) data.Count()) _
                    .Select (Function (data) New KeyValuePair(Of T,Int32) (data.Key,data.Count()))
    End Function

    ''' <summary>
    ''' Generates a random distribution of `count` Version instances 
    ''' </summary>
    Function Versions (ByVal count As Int32) As VersionDistribution
      Return Distribution(Of Global.fszmq.Version) (Generators.Version(), 2, count)
    End Function

  End Module

End Namespace