namespace fszmq.props
{
  using NUnit.Framework;
  using FsCheck;
  using System;
  using System.Linq;

  using CheckProperty = FsCheck.NUnit.PropertyAttribute;

  using fszmq;
  
  [TestFixture]
  public class Properties
  {
    /// <summary>
    /// Tests is 2 Message instances have the same size and data
    /// </summary>
    Boolean hasEqualContent (Message msg1, Message msg2)
    {
      return (msg1.Size() == msg2.Size()) 
          && msg1.Data().SequenceEqual(msg2.Data());
    }

    /// <summary>
    /// Successive clones should not change the content of a Message
    /// </summary>
    [CheckProperty(Arbitrary = new []{ typeof(Generators) })]
    public Boolean CloneIsIdempotent (Message msg)
    {
      using (var once   = msg.Clone())
      using (var twice  = msg.Clone().Clone())
      {
        return hasEqualContent(once, twice);
      }
    }

    /// <summary>
    /// Compare content before and after cloning
    /// </summary>
    [CheckProperty(Arbitrary = new []{ typeof(Generators) })]
    public Boolean CopyAltersTargetNotSource_Naive (Message msg1, Message msg2) 
    {
      var source = msg1.Data();
      var before = msg2.Data();
    
      msg1.Copy(msg2);
      var after = msg2.Data();
    
      return after.SequenceEqual(source) && !after.SequenceEqual(before);
    }

    /// <summary>
    /// Compare content before and after cloning
    /// </summary>
    [CheckProperty(Arbitrary = new []{ typeof(Generators) })]
    public Property CopyAltersTargetNotSource_Labels (Message msg1, Message msg2)
    {
      var source = msg1.Data();
      var before = msg2.Data();

      msg1.Copy(msg2);
      var after = msg2.Data();

      return after.SequenceEqual(source)
                  .Label("Source")
                  .And  (!after.SequenceEqual(before))
                  .Label("Target");
    }

    /// <summary>
    /// Compare dissimilar content before and after cloning
    /// </summary>
    [CheckProperty(Arbitrary = new []{ typeof(Generators) })]
    public Property CopyAltersTargetNotSource_Filtered ()
    {
      return Prop.ForAll<Message,Message>((msg1, msg2) => {
        var source = msg1.Data();
        var before = msg2.Data();
        
        Func<Boolean> act = () => 
        { 
          msg1.Copy(msg2);
          return msg2.Data().SequenceEqual(source);
        };

        act.When  (!before.SequenceEqual(source))
           .Label ("Source")
           .And   (!msg2.Data().SequenceEqual(before))
           .Label ("Target");
      });
    }
  }
}
