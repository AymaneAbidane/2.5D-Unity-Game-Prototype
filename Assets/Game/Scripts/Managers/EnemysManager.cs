using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemysManager : UnitsManager
{
    [Serializable]
    public class Enemy : Unit { }

    [SerializeField] private List<Enemy> currentEnemys = new();

    private const float LEVEL_MODIFIER = 0.5f;


    [Button] // remove button property after
    private void GenerateEnemy(string name, int level)
    {
        GenerateEnemyByName(name, level);
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
