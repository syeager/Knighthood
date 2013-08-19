﻿// Steve Yeager
// 8.17.2013

using UnityEngine;
using System.Collections;

/// <summary>
/// Extend the GameObject class.
/// </summary>
public static class GameObjectExtension
{
  public static T GetSafeComponent<T>(this GameObject go) where T : class
  {
    T component = go.GetComponent(typeof(T)) as T;
    if (component == null)
    {
      Debugger.LogError("Could not find component " + typeof(T).ToString(), go);
    }

    return component;
  } // end GetSafeComponent

} // end GameObjectExtension class