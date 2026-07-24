using System.Collections.Generic;
using UnityEngine;

public class ComponentPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Queue<T> available = new Queue<T>();

    public ComponentPool(T prefab, Transform parent, int initialSize)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T item = Object.Instantiate(prefab, parent);
            item.gameObject.SetActive(false);
            available.Enqueue(item);
        }
    }

    public T Get()
    {
        if (available.Count > 0)
        {
            T item = available.Dequeue();
            item.gameObject.SetActive(true);
            return item;
        }
        return Object.Instantiate(prefab, parent);
    }

    public void Release(T item)
    {
        item.gameObject.SetActive(false);
        available.Enqueue(item);
    }
}