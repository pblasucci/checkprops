namespace fszmq.props

open FsCheck
open System

open fszmq

/// A byte[] whose length is evenly divisible by 4
type Mod4Binary = Mod4Binary of data:byte[]
 
type Generators =
  /// Allows FsCheck to randomly generate fszmq.Message instances
  static member Message = 
    gen { let! (NonNull data) = Arb.generate<NonNull<Byte[]>>
          // Creates a random distribution weighted in favor of messages with random content
          return! Gen.frequency [ (1, gen { return new Message ()     })
                                  (2, gen { return new Message (data) }) ] } 
    |> Arb.fromGen

  /// Allows FsCheck to randomly generate fszmq.Version instances
  static member Version =
    let unknown = Gen.constant Version.Unknown
    let version = Arb.generate<NonNegativeInt>
                  |> Gen.three
                  |> Gen.map (fun (NonNegativeInt m
                                  ,NonNegativeInt n
                                  ,NonNegativeInt r) -> fszmq.Version (m,n,r))
    // Creates a random distribution weighted in favor of version with random values
    [ (1,unknown); (2,version) ]
    |> Gen.frequency
    |> Arb.fromGen

  /// Allows FsCheck to randomly generate byte[] instances divisible by 4
  static member Mod4Binary =
    // Mod4Binary encapsulates the following domain rules
    let isValid (data:byte[]) = data <> null    && 
                                data.Length > 0 && 
                                data.Length % 4 = 0
    
    Arb.fromGenShrink ( 
      // generator
      Arb.generate<byte[]>
      |> Gen.suchThat isValid
      |> Gen.map Mod4Binary
      // shrinker
    , fun (Mod4Binary data) ->  data 
                                |> Arb.shrink 
                                |> Seq.filter isValid
                                |> Seq.map Mod4Binary )
