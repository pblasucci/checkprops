namespace fszmq.props
{
  using FsCheck;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using VersionDistribution = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<fszmq.Version,System.Int32>>;

  public static class Distributions
  {
    /// <summary>
    /// Generates a random distribution of `count` items for the given generator
    /// </summary>
    public static IEnumerable<KeyValuePair<T,Int32>> Distribution<T> (this Arbitrary<T> arb, Int32 size, Int32 count)
    {
      return arb.Generator
                .Sample(size, count)
                .AsEnumerable() // working in FSharpList<T> would be awkward... but possible
                .GroupBy  (vsn    => vsn)
                .OrderBy  (group  => group.Count())
                .Select   (group  => new KeyValuePair<T,Int32>(group.Key,group.Count()));
    }

    /// <summary>
    /// Generates a random distribution of `count` Version instances 
    /// </summary>
    public static VersionDistribution Versions (Int32 count) 
    {
      return Distribution<fszmq.Version> (Generators.Version(), 9, count);
    }
  }
}
