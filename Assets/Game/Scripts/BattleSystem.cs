using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [Serializable]
    public class BattleEnteties
    {
        public enum ActionState { Attack, Run }
        public ActionState battleState;

        public string name;
        public int currentHealth;
        public int maxHealth;
        public int initiative;
        public int strengh;
        public int level;
        public bool isPlayer;

        public BattleVisuals battleVisuals;
        public int target;

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

        public void SetTarget(int target)
        {
            this.target = target;
        }

        public void UpdateUi()
        {
            battleVisuals.ChangeHealh(currentHealth);
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
    [SerializeField] private TextMeshProUGUI battleTextPopUp;

    private int currentBattler;
    void Start()
    {
        CreatePartyEnteties();
        CreateEnemyEnteties();
        ShowBattleMenu();
        AttackAction(allBattlers[0], allBattlers[1]);
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


    public void BTN_SelectEnemy(int currentEnemy)
    {
        //setting the current members target
        BattleEnteties currentPlayerEntetie = playerBattlers[currentBattler];
        currentPlayerEntetie.SetTarget(allBattlers.IndexOf(enemyBattlers[currentEnemy]));

        //tell battle system this member is going to atk
        currentPlayerEntetie.battleState = BattleEnteties.ActionState.Attack;
        //incerement through our party members
        currentBattler++;

        if (currentBattler >= playerBattlers.Count)//if all players have selected an action
        {
            //start the battle
            Debug.Log("Start The Battle");
            Debug.Log("We Are Attacking : " + allBattlers[currentPlayerEntetie.target].name);
        }
        else
        {
            //show the battle menu for the next player
            SetUiActivation(enemySelectionMenu, false);
            ShowBattleMenu();
        }
    }

    private void AttackAction(BattleEnteties currentAttacker, BattleEnteties currentTarget)
    {
        int damage = currentAttacker.strengh;  //get damage value
        currentAttacker.battleVisuals.PlayAttackAnimation(); //play attack animation
        currentTarget.currentHealth -= damage; //dealing damage
        currentTarget.battleVisuals.PlayGettingHitAnimation(); //play target hit animation
        currentTarget.UpdateUi(); //update the ui
        battleTextPopUp.text = string.Format("{0} Attacks {1} for {2} Damage", currentAttacker.name, currentTarget.name, damage);

    }

    private void SetUiActivation(GameObject ui, bool isActive)
    {
        ui.SetActive(isActive);
    }


}
