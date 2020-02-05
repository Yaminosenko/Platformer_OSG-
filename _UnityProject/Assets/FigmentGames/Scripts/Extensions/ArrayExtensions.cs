using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FigmentGames
{
    public static class ArrayExtensions
    {
        public static T[] AddItemToArray<T>(this T[] original, T itemToAdd)
        {
            T[] finalArray = new T[original.Length + 1];

            for (int i = 0; i < original.Length; i++)
                finalArray[i] = original[i];

            finalArray[finalArray.Length - 1] = itemToAdd;

            return finalArray;
        }

        public static T[] RemoveItemAtIndex<T>(this T[] original, int index)
        {
            List<T> list = original.ToList();
            list.RemoveAt(index);
            return list.ToArray();
        }
    }
}