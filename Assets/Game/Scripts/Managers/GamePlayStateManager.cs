using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GamePlayState { OpenWorld, Battle }

public class GamePlayStateManager : MonoBehaviour
{
    [SerializeField, SceneObjectsOnly] private PlayerBattleTigger playerBattleTigger;

    private GamePlayState currentState;

    public event EventHandler<GamePlayState> onGamePlayStateChanged;

    private void Awake()
    {
        playerBattleTigger.onPlayerStepsreachTreshHoldForEncounter += PlayerBattleTigger_onPlayerStepsreachTreshHoldForEncounter;
    }

    private void Start()
    {
        SetCurrentState(GamePlayState.OpenWorld);
    }


    private void OnDestroy()
    {
        playerBattleTigger.onPlayerStepsreachTreshHoldForEncounter -= PlayerBattleTigger_onPlayerStepsreachTreshHoldForEncounter;
    }

    private void SetCurrentState(GamePlayState state)
    {
        currentState = state;
        onGamePlayStateChanged?.Invoke(this, state);
        if (currentState == GamePlayState.Battle)
        {
            ScenesManager.Instance.LoadBattleScene();
        }
    }

    private void PlayerBattleTigger_onPlayerStepsreachTreshHoldForEncounter(object sender, System.EventArgs e)
    {
        SetCurrentState(GamePlayState.Battle);

    }
}
