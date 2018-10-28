using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomUtils
{
    //Code copied from https://stackoverflow.com/questions/3655430/selection-based-on-percentage-weighting and modified
    public static class WeightedRandom
    {
        private static System.Random random = new System.Random();
        public static Value<T> CreateValue<T>(float proportion, T value)
        {
            return new Value<T> { Proportion = proportion, Val = value };
        }
        //Implement as extension method so that collections automatically get this method
        public static T ChooseByRandom<T>(this IEnumerable<Value<T>> collection)
        {
            var rnd = random.NextDouble();
            foreach (var item in collection)
            {
                if (rnd < item.Proportion)
                    return item.Val;
                rnd -= item.Proportion;
            }
            throw new System.InvalidOperationException("The proportions in the collection do not add up to 1.");
        }
        public class Value<T>
        {
            public float Proportion { get; set; }
            public T Val { get; set; }
        }
    }
}
