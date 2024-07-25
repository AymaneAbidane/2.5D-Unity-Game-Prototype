using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Encounter
{
    public EnemyDataSO enemy;
    public int minLevel;
    public int maxLevel;
}
public class EncounterSystem : MonoBehaviour
{

    [SerializeField] private Encounter[] enemiesInScene;
    [SerializeField] private int maxnumberEnemies;

    private EnemysManager enemysManager;

    private void Start()
    {
        enemysManager = GameObject.FindFirstObjectByType<EnemysManager>();
        enemysManager.GenerateEnemiesByEncounter(enemiesInScene, maxnumberEnemies);
    }

}
