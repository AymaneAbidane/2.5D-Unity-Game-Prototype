using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemysManager : UnitsManager
{
    [Serializable]
    public class Enemy : Unit { }

    [SerializeField] private List<Enemy> currentEnemys = new();

    private const float LEVEL_MODIFIER = 0.5f;

    private static GameObject Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this.gameObject;
        }

        DontDestroyOnLoad(gameObject);
    }

    [Button] // remove button property after
    private void GenerateEnemy(string name, int level)
    {
        GenerateEnemyByName(name, level);
    }

    public void GenerateEnemiesByEncounter(Encounter[] encounters, int maxNumenemies)
    {
        currentEnemys.Clear();
        int numEnemies = UnityEngine.Random.Range(1, maxNumenemies + 1);

        for (int i = 0; i < numEnemies; i++)
        {
            Encounter tempEncounter = encounters[UnityEngine.Random.Range(0, encounters.Length)];
            int level = UnityEngine.Random.Range(tempEncounter.minLevel, tempEncounter.maxLevel + 1);
            GenerateEnemyByName(tempEncounter.enemy.name, level);
        }
    }

    public List<Enemy> GetCurrentEnemysList()
    {
        return currentEnemys;
    }

    protected override void GenerateEnemyByName(string enemyName, int level)
    {
        foreach (var enemy in allEnemys)
        {
            if (enemyName == enemy.ownName)
            {
                AddToCurrentEnemysList(enemy, level);
                break;
            }
        }
    }

    private void AddToCurrentEnemysList(EnemyDataSO enemy, int level)
    {
        Enemy newEnemy = new();
        newEnemy.memberName = enemy.ownName;
        newEnemy.level = level;

        float levelModifier = (LEVEL_MODIFIER * newEnemy.level);

        newEnemy.maxHealth = DataRounding(enemy.baseHealth, levelModifier);
        newEnemy.currentHealth = newEnemy.maxHealth;
        newEnemy.strength = DataRounding(enemy.baseStrengh, levelModifier);
        newEnemy.initiative = DataRounding(enemy.baseInitiative, levelModifier);
        newEnemy.unitBattleVisualPrefab = enemy.visualPrefabs;

        currentEnemys.Add(newEnemy);
    }

    private int DataRounding(int value, float levelModifier)//this method will prevent us to not having the same data value every time
    {
        return Mathf.RoundToInt(value + (value * levelModifier));
    }
}
