namespace fszmq.props
{
  using NUnit.Framework;
  using FsCheck;
  using System;
  using System.Linq;
  using System.Runtime.CompilerServices;

  using CheckProperty = FsCheck.NUnit.PropertyAttribute;

  using fszmq;

  public static class Classifiers
  {
    public static Boolean Within (this Int32 item, Int32 min, Int32 max)
    {
      return (item >= min) && (item <= max);
    }

    public static Boolean IsTiny(this Mod4Binary input)
    {
      return input.Data.Length.Within(Int32.MinValue, 4);
    }

    public static Boolean IsSmall(this Mod4Binary input)
    {
      return input.Data.Length.Within(8, 20);
    }

    public static Boolean IsMedium(this Mod4Binary input) 
    {
      return input.Data.Length.Within(24, 60);
    }

    public static Boolean IsLarge(this Mod4Binary input)
    {
      return input.Data.Length.Within(64, Int32.MaxValue);
    }
  }

  [TestFixture]
  public class Observations
  {
    [CheckProperty(Arbitrary = new []{ typeof(Generators) })]
    public Property EncodeDecodeAreDuals_Trivial (Mod4Binary input)
    {
      var processed = Z85.Decode(Z85.Encode(input.Data));
      return processed.SequenceEqual(input.Data)
                      // identify a special case
                      .Trivial(input.Data.Any (b => b == 70));
    }

    [CheckProperty(Arbitrary = new []{ typeof(Generators) })]
    public Property EncodeDecodeAreDuals_Classified (Mod4Binary input)
    {
      var processed = Z85.Decode(Z85.Encode(input.Data));
      return processed.SequenceEqual(input.Data)
                      // bucket by common sizes
                      .Classify(input.IsLarge (),"Large (64 .. ∞ bytes)"  )
                      .Classify(input.IsMedium(),"Medium (24 .. 60 bytes)")
                      .Classify(input.IsSmall (),"Small (8 .. 20 bytes)"  )
                      .Classify(input.IsTiny  (),"Tiny (0 .. 4 bytes)"    );
    }

    [CheckProperty(Arbitrary = new []{ typeof(Generators) })]
    public Property EncodeDecodeAreDuals_Collected (Mod4Binary input)
    {
      var processed = Z85.Decode(Z85.Encode(input.Data));
      return processed.SequenceEqual(input.Data)
                      // gather individual sizes
                      .Collect(input.Data.Length);
    }

    [CheckProperty(Arbitrary = new []{ typeof(Generators) })]
    public Property EncodeDecodeAreDuals_Combined (Mod4Binary input)
    {
      var processed = Z85.Decode(Z85.Encode(input.Data));
      return processed.SequenceEqual(input.Data)
                      // identify a special case
                      .Trivial(input.Data.Any (b => b == 70))
                      // bucket by common sizes
                      .Classify(input.IsLarge (),"Large (64 .. ∞ bytes)"  )
                      .Classify(input.IsMedium(),"Medium (24 .. 60 bytes)")
                      .Classify(input.IsSmall (),"Small (8 .. 20 bytes)"  )
                      .Classify(input.IsTiny  (),"Tiny (0 .. 4 bytes)"    )
                      // gather individual sizes
                      .Collect(input.Data.Length);
    }
  }
}
