using System;


/// <summary>
///     A priority queue implementation using Fibonacci heaps.
/// </summary>
public class FibonacciHeapPriorityQueue<T> where T : IComparable<T>
{
    private readonly FibonacciHeap<T> heap;

    public FibonacciHeapPriorityQueue(SortDirection sortDirection = SortDirection.Ascending)
    {
        heap = new FibonacciHeap<T>(sortDirection);
    }

    // Insert an item into the priority queue.
    public void Enqueue(T item)
    {
        heap.Insert(item);
    }

    // Extract the min/max (based on the heap type) item from the priority queue.
    public T Dequeue()
    {
        if (heap.Count == 0)
        {
            throw new InvalidOperationException("The priority queue is empty.");
        }

        return heap.Extract();
    }

    // Peek at the min/max (based on the heap type) item without removing it.
    public T Peek()
    {
        if (heap.Count == 0)
        {
            throw new InvalidOperationException("The priority queue is empty.");
        }

        return heap.Peek();
    }

    // Get the number of items in the priority queue.
    public int Count => heap.Count;

    // Update a value in the heap. This is a more advanced operation that might be necessary
    // for certain algorithms like Dijkstra's or Prim's.
    public void UpdateKey(T currentValue, T newValue)
    {
        heap.UpdateKey(currentValue, newValue);
    }

    // Merge another priority queue into this one. Useful for melding two priority queues.
    public void Merge(FibonacciHeapPriorityQueue<T> otherQueue)
    {
        heap.Merge(otherQueue.heap);
    }
}
