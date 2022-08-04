// A part of the C# Language Syntactic Sugar suite.

using System;
using System.Linq;
using System.Collections.Generic;

namespace CLSS
{
  public static partial class IListGetWeightedRandom
  {
    /// <summary>
    /// Returns a random index with weighted probabilities selected by a
    /// specified function. Weights lesser than or equal to 0 will be ignored.
    /// </summary>
    /// <typeparam name="T">The type of the elements of
    /// <paramref name="source"/>.</typeparam>
    /// <param name="source">The <see cref="IList{T}"/> to select a random
    /// element from.</param>
    /// <param name="weightSelector">A function that selects a weight for each
    /// <see cref="IList{T}"/> element.</param>
    /// <param name="rng">Optional custom-seeded random number generator to use
    /// for the sample rolls.</param>
    /// <returns>A weight-distributed randomly-selected index.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is
    /// null.</exception>
    public static int GetWeightedRandomIndex<T>(this IList<T> source,
      Func<T, double> weightSelector,
      Random rng = null)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (weightSelector == null)
        throw new ArgumentNullException("weightSelector");
      if (rng == null) rng = DefaultRandom.Instance;

      var weights = new double[source.Count];
      for (int i = 0; i < source.Count; ++i)
      {
        var weight = weightSelector(source[i]);
        if (weight < 0.0) weight = 0.0;
        weights[i] = weight;
      }
      int idx = 0;
      for (var roll = rng.NextDouble() * weights.Sum();
        idx < weights.Length;
        ++idx)
      {
        if (weights[idx] > roll) break;
        roll -= weights[idx];
      }
      return idx;
    }

    /// <summary>
    /// Returns a random element with weighted probabilities selected by a
    /// specified function. Weights lesser than or equal to 0 will be ignored.
    /// </summary>
    /// <typeparam name="T">
    /// <inheritdoc cref="GetWeightedRandomIndex{T}(IList{T}, Func{T, double}, Random)"
    /// path="/typeparam[@name='T']"/></typeparam>
    /// <param name="source">
    /// <inheritdoc cref="GetWeightedRandomIndex{T}(IList{T}, Func{T, double}, Random)"
    /// path="/param[@name='source']"/></param>
    /// <param name="weightSelector">
    /// <inheritdoc cref="GetWeightedRandomIndex{T}(IList{T}, Func{T, double}, Random)"
    /// path="/param[@name='weightSelector']"/></param>
    /// <param name="rng">
    /// <inheritdoc cref="GetWeightedRandomIndex{T}(IList{T}, Func{T, double}, Random)"
    /// path="/param[@name='rng']"/></param>
    /// <returns>A weight-distributed randomly-selected element.</returns>
    /// <exception cref="ArgumentNullException">
    /// <inheritdoc cref="GetWeightedRandomIndex{T}(IList{T}, Func{T, double}, Random)"
    /// path="/exception[@cref='ArgumentNullException']"/></exception>
    public static T GetWeightedRandom<T>(this IList<T> source,
      Func<T, double> weightSelector,
      Random rng = null)
    { return source[source.GetWeightedRandomIndex(weightSelector, rng)]; }
  }
}