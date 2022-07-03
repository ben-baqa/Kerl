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
    RockSelector rockSelector;
    FakeSkipper skipper;
    FakeSweeper sweeper;
    RockPile rockPile;

    ScoreHUD scoreHUD;
    InputIconHUDManager inputIconHUDManager;
    LoadSceneOnInput endLoader;

    CameraAngleManager cameraManager;
    BrushingCamera brushCam;
    TeamIntro teamIntro;

    AudioEffects audioEffects;

    int throwCount;

    void Start()
    {
        instance = this;

        turnManager = FindObjectOfType<TurnManager>();
        rockSelector = FindObjectOfType<RockSelector>();
        skipper = FindObjectOfType<FakeSkipper>();
        sweeper = FindObjectOfType<FakeSweeper>();
        rockPile = FindObjectOfType<RockPile>();
        rockPile.PlaceRocks(rocks);

        scoreHUD = FindObjectOfType<ScoreHUD>();
        inputIconHUDManager = FindObjectOfType<InputIconHUDManager>();
        endLoader = FindObjectOfType<LoadSceneOnInput>();
        endLoader.enabled = false;

        cameraManager = FindObjectOfType<CameraAngleManager>();
        cameraManager.TransitionComplete += OnCameraTransitionComplete;
        cameraManager.ApplyGameState(GameState.Establishing);

        brushCam = FindObjectOfType<BrushingCamera>();
        teamIntro = FindObjectOfType<TeamIntro>();
        teamIntro.Complete += OnTeamIntroComplete;

        audioEffects = FindObjectOfType<AudioEffects>();
    }

    public void Proceed()
    {
        GameState++;
        if (GameState > GameState.Complete)
        {
            // return to menu
        }
        //ApplyState();
    }

    void ApplyState()
    {
        cameraManager.ApplyGameState(GameState);
        if (GameState == GameState.TeamIntro)
            teamIntro.StartSequence();

        //print("New game State: " + _gameState);
    }

    void OnCameraTransitionComplete(object sender, GameState newState)
    {
        //print("Camera transition was completed");
        //GameState = newState;
        if (newState == GameState.Establishing)
            GameState = GameState.TeamIntro;
        if (newState == GameState.Brushing)
            brushCam.followRock = true;
    }


    void OnTeamIntroComplete(object sender, EventArgs e) => StartTurn();

    public void StartTurn()
    {
        if(throwCount >= rocks * 2)
        {
            if (NetworkMessageHandler.isHost)
                StartCoroutine(EnableEndLoader());

            return;
        }

        audioEffects.OnTurnStart(blueTurn);
        turnManager.OnTurnStart(blueTurn);
        scoreHUD.OnTurnStart();
        inputIconHUDManager.OnTurnStart(blueTurn);
        skipper.OnTurnStart(blueTurn);
        rockSelector.StartSelecting(blueTurn);

        GameState = GameState.RockSelection;
    }

    public void OnRockSelect(Rock rock)
    {
        rockPile.RemoveRock(blueTurn);
        inputIconHUDManager.OnRockSelected();
        skipper.StartTargetSelection(rock);
        sweeper.SetRock(rock);
        brushCam.SetRock(rock);
        audioEffects.OnRockSelection();

        GameState = GameState.TargetSelection;
    }

    public void OnTargetSelect()
    {
        inputIconHUDManager.OnTargetSelected();
        audioEffects.OnTargetSelection();
        GameState = GameState.Throwing;
    }

    public void OnThrow()
    {
        brushCam.followRock = true;
        turnManager.OnThrow();
        inputIconHUDManager.OnThrow();
        sweeper.OnThrow();
        //scoreHUD.OnThrow();
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


    // wait a few seconds at the podium view before the player can load back to the menu
    IEnumerator EnableEndLoader()
    {
        yield return new WaitForSecondsRealtime(finishDelay);
        scoreHUD.ShowEndCard();
        GameState = GameState.Complete;
        yield return new WaitForSecondsRealtime(endLoaderDelay);
        scoreHUD.ShowEndLeaveText();
        endLoader.enabled = true;
    }
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