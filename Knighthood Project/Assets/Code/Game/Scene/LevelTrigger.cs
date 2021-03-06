﻿// Steve Yeager
// 10.6.2013

using UnityEngine;

/// <summary>
/// Triggers in the level that call methods on the LevelManager when touched by a character.
/// </summary>
public class LevelTrigger : BaseMono
{
    #region Public Fields

    public string method;

    #endregion


    #region MonoBehaviour Overrides

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        LevelManager.Instance.RecieveTrigger(method);
        Destroy(gameObject);
    }

    #endregion
}