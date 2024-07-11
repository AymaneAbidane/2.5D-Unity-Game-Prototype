using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class PlayerBattleTigger : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly] private PlayerController playerController;

    [SerializeField] private int stepsInGrass;
    [SerializeField] private float timePerStep;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private Vector2Int minAndMaxStepsToEncounter;

    public event EventHandler onPlayerStepsreachTreshHoldForEncounter;

    private bool movingInGrass;
    private float stepTimer;
    private int stepsToEncounter;

    private void Awake()
    {
        CalculateStepsToNextEncounter();
    }

    public void SetMovingInGrass(bool value)
    {
        movingInGrass = value;
    }

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1, grassLayer);

        movingInGrass = colliders.Length != 0 && playerController.GetMovmentVector() != Vector3.zero;

        if (movingInGrass == true)
        {
            stepTimer += Time.fixedDeltaTime;

            if (stepTimer >= timePerStep)
            {
                stepsInGrass++;
                stepTimer = 0;

                if (stepsInGrass >= stepsToEncounter)
                {
                    Debug.Log("Change to battle state");
                    onPlayerStepsreachTreshHoldForEncounter?.Invoke(this, EventArgs.Empty);
                }
                //send an event to trigger the battle if the player reach the tresh hold of the steps before battle
            }
        }

    }

    private void CalculateStepsToNextEncounter()
    {
        stepsToEncounter = Random.Range(minAndMaxStepsToEncounter.x, minAndMaxStepsToEncounter.y);
        Debug.Log(stepsToEncounter);
    }

}
