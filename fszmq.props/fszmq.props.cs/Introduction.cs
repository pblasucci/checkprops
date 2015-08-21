namespace fszmq.props
{
  using NUnit.Framework;
  using FsCheck;
  using System;
  using System.Linq;

  using CheckProperty = FsCheck.NUnit.PropertyAttribute;

  using fszmq;

  [TestFixture]
  public class Introduction
  {
    /// <summary>
    /// Runs 1 test against 1 fixed input
    /// </summary>
    [Test]
    public void CloneReturnsNewInstance ()
    {
      using (var msg1 = new Message(new Byte[]{ 0x74, 0x65, 0x73, 0x74 }))
      using (var msg2 = msg1.Clone())
      {
        Assert.That(msg2, Is.Not.EqualTo(msg1));
      }
    }

    /// <summary>
    /// Runs 100 tests against 100 random inputs
    /// </summary>
    [CheckProperty]
    public Boolean NotEqualToOriginal (Byte[] data) 
    {
      using (var msg1 = new Message(data))
      using (var msg2 = msg1.Clone())
      {
        return msg1 != msg2;
      }
    }

    /**
     * Since properties are just functions of type (Func<T,Boolean>), we can
     * work with them just like any other functions. This includes using them 
     * with higher-order functions or partially applied functions.
    */

    /// <summary>
    /// Creates a Message from the given data, clones it, and passes both
    /// Message instances to the given compare function.
    /// </summary>
    Boolean withClone (Func<Message,Message,Boolean> compare,Byte[] data) 
    {
      using (var msg1 = new Message(data)) 
      using (var msg2 = msg1.Clone()) 
      {
        return compare(msg1, msg2);
      }
    }

    /// <summary>
    /// Compares the size of 2 Message instances
    /// </summary>
    [CheckProperty]
    public Boolean HasSameSizeAsOriginal (Byte[] data)
    {
      return withClone ((msg1,msg2) => msg1.Size() == msg2.Size(), data);
    }

    /// <summary>
    /// Compares the data of 2 Message instances
    /// </summary>
    [CheckProperty]
    public Boolean HasSameDataAsOriginal (Byte[] data)
    {
      return withClone ((msg1,msg2) => msg1.Data().SequenceEqual(msg2.Data()), data);
    }
  }
}
