namespace fszmq.props
{
  using FsCheck;
  using System;
  using System.Text;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  
  using GenMessage    = FsCheck.Gen<fszmq.Message>;
  using GenVersion    = FsCheck.Gen<fszmq.Version>;

  /// <summary>
  /// A byte[] whose length is evenly divisible by 4
  /// </summary>
  public sealed class Mod4Binary
  {
    private readonly  Byte[] data;
    public Mod4Binary(Byte[] data) 
    { 
      // Mod4Binary encapsulates the following domain rules
      if ((data == null) || (data.Length % 4 != 0))
        { throw new ArgumentException("Not evenly divisble by 4", "data"); }
      this.data = data; 
    }

    public Byte[] Data { get { return data; } }

    public override Boolean Equals (object obj)
    {
      var that = obj as Mod4Binary;
      if (that == null) return false;

      return this.data.SequenceEqual(that.data);
    }

    public override Int32 GetHashCode ()
    {
      const Int32 seed = -1640531527;
      return seed ^ this.data.GetHashCode();
    }

    public override String ToString ()
    {
      var buffer = new StringBuilder("Mod4Binary ");
      buffer.Append("[| ");
      this.data.Aggregate(buffer, (a,d) => a.AppendFormat("{0:X2}; ",d));
      buffer.Append(" |]");      
      return buffer.ToString();
    }
  }

  public static class Generators 
  {
    /// <summary>
    /// Allows FsCheck to randomly generate fszmq.Message instances
    /// </summary>
    public static Arbitrary<fszmq.Message> Message () 
    {
      // Generates an empty fszmq.Message
      var empty = Gen.Constant(new fszmq.Message());

      // Generates a fszmq.Message with random content
      var random = from data in Arb.Generate<NonNull<Byte[]>>() 
                   select new fszmq.Message(data.Get);
      
      // Creates a random distribution weighted in favor of messages with random content
      var distribution = Gen.Frequency(
          new WeightAndValue<GenMessage>(2, random)
         ,new WeightAndValue<GenMessage>(1, empty )
      );
      
      return Arb.From(distribution);
    }

    /// <summary>
    /// Allows FsCheck to randomly generate fszmq.Version instances
    /// </summary>
    public static Arbitrary<fszmq.Version> Version ()
    {
      // Generates a version when the actual values are unknown
      var unknown = Gen.Constant(fszmq.Version.Unknown);
      // Generates a version from random known values
      var random =  from data in Arb.Generate<NonNegativeInt>().Three()
                    let major     = data.Item1.Item
                    let minor     = data.Item2.Item
                    let revision  = data.Item3.Item
                    select fszmq.Version.NewVersion(major, minor, revision);
        
        
      // Creates a random distribution weighted in favor of version with random values
      var distribution = Gen.Frequency(
          new WeightAndValue<GenVersion>(2, random )
         ,new WeightAndValue<GenVersion>(1, unknown)
      );
      
      return Arb.From(distribution);
    }

    /// <summary>
    /// Allows FsCheck to randomly generate byte[] instances divisible by 4
    /// </summary>
    public static Arbitrary<Mod4Binary> Mod4Binary ()
    {
      // Mod4Binary encapsulates the following domain rules
      Func<Byte[],Boolean> isValid = (data) =>  (data != null)    && 
                                                (data.Length > 0) && 
                                                (data.Length % 4 == 0);
      
      return Arb.From(// generator
                      from data in Arb.Generate<Byte[]>()
                      where isValid(data)
                      select new Mod4Binary(data)
                      // shrinker
                     ,(input) => from data in Arb.Shrink(input.Data) 
                                 where  isValid(data)
                                 select new Mod4Binary(data));
    }
  }
}