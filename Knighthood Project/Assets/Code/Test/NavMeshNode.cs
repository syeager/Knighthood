﻿// Steve Yeager
// 9.16.2013

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A node in the nav mesh.
/// </summary>
[ExecuteInEditMode]
public class NavMeshNode : BaseMono
{
    public NavMeshNode[] neighbors;
    public float[] distances;
    public bool edge;

    public bool drawConnections;


    private void Update()
    {
        if (drawConnections)
        {
            DrawNeighborConnections();
        }
    }


    private void DrawNeighborConnections()
    {
        foreach (var neighbor in neighbors)
        {
            Debug.DrawLine(transform.position, neighbor.transform.position, (Selection.activeGameObject == gameObject ? Color.green : Color.magenta));
        }
    }


    public void Bake(Transform[] allNodes, float jumpHeight, float dropHorDist)
    {
        List<NavMeshNode> neighborList = new List<NavMeshNode>();
        List<float> distanceList = new List<float>();

        foreach (var nextNode in allNodes)
        {
            // this node
            if (nextNode == transform) continue;
            // not navemeshnode
            if (nextNode.GetComponent<NavMeshNode>() == null) continue;

            // get difference
            Vector3 difference = nextNode.position - transform.position;

            // can reach node
            RaycastHit[] hits = Physics.RaycastAll(transform.position, difference.normalized, difference.magnitude);
            if (hits.Length == 2)
            {
                if (!((hits[0].collider == nextNode.collider || hits[0].collider.isTrigger) &&
                    (hits[1].collider == nextNode.collider || hits[1].collider.isTrigger)))
                {
                    continue;
                }
            }
            else if (hits.Length >= 3)
            {
                continue;
            }

            // fall
            if (difference.y < 0 && Mathf.Abs(difference.x) <= dropHorDist)
            {
                neighborList.Add(nextNode.GetComponent<NavMeshNode>());
                distanceList.Add(difference.magnitude);
                continue;
            }
            // jump
            if (difference.magnitude <= jumpHeight)
            {
                neighborList.Add(nextNode.GetComponent<NavMeshNode>());
                distanceList.Add(difference.magnitude);
                continue;
                
            }
        }

        neighbors = neighborList.ToArray();
        distances = distanceList.ToArray();
    }
}