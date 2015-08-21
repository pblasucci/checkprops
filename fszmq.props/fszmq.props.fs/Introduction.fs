namespace fszmq.props

open NUnit.Framework
open FsCheck
open FsCheck.NUnit

open fszmq

[<TestFixture>]
module Introduction =

  /// Runs 1 test against 1 fixed input
  [<Test>]
  let CloneReturnsNewInstance () =
    use msg1 = new Message ("test"B)
    use msg2 = Message.clone msg1
    
    Assert.That (msg2, Is.Not.EqualTo(msg1))
    
  /// Runs 100 tests against 100 random inputs
  [<Property>]
  let ``not equal to original`` data =
      use msg1 = new Message (data)
      use msg2 = Message.clone msg1
      
      msg1 <> msg2
      
  (** 
    Since properties are just functions of type ('a -> bool), we can work with 
    them just like any other functions. This includes using them with 
    higher-order functions or partially applied functions.
  *)

  /// Creates a Message from the given data, clones it, and passes both
  /// Message instances to the given compare function.
  let withClone compare data =
    use msg1 = new Message (data)
    use msg2 = Message.clone msg1
    compare msg1 msg2

  /// Compares the size of 2 Message instances
  [<Property>]
  let ``has same size as original`` data = 
    withClone (fun msg1 msg2 -> Message.size msg1 = Message.size msg2) data

  /// Compares the data of 2 Message instances
  [<Property>]
  let ``has same data as original`` data = 
    withClone (fun msg1 msg2 -> Message.data msg1 = Message.data msg2) data
