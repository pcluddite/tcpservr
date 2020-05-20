// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======

namespace Tbasic.Components
{
    /// <summary>
    /// A tuple that is a value type and holds 1 object
    /// </summary>
    /// <typeparam name="T1">the type for Item1</typeparam>
    internal struct ValueTuple<T1>
    {
        public T1 Item1 { get; }

        public ValueTuple(T1 item1)
        {
			Item1 = item1;
        }
    }

    /// <summary>
    /// A tuple that is a value type and holds 2 objects
    /// </summary>
    /// <typeparam name="T1">the type for Item1</typeparam>
    /// <typeparam name="T2">the type for Item2</typeparam>
    internal struct ValueTuple<T1, T2>
    {
        public T1 Item1 { get; }
        public T2 Item2 { get; }

        public ValueTuple(T1 item1, T2 item2)
        {
			Item1 = item1;
			Item2 = item2;
        }
    }

    /// <summary>
    /// A tuple that is a value type and holds 3 objects
    /// </summary>
    /// <typeparam name="T1">the type for Item1</typeparam>
    /// <typeparam name="T2">the type for Item2</typeparam>
    /// <typeparam name="T3">the type for Item3</typeparam>
    internal struct ValueTuple<T1, T2, T3>
    {
        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public T3 Item3 { get; }

        public ValueTuple(T1 item1, T2 item2, T3 item3)
        {
			Item1 = item1;
			Item2 = item2;
			Item3 = item3;
        }
    }

    /// <summary>
    /// A tuple that is a value type and holds 4 objects
    /// </summary>
    /// <typeparam name="T1">the type for Item1</typeparam>
    /// <typeparam name="T2">the type for Item2</typeparam>
    /// <typeparam name="T3">the type for Item3</typeparam>
    /// <typeparam name="T4">the type for Item4</typeparam>
    internal struct ValueTuple<T1, T2, T3, T4>
    {
        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public T3 Item3 { get; }
        public T4 Item4 { get; }

        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
			Item1 = item1;
			Item2 = item2;
			Item3 = item3;
			Item4 = item4;
        }
    }

    /// <summary>
    /// A tuple that is a value type and holds 5 objects
    /// </summary>
    /// <typeparam name="T1">the type for Item1</typeparam>
    /// <typeparam name="T2">the type for Item2</typeparam>
    /// <typeparam name="T3">the type for Item3</typeparam>
    /// <typeparam name="T4">the type for Item4</typeparam>
    /// <typeparam name="T5">the type for Item5</typeparam>
    internal struct ValueTuple<T1, T2, T3, T4, T5>
    {
        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public T3 Item3 { get; }
        public T4 Item4 { get; }
        public T5 Item5 { get; }

        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
			Item1 = item1;
			Item2 = item2;
			Item3 = item3;
			Item4 = item4;
			Item5 = item5;
        }
    }

    /// <summary>
    /// A tuple that is a value type and holds 6 objects
    /// </summary>
    /// <typeparam name="T1">the type for Item1</typeparam>
    /// <typeparam name="T2">the type for Item2</typeparam>
    /// <typeparam name="T3">the type for Item3</typeparam>
    /// <typeparam name="T4">the type for Item4</typeparam>
    /// <typeparam name="T5">the type for Item5</typeparam>
    /// <typeparam name="T6">the type for Item6</typeparam>
    internal struct ValueTuple<T1, T2, T3, T4, T5, T6>
    {
        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public T3 Item3 { get; }
        public T4 Item4 { get; }
        public T5 Item5 { get; }
        public T6 Item6 { get; }

        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
			Item1 = item1;
			Item2 = item2;
			Item3 = item3;
			Item4 = item4;
			Item5 = item5;
			Item6 = item6;
        }
    }

    /// <summary>
    /// A tuple that is a value type and holds 7 objects
    /// </summary>
    /// <typeparam name="T1">the type for Item1</typeparam>
    /// <typeparam name="T2">the type for Item2</typeparam>
    /// <typeparam name="T3">the type for Item3</typeparam>
    /// <typeparam name="T4">the type for Item4</typeparam>
    /// <typeparam name="T5">the type for Item5</typeparam>
    /// <typeparam name="T6">the type for Item6</typeparam>
    /// <typeparam name="T7">the type for Item7</typeparam>
    internal struct ValueTuple<T1, T2, T3, T4, T5, T6, T7>
    {
        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public T3 Item3 { get; }
        public T4 Item4 { get; }
        public T5 Item5 { get; }
        public T6 Item6 { get; }
        public T7 Item7 { get; }

        public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        {
			Item1 = item1;
			Item2 = item2;
			Item3 = item3;
			Item4 = item4;
			Item5 = item5;
			Item6 = item6;
			Item7 = item7;
        }
    }

}
