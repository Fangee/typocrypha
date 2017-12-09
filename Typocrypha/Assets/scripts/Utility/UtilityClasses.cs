using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ref<T>
{
    public Ref(T element)
    {
        this.element = element;
    }
    T element;
    public T Obj { get { return element; } set { element = value; } }
}

