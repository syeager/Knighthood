﻿// Steve Yeager
// 8.18.2013

using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Singleton to hold references to widely used objects.
/// </summary>
public class GameResources : Singleton<GameResources>
{
  #region Player Fields

  public GameObject[] Player_Prefabs;

  #endregion

  #region GUI

  public GameObject DamageIndicator_Prefab;
  public ObjectRecycler DamageIndicator_Pool;
  public GameObject Hitbox_Prefab;
  public ObjectRecycler Hitbox_Pool;

  #endregion

  #region Doc Fields

  public TextAsset NPCStats;

  #endregion

  #region MonoBehaviour Overrides

  private void Awake()
  {
    DontDestroyOnLoad(gameObject);

    DamageIndicator_Pool = new ObjectRecycler(DamageIndicator_Prefab);
    Hitbox_Pool = new ObjectRecycler(Hitbox_Prefab);
  } // end Awake

  #endregion

} // end GameResources class