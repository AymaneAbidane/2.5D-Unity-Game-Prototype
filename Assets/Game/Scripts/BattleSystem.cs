using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [Serializable]
    public class BattleEnteties
    {
        public string name;
        public int currentHealth;
        public int maxHealth;
        public int initiative;
        public int strengh;
        public int level;
        public bool isPlayer;

        public BattleVisuals battleVisuals;

        public void SetEntetiesValues(string name, int currenthealth, int maxhealth, int initiative, int strengh, int level, bool isPlayer)
        {
            this.name = name;
            this.currentHealth = currenthealth;
            this.maxHealth = maxhealth;
            this.initiative = initiative;
            this.strengh = strengh;
            this.level = level;
            this.isPlayer = isPlayer;
        }
    }

    private const string ACTION_MESSAGE = "'s Action:";

    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPointsTransforms;
    [SerializeField] private Transform[] enemySpawnPointsTransforms;

    [Header("Managers")]
    [SerializeField] private PartyManager partyManager;
    [SerializeField] private EnemysManager enemyManager;

    [Header("Battlers")]
    [SerializeField] private List<BattleEnteties> allBattlers = new();
    [SerializeField] private List<BattleEnteties> enemyBattlers = new();
    [SerializeField] private List<BattleEnteties> playerBattlers = new();

    [Header("Ui")]
    [SerializeField] private GameObject[] enemySelectionButtonsArray;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject enemySelectionMenu;
    [SerializeField] private TextMeshProUGUI actionText;


    private int currentBattler;
    void Start()
    {
        CreatePartyEnteties();
        CreateEnemyEnteties();
        ShowBattleMenu();
    }

    private void CreatePartyEnteties()
    {
        List<PartyManager.PartyMember> currentParty = new();
        currentParty = partyManager.GetCurrentPartyMembersList();

        CreateEnteties(currentParty, playerBattlers, true);
    }

    private void CreateEnemyEnteties()
    {
        List<EnemysManager.Enemy> currentEnemys = new();
        currentEnemys = enemyManager.GetCurrentEnemysList();
        CreateEnteties(currentEnemys, enemyBattlers, false);
    }

    private void CreateEnteties<T>(List<T> currentMembers, List<BattleEnteties> battlersList, bool isPlayer) where T : Unit
    {
        // i use the generict because i know that boths lists that i will get from the managers there class they inherit from class 'unit'

        for (int i = 0; i < currentMembers.Count; i++)
        {
            T member = currentMembers[i];
            Vector3 spawnPosition;

            BattleEnteties tempEntety = new();
            tempEntety.SetEntetiesValues(member.memberName, member.currentHealth, member.maxHealth, member.initiative, member.strength, member.level, isPlayer);

            if (isPlayer)
            {
                spawnPosition = partySpawnPointsTransforms[i].position;
            }
            else
            {
                spawnPosition = enemySpawnPointsTransforms[i].position;
            }

            //the visual exist on array on partymanager and enemysmanager
            BattleVisuals tempBattleVisual = Instantiate(member.unitBattleVisualPrefab, spawnPosition, Quaternion.identity).GetComponent<BattleVisuals>();
            tempBattleVisual.SetStartingValues(member.maxHealth, member.maxHealth, member.level);
            tempEntety.battleVisuals = tempBattleVisual;


            allBattlers.Add(tempEntety);
            battlersList.Add(tempEntety);
        }
    }

    private void ShowBattleMenu()
    {
        actionText.text = playerBattlers[currentBattler].name + ACTION_MESSAGE;
        SetUiActivation(battleMenu, true);
    }

    public void BTN_ShowEnemySelectionMenu()
    {
        SetUiActivation(battleMenu, false);
        SetEnemySelectionButton();
        SetUiActivation(enemySelectionMenu, true);
    }

    private void SetEnemySelectionButton()
    {
        foreach (var button in enemySelectionButtonsArray)
        {
            SetUiActivation(button, false);
        }

        for (int i = 0; i < enemyBattlers.Count; i++)
        {
            GameObject enemyButton = enemySelectionButtonsArray[i];
            SetUiActivation(enemyButton, true);
            enemyButton.GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[i].name;
        }
    }

    private void SetUiActivation(GameObject ui, bool isActive)
    {
        ui.SetActive(isActive);
    }


}
