namespace fszmq.props

open NUnit.Framework
open FsCheck
open FsCheck.NUnit

open fszmq

[<TestFixture>]
module Observations = 

  [<Property(Arbitrary = [| typeof<Generators> |])>]
  let ``encode,decode are duals -- trivial`` (Mod4Binary data) =
    (data |> Z85.encode |> Z85.decode = data) 
    // identify a special case
    |> Prop.trivial (data |> Array.exists ((=) 70uy))

  let (|Tiny|Small|Medium|Large|) value =
    match Array.length value with
    | n when n <=  4            -> Tiny
    | n when n >=  8 && n <= 20 -> Small
    | n when n >= 24 && n <= 60 -> Medium
    | n when n >= 64            -> Large
    | _                         -> failwith "not a valid Mod5String"
   
  let tiny    = function Tiny   -> true | _ -> false
  let small   = function Small  -> true | _ -> false
  let medium  = function Medium -> true | _ -> false
  let large   = function Large  -> true | _ -> false

  [<Property(Arbitrary = [| typeof<Generators> |])>]
  let ``encode,decode are duals -- classified`` (Mod4Binary data) =
    (data |> Z85.encode |> Z85.decode = data) 
    // bucket by common sizes
    |> Prop.classify (large  data) "Large (64 .. ∞ bytes)"
    |> Prop.classify (medium data) "Medium (24 .. 60 bytes)"
    |> Prop.classify (small  data) "Small (8 .. 20 bytes)"
    |> Prop.classify (tiny   data) "Tiny (0 .. 4 bytes)"

  [<Property(Arbitrary = [| typeof<Generators> |])>]
  let ``encode,decode are duals -- collected`` (Mod4Binary data) =
    (data |> Z85.encode |> Z85.decode = data) 
    // gather individual sizes
    |> Prop.collect (Array.length data)

  [<Property(Arbitrary = [| typeof<Generators> |])>]
  let ``encode,decode are duals -- combined`` (Mod4Binary data) =
    (data |> Z85.encode |> Z85.decode = data) 
    // identify a special case
    |> Prop.trivial (data |> Array.exists ((=) 70uy))
    // bucket by common sizes
    |> Prop.classify (large  data) "Large (64 .. ∞ bytes)"
    |> Prop.classify (medium data) "Medium (24 .. 60 bytes)"
    |> Prop.classify (small  data) "Small (8 .. 20 bytes)"
    |> Prop.classify (tiny   data) "Tiny (0 .. 4 bytes)"
    // gather individual sizes
    |> Prop.collect (Array.length data)
