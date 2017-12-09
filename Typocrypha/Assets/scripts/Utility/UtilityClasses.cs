using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class that allows integral values to be stored as references in Unity/c# 4
public class Ref<T>
{
    public Ref(T element)
    {
        this.element = element;
    }
    T element;
    public T Obj { get { return element; } set { element = value; } }
}

