using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;
    public static bool blueTurn = true;

    public GameState GameState
    {
        get => _gameState;
        set
        {
            _gameState = value;
            ApplyState();
        }
    }
    GameState _gameState = GameState.Establishing;

    public int rocks = 5;
    public float finishDelay = 5;
    public float endLoaderDelay = 3;

    TurnManager turnManager;
    CharacterManager characterManager;
    Thrower thrower;
    Sweeper sweeper;
    AIScript ai;
    RockSelector rockSelector;
    RockPile rockPile;

    Scorekeeper scorekeeper;
    ScoreHUD scoreHUD;
    InputIconHUDManager inputIconHUDManager;
    ExitOptions exitOptions;

    CameraAngleManager cameraManager;
    BrushingCamera brushCam;
    TeamIntro teamIntro;
    PodiumView podiumView;

    AudioEffects audioEffects;

    int throwCount;

    void Start()
    {
        instance = this;

        turnManager = FindObjectOfType<TurnManager>();
        characterManager = FindObjectOfType<CharacterManager>();
        thrower = FindObjectOfType<Thrower>();
        sweeper = FindObjectOfType<Sweeper>();
        ai = FindObjectOfType<AIScript>();
        rockSelector = FindObjectOfType<RockSelector>();
        rockPile = FindObjectOfType<RockPile>();
        rockPile.PlaceRocks(rocks);

        scorekeeper = FindObjectOfType<Scorekeeper>();
        scoreHUD = FindObjectOfType<ScoreHUD>();
        inputIconHUDManager = FindObjectOfType<InputIconHUDManager>();
        exitOptions = FindObjectOfType<ExitOptions>();

        cameraManager = FindObjectOfType<CameraAngleManager>();
        cameraManager.TransitionComplete += OnCameraTransitionComplete;
        cameraManager.ApplyGameState(GameState.Establishing);

        brushCam = FindObjectOfType<BrushingCamera>();
        teamIntro = FindObjectOfType<TeamIntro>();
        teamIntro.Complete = StartTurn;
        podiumView = FindObjectOfType<PodiumView>();

        audioEffects = FindObjectOfType<AudioEffects>();
    }

    void ApplyState()
    {
        cameraManager.ApplyGameState(GameState);

        //print("New game State: " + _gameState);
    }

    void OnCameraTransitionComplete(object sender, GameState newState)
    {
        //print("Camera transition was completed");
        //GameState = newState;
        if (newState == GameState.Establishing)
        {
            teamIntro.StartSequence();
            GameState = GameState.TeamIntro;
        }
        if (newState == GameState.Brushing)
            brushCam.followRock = true;
    }

    public void StartTurn()
    {
        if(throwCount >= rocks * 2)
        {
            if (NetworkMessageHandler.isHost)
                scorekeeper.GetFinalResult();
                //StartCoroutine(EnableEndLoader());

            return;
        }

        audioEffects.OnTurnStart(blueTurn);
        turnManager.OnTurnStart(blueTurn);
        scoreHUD.OnTurnStart();
        inputIconHUDManager.OnTurnStart(blueTurn);
        sweeper.OnTurnStart();
        rockSelector.StartSelecting(blueTurn);

        GameState = GameState.RockSelection;
    }

    public void OnRockSelect(Rock rock)
    {
        rockPile.RemoveRock(blueTurn);
        inputIconHUDManager.OnRockSelected();
        thrower.StartTargetSelection(rock);
        sweeper.SetRock(rock);
        ai.StartTimer();
        scorekeeper.OnRockSelected(rock);
        brushCam.SetRock(rock);
        audioEffects.OnRockSelection();

        GameState = GameState.TargetSelection;
    }

    public void OnTargetSelect()
    {
        ai.StartTimer();
        inputIconHUDManager.OnTargetSelected();
        audioEffects.OnTargetSelection();
        GameState = GameState.Throwing;
    }

    public void OnThrow()
    {
        brushCam.followRock = true;
        turnManager.OnThrow();
        inputIconHUDManager.OnThrow();
        characterManager.OnThrow();
        //sweeper.OnThrow();
        audioEffects.OnThrow();
        throwCount++;
        GameState = GameState.Brushing;
    }

    public static void OnRockPassResultThreshold() => instance.OnBrushingComplete();
    public void OnBrushingComplete()
    {
        brushCam.followRock = false;
        inputIconHUDManager.OnResult();
        scoreHUD.OnResult();
        sweeper.OnResult();
        GameState = GameState.Result;
    }

    public static void OnRockStop()
    {
        blueTurn = !blueTurn;
        instance.StartTurn();
    }

    public void ShowFinalResult() => StartCoroutine(EnableEndLoader());

    // wait a few seconds at the podium view before the player can load back to the menu
    IEnumerator EnableEndLoader()
    {
        scoreHUD.ShowWinner();
        yield return new WaitForSecondsRealtime(finishDelay);
        GameState = GameState.Complete;

        scoreHUD.Hide();
        sweeper.OnTurnStart();
        podiumView.Apply(turnManager, characterManager);
        yield return new WaitForSecondsRealtime(endLoaderDelay);
        exitOptions.Activate();
    }

    //public void Reset()
    //{
    //    print("Attempting to reset game, but method is not implemented");
    //}
}

public enum GameState
{
    Establishing,
    TeamIntro,
    RockSelection,
    TargetSelection,
    Throwing,
    Brushing,
    Result,
    Complete
}