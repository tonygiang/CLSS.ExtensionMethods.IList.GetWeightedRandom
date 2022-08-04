# CLSS.ExtensionMethods.IList.GetWeightedRandom

### Problem

Sampling a list of elements with weight is a common use case without built-in support in the standard library.

```
var rng = new System.Random();
var weights = new double[collection.Count];
for (int i = 0; i < weights.Length; ++i)
  weights[i] = ConvertElementToWeights(collection[i]);
var weightStages = weights
  .Select((w, i) => weights.Take(i + 1).Sum());
var roll = rng.NextDouble() * weights.Sum();
int selectedIndex = 0;
foreach (var ws in weightStages)
{
  if (ws > roll) break;
  ++selectedIndex;
}
```

Above is a seemingly correct weighted randomization implementation that contains some obvious and non-obvious performance and correctness issues (negative weights are accepted and added to the weight sum). These pitfalls are often overlooked when you have to write weighted randomization on the fly.

### Solution

`GetWeightedRandom` is a method that implements weighted randomization so that it can be written in one line. It takes in a weight selector function with a `Func<T, double>` signature.

```
using CLSS;
using System.Linq;

var weights = new double[] { 1, 3, 6 };
var oneroll = weights.GetWeightedRandom(w => w);

// Distribution test
var rolls = Enumerable.Repeat(0, 10000)
  .Select(_ => weights.GetWeightedRandom(w => w)).ToArray();
Console.WriteLine($"1: {rolls.Count(i => i == 1)}"); // 1: 975
Console.WriteLine($"3: {rolls.Count(i => i == 3)}"); // 3: 3010
Console.WriteLine($"6: {rolls.Count(i => i == 6)}"); // 6: 6015
```

The probability of each element being chosen for each roll is its own weight divided by the sum of all the element's weights. If the specified weight selector function returns a negative weight, it will be treated no differently than 0 weight.

Also included in this package is `GetWeightedRandomIndex` to select only the index number, not the weighted element itself.

Internally, this package uses and depends on the `DefaultRandom` package in CLSS to save on the allocation of a new `System.Random` instance.

Optionally, `GetWeightedRandom` and `GetWeightedRandomIndex` also take in a `System.Random` of your choosing in case you want a custom-seeded random number generator:

```
using CLSS;

var chosenElement = list.GetWeightedRandom(weightSelector, customrng);
```

If you are on .NET 6, you can pass in [`System.Random.Shared`](https://docs.microsoft.com/en-us/dotnet/api/system.random.shared).

`GetWeightedRandom` and the `WeightedSampler<T>` type fulfill similar roles. They have their own trade-offs. The table below compares their key differences:

| Factors | `GetWeightedRandom` | `WeightedSampler<T>` |
| ---     | ---                 | ---               |
| Memory allocation per invocation | 1 array equal in length to source list. | No allocation. |
| Syntax | Extension method, called directly from `IList<T>` types. | Wrapper struct around a list.
| Reflect changes | All list and member mutations are reflected. | Changes in element weights and list mutations are not reflected until manually refreshed. |


**Note**: `GetWeightedRandom` works on all types implementing the [`IList<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1) interface, *including raw C# array*.

##### This package is a part of the [C# Language Syntactic Sugar suite](https://github.com/tonygiang/CLSS).