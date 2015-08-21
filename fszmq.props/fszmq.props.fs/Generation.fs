namespace fszmq.props

open NUnit.Framework
open FsCheck
open FsCheck.NUnit

open fszmq

module Distributions =

  /// Generates a random distribution of `count` items for the given generator
  let distrib size count (arb:Arbitrary<_>) =
    arb.Generator
    |> Gen.sample   size count
    |> Seq.groupBy  id 
    |> Seq.map      (fun (key,value) -> key,Seq.length value)
    |> Seq.sortBy   (fun (_  ,count) -> count)

  /// Generates a random distribution of `count` Version instances 
  let Versions count =
    Generators.Version |> distrib 2 count


[<TestFixture>]
module Generation = 
  
  [<Test>]
  let ``print Version distribution`` () =
    25
    |> Distributions.Versions
    //HACK: verbose, 2-step printing is a work-around for weirdness with VS test runner output
    |> Seq.iter (fun (vsn,count) -> let info = sprintf "%i,%A" count vsn
                                    printfn "%s" info)
    
  [<Property>]
  let ``encode,decode are duals -- failing`` data =
    data |> Z85.encode |> Z85.decode = data

  [<Property(Arbitrary = [| typeof<Generators> |])>]
  let ``encode,decode are duals -- passing`` (Mod4Binary data) =
    data |> Z85.encode |> Z85.decode = data
