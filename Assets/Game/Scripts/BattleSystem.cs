using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleSystem : MonoBehaviour
{
    [Serializable]
    public class BattleEnteties
    {
        public enum ActionState { Attack, Run }
        public ActionState actionState;

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


    [SerializeField]
    private enum BattleState { Start, Selection, Battle, Won, Lost, Run }

    [Header("Battle State")]
    [SerializeField] private BattleState state;

    [Header("Managers")]
    [SerializeField] private PartyManager partyManager;
    [SerializeField] private EnemysManager enemyManager;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPointsTransforms;
    [SerializeField] private Transform[] enemySpawnPointsTransforms;


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
    [SerializeField] private GameObject battleTextPopParentObejctHolder;


    private int currentBattler;
    private string LOOS_MESSAGE = " You Loss The Battle";
    private const string SUCCESFULY_RUN_MESSAGE = " You Run Away";
    private const int RUN_CHANCE = 50;
    private const string WIN_MESSAGE = " You Won The Battle";
    private const string ACTION_MESSAGE = "'s Action:";
    private const float TURN_DURATION = 1.3f;
    private const string UNSUCCESUFUL_RUN_MESSAGE = " You Can't Run";

    private void Start()
    {
        enemyManager = GameObject.FindFirstObjectByType<EnemysManager>();
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();

        CreatePartyEnteties();
        CreateEnemyEnteties();
        ShowBattleMenu();
        DetermineBattleOrder();
    }


    private IEnumerator COR_BattleRoutine()
    {
        SetUiActivation(enemySelectionMenu, false); //enemy slection menu disable
        ChangeBattleState(BattleState.Battle); //change our state to the battle state
        SetUiActivation(battleTextPopParentObejctHolder, true); //enable our bottom text

        //loop through all battlers
        //==> do theire appropriate action 

        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (state == BattleState.Battle && allBattlers[i].currentHealth > 0)
            {
                switch (allBattlers[i].actionState)
                {
                    case BattleEnteties.ActionState.Attack:
                        //do the attack
                        yield return StartCoroutine(COR_AttackRoutine(i));
                        break;
                    case BattleEnteties.ActionState.Run:
                        //run
                        yield return StartCoroutine(COR_RunRoutine());
                        break;
                    default:
                        Debug.Log("Error incorrect battle action");
                        break;
                }
            }
        }

        RemoveDeadBattlers();

        if (state == BattleState.Battle)
        {
            SetUiActivation(battleTextPopParentObejctHolder, false);
            currentBattler = 0;
            ShowBattleMenu();
        }
        yield return null;
        // if we havent won or lost , repeat the loop by opening battle menu
    }

    private IEnumerator COR_AttackRoutine(int i)
    {
        BattleEnteties currentAttacker = allBattlers[i];
        //player turn
        if (currentAttacker.isPlayer == true)
        {
            if (allBattlers[currentAttacker.target].currentHealth <= 0)
            {
                currentAttacker.SetTarget(GetRandomUnitIndex(false));
            }

            BattleEnteties currentTarget = allBattlers[currentAttacker.target];
            AttackAction(currentAttacker, currentTarget); //attack selected enemy (attack action)
            yield return new WaitForSeconds(TURN_DURATION);  //wait a few seconds

            if (currentTarget.currentHealth <= 0)//kill the enemy
            {
                battleTextPopUp.text = string.Format("{0} defeated {1}", currentAttacker.name, currentTarget.name);
                yield return new WaitForSeconds(TURN_DURATION);
                RemoveDeadUnits(currentTarget, enemyBattlers);

                if (enemyBattlers.Count <= 0)//if no enemies remain
                {
                    //=> we won the battle
                    ChangeBattleState(BattleState.Won);
                    yield return new WaitForSeconds(TURN_DURATION);
                    ScenesManager.Instance.LoadOpenWorldScene();
                    Debug.Log("Go back to overworld scene");
                }
            }
        }

        if (i < allBattlers.Count && currentAttacker.isPlayer == false)//enemies turn
        {

            currentAttacker.SetTarget(GetRandomUnitIndex(true));
            BattleEnteties currentTarget = allBattlers[currentAttacker.target];
            AttackAction(currentAttacker, currentTarget); //attack selected party member (attack action)
            yield return new WaitForSeconds(TURN_DURATION); //wait few seconds


            if (currentTarget.currentHealth <= 0)
            {
                //kill the party member
                battleTextPopUp.text = string.Format("{0} defeated {1}", currentAttacker.name, currentTarget.name);
                yield return new WaitForSeconds(TURN_DURATION);
                RemoveDeadUnits(currentTarget, playerBattlers);

                if (playerBattlers.Count <= 0) //if no party members remain
                {
                    //=> we lost the battle
                    ChangeBattleState(BattleState.Lost);
                    yield return new WaitForSeconds(TURN_DURATION);
                    Debug.Log("Game Over");
                }

            }
        }
    }

    private void ChangeBattleState(BattleState battleState)
    {
        state = battleState;
        if (state == BattleState.Won)
        {
            battleTextPopUp.text = WIN_MESSAGE;
        }
        else if (state == BattleState.Lost)
        {
            battleTextPopUp.text = LOOS_MESSAGE;
        }
        else if (state == BattleState.Run)
        {
            battleTextPopUp.text = SUCCESFULY_RUN_MESSAGE;
        }
    }

    private void RemoveDeadUnits(BattleEnteties currentTarger, List<BattleEnteties> battleUnits)
    {
        battleUnits.Remove(currentTarger);
    }

    private int GetRandomUnitIndex(bool isAPartyMember)
    {
        List<int> playerBattlersIndexes = new();
        List<int> enemysBattlersIndexes = new();

        int indexValue;

        for (int i = 0; i < allBattlers.Count; i++)
        {
            BattleEnteties battler = allBattlers[i];

            if (battler.isPlayer == true)
            {
                playerBattlersIndexes.Add(i);
            }
            else
            {
                enemysBattlersIndexes.Add(i);
            }

        }

        if (isAPartyMember == true)
        {
            indexValue = playerBattlersIndexes[UnityEngine.Random.Range(0, playerBattlersIndexes.Count)];
        }
        else
        {
            indexValue = enemysBattlersIndexes[UnityEngine.Random.Range(0, enemysBattlersIndexes.Count)];
        }

        return indexValue;
    }

    private void RemoveDeadBattlers()
    {
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].currentHealth <= 0)
            {
                allBattlers.RemoveAt(i);
            }
        }
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
            int healthPoints;
            if (isPlayer == true)
            {
                healthPoints = member.currentHealth;
            }
            else
            {
                healthPoints = member.maxHealth;
            }
            tempBattleVisual.SetStartingValues(healthPoints, member.maxHealth, member.level);
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
        currentPlayerEntetie.actionState = BattleEnteties.ActionState.Attack;
        //incerement through our party members
        currentBattler++;

        if (currentBattler >= playerBattlers.Count)//if all players have selected an action
        {
            //start the battle
            Debug.Log("Start The Battle");
            StartCoroutine(COR_BattleRoutine());
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

        SaveHealth();
    }

    private void SetUiActivation(GameObject ui, bool isActive)
    {
        ui.SetActive(isActive);
    }

    private void SaveHealth()
    {
        for (int i = 0; i < playerBattlers.Count; i++)
        {
            partyManager.Savehealth(i, playerBattlers[i].currentHealth);
        }
    }

    private void DetermineBattleOrder()
    {
        allBattlers.Sort((bi1, bi2) => -bi1.initiative.CompareTo(bi2.initiative));
    }

    public void BTN_RunAction()
    {
        state = BattleState.Selection;

        BattleEnteties currentPlayerEntetiy = playerBattlers[currentBattler];
        currentPlayerEntetiy.actionState = BattleEnteties.ActionState.Run;

        SetUiActivation(battleMenu, false);

        currentBattler++;

        if (currentBattler >= playerBattlers.Count)
        {
            StartCoroutine(COR_BattleRoutine());
        }
        else
        {
            //show the battle menu for the next player
            SetUiActivation(enemySelectionMenu, false);
            ShowBattleMenu();
        }
    }

    private IEnumerator COR_RunRoutine()
    {
        if (state == BattleState.Battle)
        {
            if (Random.Range(1, 101) >= RUN_CHANCE)
            {
                //we ve ran away
                ChangeBattleState(BattleState.Run);
                allBattlers.Clear();

                yield return new WaitForSeconds(TURN_DURATION);

                ScenesManager.Instance.LoadOpenWorldScene();
                yield break;
            }
            else
            {
                //we cant run
                battleTextPopUp.text = UNSUCCESUFUL_RUN_MESSAGE;
                yield return new WaitForSeconds(TURN_DURATION);
            }
        }
    }
}
