﻿// Steve Yeager
// 8.17.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all singletons.
/// </summary>
/// <typeparam name="T">Inherited class.</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
  protected static T instance;


  public static T Instance
  {
    get
    {
      if (instance == null)
      {
        instance = (T)FindObjectOfType(typeof(T));

        if (instance == null)
        {
          Debugger.Log("Creating " + typeof(T) + " singleton");
          GameObject i = new GameObject(typeof(T).ToString());
          i.AddComponent<T>();
          instance = i.GetComponent<T>();
        }
      }

      return instance;
    }
  }

} // end Singleton class