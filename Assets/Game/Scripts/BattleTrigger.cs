using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly] private BoxCollider boxCollider;

    [Header("Trigger Size")]
    [SerializeField, Range(1f, 50f)] private float width;
    [SerializeField, Range(1f, 50f)] private float height;

    private void OnValidate()
    {
        boxCollider.size = new Vector3(width, 1f, height);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, boxCollider.size);
    }
}
