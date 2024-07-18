using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [Serializable]
    public struct PartyMember
    {
        public string memberName;
        public int level;
        public int currenthealth;
        public int maxHealth;
        public int strength;
        public int initiative;
        public int currentExperience;
        public int maxExperience;
        public GameObject memberBattleVisualPrefab;
        public GameObject memberOverworldlPrefab;
    }

    [SerializeField, AssetsOnly] private PartyMemeberDataSO[] allMembers;
    [Space]
    [SerializeField] private List<PartyMember> currentParty;


    [Button]//remove button property later
    public void AddMemberToPartyByName(string newMemberName)
    {
        foreach (var member in allMembers)
        {
            if (member.name == newMemberName)
            {
                AddToParty(member);
                break;
            }
        }
    }

    private void AddToParty(PartyMemeberDataSO member)
    {
        PartyMember newPartyMember = new();
        newPartyMember.memberName = member.ownName;
        newPartyMember.level = member.startingLevel;
        newPartyMember.currenthealth = member.baseHealth;
        newPartyMember.maxExperience = newPartyMember.currenthealth;
        newPartyMember.strength = member.baseStrengh;
        newPartyMember.initiative = member.baseInitiative;
        newPartyMember.memberBattleVisualPrefab = member.visualPrefabs;
        newPartyMember.memberOverworldlPrefab = member.memberOverworldVisualPrefab;

        currentParty.Add(newPartyMember);
    }
}
