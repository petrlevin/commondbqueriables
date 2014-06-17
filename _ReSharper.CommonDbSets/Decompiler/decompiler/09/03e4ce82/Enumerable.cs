// Type: System.Linq.Enumerable
// Assembly: System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Threading;

namespace System.Linq
{
  /// <summary>
  /// Provides a set of static (Shared in Visual Basic) methods for querying objects that implement <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
  /// </summary>
  [__DynamicallyInvokable]
  public static class Enumerable
  {
    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements from the input sequence that satisfy the condition.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to filter.</param><param name="predicate">A function to test each element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      if (source is Enumerable.Iterator<TSource>)
        return ((Enumerable.Iterator<TSource>) source).Where(predicate);
      if (source is TSource[])
        return (IEnumerable<TSource>) new Enumerable.WhereArrayIterator<TSource>((TSource[]) source, predicate);
      if (source is List<TSource>)
        return (IEnumerable<TSource>) new Enumerable.WhereListIterator<TSource>((List<TSource>) source, predicate);
      else
        return (IEnumerable<TSource>) new Enumerable.WhereEnumerableIterator<TSource>(source, predicate);
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate. Each element's index is used in the logic of the predicate function.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements from the input sequence that satisfy the condition.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to filter.</param><param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      else
        return Enumerable.WhereIterator<TSource>(source, predicate);
    }

    private static IEnumerable<TSource> WhereIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
    {
      int index = -1;
      foreach (TSource source1 in source)
      {
        checked { ++index; }
        if (predicate(source1, index))
          yield return source1;
      }
    }

    /// <summary>
    /// Projects each element of a sequence into a new form.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements are the result of invoking the transform function on each element of <paramref name="source"/>.
    /// </returns>
    /// <param name="source">A sequence of values to invoke a transform function on.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (selector == null)
        throw Error.ArgumentNull("selector");
      if (source is Enumerable.Iterator<TSource>)
        return ((Enumerable.Iterator<TSource>) source).Select<TResult>(selector);
      if (source is TSource[])
        return (IEnumerable<TResult>) new Enumerable.WhereSelectArrayIterator<TSource, TResult>((TSource[]) source, (Func<TSource, bool>) null, selector);
      if (source is List<TSource>)
        return (IEnumerable<TResult>) new Enumerable.WhereSelectListIterator<TSource, TResult>((List<TSource>) source, (Func<TSource, bool>) null, selector);
      else
        return (IEnumerable<TResult>) new Enumerable.WhereSelectEnumerableIterator<TSource, TResult>(source, (Func<TSource, bool>) null, selector);
    }

    /// <summary>
    /// Projects each element of a sequence into a new form by incorporating the element's index.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements are the result of invoking the transform function on each element of <paramref name="source"/>.
    /// </returns>
    /// <param name="source">A sequence of values to invoke a transform function on.</param><param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (selector == null)
        throw Error.ArgumentNull("selector");
      else
        return Enumerable.SelectIterator<TSource, TResult>(source, selector);
    }

    private static IEnumerable<TResult> SelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
    {
      int index = -1;
      foreach (TSource source1 in source)
      {
        checked { ++index; }
        yield return selector(source1, index);
      }
    }

    private static Func<TSource, bool> CombinePredicates<TSource>(Func<TSource, bool> predicate1, Func<TSource, bool> predicate2)
    {
      return (Func<TSource, bool>) (x =>
      {
        if (predicate1(x))
          return predicate2(x);
        else
          return false;
      });
    }

    private static Func<TSource, TResult> CombineSelectors<TSource, TMiddle, TResult>(Func<TSource, TMiddle> selector1, Func<TMiddle, TResult> selector2)
    {
      return (Func<TSource, TResult>) (x => selector2(selector1(x)));
    }

    /// <summary>
    /// Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IEnumerable`1"/> and flattens the resulting sequences into one sequence.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements are the result of invoking the one-to-many transform function on each element of the input sequence.
    /// </returns>
    /// <param name="source">A sequence of values to project.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TResult">The type of the elements of the sequence returned by <paramref name="selector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (selector == null)
        throw Error.ArgumentNull("selector");
      else
        return Enumerable.SelectManyIterator<TSource, TResult>(source, selector);
    }

    private static IEnumerable<TResult> SelectManyIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
    {
      foreach (TSource source1 in source)
      {
        foreach (TResult result in selector(source1))
          yield return result;
      }
    }

    /// <summary>
    /// Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IEnumerable`1"/>, and flattens the resulting sequences into one sequence. The index of each source element is used in the projected form of that element.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements are the result of invoking the one-to-many transform function on each element of an input sequence.
    /// </returns>
    /// <param name="source">A sequence of values to project.</param><param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TResult">The type of the elements of the sequence returned by <paramref name="selector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (selector == null)
        throw Error.ArgumentNull("selector");
      else
        return Enumerable.SelectManyIterator<TSource, TResult>(source, selector);
    }

    private static IEnumerable<TResult> SelectManyIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
    {
      int index = -1;
      foreach (TSource source1 in source)
      {
        checked { ++index; }
        foreach (TResult result in selector(source1, index))
          yield return result;
      }
    }

    /// <summary>
    /// Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IEnumerable`1"/>, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein. The index of each source element is used in the intermediate projected form of that element.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements are the result of invoking the one-to-many transform function <paramref name="collectionSelector"/> on each element of <paramref name="source"/> and then mapping each of those sequence elements and their corresponding source element to a result element.
    /// </returns>
    /// <param name="source">A sequence of values to project.</param><param name="collectionSelector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param><param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TCollection">The type of the intermediate elements collected by <paramref name="collectionSelector"/>.</typeparam><typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="collectionSelector"/> or <paramref name="resultSelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (collectionSelector == null)
        throw Error.ArgumentNull("collectionSelector");
      if (resultSelector == null)
        throw Error.ArgumentNull("resultSelector");
      else
        return Enumerable.SelectManyIterator<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
    }

    private static IEnumerable<TResult> SelectManyIterator<TSource, TCollection, TResult>(IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
    {
      int index = -1;
      foreach (TSource source1 in source)
      {
        checked { ++index; }
        foreach (TCollection collection in collectionSelector(source1, index))
          yield return resultSelector(source1, collection);
      }
    }

    /// <summary>
    /// Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IEnumerable`1"/>, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements are the result of invoking the one-to-many transform function <paramref name="collectionSelector"/> on each element of <paramref name="source"/> and then mapping each of those sequence elements and their corresponding source element to a result element.
    /// </returns>
    /// <param name="source">A sequence of values to project.</param><param name="collectionSelector">A transform function to apply to each element of the input sequence.</param><param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TCollection">The type of the intermediate elements collected by <paramref name="collectionSelector"/>.</typeparam><typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="collectionSelector"/> or <paramref name="resultSelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (collectionSelector == null)
        throw Error.ArgumentNull("collectionSelector");
      if (resultSelector == null)
        throw Error.ArgumentNull("resultSelector");
      else
        return Enumerable.SelectManyIterator<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);
    }

    private static IEnumerable<TResult> SelectManyIterator<TSource, TCollection, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
    {
      foreach (TSource source1 in source)
      {
        foreach (TCollection collection in collectionSelector(source1))
          yield return resultSelector(source1, collection);
      }
    }

    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a sequence.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the specified number of elements from the start of the input sequence.
    /// </returns>
    /// <param name="source">The sequence to return elements from.</param><param name="count">The number of elements to return.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return Enumerable.TakeIterator<TSource>(source, count);
    }

    private static IEnumerable<TSource> TakeIterator<TSource>(IEnumerable<TSource> source, int count)
    {
      if (count > 0)
      {
        foreach (TSource source1 in source)
        {
          yield return source1;
          if (--count == 0)
            break;
        }
      }
    }

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the elements from the input sequence that occur before the element at which the test no longer passes.
    /// </returns>
    /// <param name="source">A sequence to return elements from.</param><param name="predicate">A function to test each element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      else
        return Enumerable.TakeWhileIterator<TSource>(source, predicate);
    }

    private static IEnumerable<TSource> TakeWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      foreach (TSource source1 in source)
      {
        if (predicate(source1))
          yield return source1;
        else
          break;
      }
    }

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true. The element's index is used in the logic of the predicate function.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements from the input sequence that occur before the element at which the test no longer passes.
    /// </returns>
    /// <param name="source">The sequence to return elements from.</param><param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      else
        return Enumerable.TakeWhileIterator<TSource>(source, predicate);
    }

    private static IEnumerable<TSource> TakeWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
    {
      int index = -1;
      foreach (TSource source1 in source)
      {
        checked { ++index; }
        if (predicate(source1, index))
          yield return source1;
        else
          break;
      }
    }

    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the elements that occur after the specified index in the input sequence.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return elements from.</param><param name="count">The number of elements to skip before returning the remaining elements.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return Enumerable.SkipIterator<TSource>(source, count);
    }

    private static IEnumerable<TSource> SkipIterator<TSource>(IEnumerable<TSource> source, int count)
    {
      using (IEnumerator<TSource> enumerator = source.GetEnumerator())
      {
        while (count > 0 && enumerator.MoveNext())
          --count;
        if (count <= 0)
        {
          while (enumerator.MoveNext())
            yield return enumerator.Current;
        }
      }
    }

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the elements from the input sequence starting at the first element in the linear series that does not pass the test specified by <paramref name="predicate"/>.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return elements from.</param><param name="predicate">A function to test each element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      else
        return Enumerable.SkipWhileIterator<TSource>(source, predicate);
    }

    private static IEnumerable<TSource> SkipWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      bool yielding = false;
      foreach (TSource source1 in source)
      {
        if (!yielding && !predicate(source1))
          yielding = true;
        if (yielding)
          yield return source1;
      }
    }

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements. The element's index is used in the logic of the predicate function.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the elements from the input sequence starting at the first element in the linear series that does not pass the test specified by <paramref name="predicate"/>.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return elements from.</param><param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      else
        return Enumerable.SkipWhileIterator<TSource>(source, predicate);
    }

    private static IEnumerable<TSource> SkipWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
    {
      int index = -1;
      bool yielding = false;
      foreach (TSource source1 in source)
      {
        checked { ++index; }
        if (!yielding && !predicate(source1, index))
          yielding = true;
        if (yielding)
          yield return source1;
      }
    }

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that has elements of type <paramref name="TResult"/> that are obtained by performing an inner join on two sequences.
    /// </returns>
    /// <param name="outer">The first sequence to join.</param><param name="inner">The sequence to join to the first sequence.</param><param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param><param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param><param name="resultSelector">A function to create a result element from two matching elements.</param><typeparam name="TOuter">The type of the elements of the first sequence.</typeparam><typeparam name="TInner">The type of the elements of the second sequence.</typeparam><typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam><typeparam name="TResult">The type of the result elements.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="outer"/> or <paramref name="inner"/> or <paramref name="outerKeySelector"/> or <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
    {
      if (outer == null)
        throw Error.ArgumentNull("outer");
      if (inner == null)
        throw Error.ArgumentNull("inner");
      if (outerKeySelector == null)
        throw Error.ArgumentNull("outerKeySelector");
      if (innerKeySelector == null)
        throw Error.ArgumentNull("innerKeySelector");
      if (resultSelector == null)
        throw Error.ArgumentNull("resultSelector");
      else
        return Enumerable.JoinIterator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, (IEqualityComparer<TKey>) null);
    }

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. A specified <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> is used to compare keys.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that has elements of type <paramref name="TResult"/> that are obtained by performing an inner join on two sequences.
    /// </returns>
    /// <param name="outer">The first sequence to join.</param><param name="inner">The sequence to join to the first sequence.</param><param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param><param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param><param name="resultSelector">A function to create a result element from two matching elements.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to hash and compare keys.</param><typeparam name="TOuter">The type of the elements of the first sequence.</typeparam><typeparam name="TInner">The type of the elements of the second sequence.</typeparam><typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam><typeparam name="TResult">The type of the result elements.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="outer"/> or <paramref name="inner"/> or <paramref name="outerKeySelector"/> or <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
    {
      if (outer == null)
        throw Error.ArgumentNull("outer");
      if (inner == null)
        throw Error.ArgumentNull("inner");
      if (outerKeySelector == null)
        throw Error.ArgumentNull("outerKeySelector");
      if (innerKeySelector == null)
        throw Error.ArgumentNull("innerKeySelector");
      if (resultSelector == null)
        throw Error.ArgumentNull("resultSelector");
      else
        return Enumerable.JoinIterator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
    }

    private static IEnumerable<TResult> JoinIterator<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
    {
      Lookup<TKey, TInner> lookup = Lookup<TKey, TInner>.CreateForJoin(inner, innerKeySelector, comparer);
      foreach (TOuter outer1 in outer)
      {
        Lookup<TKey, TInner>.Grouping g = lookup.GetGrouping(outerKeySelector(outer1), false);
        if (g != null)
        {
          for (int i = 0; i < g.count; ++i)
            yield return resultSelector(outer1, g.elements[i]);
        }
      }
    }

    /// <summary>
    /// Correlates the elements of two sequences based on equality of keys and groups the results. The default equality comparer is used to compare keys.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements of type <paramref name="TResult"/> that are obtained by performing a grouped join on two sequences.
    /// </returns>
    /// <param name="outer">The first sequence to join.</param><param name="inner">The sequence to join to the first sequence.</param><param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param><param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param><param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param><typeparam name="TOuter">The type of the elements of the first sequence.</typeparam><typeparam name="TInner">The type of the elements of the second sequence.</typeparam><typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam><typeparam name="TResult">The type of the result elements.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="outer"/> or <paramref name="inner"/> or <paramref name="outerKeySelector"/> or <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
    {
      if (outer == null)
        throw Error.ArgumentNull("outer");
      if (inner == null)
        throw Error.ArgumentNull("inner");
      if (outerKeySelector == null)
        throw Error.ArgumentNull("outerKeySelector");
      if (innerKeySelector == null)
        throw Error.ArgumentNull("innerKeySelector");
      if (resultSelector == null)
        throw Error.ArgumentNull("resultSelector");
      else
        return Enumerable.GroupJoinIterator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, (IEqualityComparer<TKey>) null);
    }

    /// <summary>
    /// Correlates the elements of two sequences based on key equality and groups the results. A specified <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> is used to compare keys.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements of type <paramref name="TResult"/> that are obtained by performing a grouped join on two sequences.
    /// </returns>
    /// <param name="outer">The first sequence to join.</param><param name="inner">The sequence to join to the first sequence.</param><param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param><param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param><param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to hash and compare keys.</param><typeparam name="TOuter">The type of the elements of the first sequence.</typeparam><typeparam name="TInner">The type of the elements of the second sequence.</typeparam><typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam><typeparam name="TResult">The type of the result elements.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="outer"/> or <paramref name="inner"/> or <paramref name="outerKeySelector"/> or <paramref name="innerKeySelector"/> or <paramref name="resultSelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
    {
      if (outer == null)
        throw Error.ArgumentNull("outer");
      if (inner == null)
        throw Error.ArgumentNull("inner");
      if (outerKeySelector == null)
        throw Error.ArgumentNull("outerKeySelector");
      if (innerKeySelector == null)
        throw Error.ArgumentNull("innerKeySelector");
      if (resultSelector == null)
        throw Error.ArgumentNull("resultSelector");
      else
        return Enumerable.GroupJoinIterator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
    }

    private static IEnumerable<TResult> GroupJoinIterator<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
    {
      Lookup<TKey, TInner> lookup = Lookup<TKey, TInner>.CreateForJoin(inner, innerKeySelector, comparer);
      foreach (TOuter outer1 in outer)
        yield return resultSelector(outer1, lookup[outerKeySelector(outer1)]);
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a key.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.IOrderedEnumerable`1"/> whose elements are sorted according to a key.
    /// </returns>
    /// <param name="source">A sequence of values to order.</param><param name="keySelector">A function to extract a key from an element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      return (IOrderedEnumerable<TSource>) new OrderedEnumerable<TSource, TKey>(source, keySelector, (IComparer<TKey>) null, false);
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order by using a specified comparer.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.IOrderedEnumerable`1"/> whose elements are sorted according to a key.
    /// </returns>
    /// <param name="source">A sequence of values to order.</param><param name="keySelector">A function to extract a key from an element.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IComparer`1"/> to compare keys.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
    {
      return (IOrderedEnumerable<TSource>) new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, false);
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to a key.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.IOrderedEnumerable`1"/> whose elements are sorted in descending order according to a key.
    /// </returns>
    /// <param name="source">A sequence of values to order.</param><param name="keySelector">A function to extract a key from an element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      return (IOrderedEnumerable<TSource>) new OrderedEnumerable<TSource, TKey>(source, keySelector, (IComparer<TKey>) null, true);
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order by using a specified comparer.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.IOrderedEnumerable`1"/> whose elements are sorted in descending order according to a key.
    /// </returns>
    /// <param name="source">A sequence of values to order.</param><param name="keySelector">A function to extract a key from an element.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IComparer`1"/> to compare keys.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
    {
      return (IOrderedEnumerable<TSource>) new OrderedEnumerable<TSource, TKey>(source, keySelector, comparer, true);
    }

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.IOrderedEnumerable`1"/> whose elements are sorted according to a key.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Linq.IOrderedEnumerable`1"/> that contains elements to sort.</param><param name="keySelector">A function to extract a key from each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return source.CreateOrderedEnumerable<TKey>(keySelector, (IComparer<TKey>) null, false);
    }

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in ascending order by using a specified comparer.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.IOrderedEnumerable`1"/> whose elements are sorted according to a key.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Linq.IOrderedEnumerable`1"/> that contains elements to sort.</param><param name="keySelector">A function to extract a key from each element.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IComparer`1"/> to compare keys.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, false);
    }

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in descending order, according to a key.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.IOrderedEnumerable`1"/> whose elements are sorted in descending order according to a key.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Linq.IOrderedEnumerable`1"/> that contains elements to sort.</param><param name="keySelector">A function to extract a key from each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return source.CreateOrderedEnumerable<TKey>(keySelector, (IComparer<TKey>) null, true);
    }

    /// <summary>
    /// Performs a subsequent ordering of the elements in a sequence in descending order by using a specified comparer.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Linq.IOrderedEnumerable`1"/> whose elements are sorted in descending order according to a key.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Linq.IOrderedEnumerable`1"/> that contains elements to sort.</param><param name="keySelector">A function to extract a key from each element.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IComparer`1"/> to compare keys.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return source.CreateOrderedEnumerable<TKey>(keySelector, comparer, true);
    }

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function.
    /// </summary>
    /// 
    /// <returns>
    /// An IEnumerable&lt;IGrouping&lt;TKey, TSource&gt;&gt; in C# or IEnumerable(Of IGrouping(Of TKey, TSource)) in Visual Basic where each <see cref="T:System.Linq.IGrouping`2"/> object contains a sequence of objects and a key.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements to group.</param><param name="keySelector">A function to extract the key for each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      return (IEnumerable<IGrouping<TKey, TSource>>) new GroupedEnumerable<TSource, TKey, TSource>(source, keySelector, IdentityFunction<TSource>.Instance, (IEqualityComparer<TKey>) null);
    }

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and compares the keys by using a specified comparer.
    /// </summary>
    /// 
    /// <returns>
    /// An IEnumerable&lt;IGrouping&lt;TKey, TSource&gt;&gt; in C# or IEnumerable(Of IGrouping(Of TKey, TSource)) in Visual Basic where each <see cref="T:System.Linq.IGrouping`2"/> object contains a collection of objects and a key.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements to group.</param><param name="keySelector">A function to extract the key for each element.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare keys.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
    {
      return (IEnumerable<IGrouping<TKey, TSource>>) new GroupedEnumerable<TSource, TKey, TSource>(source, keySelector, IdentityFunction<TSource>.Instance, comparer);
    }

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and projects the elements for each group by using a specified function.
    /// </summary>
    /// 
    /// <returns>
    /// An IEnumerable&lt;IGrouping&lt;TKey, TElement&gt;&gt; in C# or IEnumerable(Of IGrouping(Of TKey, TElement)) in Visual Basic where each <see cref="T:System.Linq.IGrouping`2"/> object contains a collection of objects of type <paramref name="TElement"/> and a key.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements to group.</param><param name="keySelector">A function to extract the key for each element.</param><param name="elementSelector">A function to map each source element to an element in the <see cref="T:System.Linq.IGrouping`2"/>.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><typeparam name="TElement">The type of the elements in the <see cref="T:System.Linq.IGrouping`2"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
    {
      return (IEnumerable<IGrouping<TKey, TElement>>) new GroupedEnumerable<TSource, TKey, TElement>(source, keySelector, elementSelector, (IEqualityComparer<TKey>) null);
    }

    /// <summary>
    /// Groups the elements of a sequence according to a key selector function. The keys are compared by using a comparer and each group's elements are projected by using a specified function.
    /// </summary>
    /// 
    /// <returns>
    /// An IEnumerable&lt;IGrouping&lt;TKey, TElement&gt;&gt; in C# or IEnumerable(Of IGrouping(Of TKey, TElement)) in Visual Basic where each <see cref="T:System.Linq.IGrouping`2"/> object contains a collection of objects of type <paramref name="TElement"/> and a key.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements to group.</param><param name="keySelector">A function to extract the key for each element.</param><param name="elementSelector">A function to map each source element to an element in an <see cref="T:System.Linq.IGrouping`2"/>.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare keys.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><typeparam name="TElement">The type of the elements in the <see cref="T:System.Linq.IGrouping`2"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
    {
      return (IEnumerable<IGrouping<TKey, TElement>>) new GroupedEnumerable<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer);
    }

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key.
    /// </summary>
    /// 
    /// <returns>
    /// A collection of elements of type <paramref name="TResult"/> where each element represents a projection over a group and its key.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements to group.</param><param name="keySelector">A function to extract the key for each element.</param><param name="resultSelector">A function to create a result value from each group.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
    {
      return (IEnumerable<TResult>) new GroupedEnumerable<TSource, TKey, TSource, TResult>(source, keySelector, IdentityFunction<TSource>.Instance, resultSelector, (IEqualityComparer<TKey>) null);
    }

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function.
    /// </summary>
    /// 
    /// <returns>
    /// A collection of elements of type <paramref name="TResult"/> where each element represents a projection over a group and its key.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements to group.</param><param name="keySelector">A function to extract the key for each element.</param><param name="elementSelector">A function to map each source element to an element in an <see cref="T:System.Linq.IGrouping`2"/>.</param><param name="resultSelector">A function to create a result value from each group.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><typeparam name="TElement">The type of the elements in each <see cref="T:System.Linq.IGrouping`2"/>.</typeparam><typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
    {
      return (IEnumerable<TResult>) new GroupedEnumerable<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, (IEqualityComparer<TKey>) null);
    }

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The keys are compared by using a specified comparer.
    /// </summary>
    /// 
    /// <returns>
    /// A collection of elements of type <paramref name="TResult"/> where each element represents a projection over a group and its key.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements to group.</param><param name="keySelector">A function to extract the key for each element.</param><param name="resultSelector">A function to create a result value from each group.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare keys with.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
    {
      return (IEnumerable<TResult>) new GroupedEnumerable<TSource, TKey, TSource, TResult>(source, keySelector, IdentityFunction<TSource>.Instance, resultSelector, comparer);
    }

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. Key values are compared by using a specified comparer, and the elements of each group are projected by using a specified function.
    /// </summary>
    /// 
    /// <returns>
    /// A collection of elements of type <paramref name="TResult"/> where each element represents a projection over a group and its key.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements to group.</param><param name="keySelector">A function to extract the key for each element.</param><param name="elementSelector">A function to map each source element to an element in an <see cref="T:System.Linq.IGrouping`2"/>.</param><param name="resultSelector">A function to create a result value from each group.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare keys with.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><typeparam name="TElement">The type of the elements in each <see cref="T:System.Linq.IGrouping`2"/>.</typeparam><typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector"/>.</typeparam>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
    {
      return (IEnumerable<TResult>) new GroupedEnumerable<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, comparer);
    }

    /// <summary>
    /// Concatenates two sequences.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the concatenated elements of the two input sequences.
    /// </returns>
    /// <param name="first">The first sequence to concatenate.</param><param name="second">The sequence to concatenate to the first sequence.</param><typeparam name="TSource">The type of the elements of the input sequences.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
    {
      if (first == null)
        throw Error.ArgumentNull("first");
      if (second == null)
        throw Error.ArgumentNull("second");
      else
        return Enumerable.ConcatIterator<TSource>(first, second);
    }

    private static IEnumerable<TSource> ConcatIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second)
    {
      foreach (TSource source in first)
        yield return source;
      foreach (TSource source in second)
        yield return source;
    }

    /// <summary>
    /// Merges two sequences by using the specified predicate function.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains merged elements of two input sequences.
    /// </returns>
    /// <param name="first">The first sequence to merge.</param><param name="second">The second sequence to merge.</param><param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param><typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam><typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam><typeparam name="TResult">The type of the elements of the result sequence.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
    {
      if (first == null)
        throw Error.ArgumentNull("first");
      if (second == null)
        throw Error.ArgumentNull("second");
      if (resultSelector == null)
        throw Error.ArgumentNull("resultSelector");
      else
        return Enumerable.ZipIterator<TFirst, TSecond, TResult>(first, second, resultSelector);
    }

    private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
    {
      using (IEnumerator<TFirst> enumerator1 = first.GetEnumerator())
      {
        using (IEnumerator<TSecond> enumerator2 = second.GetEnumerator())
        {
          while (enumerator1.MoveNext() && enumerator2.MoveNext())
            yield return resultSelector(enumerator1.Current, enumerator2.Current);
        }
      }
    }

    /// <summary>
    /// Returns distinct elements from a sequence by using the default equality comparer to compare values.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains distinct elements from the source sequence.
    /// </returns>
    /// <param name="source">The sequence to remove duplicate elements from.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return Enumerable.DistinctIterator<TSource>(source, (IEqualityComparer<TSource>) null);
    }

    /// <summary>
    /// Returns distinct elements from a sequence by using a specified <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare values.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains distinct elements from the source sequence.
    /// </returns>
    /// <param name="source">The sequence to remove duplicate elements from.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare values.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return Enumerable.DistinctIterator<TSource>(source, comparer);
    }

    private static IEnumerable<TSource> DistinctIterator<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
    {
      Set<TSource> set = new Set<TSource>(comparer);
      foreach (TSource source1 in source)
      {
        if (set.Add(source1))
          yield return source1;
      }
    }

    /// <summary>
    /// Produces the set union of two sequences by using the default equality comparer.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the elements from both input sequences, excluding duplicates.
    /// </returns>
    /// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose distinct elements form the first set for the union.</param><param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose distinct elements form the second set for the union.</param><typeparam name="TSource">The type of the elements of the input sequences.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
    {
      if (first == null)
        throw Error.ArgumentNull("first");
      if (second == null)
        throw Error.ArgumentNull("second");
      else
        return Enumerable.UnionIterator<TSource>(first, second, (IEqualityComparer<TSource>) null);
    }

    /// <summary>
    /// Produces the set union of two sequences by using a specified <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the elements from both input sequences, excluding duplicates.
    /// </returns>
    /// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose distinct elements form the first set for the union.</param><param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose distinct elements form the second set for the union.</param><param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare values.</param><typeparam name="TSource">The type of the elements of the input sequences.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
    {
      if (first == null)
        throw Error.ArgumentNull("first");
      if (second == null)
        throw Error.ArgumentNull("second");
      else
        return Enumerable.UnionIterator<TSource>(first, second, comparer);
    }

    private static IEnumerable<TSource> UnionIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
    {
      Set<TSource> set = new Set<TSource>(comparer);
      foreach (TSource source in first)
      {
        if (set.Add(source))
          yield return source;
      }
      foreach (TSource source in second)
      {
        if (set.Add(source))
          yield return source;
      }
    }

    /// <summary>
    /// Produces the set intersection of two sequences by using the default equality comparer to compare values.
    /// </summary>
    /// 
    /// <returns>
    /// A sequence that contains the elements that form the set intersection of two sequences.
    /// </returns>
    /// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose distinct elements that also appear in <paramref name="second"/> will be returned.</param><param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose distinct elements that also appear in the first sequence will be returned.</param><typeparam name="TSource">The type of the elements of the input sequences.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
    {
      if (first == null)
        throw Error.ArgumentNull("first");
      if (second == null)
        throw Error.ArgumentNull("second");
      else
        return Enumerable.IntersectIterator<TSource>(first, second, (IEqualityComparer<TSource>) null);
    }

    /// <summary>
    /// Produces the set intersection of two sequences by using the specified <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare values.
    /// </summary>
    /// 
    /// <returns>
    /// A sequence that contains the elements that form the set intersection of two sequences.
    /// </returns>
    /// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose distinct elements that also appear in <paramref name="second"/> will be returned.</param><param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose distinct elements that also appear in the first sequence will be returned.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare values.</param><typeparam name="TSource">The type of the elements of the input sequences.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
    {
      if (first == null)
        throw Error.ArgumentNull("first");
      if (second == null)
        throw Error.ArgumentNull("second");
      else
        return Enumerable.IntersectIterator<TSource>(first, second, comparer);
    }

    private static IEnumerable<TSource> IntersectIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
    {
      Set<TSource> set = new Set<TSource>(comparer);
      foreach (TSource source in second)
        set.Add(source);
      foreach (TSource source in first)
      {
        if (set.Remove(source))
          yield return source;
      }
    }

    /// <summary>
    /// Produces the set difference of two sequences by using the default equality comparer to compare values.
    /// </summary>
    /// 
    /// <returns>
    /// A sequence that contains the set difference of the elements of two sequences.
    /// </returns>
    /// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements that are not also in <paramref name="second"/> will be returned.</param><param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param><typeparam name="TSource">The type of the elements of the input sequences.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
    {
      if (first == null)
        throw Error.ArgumentNull("first");
      if (second == null)
        throw Error.ArgumentNull("second");
      else
        return Enumerable.ExceptIterator<TSource>(first, second, (IEqualityComparer<TSource>) null);
    }

    /// <summary>
    /// Produces the set difference of two sequences by using the specified <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare values.
    /// </summary>
    /// 
    /// <returns>
    /// A sequence that contains the set difference of the elements of two sequences.
    /// </returns>
    /// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements that are not also in <paramref name="second"/> will be returned.</param><param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare values.</param><typeparam name="TSource">The type of the elements of the input sequences.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
    {
      if (first == null)
        throw Error.ArgumentNull("first");
      if (second == null)
        throw Error.ArgumentNull("second");
      else
        return Enumerable.ExceptIterator<TSource>(first, second, comparer);
    }

    private static IEnumerable<TSource> ExceptIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
    {
      Set<TSource> set = new Set<TSource>(comparer);
      foreach (TSource source in second)
        set.Add(source);
      foreach (TSource source in first)
      {
        if (set.Add(source))
          yield return source;
      }
    }

    /// <summary>
    /// Inverts the order of the elements in a sequence.
    /// </summary>
    /// 
    /// <returns>
    /// A sequence whose elements correspond to those of the input sequence in reverse order.
    /// </returns>
    /// <param name="source">A sequence of values to reverse.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return Enumerable.ReverseIterator<TSource>(source);
    }

    private static IEnumerable<TSource> ReverseIterator<TSource>(IEnumerable<TSource> source)
    {
      Buffer<TSource> buffer = new Buffer<TSource>(source);
      for (int i = buffer.count - 1; i >= 0; --i)
        yield return buffer.items[i];
    }

    /// <summary>
    /// Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.
    /// </summary>
    /// 
    /// <returns>
    /// true if the two source sequences are of equal length and their corresponding elements are equal according to the default equality comparer for their type; otherwise, false.
    /// </returns>
    /// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to compare to <paramref name="second"/>.</param><param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to compare to the first sequence.</param><typeparam name="TSource">The type of the elements of the input sequences.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
    {
      return Enumerable.SequenceEqual<TSource>(first, second, (IEqualityComparer<TSource>) null);
    }

    /// <summary>
    /// Determines whether two sequences are equal by comparing their elements by using a specified <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true if the two source sequences are of equal length and their corresponding elements compare equal according to <paramref name="comparer"/>; otherwise, false.
    /// </returns>
    /// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to compare to <paramref name="second"/>.</param><param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to compare to the first sequence.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to use to compare elements.</param><typeparam name="TSource">The type of the elements of the input sequences.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="first"/> or <paramref name="second"/> is null.</exception>
    [__DynamicallyInvokable]
    public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
    {
      if (comparer == null)
        comparer = (IEqualityComparer<TSource>) EqualityComparer<TSource>.Default;
      if (first == null)
        throw Error.ArgumentNull("first");
      if (second == null)
        throw Error.ArgumentNull("second");
      using (IEnumerator<TSource> enumerator1 = first.GetEnumerator())
      {
        using (IEnumerator<TSource> enumerator2 = second.GetEnumerator())
        {
          while (enumerator1.MoveNext())
          {
            if (!enumerator2.MoveNext() || !comparer.Equals(enumerator1.Current, enumerator2.Current))
              return false;
          }
          if (enumerator2.MoveNext())
            return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Returns the input typed as <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The input sequence typed as <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
    /// </returns>
    /// <param name="source">The sequence to type as <see cref="T:System.Collections.Generic.IEnumerable`1"/>.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable<TSource> source)
    {
      return source;
    }

    /// <summary>
    /// Creates an array from a <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// An array that contains the elements from the input sequence.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create an array from.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return new Buffer<TSource>(source).ToArray();
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.List`1"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.List`1"/> that contains elements from the input sequence.
    /// </returns>
    /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Collections.Generic.List`1"/> from.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return new List<TSource>(source);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/> according to a specified key selector function.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.Dictionary`2"/> that contains keys and values.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Collections.Generic.Dictionary`2"/> from.</param><param name="keySelector">A function to extract a key from each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.-or-<paramref name="keySelector"/> produces a key that is null.</exception><exception cref="T:System.ArgumentException"><paramref name="keySelector"/> produces duplicate keys for two elements.</exception>
    [__DynamicallyInvokable]
    public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      return Enumerable.ToDictionary<TSource, TKey, TSource>(source, keySelector, IdentityFunction<TSource>.Instance, (IEqualityComparer<TKey>) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/> according to a specified key selector function and key comparer.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.Dictionary`2"/> that contains keys and values.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Collections.Generic.Dictionary`2"/> from.</param><param name="keySelector">A function to extract a key from each element.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare keys.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the keys returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.-or-<paramref name="keySelector"/> produces a key that is null.</exception><exception cref="T:System.ArgumentException"><paramref name="keySelector"/> produces duplicate keys for two elements.</exception>
    [__DynamicallyInvokable]
    public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
    {
      return Enumerable.ToDictionary<TSource, TKey, TSource>(source, keySelector, IdentityFunction<TSource>.Instance, comparer);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/> according to specified key selector and element selector functions.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.Dictionary`2"/> that contains values of type <paramref name="TElement"/> selected from the input sequence.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Collections.Generic.Dictionary`2"/> from.</param><param name="keySelector">A function to extract a key from each element.</param><param name="elementSelector">A transform function to produce a result element value from each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null.-or-<paramref name="keySelector"/> produces a key that is null.</exception><exception cref="T:System.ArgumentException"><paramref name="keySelector"/> produces duplicate keys for two elements.</exception>
    [__DynamicallyInvokable]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
    {
      return Enumerable.ToDictionary<TSource, TKey, TElement>(source, keySelector, elementSelector, (IEqualityComparer<TKey>) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Collections.Generic.Dictionary`2"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/> according to a specified key selector function, a comparer, and an element selector function.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.Dictionary`2"/> that contains values of type <paramref name="TElement"/> selected from the input sequence.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Collections.Generic.Dictionary`2"/> from.</param><param name="keySelector">A function to extract a key from each element.</param><param name="elementSelector">A transform function to produce a result element value from each element.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare keys.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null.-or-<paramref name="keySelector"/> produces a key that is null.</exception><exception cref="T:System.ArgumentException"><paramref name="keySelector"/> produces duplicate keys for two elements.</exception>
    [__DynamicallyInvokable]
    public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (keySelector == null)
        throw Error.ArgumentNull("keySelector");
      if (elementSelector == null)
        throw Error.ArgumentNull("elementSelector");
      Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(comparer);
      foreach (TSource source1 in source)
        dictionary.Add(keySelector(source1), elementSelector(source1));
      return dictionary;
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Lookup`2"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/> according to a specified key selector function.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Lookup`2"/> that contains keys and values.
    /// </returns>
    /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Linq.Lookup`2"/> from.</param><param name="keySelector">A function to extract a key from each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
      return (ILookup<TKey, TSource>) Lookup<TKey, TSource>.Create<TSource>(source, keySelector, IdentityFunction<TSource>.Instance, (IEqualityComparer<TKey>) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Lookup`2"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/> according to a specified key selector function and key comparer.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Lookup`2"/> that contains keys and values.
    /// </returns>
    /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Linq.Lookup`2"/> from.</param><param name="keySelector">A function to extract a key from each element.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare keys.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
    {
      return (ILookup<TKey, TSource>) Lookup<TKey, TSource>.Create<TSource>(source, keySelector, IdentityFunction<TSource>.Instance, comparer);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Lookup`2"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/> according to specified key selector and element selector functions.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Lookup`2"/> that contains values of type <paramref name="TElement"/> selected from the input sequence.
    /// </returns>
    /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Linq.Lookup`2"/> from.</param><param name="keySelector">A function to extract a key from each element.</param><param name="elementSelector">A transform function to produce a result element value from each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
    {
      return (ILookup<TKey, TElement>) Lookup<TKey, TElement>.Create<TSource>(source, keySelector, elementSelector, (IEqualityComparer<TKey>) null);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Lookup`2"/> from an <see cref="T:System.Collections.Generic.IEnumerable`1"/> according to a specified key selector function, a comparer and an element selector function.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Linq.Lookup`2"/> that contains values of type <paramref name="TElement"/> selected from the input sequence.
    /// </returns>
    /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to create a <see cref="T:System.Linq.Lookup`2"/> from.</param><param name="keySelector">A function to extract a key from each element.</param><param name="elementSelector">A transform function to produce a result element value from each element.</param><param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare keys.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam><typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> or <paramref name="elementSelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
    {
      return (ILookup<TKey, TElement>) Lookup<TKey, TElement>.Create<TSource>(source, keySelector, elementSelector, comparer);
    }

    /// <summary>
    /// Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> object that contains the default value for the <paramref name="TSource"/> type if <paramref name="source"/> is empty; otherwise, <paramref name="source"/>.
    /// </returns>
    /// <param name="source">The sequence to return a default value for if it is empty.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
    {
      return Enumerable.DefaultIfEmpty<TSource>(source, default (TSource));
    }

    /// <summary>
    /// Returns the elements of the specified sequence or the specified value in a singleton collection if the sequence is empty.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains <paramref name="defaultValue"/> if <paramref name="source"/> is empty; otherwise, <paramref name="source"/>.
    /// </returns>
    /// <param name="source">The sequence to return the specified value for if it is empty.</param><param name="defaultValue">The value to return if the sequence is empty.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    [__DynamicallyInvokable]
    public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return Enumerable.DefaultIfEmptyIterator<TSource>(source, defaultValue);
    }

    private static IEnumerable<TSource> DefaultIfEmptyIterator<TSource>(IEnumerable<TSource> source, TSource defaultValue)
    {
      using (IEnumerator<TSource> enumerator = source.GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          do
          {
            yield return enumerator.Current;
          }
          while (enumerator.MoveNext());
        }
        else
          yield return defaultValue;
      }
    }

    /// <summary>
    /// Filters the elements of an <see cref="T:System.Collections.IEnumerable"/> based on a specified type.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains elements from the input sequence of type <paramref name="TResult"/>.
    /// </returns>
    /// <param name="source">The <see cref="T:System.Collections.IEnumerable"/> whose elements to filter.</param><typeparam name="TResult">The type to filter the elements of the sequence on.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return Enumerable.OfTypeIterator<TResult>(source);
    }

    private static IEnumerable<TResult> OfTypeIterator<TResult>(IEnumerable source)
    {
      foreach (object obj in source)
      {
        if (obj is TResult)
          yield return (TResult) obj;
      }
    }

    /// <summary>
    /// Casts the elements of an <see cref="T:System.Collections.IEnumerable"/> to the specified type.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains each element of the source sequence cast to the specified type.
    /// </returns>
    /// <param name="source">The <see cref="T:System.Collections.IEnumerable"/> that contains the elements to be cast to type <paramref name="TResult"/>.</param><typeparam name="TResult">The type to cast the elements of <paramref name="source"/> to.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidCastException">An element in the sequence cannot be cast to type <paramref name="TResult"/>.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
    {
      IEnumerable<TResult> enumerable = source as IEnumerable<TResult>;
      if (enumerable != null)
        return enumerable;
      if (source == null)
        throw Error.ArgumentNull("source");
      else
        return Enumerable.CastIterator<TResult>(source);
    }

    private static IEnumerable<TResult> CastIterator<TResult>(IEnumerable source)
    {
      foreach (object obj in source)
        yield return (TResult) obj;
    }

    /// <summary>
    /// Returns the first element of a sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The first element in the specified sequence.
    /// </returns>
    /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return the first element of.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
    [__DynamicallyInvokable]
    public static TSource First<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      IList<TSource> list = source as IList<TSource>;
      if (list != null)
      {
        if (list.Count > 0)
          return list[0];
      }
      else
      {
        using (IEnumerator<TSource> enumerator = source.GetEnumerator())
        {
          if (enumerator.MoveNext())
            return enumerator.Current;
        }
      }
      throw Error.NoElements();
    }

    /// <summary>
    /// Returns the first element in a sequence that satisfies a specified condition.
    /// </summary>
    /// 
    /// <returns>
    /// The first element in the sequence that passes the test in the specified predicate function.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return an element from.</param><param name="predicate">A function to test each element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception><exception cref="T:System.InvalidOperationException">No element satisfies the condition in <paramref name="predicate"/>.-or-The source sequence is empty.</exception>
    [__DynamicallyInvokable]
    public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      foreach (TSource source1 in source)
      {
        if (predicate(source1))
          return source1;
      }
      throw Error.NoMatch();
    }

    /// <summary>
    /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    /// 
    /// <returns>
    /// default(<paramref name="TSource"/>) if <paramref name="source"/> is empty; otherwise, the first element in <paramref name="source"/>.
    /// </returns>
    /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return the first element of.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      IList<TSource> list = source as IList<TSource>;
      if (list != null)
      {
        if (list.Count > 0)
          return list[0];
      }
      else
      {
        using (IEnumerator<TSource> enumerator = source.GetEnumerator())
        {
          if (enumerator.MoveNext())
            return enumerator.Current;
        }
      }
      return default (TSource);
    }

    /// <summary>
    /// Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    /// 
    /// <returns>
    /// default(<paramref name="TSource"/>) if <paramref name="source"/> is empty or if no element passes the test specified by <paramref name="predicate"/>; otherwise, the first element in <paramref name="source"/> that passes the test specified by <paramref name="predicate"/>.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return an element from.</param><param name="predicate">A function to test each element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      foreach (TSource source1 in source)
      {
        if (predicate(source1))
          return source1;
      }
      return default (TSource);
    }

    /// <summary>
    /// Returns the last element of a sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The value at the last position in the source sequence.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return the last element of.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
    [__DynamicallyInvokable]
    public static TSource Last<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      IList<TSource> list = source as IList<TSource>;
      if (list != null)
      {
        int count = list.Count;
        if (count > 0)
          return list[count - 1];
      }
      else
      {
        using (IEnumerator<TSource> enumerator = source.GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            TSource current;
            do
            {
              current = enumerator.Current;
            }
            while (enumerator.MoveNext());
            return current;
          }
        }
      }
      throw Error.NoElements();
    }

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition.
    /// </summary>
    /// 
    /// <returns>
    /// The last element in the sequence that passes the test in the specified predicate function.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return an element from.</param><param name="predicate">A function to test each element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception><exception cref="T:System.InvalidOperationException">No element satisfies the condition in <paramref name="predicate"/>.-or-The source sequence is empty.</exception>
    [__DynamicallyInvokable]
    public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      TSource source1 = default (TSource);
      bool flag = false;
      foreach (TSource source2 in source)
      {
        if (predicate(source2))
        {
          source1 = source2;
          flag = true;
        }
      }
      if (flag)
        return source1;
      else
        throw Error.NoMatch();
    }

    /// <summary>
    /// Returns the last element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    /// 
    /// <returns>
    /// default(<paramref name="TSource"/>) if the source sequence is empty; otherwise, the last element in the <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return the last element of.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      IList<TSource> list = source as IList<TSource>;
      if (list != null)
      {
        int count = list.Count;
        if (count > 0)
          return list[count - 1];
      }
      else
      {
        using (IEnumerator<TSource> enumerator = source.GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            TSource current;
            do
            {
              current = enumerator.Current;
            }
            while (enumerator.MoveNext());
            return current;
          }
        }
      }
      return default (TSource);
    }

    /// <summary>
    /// Returns the last element of a sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    /// 
    /// <returns>
    /// default(<paramref name="TSource"/>) if the sequence is empty or if no elements pass the test in the predicate function; otherwise, the last element that passes the test in the predicate function.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return an element from.</param><param name="predicate">A function to test each element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      TSource source1 = default (TSource);
      foreach (TSource source2 in source)
      {
        if (predicate(source2))
          source1 = source2;
      }
      return source1;
    }

    /// <summary>
    /// Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The single element of the input sequence.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return the single element of.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException">The input sequence contains more than one element.-or-The input sequence is empty.</exception>
    [__DynamicallyInvokable]
    public static TSource Single<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      IList<TSource> list = source as IList<TSource>;
      if (list != null)
      {
        switch (list.Count)
        {
          case 0:
            throw Error.NoElements();
          case 1:
            return list[0];
        }
      }
      else
      {
        using (IEnumerator<TSource> enumerator = source.GetEnumerator())
        {
          if (!enumerator.MoveNext())
            throw Error.NoElements();
          TSource current = enumerator.Current;
          if (!enumerator.MoveNext())
            return current;
        }
      }
      throw Error.MoreThanOneElement();
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if more than one such element exists.
    /// </summary>
    /// 
    /// <returns>
    /// The single element of the input sequence that satisfies a condition.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return a single element from.</param><param name="predicate">A function to test an element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception><exception cref="T:System.InvalidOperationException">No element satisfies the condition in <paramref name="predicate"/>.-or-More than one element satisfies the condition in <paramref name="predicate"/>.-or-The source sequence is empty.</exception>
    [__DynamicallyInvokable]
    public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      TSource source1 = default (TSource);
      long num = 0L;
      foreach (TSource source2 in source)
      {
        if (predicate(source2))
        {
          source1 = source2;
          checked { ++num; }
        }
      }
      switch (num)
      {
        case 0L:
          throw Error.NoMatch();
        case 1L:
          return source1;
        default:
          throw Error.MoreThanOneMatch();
      }
    }

    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The single element of the input sequence, or default(<paramref name="TSource"/>) if the sequence contains no elements.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return the single element of.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException">The input sequence contains more than one element.</exception>
    [__DynamicallyInvokable]
    public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      IList<TSource> list = source as IList<TSource>;
      if (list != null)
      {
        switch (list.Count)
        {
          case 0:
            return default (TSource);
          case 1:
            return list[0];
        }
      }
      else
      {
        using (IEnumerator<TSource> enumerator = source.GetEnumerator())
        {
          if (!enumerator.MoveNext())
            return default (TSource);
          TSource current = enumerator.Current;
          if (!enumerator.MoveNext())
            return current;
        }
      }
      throw Error.MoreThanOneElement();
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.
    /// </summary>
    /// 
    /// <returns>
    /// The single element of the input sequence that satisfies the condition, or default(<paramref name="TSource"/>) if no such element is found.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return a single element from.</param><param name="predicate">A function to test an element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      TSource source1 = default (TSource);
      long num = 0L;
      foreach (TSource source2 in source)
      {
        if (predicate(source2))
        {
          source1 = source2;
          checked { ++num; }
        }
      }
      switch (num)
      {
        case 0L:
          return default (TSource);
        case 1L:
          return source1;
        default:
          throw Error.MoreThanOneMatch();
      }
    }

    /// <summary>
    /// Returns the element at a specified index in a sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The element at the specified position in the source sequence.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return an element from.</param><param name="index">The zero-based index of the element to retrieve.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than or equal to the number of elements in <paramref name="source"/>.</exception>
    [__DynamicallyInvokable]
    public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      IList<TSource> list = source as IList<TSource>;
      if (list != null)
        return list[index];
      if (index < 0)
        throw Error.ArgumentOutOfRange("index");
      using (IEnumerator<TSource> enumerator = source.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          if (index == 0)
            return enumerator.Current;
          --index;
        }
        throw Error.ArgumentOutOfRange("index");
      }
    }

    /// <summary>
    /// Returns the element at a specified index in a sequence or a default value if the index is out of range.
    /// </summary>
    /// 
    /// <returns>
    /// default(<paramref name="TSource"/>) if the index is outside the bounds of the source sequence; otherwise, the element at the specified position in the source sequence.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return an element from.</param><param name="index">The zero-based index of the element to retrieve.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (index >= 0)
      {
        IList<TSource> list = source as IList<TSource>;
        if (list != null)
        {
          if (index < list.Count)
            return list[index];
        }
        else
        {
          foreach (TSource source1 in source)
          {
            if (index == 0)
              return source1;
            --index;
          }
        }
      }
      return default (TSource);
    }

    /// <summary>
    /// Generates a sequence of integral numbers within a specified range.
    /// </summary>
    /// 
    /// <returns>
    /// An IEnumerable&lt;Int32&gt; in C# or IEnumerable(Of Int32) in Visual Basic that contains a range of sequential integral numbers.
    /// </returns>
    /// <param name="start">The value of the first integer in the sequence.</param><param name="count">The number of sequential integers to generate.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="count"/> is less than 0.-or-<paramref name="start"/> + <paramref name="count"/> -1 is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<int> Range(int start, int count)
    {
      long num = (long) start + (long) count - 1L;
      if (count < 0 || num > (long) int.MaxValue)
        throw Error.ArgumentOutOfRange("count");
      else
        return Enumerable.RangeIterator(start, count);
    }

    /// <summary>
    /// Generates a sequence that contains one repeated value.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains a repeated value.
    /// </returns>
    /// <param name="element">The value to be repeated.</param><param name="count">The number of times to repeat the value in the generated sequence.</param><typeparam name="TResult">The type of the value to be repeated in the result sequence.</typeparam><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="count"/> is less than 0.</exception>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
    {
      if (count < 0)
        throw Error.ArgumentOutOfRange("count");
      else
        return Enumerable.RepeatIterator<TResult>(element, count);
    }

    private static IEnumerable<TResult> RepeatIterator<TResult>(TResult element, int count)
    {
      for (int i = 0; i < count; ++i)
        yield return element;
    }

    /// <summary>
    /// Returns an empty <see cref="T:System.Collections.Generic.IEnumerable`1"/> that has the specified type argument.
    /// </summary>
    /// 
    /// <returns>
    /// An empty <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose type argument is <paramref name="TResult"/>.
    /// </returns>
    /// <typeparam name="TResult">The type to assign to the type parameter of the returned generic <see cref="T:System.Collections.Generic.IEnumerable`1"/>.</typeparam>
    [__DynamicallyInvokable]
    public static IEnumerable<TResult> Empty<TResult>()
    {
      return EmptyEnumerable<TResult>.Instance;
    }

    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    /// 
    /// <returns>
    /// true if the source sequence contains any elements; otherwise, false.
    /// </returns>
    /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1"/> to check for emptiness.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static bool Any<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      using (IEnumerator<TSource> enumerator = source.GetEnumerator())
      {
        if (enumerator.MoveNext())
          return true;
      }
      return false;
    }

    /// <summary>
    /// Determines whether any element of a sequence satisfies a condition.
    /// </summary>
    /// 
    /// <returns>
    /// true if any elements in the source sequence pass the test in the specified predicate; otherwise, false.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose elements to apply the predicate to.</param><param name="predicate">A function to test each element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
    [__DynamicallyInvokable]
    public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      foreach (TSource source1 in source)
      {
        if (predicate(source1))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Determines whether all elements of a sequence satisfy a condition.
    /// </summary>
    /// 
    /// <returns>
    /// true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the elements to apply the predicate to.</param><param name="predicate">A function to test each element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception>
    [__DynamicallyInvokable]
    public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      foreach (TSource source1 in source)
      {
        if (!predicate(source1))
          return false;
      }
      return true;
    }

    /// <summary>
    /// Returns the number of elements in a sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The number of elements in the input sequence.
    /// </returns>
    /// <param name="source">A sequence that contains elements to be counted.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.OverflowException">The number of elements in <paramref name="source"/> is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static int Count<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      ICollection<TSource> collection1 = source as ICollection<TSource>;
      if (collection1 != null)
        return collection1.Count;
      ICollection collection2 = source as ICollection;
      if (collection2 != null)
        return collection2.Count;
      int num = 0;
      using (IEnumerator<TSource> enumerator = source.GetEnumerator())
      {
        while (enumerator.MoveNext())
          checked { ++num; }
      }
      return num;
    }

    /// <summary>
    /// Returns a number that represents how many elements in the specified sequence satisfy a condition.
    /// </summary>
    /// 
    /// <returns>
    /// A number that represents how many elements in the sequence satisfy the condition in the predicate function.
    /// </returns>
    /// <param name="source">A sequence that contains elements to be tested and counted.</param><param name="predicate">A function to test each element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception><exception cref="T:System.OverflowException">The number of elements in <paramref name="source"/> is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      int num = 0;
      foreach (TSource source1 in source)
      {
        if (predicate(source1))
          checked { ++num; }
      }
      return num;
    }

    /// <summary>
    /// Returns an <see cref="T:System.Int64"/> that represents the total number of elements in a sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The number of elements in the source sequence.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the elements to be counted.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.OverflowException">The number of elements exceeds <see cref="F:System.Int64.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static long LongCount<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      long num = 0L;
      using (IEnumerator<TSource> enumerator = source.GetEnumerator())
      {
        while (enumerator.MoveNext())
          checked { ++num; }
      }
      return num;
    }

    /// <summary>
    /// Returns an <see cref="T:System.Int64"/> that represents how many elements in a sequence satisfy a condition.
    /// </summary>
    /// 
    /// <returns>
    /// A number that represents how many elements in the sequence satisfy the condition in the predicate function.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the elements to be counted.</param><param name="predicate">A function to test each element for a condition.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null.</exception><exception cref="T:System.OverflowException">The number of matching elements exceeds <see cref="F:System.Int64.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (predicate == null)
        throw Error.ArgumentNull("predicate");
      long num = 0L;
      foreach (TSource source1 in source)
      {
        if (predicate(source1))
          checked { ++num; }
      }
      return num;
    }

    /// <summary>
    /// Determines whether a sequence contains a specified element by using the default equality comparer.
    /// </summary>
    /// 
    /// <returns>
    /// true if the source sequence contains an element that has the specified value; otherwise, false.
    /// </returns>
    /// <param name="source">A sequence in which to locate a value.</param><param name="value">The value to locate in the sequence.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
    {
      ICollection<TSource> collection = source as ICollection<TSource>;
      if (collection != null)
        return collection.Contains(value);
      else
        return Enumerable.Contains<TSource>(source, value, (IEqualityComparer<TSource>) null);
    }

    /// <summary>
    /// Determines whether a sequence contains a specified element by using a specified <see cref="T:System.Collections.Generic.IEqualityComparer`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true if the source sequence contains an element that has the specified value; otherwise, false.
    /// </returns>
    /// <param name="source">A sequence in which to locate a value.</param><param name="value">The value to locate in the sequence.</param><param name="comparer">An equality comparer to compare values.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
    {
      if (comparer == null)
        comparer = (IEqualityComparer<TSource>) EqualityComparer<TSource>.Default;
      if (source == null)
        throw Error.ArgumentNull("source");
      foreach (TSource x in source)
      {
        if (comparer.Equals(x, value))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Applies an accumulator function over a sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The final accumulator value.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to aggregate over.</param><param name="func">An accumulator function to be invoked on each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="func"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (func == null)
        throw Error.ArgumentNull("func");
      using (IEnumerator<TSource> enumerator = source.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          throw Error.NoElements();
        TSource source1 = enumerator.Current;
        while (enumerator.MoveNext())
          source1 = func(source1, enumerator.Current);
        return source1;
      }
    }

    /// <summary>
    /// Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value.
    /// </summary>
    /// 
    /// <returns>
    /// The final accumulator value.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to aggregate over.</param><param name="seed">The initial accumulator value.</param><param name="func">An accumulator function to be invoked on each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TAccumulate">The type of the accumulator value.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="func"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (func == null)
        throw Error.ArgumentNull("func");
      TAccumulate accumulate = seed;
      foreach (TSource source1 in source)
        accumulate = func(accumulate, source1);
      return accumulate;
    }

    /// <summary>
    /// Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value, and the specified function is used to select the result value.
    /// </summary>
    /// 
    /// <returns>
    /// The transformed final accumulator value.
    /// </returns>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to aggregate over.</param><param name="seed">The initial accumulator value.</param><param name="func">An accumulator function to be invoked on each element.</param><param name="resultSelector">A function to transform the final accumulator value into the result value.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TAccumulate">The type of the accumulator value.</typeparam><typeparam name="TResult">The type of the resulting value.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="func"/> or <paramref name="resultSelector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      if (func == null)
        throw Error.ArgumentNull("func");
      if (resultSelector == null)
        throw Error.ArgumentNull("resultSelector");
      TAccumulate accumulate = seed;
      foreach (TSource source1 in source)
        accumulate = func(accumulate, source1);
      return resultSelector(accumulate);
    }

    /// <summary>
    /// Computes the sum of a sequence of <see cref="T:System.Int32"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the values in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Int32"/> values to calculate the sum of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static int Sum(this IEnumerable<int> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      int num1 = 0;
      foreach (int num2 in source)
        checked { num1 += num2; }
      return num1;
    }

    /// <summary>
    /// Computes the sum of a sequence of nullable <see cref="T:System.Int32"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the values in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Int32"/> values to calculate the sum of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static int? Sum(this IEnumerable<int?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      int num = 0;
      foreach (int? nullable in source)
      {
        if (nullable.HasValue)
          checked { num += nullable.GetValueOrDefault(); }
      }
      return new int?(num);
    }

    /// <summary>
    /// Computes the sum of a sequence of <see cref="T:System.Int64"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the values in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Int64"/> values to calculate the sum of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static long Sum(this IEnumerable<long> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      long num1 = 0L;
      foreach (long num2 in source)
        checked { num1 += num2; }
      return num1;
    }

    /// <summary>
    /// Computes the sum of a sequence of nullable <see cref="T:System.Int64"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the values in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Int64"/> values to calculate the sum of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static long? Sum(this IEnumerable<long?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      long num = 0L;
      foreach (long? nullable in source)
      {
        if (nullable.HasValue)
          checked { num += nullable.GetValueOrDefault(); }
      }
      return new long?(num);
    }

    /// <summary>
    /// Computes the sum of a sequence of <see cref="T:System.Single"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the values in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Single"/> values to calculate the sum of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static float Sum(this IEnumerable<float> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      double num1 = 0.0;
      foreach (float num2 in source)
        num1 += (double) num2;
      return (float) num1;
    }

    /// <summary>
    /// Computes the sum of a sequence of nullable <see cref="T:System.Single"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the values in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Single"/> values to calculate the sum of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static float? Sum(this IEnumerable<float?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      double num = 0.0;
      foreach (float? nullable in source)
      {
        if (nullable.HasValue)
          num += (double) nullable.GetValueOrDefault();
      }
      return new float?((float) num);
    }

    /// <summary>
    /// Computes the sum of a sequence of <see cref="T:System.Double"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the values in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Double"/> values to calculate the sum of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static double Sum(this IEnumerable<double> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      double num1 = 0.0;
      foreach (double num2 in source)
        num1 += num2;
      return num1;
    }

    /// <summary>
    /// Computes the sum of a sequence of nullable <see cref="T:System.Double"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the values in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Double"/> values to calculate the sum of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static double? Sum(this IEnumerable<double?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      double num = 0.0;
      foreach (double? nullable in source)
      {
        if (nullable.HasValue)
          num += nullable.GetValueOrDefault();
      }
      return new double?(num);
    }

    /// <summary>
    /// Computes the sum of a sequence of <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the values in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Decimal"/> values to calculate the sum of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static Decimal Sum(this IEnumerable<Decimal> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      Decimal num1 = new Decimal(0);
      foreach (Decimal num2 in source)
        num1 += num2;
      return num1;
    }

    /// <summary>
    /// Computes the sum of a sequence of nullable <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the values in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Decimal"/> values to calculate the sum of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static Decimal? Sum(this IEnumerable<Decimal?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      Decimal num = new Decimal(0);
      foreach (Decimal? nullable in source)
      {
        if (nullable.HasValue)
          num += nullable.GetValueOrDefault();
      }
      return new Decimal?(num);
    }

    /// <summary>
    /// Computes the sum of the sequence of <see cref="T:System.Int32"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the projected values.
    /// </returns>
    /// <param name="source">A sequence of values that are used to calculate a sum.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
    {
      return Enumerable.Sum(Enumerable.Select<TSource, int>(source, selector));
    }

    /// <summary>
    /// Computes the sum of the sequence of nullable <see cref="T:System.Int32"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the projected values.
    /// </returns>
    /// <param name="source">A sequence of values that are used to calculate a sum.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static int? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
    {
      return Enumerable.Sum(Enumerable.Select<TSource, int?>(source, selector));
    }

    /// <summary>
    /// Computes the sum of the sequence of <see cref="T:System.Int64"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the projected values.
    /// </returns>
    /// <param name="source">A sequence of values that are used to calculate a sum.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
    {
      return Enumerable.Sum(Enumerable.Select<TSource, long>(source, selector));
    }

    /// <summary>
    /// Computes the sum of the sequence of nullable <see cref="T:System.Int64"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the projected values.
    /// </returns>
    /// <param name="source">A sequence of values that are used to calculate a sum.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static long? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
    {
      return Enumerable.Sum(Enumerable.Select<TSource, long?>(source, selector));
    }

    /// <summary>
    /// Computes the sum of the sequence of <see cref="T:System.Single"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the projected values.
    /// </returns>
    /// <param name="source">A sequence of values that are used to calculate a sum.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static float Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
    {
      return Enumerable.Sum(Enumerable.Select<TSource, float>(source, selector));
    }

    /// <summary>
    /// Computes the sum of the sequence of nullable <see cref="T:System.Single"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the projected values.
    /// </returns>
    /// <param name="source">A sequence of values that are used to calculate a sum.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static float? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
    {
      return Enumerable.Sum(Enumerable.Select<TSource, float?>(source, selector));
    }

    /// <summary>
    /// Computes the sum of the sequence of <see cref="T:System.Double"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the projected values.
    /// </returns>
    /// <param name="source">A sequence of values that are used to calculate a sum.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
    {
      return Enumerable.Sum(Enumerable.Select<TSource, double>(source, selector));
    }

    /// <summary>
    /// Computes the sum of the sequence of nullable <see cref="T:System.Double"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the projected values.
    /// </returns>
    /// <param name="source">A sequence of values that are used to calculate a sum.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static double? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
    {
      return Enumerable.Sum(Enumerable.Select<TSource, double?>(source, selector));
    }

    /// <summary>
    /// Computes the sum of the sequence of <see cref="T:System.Decimal"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the projected values.
    /// </returns>
    /// <param name="source">A sequence of values that are used to calculate a sum.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static Decimal Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal> selector)
    {
      return Enumerable.Sum(Enumerable.Select<TSource, Decimal>(source, selector));
    }

    /// <summary>
    /// Computes the sum of the sequence of nullable <see cref="T:System.Decimal"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The sum of the projected values.
    /// </returns>
    /// <param name="source">A sequence of values that are used to calculate a sum.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static Decimal? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal?> selector)
    {
      return Enumerable.Sum(Enumerable.Select<TSource, Decimal?>(source, selector));
    }

    /// <summary>
    /// Returns the minimum value in a sequence of <see cref="T:System.Int32"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Int32"/> values to determine the minimum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static int Min(this IEnumerable<int> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      int num1 = 0;
      bool flag = false;
      foreach (int num2 in source)
      {
        if (flag)
        {
          if (num2 < num1)
            num1 = num2;
        }
        else
        {
          num1 = num2;
          flag = true;
        }
      }
      if (flag)
        return num1;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Returns the minimum value in a sequence of nullable <see cref="T:System.Int32"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// A value of type Nullable&lt;Int32&gt; in C# or Nullable(Of Int32) in Visual Basic that corresponds to the minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Int32"/> values to determine the minimum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static int? Min(this IEnumerable<int?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      int? nullable1 = new int?();
      foreach (int? nullable2 in source)
      {
        if (nullable1.HasValue)
        {
          int? nullable3 = nullable2;
          int? nullable4 = nullable1;
          if ((nullable3.GetValueOrDefault() >= nullable4.GetValueOrDefault() ? 0 : (nullable3.HasValue & nullable4.HasValue ? 1 : 0)) == 0)
            continue;
        }
        nullable1 = nullable2;
      }
      return nullable1;
    }

    /// <summary>
    /// Returns the minimum value in a sequence of <see cref="T:System.Int64"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Int64"/> values to determine the minimum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static long Min(this IEnumerable<long> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      long num1 = 0L;
      bool flag = false;
      foreach (long num2 in source)
      {
        if (flag)
        {
          if (num2 < num1)
            num1 = num2;
        }
        else
        {
          num1 = num2;
          flag = true;
        }
      }
      if (flag)
        return num1;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Returns the minimum value in a sequence of nullable <see cref="T:System.Int64"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// A value of type Nullable&lt;Int64&gt; in C# or Nullable(Of Int64) in Visual Basic that corresponds to the minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Int64"/> values to determine the minimum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static long? Min(this IEnumerable<long?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      long? nullable1 = new long?();
      foreach (long? nullable2 in source)
      {
        if (nullable1.HasValue)
        {
          long? nullable3 = nullable2;
          long? nullable4 = nullable1;
          if ((nullable3.GetValueOrDefault() >= nullable4.GetValueOrDefault() ? 0 : (nullable3.HasValue & nullable4.HasValue ? 1 : 0)) == 0)
            continue;
        }
        nullable1 = nullable2;
      }
      return nullable1;
    }

    /// <summary>
    /// Returns the minimum value in a sequence of <see cref="T:System.Single"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Single"/> values to determine the minimum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static float Min(this IEnumerable<float> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      float num = 0.0f;
      bool flag = false;
      foreach (float f in source)
      {
        if (flag)
        {
          if ((double) f < (double) num || float.IsNaN(f))
            num = f;
        }
        else
        {
          num = f;
          flag = true;
        }
      }
      if (flag)
        return num;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Returns the minimum value in a sequence of nullable <see cref="T:System.Single"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// A value of type Nullable&lt;Single&gt; in C# or Nullable(Of Single) in Visual Basic that corresponds to the minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Single"/> values to determine the minimum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static float? Min(this IEnumerable<float?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      float? nullable1 = new float?();
      foreach (float? nullable2 in source)
      {
        if (nullable2.HasValue)
        {
          if (nullable1.HasValue)
          {
            float? nullable3 = nullable2;
            float? nullable4 = nullable1;
            if (((double) nullable3.GetValueOrDefault() >= (double) nullable4.GetValueOrDefault() ? 0 : (nullable3.HasValue & nullable4.HasValue ? 1 : 0)) == 0 && !float.IsNaN(nullable2.Value))
              continue;
          }
          nullable1 = nullable2;
        }
      }
      return nullable1;
    }

    /// <summary>
    /// Returns the minimum value in a sequence of <see cref="T:System.Double"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Double"/> values to determine the minimum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static double Min(this IEnumerable<double> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      double num = 0.0;
      bool flag = false;
      foreach (double d in source)
      {
        if (flag)
        {
          if (d < num || double.IsNaN(d))
            num = d;
        }
        else
        {
          num = d;
          flag = true;
        }
      }
      if (flag)
        return num;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Returns the minimum value in a sequence of nullable <see cref="T:System.Double"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// A value of type Nullable&lt;Double&gt; in C# or Nullable(Of Double) in Visual Basic that corresponds to the minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Double"/> values to determine the minimum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static double? Min(this IEnumerable<double?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      double? nullable1 = new double?();
      foreach (double? nullable2 in source)
      {
        if (nullable2.HasValue)
        {
          if (nullable1.HasValue)
          {
            double? nullable3 = nullable2;
            double? nullable4 = nullable1;
            if ((nullable3.GetValueOrDefault() >= nullable4.GetValueOrDefault() ? 0 : (nullable3.HasValue & nullable4.HasValue ? 1 : 0)) == 0 && !double.IsNaN(nullable2.Value))
              continue;
          }
          nullable1 = nullable2;
        }
      }
      return nullable1;
    }

    /// <summary>
    /// Returns the minimum value in a sequence of <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Decimal"/> values to determine the minimum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static Decimal Min(this IEnumerable<Decimal> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      Decimal num1 = new Decimal(0);
      bool flag = false;
      foreach (Decimal num2 in source)
      {
        if (flag)
        {
          if (num2 < num1)
            num1 = num2;
        }
        else
        {
          num1 = num2;
          flag = true;
        }
      }
      if (flag)
        return num1;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Returns the minimum value in a sequence of nullable <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// A value of type Nullable&lt;Decimal&gt; in C# or Nullable(Of Decimal) in Visual Basic that corresponds to the minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Decimal"/> values to determine the minimum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static Decimal? Min(this IEnumerable<Decimal?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      Decimal? nullable1 = new Decimal?();
      foreach (Decimal? nullable2 in source)
      {
        if (nullable1.HasValue)
        {
          Decimal? nullable3 = nullable2;
          Decimal? nullable4 = nullable1;
          if ((!(nullable3.GetValueOrDefault() < nullable4.GetValueOrDefault()) ? 0 : (nullable3.HasValue & nullable4.HasValue ? 1 : 0)) == 0)
            continue;
        }
        nullable1 = nullable2;
      }
      return nullable1;
    }

    /// <summary>
    /// Returns the minimum value in a generic sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the minimum value of.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TSource Min<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      Comparer<TSource> @default = Comparer<TSource>.Default;
      TSource y = default (TSource);
      if ((object) y == null)
      {
        foreach (TSource x in source)
        {
          if ((object) x != null && ((object) y == null || @default.Compare(x, y) < 0))
            y = x;
        }
        return y;
      }
      else
      {
        bool flag = false;
        foreach (TSource x in source)
        {
          if (flag)
          {
            if (@default.Compare(x, y) < 0)
              y = x;
          }
          else
          {
            y = x;
            flag = true;
          }
        }
        if (flag)
          return y;
        else
          throw Error.NoElements();
      }
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Int32"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the minimum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
    {
      return Enumerable.Min(Enumerable.Select<TSource, int>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Int32"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The value of type Nullable&lt;Int32&gt; in C# or Nullable(Of Int32) in Visual Basic that corresponds to the minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the minimum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static int? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
    {
      return Enumerable.Min(Enumerable.Select<TSource, int?>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Int64"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the minimum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static long Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
    {
      return Enumerable.Min(Enumerable.Select<TSource, long>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Int64"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The value of type Nullable&lt;Int64&gt; in C# or Nullable(Of Int64) in Visual Basic that corresponds to the minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the minimum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
    {
      return Enumerable.Min(Enumerable.Select<TSource, long?>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Single"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the minimum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static float Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
    {
      return Enumerable.Min(Enumerable.Select<TSource, float>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Single"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The value of type Nullable&lt;Single&gt; in C# or Nullable(Of Single) in Visual Basic that corresponds to the minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the minimum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
    {
      return Enumerable.Min(Enumerable.Select<TSource, float?>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Double"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the minimum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static double Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
    {
      return Enumerable.Min(Enumerable.Select<TSource, double>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Double"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The value of type Nullable&lt;Double&gt; in C# or Nullable(Of Double) in Visual Basic that corresponds to the minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the minimum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
    {
      return Enumerable.Min(Enumerable.Select<TSource, double?>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Decimal"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the minimum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static Decimal Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal> selector)
    {
      return Enumerable.Min(Enumerable.Select<TSource, Decimal>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Decimal"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The value of type Nullable&lt;Decimal&gt; in C# or Nullable(Of Decimal) in Visual Basic that corresponds to the minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the minimum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static Decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal?> selector)
    {
      return Enumerable.Min(Enumerable.Select<TSource, Decimal?>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a generic sequence and returns the minimum resulting value.
    /// </summary>
    /// 
    /// <returns>
    /// The minimum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the minimum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
      return Enumerable.Min<TResult>(Enumerable.Select<TSource, TResult>(source, selector));
    }

    /// <summary>
    /// Returns the maximum value in a sequence of <see cref="T:System.Int32"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Int32"/> values to determine the maximum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static int Max(this IEnumerable<int> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      int num1 = 0;
      bool flag = false;
      foreach (int num2 in source)
      {
        if (flag)
        {
          if (num2 > num1)
            num1 = num2;
        }
        else
        {
          num1 = num2;
          flag = true;
        }
      }
      if (flag)
        return num1;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Returns the maximum value in a sequence of nullable <see cref="T:System.Int32"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// A value of type Nullable&lt;Int32&gt; in C# or Nullable(Of Int32) in Visual Basic that corresponds to the maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Int32"/> values to determine the maximum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static int? Max(this IEnumerable<int?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      int? nullable1 = new int?();
      foreach (int? nullable2 in source)
      {
        if (nullable1.HasValue)
        {
          int? nullable3 = nullable2;
          int? nullable4 = nullable1;
          if ((nullable3.GetValueOrDefault() <= nullable4.GetValueOrDefault() ? 0 : (nullable3.HasValue & nullable4.HasValue ? 1 : 0)) == 0)
            continue;
        }
        nullable1 = nullable2;
      }
      return nullable1;
    }

    /// <summary>
    /// Returns the maximum value in a sequence of <see cref="T:System.Int64"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Int64"/> values to determine the maximum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static long Max(this IEnumerable<long> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      long num1 = 0L;
      bool flag = false;
      foreach (long num2 in source)
      {
        if (flag)
        {
          if (num2 > num1)
            num1 = num2;
        }
        else
        {
          num1 = num2;
          flag = true;
        }
      }
      if (flag)
        return num1;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Returns the maximum value in a sequence of nullable <see cref="T:System.Int64"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// A value of type Nullable&lt;Int64&gt; in C# or Nullable(Of Int64) in Visual Basic that corresponds to the maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Int64"/> values to determine the maximum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static long? Max(this IEnumerable<long?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      long? nullable1 = new long?();
      foreach (long? nullable2 in source)
      {
        if (nullable1.HasValue)
        {
          long? nullable3 = nullable2;
          long? nullable4 = nullable1;
          if ((nullable3.GetValueOrDefault() <= nullable4.GetValueOrDefault() ? 0 : (nullable3.HasValue & nullable4.HasValue ? 1 : 0)) == 0)
            continue;
        }
        nullable1 = nullable2;
      }
      return nullable1;
    }

    /// <summary>
    /// Returns the maximum value in a sequence of <see cref="T:System.Double"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Double"/> values to determine the maximum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static double Max(this IEnumerable<double> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      double d = 0.0;
      bool flag = false;
      foreach (double num in source)
      {
        if (flag)
        {
          if (num > d || double.IsNaN(d))
            d = num;
        }
        else
        {
          d = num;
          flag = true;
        }
      }
      if (flag)
        return d;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Returns the maximum value in a sequence of nullable <see cref="T:System.Double"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// A value of type Nullable&lt;Double&gt; in C# or Nullable(Of Double) in Visual Basic that corresponds to the maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Double"/> values to determine the maximum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static double? Max(this IEnumerable<double?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      double? nullable1 = new double?();
      foreach (double? nullable2 in source)
      {
        if (nullable2.HasValue)
        {
          if (nullable1.HasValue)
          {
            double? nullable3 = nullable2;
            double? nullable4 = nullable1;
            if ((nullable3.GetValueOrDefault() <= nullable4.GetValueOrDefault() ? 0 : (nullable3.HasValue & nullable4.HasValue ? 1 : 0)) == 0 && !double.IsNaN(nullable1.Value))
              continue;
          }
          nullable1 = nullable2;
        }
      }
      return nullable1;
    }

    /// <summary>
    /// Returns the maximum value in a sequence of <see cref="T:System.Single"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Single"/> values to determine the maximum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static float Max(this IEnumerable<float> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      float num1 = 0.0f;
      bool flag = false;
      foreach (float num2 in source)
      {
        if (flag)
        {
          if ((double) num2 > (double) num1 || double.IsNaN((double) num1))
            num1 = num2;
        }
        else
        {
          num1 = num2;
          flag = true;
        }
      }
      if (flag)
        return num1;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Returns the maximum value in a sequence of nullable <see cref="T:System.Single"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// A value of type Nullable&lt;Single&gt; in C# or Nullable(Of Single) in Visual Basic that corresponds to the maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Single"/> values to determine the maximum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static float? Max(this IEnumerable<float?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      float? nullable1 = new float?();
      foreach (float? nullable2 in source)
      {
        if (nullable2.HasValue)
        {
          if (nullable1.HasValue)
          {
            float? nullable3 = nullable2;
            float? nullable4 = nullable1;
            if (((double) nullable3.GetValueOrDefault() <= (double) nullable4.GetValueOrDefault() ? 0 : (nullable3.HasValue & nullable4.HasValue ? 1 : 0)) == 0 && !float.IsNaN(nullable1.Value))
              continue;
          }
          nullable1 = nullable2;
        }
      }
      return nullable1;
    }

    /// <summary>
    /// Returns the maximum value in a sequence of <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Decimal"/> values to determine the maximum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static Decimal Max(this IEnumerable<Decimal> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      Decimal num1 = new Decimal(0);
      bool flag = false;
      foreach (Decimal num2 in source)
      {
        if (flag)
        {
          if (num2 > num1)
            num1 = num2;
        }
        else
        {
          num1 = num2;
          flag = true;
        }
      }
      if (flag)
        return num1;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Returns the maximum value in a sequence of nullable <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// A value of type Nullable&lt;Decimal&gt; in C# or Nullable(Of Decimal) in Visual Basic that corresponds to the maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Decimal"/> values to determine the maximum value of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static Decimal? Max(this IEnumerable<Decimal?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      Decimal? nullable1 = new Decimal?();
      foreach (Decimal? nullable2 in source)
      {
        if (nullable1.HasValue)
        {
          Decimal? nullable3 = nullable2;
          Decimal? nullable4 = nullable1;
          if ((!(nullable3.GetValueOrDefault() > nullable4.GetValueOrDefault()) ? 0 : (nullable3.HasValue & nullable4.HasValue ? 1 : 0)) == 0)
            continue;
        }
        nullable1 = nullable2;
      }
      return nullable1;
    }

    /// <summary>
    /// Returns the maximum value in a generic sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the maximum value of.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TSource Max<TSource>(this IEnumerable<TSource> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      Comparer<TSource> @default = Comparer<TSource>.Default;
      TSource y = default (TSource);
      if ((object) y == null)
      {
        foreach (TSource x in source)
        {
          if ((object) x != null && ((object) y == null || @default.Compare(x, y) > 0))
            y = x;
        }
        return y;
      }
      else
      {
        bool flag = false;
        foreach (TSource x in source)
        {
          if (flag)
          {
            if (@default.Compare(x, y) > 0)
              y = x;
          }
          else
          {
            y = x;
            flag = true;
          }
        }
        if (flag)
          return y;
        else
          throw Error.NoElements();
      }
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Int32"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the maximum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
    {
      return Enumerable.Max(Enumerable.Select<TSource, int>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Int32"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The value of type Nullable&lt;Int32&gt; in C# or Nullable(Of Int32) in Visual Basic that corresponds to the maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the maximum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static int? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
    {
      return Enumerable.Max(Enumerable.Select<TSource, int?>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Int64"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the maximum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static long Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
    {
      return Enumerable.Max(Enumerable.Select<TSource, long>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Int64"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The value of type Nullable&lt;Int64&gt; in C# or Nullable(Of Int64) in Visual Basic that corresponds to the maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the maximum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
    {
      return Enumerable.Max(Enumerable.Select<TSource, long?>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Single"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the maximum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static float Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
    {
      return Enumerable.Max(Enumerable.Select<TSource, float>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Single"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The value of type Nullable&lt;Single&gt; in C# or Nullable(Of Single) in Visual Basic that corresponds to the maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the maximum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
    {
      return Enumerable.Max(Enumerable.Select<TSource, float?>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Double"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the maximum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static double Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
    {
      return Enumerable.Max(Enumerable.Select<TSource, double>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Double"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The value of type Nullable&lt;Double&gt; in C# or Nullable(Of Double) in Visual Basic that corresponds to the maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the maximum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
    {
      return Enumerable.Max(Enumerable.Select<TSource, double?>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Decimal"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the maximum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static Decimal Max<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal> selector)
    {
      return Enumerable.Max(Enumerable.Select<TSource, Decimal>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Decimal"/> value.
    /// </summary>
    /// 
    /// <returns>
    /// The value of type Nullable&lt;Decimal&gt; in C# or Nullable(Of Decimal) in Visual Basic that corresponds to the maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the maximum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static Decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal?> selector)
    {
      return Enumerable.Max(Enumerable.Select<TSource, Decimal?>(source, selector));
    }

    /// <summary>
    /// Invokes a transform function on each element of a generic sequence and returns the maximum resulting value.
    /// </summary>
    /// 
    /// <returns>
    /// The maximum value in the sequence.
    /// </returns>
    /// <param name="source">A sequence of values to determine the maximum value of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
      return Enumerable.Max<TResult>(Enumerable.Select<TSource, TResult>(source, selector));
    }

    /// <summary>
    /// Computes the average of a sequence of <see cref="T:System.Int32"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Int32"/> values to calculate the average of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static double Average(this IEnumerable<int> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      long num1 = 0L;
      long num2 = 0L;
      foreach (int num3 in source)
      {
        checked { num1 += (long) num3; }
        checked { ++num2; }
      }
      if (num2 > 0L)
        return (double) num1 / (double) num2;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Computes the average of a sequence of nullable <see cref="T:System.Int32"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Int32"/> values to calculate the average of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static double? Average(this IEnumerable<int?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      long num1 = 0L;
      long num2 = 0L;
      foreach (int? nullable in source)
      {
        if (nullable.HasValue)
        {
          checked { num1 += (long) nullable.GetValueOrDefault(); }
          checked { ++num2; }
        }
      }
      if (num2 > 0L)
        return new double?((double) num1 / (double) num2);
      else
        return new double?();
    }

    /// <summary>
    /// Computes the average of a sequence of <see cref="T:System.Int64"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Int64"/> values to calculate the average of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static double Average(this IEnumerable<long> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      long num1 = 0L;
      long num2 = 0L;
      foreach (long num3 in source)
      {
        checked { num1 += num3; }
        checked { ++num2; }
      }
      if (num2 > 0L)
        return (double) num1 / (double) num2;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Computes the average of a sequence of nullable <see cref="T:System.Int64"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Int64"/> values to calculate the average of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static double? Average(this IEnumerable<long?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      long num1 = 0L;
      long num2 = 0L;
      foreach (long? nullable in source)
      {
        if (nullable.HasValue)
        {
          checked { num1 += nullable.GetValueOrDefault(); }
          checked { ++num2; }
        }
      }
      if (num2 > 0L)
        return new double?((double) num1 / (double) num2);
      else
        return new double?();
    }

    /// <summary>
    /// Computes the average of a sequence of <see cref="T:System.Single"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Single"/> values to calculate the average of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static float Average(this IEnumerable<float> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      double num1 = 0.0;
      long num2 = 0L;
      foreach (float num3 in source)
      {
        num1 += (double) num3;
        checked { ++num2; }
      }
      if (num2 > 0L)
        return (float) num1 / (float) num2;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Computes the average of a sequence of nullable <see cref="T:System.Single"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Single"/> values to calculate the average of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static float? Average(this IEnumerable<float?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      double num1 = 0.0;
      long num2 = 0L;
      foreach (float? nullable in source)
      {
        if (nullable.HasValue)
        {
          num1 += (double) nullable.GetValueOrDefault();
          checked { ++num2; }
        }
      }
      if (num2 > 0L)
        return new float?((float) num1 / (float) num2);
      else
        return new float?();
    }

    /// <summary>
    /// Computes the average of a sequence of <see cref="T:System.Double"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Double"/> values to calculate the average of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static double Average(this IEnumerable<double> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      double num1 = 0.0;
      long num2 = 0L;
      foreach (double num3 in source)
      {
        num1 += num3;
        checked { ++num2; }
      }
      if (num2 > 0L)
        return num1 / (double) num2;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Computes the average of a sequence of nullable <see cref="T:System.Double"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Double"/> values to calculate the average of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
    [__DynamicallyInvokable]
    public static double? Average(this IEnumerable<double?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      double num1 = 0.0;
      long num2 = 0L;
      foreach (double? nullable in source)
      {
        if (nullable.HasValue)
        {
          num1 += nullable.GetValueOrDefault();
          checked { ++num2; }
        }
      }
      if (num2 > 0L)
        return new double?(num1 / (double) num2);
      else
        return new double?();
    }

    /// <summary>
    /// Computes the average of a sequence of <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values.
    /// </returns>
    /// <param name="source">A sequence of <see cref="T:System.Decimal"/> values to calculate the average of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static Decimal Average(this IEnumerable<Decimal> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      Decimal num1 = new Decimal(0);
      long num2 = 0L;
      foreach (Decimal num3 in source)
      {
        num1 += num3;
        checked { ++num2; }
      }
      if (num2 > 0L)
        return num1 / (Decimal) num2;
      else
        throw Error.NoElements();
    }

    /// <summary>
    /// Computes the average of a sequence of nullable <see cref="T:System.Decimal"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.
    /// </returns>
    /// <param name="source">A sequence of nullable <see cref="T:System.Decimal"/> values to calculate the average of.</param><exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception><exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static Decimal? Average(this IEnumerable<Decimal?> source)
    {
      if (source == null)
        throw Error.ArgumentNull("source");
      Decimal num1 = new Decimal(0);
      long num2 = 0L;
      foreach (Decimal? nullable in source)
      {
        if (nullable.HasValue)
        {
          num1 += nullable.GetValueOrDefault();
          checked { ++num2; }
        }
      }
      if (num2 > 0L)
        return new Decimal?(num1 / (Decimal) num2);
      else
        return new Decimal?();
    }

    /// <summary>
    /// Computes the average of a sequence of <see cref="T:System.Int32"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values.
    /// </returns>
    /// <param name="source">A sequence of values to calculate the average of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception><exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
    {
      return Enumerable.Average(Enumerable.Select<TSource, int>(source, selector));
    }

    /// <summary>
    /// Computes the average of a sequence of nullable <see cref="T:System.Int32"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.
    /// </returns>
    /// <param name="source">A sequence of values to calculate the average of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
    {
      return Enumerable.Average(Enumerable.Select<TSource, int?>(source, selector));
    }

    /// <summary>
    /// Computes the average of a sequence of <see cref="T:System.Int64"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values.
    /// </returns>
    /// <param name="source">A sequence of values to calculate the average of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of source.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception><exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
    {
      return Enumerable.Average(Enumerable.Select<TSource, long>(source, selector));
    }

    /// <summary>
    /// Computes the average of a sequence of nullable <see cref="T:System.Int64"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.
    /// </returns>
    /// <param name="source">A sequence of values to calculate the average of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    [__DynamicallyInvokable]
    public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
    {
      return Enumerable.Average(Enumerable.Select<TSource, long?>(source, selector));
    }

    /// <summary>
    /// Computes the average of a sequence of <see cref="T:System.Single"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values.
    /// </returns>
    /// <param name="source">A sequence of values to calculate the average of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
    {
      return Enumerable.Average(Enumerable.Select<TSource, float>(source, selector));
    }

    /// <summary>
    /// Computes the average of a sequence of nullable <see cref="T:System.Single"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.
    /// </returns>
    /// <param name="source">A sequence of values to calculate the average of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static float? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
    {
      return Enumerable.Average(Enumerable.Select<TSource, float?>(source, selector));
    }

    /// <summary>
    /// Computes the average of a sequence of <see cref="T:System.Double"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values.
    /// </returns>
    /// <param name="source">A sequence of values to calculate the average of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
    [__DynamicallyInvokable]
    public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
    {
      return Enumerable.Average(Enumerable.Select<TSource, double>(source, selector));
    }

    /// <summary>
    /// Computes the average of a sequence of nullable <see cref="T:System.Double"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.
    /// </returns>
    /// <param name="source">A sequence of values to calculate the average of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    [__DynamicallyInvokable]
    public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
    {
      return Enumerable.Average(Enumerable.Select<TSource, double?>(source, selector));
    }

    /// <summary>
    /// Computes the average of a sequence of <see cref="T:System.Decimal"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values.
    /// </returns>
    /// <param name="source">A sequence of values that are used to calculate an average.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception><exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static Decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal> selector)
    {
      return Enumerable.Average(Enumerable.Select<TSource, Decimal>(source, selector));
    }

    /// <summary>
    /// Computes the average of a sequence of nullable <see cref="T:System.Decimal"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// 
    /// <returns>
    /// The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.
    /// </returns>
    /// <param name="source">A sequence of values to calculate the average of.</param><param name="selector">A transform function to apply to each element.</param><typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception><exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
    [__DynamicallyInvokable]
    public static Decimal? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, Decimal?> selector)
    {
      return Enumerable.Average(Enumerable.Select<TSource, Decimal?>(source, selector));
    }

    private static IEnumerable<int> RangeIterator(int start, int count)
    {
      for (int i = 0; i < count; ++i)
        yield return start + i;
    }

    private abstract class Iterator<TSource> : IEnumerable<TSource>, IEnumerable, IEnumerator<TSource>, IDisposable, IEnumerator
    {
      private int threadId;
      internal int state;
      internal TSource current;

      public TSource Current
      {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
        {
          return this.current;
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public Iterator()
      {
        this.threadId = Thread.CurrentThread.ManagedThreadId;
      }

      public abstract Enumerable.Iterator<TSource> Clone();

      public virtual void Dispose()
      {
        this.current = default (TSource);
        this.state = -1;
      }

      public IEnumerator<TSource> GetEnumerator()
      {
        if (this.threadId == Thread.CurrentThread.ManagedThreadId && this.state == 0)
        {
          this.state = 1;
          return (IEnumerator<TSource>) this;
        }
        else
        {
          Enumerable.Iterator<TSource> iterator = this.Clone();
          iterator.state = 1;
          return (IEnumerator<TSource>) iterator;
        }
      }

      public abstract bool MoveNext();

      public abstract IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector);

      public abstract IEnumerable<TSource> Where(Func<TSource, bool> predicate);

      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
      IEnumerator IEnumerable.GetEnumerator()
      {
        return (IEnumerator) this.GetEnumerator();
      }

      void IEnumerator.Reset()
      {
        throw new NotImplementedException();
      }
    }

    private class WhereEnumerableIterator<TSource> : Enumerable.Iterator<TSource>
    {
      private IEnumerable<TSource> source;
      private Func<TSource, bool> predicate;
      private IEnumerator<TSource> enumerator;

      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
      public WhereEnumerableIterator(IEnumerable<TSource> source, Func<TSource, bool> predicate)
      {
        this.source = source;
        this.predicate = predicate;
      }

      public override Enumerable.Iterator<TSource> Clone()
      {
        return (Enumerable.Iterator<TSource>) new Enumerable.WhereEnumerableIterator<TSource>(this.source, this.predicate);
      }

      public override void Dispose()
      {
        if (this.enumerator != null)
          this.enumerator.Dispose();
        this.enumerator = (IEnumerator<TSource>) null;
        base.Dispose();
      }

      public override bool MoveNext()
      {
        switch (this.state)
        {
          case 1:
            this.enumerator = this.source.GetEnumerator();
            this.state = 2;
            goto case 2;
          case 2:
            while (this.enumerator.MoveNext())
            {
              TSource current = this.enumerator.Current;
              if (this.predicate(current))
              {
                this.current = current;
                return true;
              }
            }
            this.Dispose();
            break;
        }
        return false;
      }

      public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
      {
        return (IEnumerable<TResult>) new Enumerable.WhereSelectEnumerableIterator<TSource, TResult>(this.source, this.predicate, selector);
      }

      public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
      {
        return (IEnumerable<TSource>) new Enumerable.WhereEnumerableIterator<TSource>(this.source, Enumerable.CombinePredicates<TSource>(this.predicate, predicate));
      }
    }

    private class WhereArrayIterator<TSource> : Enumerable.Iterator<TSource>
    {
      private TSource[] source;
      private Func<TSource, bool> predicate;
      private int index;

      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
      public WhereArrayIterator(TSource[] source, Func<TSource, bool> predicate)
      {
        this.source = source;
        this.predicate = predicate;
      }

      public override Enumerable.Iterator<TSource> Clone()
      {
        return (Enumerable.Iterator<TSource>) new Enumerable.WhereArrayIterator<TSource>(this.source, this.predicate);
      }

      public override bool MoveNext()
      {
        if (this.state == 1)
        {
          while (this.index < this.source.Length)
          {
            TSource source = this.source[this.index];
            ++this.index;
            if (this.predicate(source))
            {
              this.current = source;
              return true;
            }
          }
          this.Dispose();
        }
        return false;
      }

      public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
      {
        return (IEnumerable<TResult>) new Enumerable.WhereSelectArrayIterator<TSource, TResult>(this.source, this.predicate, selector);
      }

      public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
      {
        return (IEnumerable<TSource>) new Enumerable.WhereArrayIterator<TSource>(this.source, Enumerable.CombinePredicates<TSource>(this.predicate, predicate));
      }
    }

    private class WhereListIterator<TSource> : Enumerable.Iterator<TSource>
    {
      private List<TSource> source;
      private Func<TSource, bool> predicate;
      private List<TSource>.Enumerator enumerator;

      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
      public WhereListIterator(List<TSource> source, Func<TSource, bool> predicate)
      {
        this.source = source;
        this.predicate = predicate;
      }

      public override Enumerable.Iterator<TSource> Clone()
      {
        return (Enumerable.Iterator<TSource>) new Enumerable.WhereListIterator<TSource>(this.source, this.predicate);
      }

      public override bool MoveNext()
      {
        switch (this.state)
        {
          case 1:
            this.enumerator = this.source.GetEnumerator();
            this.state = 2;
            goto case 2;
          case 2:
            while (this.enumerator.MoveNext())
            {
              TSource current = this.enumerator.Current;
              if (this.predicate(current))
              {
                this.current = current;
                return true;
              }
            }
            this.Dispose();
            break;
        }
        return false;
      }

      public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
      {
        return (IEnumerable<TResult>) new Enumerable.WhereSelectListIterator<TSource, TResult>(this.source, this.predicate, selector);
      }

      public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
      {
        return (IEnumerable<TSource>) new Enumerable.WhereListIterator<TSource>(this.source, Enumerable.CombinePredicates<TSource>(this.predicate, predicate));
      }
    }

    private class WhereSelectEnumerableIterator<TSource, TResult> : Enumerable.Iterator<TResult>
    {
      private IEnumerable<TSource> source;
      private Func<TSource, bool> predicate;
      private Func<TSource, TResult> selector;
      private IEnumerator<TSource> enumerator;

      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
      public WhereSelectEnumerableIterator(IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
      {
        this.source = source;
        this.predicate = predicate;
        this.selector = selector;
      }

      public override Enumerable.Iterator<TResult> Clone()
      {
        return (Enumerable.Iterator<TResult>) new Enumerable.WhereSelectEnumerableIterator<TSource, TResult>(this.source, this.predicate, this.selector);
      }

      public override void Dispose()
      {
        if (this.enumerator != null)
          this.enumerator.Dispose();
        this.enumerator = (IEnumerator<TSource>) null;
        base.Dispose();
      }

      public override bool MoveNext()
      {
        switch (this.state)
        {
          case 1:
            this.enumerator = this.source.GetEnumerator();
            this.state = 2;
            goto case 2;
          case 2:
            while (this.enumerator.MoveNext())
            {
              TSource current = this.enumerator.Current;
              if (this.predicate == null || this.predicate(current))
              {
                this.current = this.selector(current);
                return true;
              }
            }
            this.Dispose();
            break;
        }
        return false;
      }

      public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
      {
        return (IEnumerable<TResult2>) new Enumerable.WhereSelectEnumerableIterator<TSource, TResult2>(this.source, this.predicate, Enumerable.CombineSelectors<TSource, TResult, TResult2>(this.selector, selector));
      }

      public override IEnumerable<TResult> Where(Func<TResult, bool> predicate)
      {
        return (IEnumerable<TResult>) new Enumerable.WhereEnumerableIterator<TResult>((IEnumerable<TResult>) this, predicate);
      }
    }

    private class WhereSelectArrayIterator<TSource, TResult> : Enumerable.Iterator<TResult>
    {
      private TSource[] source;
      private Func<TSource, bool> predicate;
      private Func<TSource, TResult> selector;
      private int index;

      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
      public WhereSelectArrayIterator(TSource[] source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
      {
        this.source = source;
        this.predicate = predicate;
        this.selector = selector;
      }

      public override Enumerable.Iterator<TResult> Clone()
      {
        return (Enumerable.Iterator<TResult>) new Enumerable.WhereSelectArrayIterator<TSource, TResult>(this.source, this.predicate, this.selector);
      }

      public override bool MoveNext()
      {
        if (this.state == 1)
        {
          while (this.index < this.source.Length)
          {
            TSource source = this.source[this.index];
            ++this.index;
            if (this.predicate == null || this.predicate(source))
            {
              this.current = this.selector(source);
              return true;
            }
          }
          this.Dispose();
        }
        return false;
      }

      public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
      {
        return (IEnumerable<TResult2>) new Enumerable.WhereSelectArrayIterator<TSource, TResult2>(this.source, this.predicate, Enumerable.CombineSelectors<TSource, TResult, TResult2>(this.selector, selector));
      }

      public override IEnumerable<TResult> Where(Func<TResult, bool> predicate)
      {
        return (IEnumerable<TResult>) new Enumerable.WhereEnumerableIterator<TResult>((IEnumerable<TResult>) this, predicate);
      }
    }

    private class WhereSelectListIterator<TSource, TResult> : Enumerable.Iterator<TResult>
    {
      private List<TSource> source;
      private Func<TSource, bool> predicate;
      private Func<TSource, TResult> selector;
      private List<TSource>.Enumerator enumerator;

      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
      public WhereSelectListIterator(List<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
      {
        this.source = source;
        this.predicate = predicate;
        this.selector = selector;
      }

      public override Enumerable.Iterator<TResult> Clone()
      {
        return (Enumerable.Iterator<TResult>) new Enumerable.WhereSelectListIterator<TSource, TResult>(this.source, this.predicate, this.selector);
      }

      public override bool MoveNext()
      {
        switch (this.state)
        {
          case 1:
            this.enumerator = this.source.GetEnumerator();
            this.state = 2;
            goto case 2;
          case 2:
            while (this.enumerator.MoveNext())
            {
              TSource current = this.enumerator.Current;
              if (this.predicate == null || this.predicate(current))
              {
                this.current = this.selector(current);
                return true;
              }
            }
            this.Dispose();
            break;
        }
        return false;
      }

      public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
      {
        return (IEnumerable<TResult2>) new Enumerable.WhereSelectListIterator<TSource, TResult2>(this.source, this.predicate, Enumerable.CombineSelectors<TSource, TResult, TResult2>(this.selector, selector));
      }

      public override IEnumerable<TResult> Where(Func<TResult, bool> predicate)
      {
        return (IEnumerable<TResult>) new Enumerable.WhereEnumerableIterator<TResult>((IEnumerable<TResult>) this, predicate);
      }
    }
  }
}
