using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Unit
{
    public string memberName;
    public int level;
    public int currentHealth;
    public int maxHealth;
    public int strength;
    public int initiative;
    public GameObject unitBattleVisualPrefab;
}

public abstract class UnitsManager : MonoBehaviour
{
    public enum UnitsType { PartyMembers, Enemys }

    [SerializeField] protected UnitsType unitsType;
    [Space]
    [SerializeField, AssetsOnly, ShowIf("unitsType", UnitsType.PartyMembers)] protected PartyMemeberDataSO[] allPartyMembers;
    [SerializeField, AssetsOnly, ShowIf("unitsType", UnitsType.Enemys)] protected EnemyDataSO[] allEnemys;


    protected virtual void AddPartyMember(string partyMemberName) { }
    protected virtual void GenerateEnemyByName(string enemyName, int level) { }
}
