using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : UnitsManager
{
    [Serializable]
    public class PartyMember : Unit
    {
        public int currentExperience;
        public int maxExperience;
        public GameObject memberOverworldlPrefab;
    }

    [SerializeField] private List<PartyMember> currentPartyMember = new();
    private Vector3 playerPosition;
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

    [Button]//remove button property later
    private void AddMemberToPartyByName(string newMemberName)
    {
        AddPartyMember(newMemberName);
    }

    protected override void AddPartyMember(string unitName)
    {
        foreach (var member in allPartyMembers)
        {
            if (member.ownName == unitName)
            {
                AddToParty(member);
                break;
            }
        }
    }

    public List<PartyMember> GetCurrentPartyMembersList()
    {
        List<PartyMember> alivePartyMembers = new();
        foreach (var member in currentPartyMember)
        {
            if (member.currentHealth > 0)
            {
                alivePartyMembers.Add(member);
            }
        }
        return alivePartyMembers;
    }

    public void Savehealth(int partyMemberIndex, int health)
    {
        currentPartyMember[partyMemberIndex].currentHealth = health;
    }

    public void SetPosition(Vector3 position)
    {
        playerPosition = position;
    }

    public Vector3 GetPosition()
    {
        return playerPosition;
    }

    private void AddToParty(PartyMemeberDataSO member)
    {
        PartyMember newPartyMember = new();
        newPartyMember.memberName = member.ownName;
        newPartyMember.level = member.startingLevel;
        newPartyMember.currentHealth = member.baseHealth;
        newPartyMember.maxHealth = member.baseHealth;
        newPartyMember.maxExperience = newPartyMember.currentHealth;
        newPartyMember.strength = member.baseStrengh;
        newPartyMember.initiative = member.baseInitiative;
        newPartyMember.unitBattleVisualPrefab = member.visualPrefabs;
        newPartyMember.memberOverworldlPrefab = member.memberOverworldVisualPrefab;

        currentPartyMember.Add(newPartyMember);
    }
}
