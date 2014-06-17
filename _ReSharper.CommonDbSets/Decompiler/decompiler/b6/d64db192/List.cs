﻿// Type: System.Collections.Generic.List`1
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.Versioning;
using System.Threading;

namespace System.Collections.Generic
{
  /// <summary>
  /// Represents a strongly typed list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
  /// </summary>
  /// <typeparam name="T">The type of elements in the list.</typeparam><filterpriority>1</filterpriority>
  [DebuggerDisplay("Count = {Count}")]
  [DebuggerTypeProxy(typeof (Mscorlib_CollectionDebugView<>))]
  [__DynamicallyInvokable]
  [Serializable]
  public class List<T> : IList<T>, ICollection<T>, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
  {
    private static readonly T[] _emptyArray = new T[0];
    private T[] _items;
    private int _size;
    private int _version;
    [NonSerialized]
    private object _syncRoot;
    private const int _defaultCapacity = 4;

    /// <summary>
    /// Gets or sets the total number of elements the internal data structure can hold without resizing.
    /// </summary>
    /// 
    /// <returns>
    /// The number of elements that the <see cref="T:System.Collections.Generic.List`1"/> can contain before resizing is required.
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException"><see cref="P:System.Collections.Generic.List`1.Capacity"/> is set to a value that is less than <see cref="P:System.Collections.Generic.List`1.Count"/>. </exception><exception cref="T:System.OutOfMemoryException">There is not enough memory available on the system.</exception>
    [__DynamicallyInvokable]
    public int Capacity
    {
      [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), __DynamicallyInvokable] get
      {
        return this._items.Length;
      }
      [__DynamicallyInvokable] set
      {
        if (value < this._size)
          ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
        if (value == this._items.Length)
          return;
        if (value > 0)
        {
          T[] objArray = new T[value];
          if (this._size > 0)
            Array.Copy((Array) this._items, 0, (Array) objArray, 0, this._size);
          this._items = objArray;
        }
        else
          this._items = List<T>._emptyArray;
      }
    }

    /// <summary>
    /// Gets the number of elements actually contained in the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The number of elements actually contained in the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </returns>
    [__DynamicallyInvokable]
    public int Count
    {
      [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._size;
      }
    }

    [__DynamicallyInvokable]
    bool IList.IsFixedSize
    {
      [__DynamicallyInvokable] get
      {
        return false;
      }
    }

    [__DynamicallyInvokable]
    bool ICollection<T>.IsReadOnly
    {
      [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), __DynamicallyInvokable] get
      {
        return false;
      }
    }

    [__DynamicallyInvokable]
    bool IList.IsReadOnly
    {
      [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), __DynamicallyInvokable] get
      {
        return false;
      }
    }

    [__DynamicallyInvokable]
    bool ICollection.IsSynchronized
    {
      [__DynamicallyInvokable] get
      {
        return false;
      }
    }

    [__DynamicallyInvokable]
    object ICollection.SyncRoot
    {
      [__DynamicallyInvokable] get
      {
        if (this._syncRoot == null)
          Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), (object) null);
        return this._syncRoot;
      }
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// 
    /// <returns>
    /// The element at the specified index.
    /// </returns>
    /// <param name="index">The zero-based index of the element to get or set.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.Generic.List`1.Count"/>. </exception>
    [__DynamicallyInvokable]
    public T this[int index]
    {
      [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), __DynamicallyInvokable] get
      {
        if ((uint) index >= (uint) this._size)
          ThrowHelper.ThrowArgumentOutOfRangeException();
        return this._items[index];
      }
      [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries"), __DynamicallyInvokable] set
      {
        if ((uint) index >= (uint) this._size)
          ThrowHelper.ThrowArgumentOutOfRangeException();
        this._items[index] = value;
        ++this._version;
      }
    }

    static List()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1"/> class that is empty and has the default initial capacity.
    /// </summary>
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    [__DynamicallyInvokable]
    public List()
    {
      this._items = List<T>._emptyArray;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1"/> class that is empty and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new list can initially store.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0. </exception>
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    [__DynamicallyInvokable]
    public List(int capacity)
    {
      if (capacity < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (capacity == 0)
        this._items = List<T>._emptyArray;
      else
        this._items = new T[capacity];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param><exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is null.</exception>
    [__DynamicallyInvokable]
    public List(IEnumerable<T> collection)
    {
      if (collection == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
      ICollection<T> collection1 = collection as ICollection<T>;
      if (collection1 != null)
      {
        int count = collection1.Count;
        if (count == 0)
        {
          this._items = List<T>._emptyArray;
        }
        else
        {
          this._items = new T[count];
          collection1.CopyTo(this._items, 0);
          this._size = count;
        }
      }
      else
      {
        this._size = 0;
        this._items = List<T>._emptyArray;
        foreach (T obj in collection)
          this.Add(obj);
      }
    }

    private static bool IsCompatibleObject(object value)
    {
      if (value is T)
        return true;
      if (value == null)
        return (object) default (T) == null;
      else
        return false;
    }

    [__DynamicallyInvokable]
    object IList.get_Item(int index)
    {
      return (object) this[index];
    }

    [__DynamicallyInvokable]
    void IList.set_Item(int index, object value)
    {
      ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);
      try
      {
        this[index] = (T) value;
      }
      catch (InvalidCastException ex)
      {
        ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof (T));
      }
    }

    /// <summary>
    /// Adds an object to the end of the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// <param name="item">The object to be added to the end of the <see cref="T:System.Collections.Generic.List`1"/>. The value can be null for reference types.</param>
    [__DynamicallyInvokable]
    public void Add(T item)
    {
      if (this._size == this._items.Length)
        this.EnsureCapacity(this._size + 1);
      this._items[this._size++] = item;
      ++this._version;
    }

    [__DynamicallyInvokable]
    int IList.Add(object item)
    {
      ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);
      try
      {
        this.Add((T) item);
      }
      catch (InvalidCastException ex)
      {
        ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof (T));
      }
      return this.Count - 1;
    }

    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// <param name="collection">The collection whose elements should be added to the end of the <see cref="T:System.Collections.Generic.List`1"/>. The collection itself cannot be null, but it can contain elements that are null, if type <paramref name="T"/> is a reference type.</param><exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is null.</exception>
    [__DynamicallyInvokable]
    public void AddRange(IEnumerable<T> collection)
    {
      this.InsertRange(this._size, collection);
    }

    /// <summary>
    /// Returns a read-only <see cref="T:System.Collections.Generic.IList`1"/> wrapper for the current collection.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1"/> that acts as a read-only wrapper around the current <see cref="T:System.Collections.Generic.List`1"/>.
    /// </returns>
    public ReadOnlyCollection<T> AsReadOnly()
    {
      return new ReadOnlyCollection<T>((IList<T>) this);
    }

    /// <summary>
    /// Searches a range of elements in the sorted <see cref="T:System.Collections.Generic.List`1"/> for an element using the specified comparer and returns the zero-based index of the element.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of <paramref name="item"/> in the sorted <see cref="T:System.Collections.Generic.List`1"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="P:System.Collections.Generic.List`1.Count"/>.
    /// </returns>
    /// <param name="index">The zero-based starting index of the range to search.</param><param name="count">The length of the range to search.</param><param name="item">The object to locate. The value can be null for reference types.</param><param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1"/> implementation to use when comparing elements, or null to use the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/>.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="count"/> is less than 0. </exception><exception cref="T:System.ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the <see cref="T:System.Collections.Generic.List`1"/>.</exception><exception cref="T:System.InvalidOperationException"><paramref name="comparer"/> is null, and the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/> cannot find an implementation of the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface for type <paramref name="T"/>.</exception>
    [__DynamicallyInvokable]
    public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
    {
      if (index < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (count < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (this._size - index < count)
        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
      return Array.BinarySearch<T>(this._items, index, count, item, comparer);
    }

    /// <summary>
    /// Searches the entire sorted <see cref="T:System.Collections.Generic.List`1"/> for an element using the default comparer and returns the zero-based index of the element.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of <paramref name="item"/> in the sorted <see cref="T:System.Collections.Generic.List`1"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="P:System.Collections.Generic.List`1.Count"/>.
    /// </returns>
    /// <param name="item">The object to locate. The value can be null for reference types.</param><exception cref="T:System.InvalidOperationException">The default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/> cannot find an implementation of the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface for type <paramref name="T"/>.</exception>
    [__DynamicallyInvokable]
    public int BinarySearch(T item)
    {
      return this.BinarySearch(0, this.Count, item, (IComparer<T>) null);
    }

    /// <summary>
    /// Searches the entire sorted <see cref="T:System.Collections.Generic.List`1"/> for an element using the specified comparer and returns the zero-based index of the element.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of <paramref name="item"/> in the sorted <see cref="T:System.Collections.Generic.List`1"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="P:System.Collections.Generic.List`1.Count"/>.
    /// </returns>
    /// <param name="item">The object to locate. The value can be null for reference types.</param><param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1"/> implementation to use when comparing elements.-or-null to use the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/>.</param><exception cref="T:System.InvalidOperationException"><paramref name="comparer"/> is null, and the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/> cannot find an implementation of the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface for type <paramref name="T"/>.</exception>
    [__DynamicallyInvokable]
    public int BinarySearch(T item, IComparer<T> comparer)
    {
      return this.BinarySearch(0, this.Count, item, comparer);
    }

    /// <summary>
    /// Removes all elements from the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    [__DynamicallyInvokable]
    public void Clear()
    {
      if (this._size > 0)
      {
        Array.Clear((Array) this._items, 0, this._size);
        this._size = 0;
      }
      ++this._version;
    }

    /// <summary>
    /// Determines whether an element is in the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.List`1"/>; otherwise, false.
    /// </returns>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be null for reference types.</param>
    [__DynamicallyInvokable]
    public bool Contains(T item)
    {
      if ((object) item == null)
      {
        for (int index = 0; index < this._size; ++index)
        {
          if ((object) this._items[index] == null)
            return true;
        }
        return false;
      }
      else
      {
        EqualityComparer<T> @default = EqualityComparer<T>.Default;
        for (int index = 0; index < this._size; ++index)
        {
          if (@default.Equals(this._items[index], item))
            return true;
        }
        return false;
      }
    }

    [__DynamicallyInvokable]
    bool IList.Contains(object item)
    {
      if (List<T>.IsCompatibleObject(item))
        return this.Contains((T) item);
      else
        return false;
    }

    /// <summary>
    /// Converts the elements in the current <see cref="T:System.Collections.Generic.List`1"/> to another type, and returns a list containing the converted elements.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.List`1"/> of the target type containing the converted elements from the current <see cref="T:System.Collections.Generic.List`1"/>.
    /// </returns>
    /// <param name="converter">A <see cref="T:System.Converter`2"/> delegate that converts each element from one type to another type.</param><typeparam name="TOutput">The type of the elements of the target array.</typeparam><exception cref="T:System.ArgumentNullException"><paramref name="converter"/> is null.</exception>
    public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
    {
      if (converter == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.converter);
      List<TOutput> list = new List<TOutput>(this._size);
      for (int index = 0; index < this._size; ++index)
        list._items[index] = converter(this._items[index]);
      list._size = this._size;
      return list;
    }

    /// <summary>
    /// Copies the entire <see cref="T:System.Collections.Generic.List`1"/> to a compatible one-dimensional array, starting at the beginning of the target array.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.List`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.List`1"/> is greater than the number of elements that the destination <paramref name="array"/> can contain.</exception>
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    [__DynamicallyInvokable]
    public void CopyTo(T[] array)
    {
      this.CopyTo(array, 0);
    }

    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    [__DynamicallyInvokable]
    void ICollection.CopyTo(Array array, int arrayIndex)
    {
      if (array != null)
      {
        if (array.Rank != 1)
          ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
      }
      try
      {
        Array.Copy((Array) this._items, 0, array, arrayIndex, this._size);
      }
      catch (ArrayTypeMismatchException ex)
      {
        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
      }
    }

    /// <summary>
    /// Copies a range of elements from the <see cref="T:System.Collections.Generic.List`1"/> to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="index">The zero-based index in the source <see cref="T:System.Collections.Generic.List`1"/> at which copying begins.</param><param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.List`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><param name="count">The number of elements to copy.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="arrayIndex"/> is less than 0.-or-<paramref name="count"/> is less than 0. </exception><exception cref="T:System.ArgumentException"><paramref name="index"/> is equal to or greater than the <see cref="P:System.Collections.Generic.List`1.Count"/> of the source <see cref="T:System.Collections.Generic.List`1"/>.-or-The number of elements from <paramref name="index"/> to the end of the source <see cref="T:System.Collections.Generic.List`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>. </exception>
    [__DynamicallyInvokable]
    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
      if (this._size - index < count)
        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
      Array.Copy((Array) this._items, index, (Array) array, arrayIndex, count);
    }

    /// <summary>
    /// Copies the entire <see cref="T:System.Collections.Generic.List`1"/> to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.List`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.List`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
    [__DynamicallyInvokable]
    public void CopyTo(T[] array, int arrayIndex)
    {
      Array.Copy((Array) this._items, 0, (Array) array, arrayIndex, this._size);
    }

    private void EnsureCapacity(int min)
    {
      if (this._items.Length >= min)
        return;
      int num = this._items.Length == 0 ? 4 : this._items.Length * 2;
      if ((uint) num > 2146435071U)
        num = 2146435071;
      if (num < min)
        num = min;
      this.Capacity = num;
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.List`1"/> contains elements that match the conditions defined by the specified predicate.
    /// </summary>
    /// 
    /// <returns>
    /// true if the <see cref="T:System.Collections.Generic.List`1"/> contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.
    /// </returns>
    /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the elements to search for.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception>
    [__DynamicallyInvokable]
    public bool Exists(Predicate<T> match)
    {
      return this.FindIndex(match) != -1;
    }

    /// <summary>
    /// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <paramref name="T"/>.
    /// </returns>
    /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception>
    [__DynamicallyInvokable]
    public T Find(Predicate<T> match)
    {
      if (match == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
      for (int index = 0; index < this._size; ++index)
      {
        if (match(this._items[index]))
          return this._items[index];
      }
      return default (T);
    }

    /// <summary>
    /// Retrieves all the elements that match the conditions defined by the specified predicate.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.List`1"/> containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="T:System.Collections.Generic.List`1"/>.
    /// </returns>
    /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the elements to search for.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception>
    [__DynamicallyInvokable]
    public List<T> FindAll(Predicate<T> match)
    {
      if (match == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
      List<T> list = new List<T>();
      for (int index = 0; index < this._size; ++index)
      {
        if (match(this._items[index]))
          list.Add(this._items[index]);
      }
      return list;
    }

    /// <summary>
    /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
    /// </returns>
    /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception>
    [__DynamicallyInvokable]
    public int FindIndex(Predicate<T> match)
    {
      return this.FindIndex(0, this._size, match);
    }

    /// <summary>
    /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that extends from the specified index to the last element.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
    /// </returns>
    /// <param name="startIndex">The zero-based starting index of the search.</param><param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
    [__DynamicallyInvokable]
    public int FindIndex(int startIndex, Predicate<T> match)
    {
      return this.FindIndex(startIndex, this._size - startIndex, match);
    }

    /// <summary>
    /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that starts at the specified index and contains the specified number of elements.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
    /// </returns>
    /// <param name="startIndex">The zero-based starting index of the search.</param><param name="count">The number of elements in the section to search.</param><param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>.-or-<paramref name="count"/> is less than 0.-or-<paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
    [__DynamicallyInvokable]
    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
      if ((uint) startIndex > (uint) this._size)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index);
      if (count < 0 || startIndex > this._size - count)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);
      if (match == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
      int num = startIndex + count;
      for (int index = startIndex; index < num; ++index)
      {
        if (match(this._items[index]))
          return index;
      }
      return -1;
    }

    /// <summary>
    /// Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence within the entire <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The last element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <paramref name="T"/>.
    /// </returns>
    /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception>
    [__DynamicallyInvokable]
    public T FindLast(Predicate<T> match)
    {
      if (match == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
      for (int index = this._size - 1; index >= 0; --index)
      {
        if (match(this._items[index]))
          return this._items[index];
      }
      return default (T);
    }

    /// <summary>
    /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the entire <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
    /// </returns>
    /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception>
    [__DynamicallyInvokable]
    public int FindLastIndex(Predicate<T> match)
    {
      return this.FindLastIndex(this._size - 1, this._size, match);
    }

    /// <summary>
    /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that extends from the first element to the specified index.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
    /// </returns>
    /// <param name="startIndex">The zero-based starting index of the backward search.</param><param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
    [__DynamicallyInvokable]
    public int FindLastIndex(int startIndex, Predicate<T> match)
    {
      return this.FindLastIndex(startIndex, startIndex + 1, match);
    }

    /// <summary>
    /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that contains the specified number of elements and ends at the specified index.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
    /// </returns>
    /// <param name="startIndex">The zero-based starting index of the backward search.</param><param name="count">The number of elements in the section to search.</param><param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the element to search for.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>.-or-<paramref name="count"/> is less than 0.-or-<paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
    [__DynamicallyInvokable]
    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
      if (match == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
      if (this._size == 0)
      {
        if (startIndex != -1)
          ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index);
      }
      else if ((uint) startIndex >= (uint) this._size)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.startIndex, ExceptionResource.ArgumentOutOfRange_Index);
      if (count < 0 || startIndex - count + 1 < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);
      int num = startIndex - count;
      for (int index = startIndex; index > num; --index)
      {
        if (match(this._items[index]))
          return index;
      }
      return -1;
    }

    /// <summary>
    /// Performs the specified action on each element of the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// <param name="action">The <see cref="T:System.Action`1"/> delegate to perform on each element of the <see cref="T:System.Collections.Generic.List`1"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="action"/> is null.</exception>
    public void ForEach(Action<T> action)
    {
      if (action == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
      int num = this._version;
      for (int index = 0; index < this._size && (num == this._version || !BinaryCompatibility.TargetsAtLeast_Desktop_V4_5); ++index)
        action(this._items[index]);
      if (num == this._version || !BinaryCompatibility.TargetsAtLeast_Desktop_V4_5)
        return;
      ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.List`1.Enumerator"/> for the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </returns>
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    [__DynamicallyInvokable]
    public List<T>.Enumerator GetEnumerator()
    {
      return new List<T>.Enumerator(this);
    }

    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    [__DynamicallyInvokable]
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return (IEnumerator<T>) new List<T>.Enumerator(this);
    }

    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    [__DynamicallyInvokable]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new List<T>.Enumerator(this);
    }

    /// <summary>
    /// Creates a shallow copy of a range of elements in the source <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A shallow copy of a range of elements in the source <see cref="T:System.Collections.Generic.List`1"/>.
    /// </returns>
    /// <param name="index">The zero-based <see cref="T:System.Collections.Generic.List`1"/> index at which the range starts.</param><param name="count">The number of elements in the range.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="count"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
    [__DynamicallyInvokable]
    public List<T> GetRange(int index, int count)
    {
      if (index < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (count < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (this._size - index < count)
        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
      List<T> list = new List<T>(count);
      Array.Copy((Array) this._items, index, (Array) list._items, 0, count);
      list._size = count;
      return list;
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of the first occurrence of <paramref name="item"/> within the entire <see cref="T:System.Collections.Generic.List`1"/>, if found; otherwise, –1.
    /// </returns>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be null for reference types.</param>
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    [__DynamicallyInvokable]
    public int IndexOf(T item)
    {
      return Array.IndexOf<T>(this._items, item, 0, this._size);
    }

    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    [__DynamicallyInvokable]
    int IList.IndexOf(object item)
    {
      if (List<T>.IsCompatibleObject(item))
        return this.IndexOf((T) item);
      else
        return -1;
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that extends from the specified index to the last element.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that extends from <paramref name="index"/> to the last element, if found; otherwise, –1.
    /// </returns>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be null for reference types.</param><param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
    [__DynamicallyInvokable]
    public int IndexOf(T item, int index)
    {
      if (index > this._size)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
      return Array.IndexOf<T>(this._items, item, index, this._size - index);
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that starts at the specified index and contains the specified number of elements.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that starts at <paramref name="index"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
    /// </returns>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be null for reference types.</param><param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param><param name="count">The number of elements in the section to search.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>.-or-<paramref name="count"/> is less than 0.-or-<paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
    [__DynamicallyInvokable]
    public int IndexOf(T item, int index, int count)
    {
      if (index > this._size)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
      if (count < 0 || index > this._size - count)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_Count);
      return Array.IndexOf<T>(this._items, item, index, count);
    }

    /// <summary>
    /// Inserts an element into the <see cref="T:System.Collections.Generic.List`1"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert. The value can be null for reference types.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.Generic.List`1.Count"/>.</exception>
    [__DynamicallyInvokable]
    public void Insert(int index, T item)
    {
      if ((uint) index > (uint) this._size)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_ListInsert);
      if (this._size == this._items.Length)
        this.EnsureCapacity(this._size + 1);
      if (index < this._size)
        Array.Copy((Array) this._items, index, (Array) this._items, index + 1, this._size - index);
      this._items[index] = item;
      ++this._size;
      ++this._version;
    }

    [__DynamicallyInvokable]
    void IList.Insert(int index, object item)
    {
      ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);
      try
      {
        this.Insert(index, (T) item);
      }
      catch (InvalidCastException ex)
      {
        ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof (T));
      }
    }

    /// <summary>
    /// Inserts the elements of a collection into the <see cref="T:System.Collections.Generic.List`1"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which the new elements should be inserted.</param><param name="collection">The collection whose elements should be inserted into the <see cref="T:System.Collections.Generic.List`1"/>. The collection itself cannot be null, but it can contain elements that are null, if type <paramref name="T"/> is a reference type.</param><exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.Generic.List`1.Count"/>.</exception>
    [__DynamicallyInvokable]
    public void InsertRange(int index, IEnumerable<T> collection)
    {
      if (collection == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
      if ((uint) index > (uint) this._size)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
      ICollection<T> collection1 = collection as ICollection<T>;
      if (collection1 != null)
      {
        int count = collection1.Count;
        if (count > 0)
        {
          this.EnsureCapacity(this._size + count);
          if (index < this._size)
            Array.Copy((Array) this._items, index, (Array) this._items, index + count, this._size - index);
          if (this == collection1)
          {
            Array.Copy((Array) this._items, 0, (Array) this._items, index, index);
            Array.Copy((Array) this._items, index + count, (Array) this._items, index * 2, this._size - index);
          }
          else
          {
            T[] array = new T[count];
            collection1.CopyTo(array, 0);
            array.CopyTo((Array) this._items, index);
          }
          this._size += count;
        }
      }
      else
      {
        foreach (T obj in collection)
          this.Insert(index++, obj);
      }
      ++this._version;
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the last occurrence within the entire <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of the last occurrence of <paramref name="item"/> within the entire the <see cref="T:System.Collections.Generic.List`1"/>, if found; otherwise, –1.
    /// </returns>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be null for reference types.</param>
    [__DynamicallyInvokable]
    public int LastIndexOf(T item)
    {
      if (this._size == 0)
        return -1;
      else
        return this.LastIndexOf(item, this._size - 1, this._size);
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that extends from the first element to the specified index.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that extends from the first element to <paramref name="index"/>, if found; otherwise, –1.
    /// </returns>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be null for reference types.</param><param name="index">The zero-based starting index of the backward search.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>. </exception>
    [__DynamicallyInvokable]
    public int LastIndexOf(T item, int index)
    {
      if (index >= this._size)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
      return this.LastIndexOf(item, index, index + 1);
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that contains the specified number of elements and ends at the specified index.
    /// </summary>
    /// 
    /// <returns>
    /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="T:System.Collections.Generic.List`1"/> that contains <paramref name="count"/> number of elements and ends at <paramref name="index"/>, if found; otherwise, –1.
    /// </returns>
    /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be null for reference types.</param><param name="index">The zero-based starting index of the backward search.</param><param name="count">The number of elements in the section to search.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the <see cref="T:System.Collections.Generic.List`1"/>.-or-<paramref name="count"/> is less than 0.-or-<paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the <see cref="T:System.Collections.Generic.List`1"/>. </exception>
    [__DynamicallyInvokable]
    public int LastIndexOf(T item, int index, int count)
    {
      if (this.Count != 0 && index < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (this.Count != 0 && count < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (this._size == 0)
        return -1;
      if (index >= this._size)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
      if (count > index + 1)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
      return Array.LastIndexOf<T>(this._items, item, index, count);
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="item"/> is successfully removed; otherwise, false.  This method also returns false if <paramref name="item"/> was not found in the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </returns>
    /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.List`1"/>. The value can be null for reference types.</param>
    [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
    [__DynamicallyInvokable]
    public bool Remove(T item)
    {
      int index = this.IndexOf(item);
      if (index < 0)
        return false;
      this.RemoveAt(index);
      return true;
    }

    [__DynamicallyInvokable]
    void IList.Remove(object item)
    {
      if (!List<T>.IsCompatibleObject(item))
        return;
      this.Remove((T) item);
    }

    /// <summary>
    /// Removes all the elements that match the conditions defined by the specified predicate.
    /// </summary>
    /// 
    /// <returns>
    /// The number of elements removed from the <see cref="T:System.Collections.Generic.List`1"/> .
    /// </returns>
    /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions of the elements to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception>
    [__DynamicallyInvokable]
    public int RemoveAll(Predicate<T> match)
    {
      if (match == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
      int index1 = 0;
      while (index1 < this._size && !match(this._items[index1]))
        ++index1;
      if (index1 >= this._size)
        return 0;
      int index2 = index1 + 1;
      while (index2 < this._size)
      {
        while (index2 < this._size && match(this._items[index2]))
          ++index2;
        if (index2 < this._size)
          this._items[index1++] = this._items[index2++];
      }
      Array.Clear((Array) this._items, index1, this._size - index1);
      int num = this._size - index1;
      this._size = index1;
      ++this._version;
      return num;
    }

    /// <summary>
    /// Removes the element at the specified index of the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.Generic.List`1.Count"/>.</exception>
    [__DynamicallyInvokable]
    public void RemoveAt(int index)
    {
      if ((uint) index >= (uint) this._size)
        ThrowHelper.ThrowArgumentOutOfRangeException();
      --this._size;
      if (index < this._size)
        Array.Copy((Array) this._items, index + 1, (Array) this._items, index, this._size - index);
      this._items[this._size] = default (T);
      ++this._version;
    }

    /// <summary>
    /// Removes a range of elements from the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range of elements to remove.</param><param name="count">The number of elements to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="count"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="T:System.Collections.Generic.List`1"/>.</exception>
    [__DynamicallyInvokable]
    public void RemoveRange(int index, int count)
    {
      if (index < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (count < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (this._size - index < count)
        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
      if (count <= 0)
        return;
      this._size -= count;
      if (index < this._size)
        Array.Copy((Array) this._items, index + count, (Array) this._items, index, this._size - index);
      Array.Clear((Array) this._items, this._size, count);
      ++this._version;
    }

    /// <summary>
    /// Reverses the order of the elements in the entire <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    [__DynamicallyInvokable]
    public void Reverse()
    {
      this.Reverse(0, this.Count);
    }

    /// <summary>
    /// Reverses the order of the elements in the specified range.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range to reverse.</param><param name="count">The number of elements in the range to reverse.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="count"/> is less than 0. </exception><exception cref="T:System.ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="T:System.Collections.Generic.List`1"/>. </exception>
    [__DynamicallyInvokable]
    public void Reverse(int index, int count)
    {
      if (index < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (count < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (this._size - index < count)
        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
      Array.Reverse((Array) this._items, index, count);
      ++this._version;
    }

    /// <summary>
    /// Sorts the elements in the entire <see cref="T:System.Collections.Generic.List`1"/> using the default comparer.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">The default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/> cannot find an implementation of the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface for type <paramref name="T"/>.</exception>
    [__DynamicallyInvokable]
    public void Sort()
    {
      this.Sort(0, this.Count, (IComparer<T>) null);
    }

    /// <summary>
    /// Sorts the elements in the entire <see cref="T:System.Collections.Generic.List`1"/> using the specified comparer.
    /// </summary>
    /// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1"/> implementation to use when comparing elements, or null to use the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/>.</param><exception cref="T:System.InvalidOperationException"><paramref name="comparer"/> is null, and the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/> cannot find implementation of the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface for type <paramref name="T"/>.</exception><exception cref="T:System.ArgumentException">The implementation of <paramref name="comparer"/> caused an error during the sort. For example, <paramref name="comparer"/> might not return 0 when comparing an item with itself.</exception>
    [__DynamicallyInvokable]
    public void Sort(IComparer<T> comparer)
    {
      this.Sort(0, this.Count, comparer);
    }

    /// <summary>
    /// Sorts the elements in a range of elements in <see cref="T:System.Collections.Generic.List`1"/> using the specified comparer.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range to sort.</param><param name="count">The length of the range to sort.</param><param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1"/> implementation to use when comparing elements, or null to use the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/>.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.-or-<paramref name="count"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not specify a valid range in the <see cref="T:System.Collections.Generic.List`1"/>.-or-The implementation of <paramref name="comparer"/> caused an error during the sort. For example, <paramref name="comparer"/> might not return 0 when comparing an item with itself.</exception><exception cref="T:System.InvalidOperationException"><paramref name="comparer"/> is null, and the default comparer <see cref="P:System.Collections.Generic.Comparer`1.Default"/> cannot find implementation of the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface for type <paramref name="T"/>.</exception>
    [__DynamicallyInvokable]
    public void Sort(int index, int count, IComparer<T> comparer)
    {
      if (index < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (count < 0)
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
      if (this._size - index < count)
        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
      Array.Sort<T>(this._items, index, count, comparer);
      ++this._version;
    }

    /// <summary>
    /// Sorts the elements in the entire <see cref="T:System.Collections.Generic.List`1"/> using the specified <see cref="T:System.Comparison`1"/>.
    /// </summary>
    /// <param name="comparison">The <see cref="T:System.Comparison`1"/> to use when comparing elements.</param><exception cref="T:System.ArgumentNullException"><paramref name="comparison"/> is null.</exception><exception cref="T:System.ArgumentException">The implementation of <paramref name="comparison"/> caused an error during the sort. For example, <paramref name="comparison"/> might not return 0 when comparing an item with itself.</exception>
    [__DynamicallyInvokable]
    public void Sort(Comparison<T> comparison)
    {
      if (comparison == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
      if (this._size <= 0)
        return;
      Array.Sort<T>(this._items, 0, this._size, (IComparer<T>) new Array.FunctorComparer<T>(comparison));
    }

    /// <summary>
    /// Copies the elements of the <see cref="T:System.Collections.Generic.List`1"/> to a new array.
    /// </summary>
    /// 
    /// <returns>
    /// An array containing copies of the elements of the <see cref="T:System.Collections.Generic.List`1"/>.
    /// </returns>
    [__DynamicallyInvokable]
    public T[] ToArray()
    {
      T[] objArray = new T[this._size];
      Array.Copy((Array) this._items, 0, (Array) objArray, 0, this._size);
      return objArray;
    }

    /// <summary>
    /// Sets the capacity to the actual number of elements in the <see cref="T:System.Collections.Generic.List`1"/>, if that number is less than a threshold value.
    /// </summary>
    [__DynamicallyInvokable]
    public void TrimExcess()
    {
      if (this._size >= (int) ((double) this._items.Length * 0.9))
        return;
      this.Capacity = this._size;
    }

    /// <summary>
    /// Determines whether every element in the <see cref="T:System.Collections.Generic.List`1"/> matches the conditions defined by the specified predicate.
    /// </summary>
    /// 
    /// <returns>
    /// true if every element in the <see cref="T:System.Collections.Generic.List`1"/> matches the conditions defined by the specified predicate; otherwise, false. If the list has no elements, the return value is true.
    /// </returns>
    /// <param name="match">The <see cref="T:System.Predicate`1"/> delegate that defines the conditions to check against the elements.</param><exception cref="T:System.ArgumentNullException"><paramref name="match"/> is null.</exception>
    [__DynamicallyInvokable]
    public bool TrueForAll(Predicate<T> match)
    {
      if (match == null)
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
      for (int index = 0; index < this._size; ++index)
      {
        if (!match(this._items[index]))
          return false;
      }
      return true;
    }

    internal static IList<T> Synchronized(List<T> list)
    {
      return (IList<T>) new List<T>.SynchronizedList(list);
    }

    [Serializable]
    internal class SynchronizedList : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
      private List<T> _list;
      private object _root;

      public int Count
      {
        get
        {
          lock (this._root)
            return this._list.Count;
        }
      }

      public bool IsReadOnly
      {
        get
        {
          return this._list.IsReadOnly;
        }
      }

      public T this[int index]
      {
        get
        {
          lock (this._root)
            return this._list[index];
        }
        set
        {
          lock (this._root)
            this._list[index] = value;
        }
      }

      internal SynchronizedList(List<T> list)
      {
        this._list = list;
        this._root = list.SyncRoot;
      }

      public void Add(T item)
      {
        lock (this._root)
          this._list.Add(item);
      }

      public void Clear()
      {
        lock (this._root)
          this._list.Clear();
      }

      public bool Contains(T item)
      {
        lock (this._root)
          return this._list.Contains(item);
      }

      public void CopyTo(T[] array, int arrayIndex)
      {
        lock (this._root)
          this._list.CopyTo(array, arrayIndex);
      }

      public bool Remove(T item)
      {
        lock (this._root)
          return this._list.Remove(item);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        lock (this._root)
          return (IEnumerator) this._list.GetEnumerator();
      }

      IEnumerator<T> IEnumerable<T>.GetEnumerator()
      {
        lock (this._root)
          return this._list.GetEnumerator();
      }

      public int IndexOf(T item)
      {
        lock (this._root)
          return this._list.IndexOf(item);
      }

      public void Insert(int index, T item)
      {
        lock (this._root)
          this._list.Insert(index, item);
      }

      public void RemoveAt(int index)
      {
        lock (this._root)
          this._list.RemoveAt(index);
      }
    }

    /// <summary>
    /// Enumerates the elements of a <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    [__DynamicallyInvokable]
    [Serializable]
    public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
    {
      private List<T> list;
      private int index;
      private int version;
      private T current;

      /// <summary>
      /// Gets the element at the current position of the enumerator.
      /// </summary>
      /// 
      /// <returns>
      /// The element in the <see cref="T:System.Collections.Generic.List`1"/> at the current position of the enumerator.
      /// </returns>
      [__DynamicallyInvokable]
      public T Current
      {
        [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
        {
          return this.current;
        }
      }

      [__DynamicallyInvokable]
      object IEnumerator.Current
      {
        [__DynamicallyInvokable] get
        {
          if (this.index == 0 || this.index == this.list._size + 1)
            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
          return (object) this.Current;
        }
      }

      internal Enumerator(List<T> list)
      {
        this.list = list;
        this.index = 0;
        this.version = list._version;
        this.current = default (T);
      }

      /// <summary>
      /// Releases all resources used by the <see cref="T:System.Collections.Generic.List`1.Enumerator"/>.
      /// </summary>
      [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
      [__DynamicallyInvokable]
      public void Dispose()
      {
      }

      /// <summary>
      /// Advances the enumerator to the next element of the <see cref="T:System.Collections.Generic.List`1"/>.
      /// </summary>
      /// 
      /// <returns>
      /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
      /// </returns>
      /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
      [__DynamicallyInvokable]
      public bool MoveNext()
      {
        List<T> list = this.list;
        if (this.version != list._version || (uint) this.index >= (uint) list._size)
          return this.MoveNextRare();
        this.current = list._items[this.index];
        ++this.index;
        return true;
      }

      private bool MoveNextRare()
      {
        if (this.version != this.list._version)
          ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
        this.index = this.list._size + 1;
        this.current = default (T);
        return false;
      }

      [__DynamicallyInvokable]
      void IEnumerator.Reset()
      {
        if (this.version != this.list._version)
          ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
        this.index = 0;
        this.current = default (T);
      }
    }
  }
}
