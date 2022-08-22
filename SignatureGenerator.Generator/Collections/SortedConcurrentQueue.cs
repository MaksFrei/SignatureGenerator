using SignatureGenerator.Generator.Collections.Abstractions;
using SignatureGenerator.Generator.Models.Abstractions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace SignatureGenerator.Generator.Queue
{
    /// <summary>
    /// Concurrent queue wich guarantees that elements will be in ordered sequence.
    /// Uses <see cref="object.Equals(object?)"/> for comparing keys
    /// </summary>
    /// <typeparam name="TStoredElementType">Queued element</typeparam>
    /// <typeparam name="TSortKey">Sort key</typeparam>
    public class SortedConcurrentQueue<TStoredElementType, TSortKey> : IConcurrentQueue<TStoredElementType>
        where TStoredElementType : ISorterable<TSortKey>
    {

        private ConcurrentQueue<TStoredElementType> queue = new ConcurrentQueue<TStoredElementType>();
        private TSortKey currentOrderValue;
        private readonly Func<TSortKey, TSortKey> nextValueGenerator;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count => queue.Count;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsSynchronized => ((ICollection)queue).IsSynchronized;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object SyncRoot => ((ICollection)queue).SyncRoot;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="startValue">Default value for order key</param>
        /// <param name="nextValueGenerator">Order key generator</param>
        public SortedConcurrentQueue(TSortKey startValue, Func<TSortKey, TSortKey> nextValueGenerator)
        {
            if (nextValueGenerator is null) throw new ArgumentNullException(nameof(nextValueGenerator));

            currentOrderValue = startValue;
            this.nextValueGenerator = nextValueGenerator;
        }

        /// <summary>
        /// Enqueue
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(TStoredElementType item)
        {
            var id = item.Order;
            lock (queue)
            {
                while (!id.Equals(currentOrderValue))
                {
                    Monitor.Wait(queue);
                }

                queue.Enqueue(item);
                currentOrderValue = nextValueGenerator(currentOrderValue);
                Monitor.PulseAll(queue);
            }
        }

        /// <summary>
        /// <see cref="ConcurrentQueue{T}.TryDequeue(out T)"/>
        /// </summary>
        /// <param name="item"></param>
        public bool TryDequeue(out TStoredElementType item)
        {
            return queue.TryDequeue(out item);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TStoredElementType> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)queue).CopyTo(array, index);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        void IProducerConsumerCollection<TStoredElementType>.CopyTo(TStoredElementType[] array, int index)
        {
            queue.CopyTo(array, index);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        TStoredElementType[] IProducerConsumerCollection<TStoredElementType>.ToArray()
        {
            return queue.ToArray();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IProducerConsumerCollection<TStoredElementType>.TryAdd(TStoredElementType item)
        {
            Enqueue(item);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IProducerConsumerCollection<TStoredElementType>.TryTake([MaybeNullWhen(false)] out TStoredElementType item)
        {
            return ((IProducerConsumerCollection<TStoredElementType>)queue).TryTake(out item);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)queue).GetEnumerator();
        }
    }
}
