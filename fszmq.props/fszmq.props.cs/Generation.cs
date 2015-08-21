namespace fszmq.props
{
  using NUnit.Framework;
  using FsCheck;
  using System;
  using System.Linq;

  using CheckProperty = FsCheck.NUnit.PropertyAttribute;

  using fszmq;

  [TestFixture]
  public class Generation
  {
    [Test]
    public void PrintVersionDistribution ()
    {
      foreach (var pair in Distributions.Versions(25))
      {
        var version = pair.Key;
        var count   = pair.Value;
        Console.WriteLine("{0:00},{1}", count, version);
      }
    }

    [CheckProperty]
    public Boolean EncodeDecodeAreDuals_Failing (Byte[] data)
    {
      var processed = Z85.Decode(Z85.Encode(data));
      return processed.SequenceEqual(data);
    }

    [CheckProperty(Arbitrary = new []{ typeof(Generators) })]
    public Boolean EncodeDecodeAreDuals_Passing (Mod4Binary input)
    {
      var processed = Z85.Decode(Z85.Encode(input.Data));
      return processed.SequenceEqual(input.Data);
    }
  }
}
