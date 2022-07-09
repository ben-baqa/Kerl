using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

/// <summary>
/// Keeps track of which player has active input, and the flow of game state
/// </summary>
public class TurnManager : MonoBehaviour
{
    // Identifies this instance as the authoritative Network Host
    public static bool isHost = true;

    public bool IsAI => (blueTurn ? blueTeam : redTeam).aiTeam;
    public int CurrentPlayer => (blueTurn ? blueTeam : redTeam).CurrentPlayer;
    public int NextPlayer => (blueTurn ? blueTeam : redTeam).NextPlayer;

    InputIconHUDManager inputIconHUDManager;
    CharacterManager characterManager;
    AIScript ai;
    Team blueTeam, redTeam;

    private bool blueTurn = true;

    // Start is called before the first frame update
    void Start()
    {
        inputIconHUDManager = FindObjectOfType<InputIconHUDManager>();
        characterManager = GetComponent<CharacterManager>();
        ai = GetComponent<AIScript>();

        if (MenuSelections.teams == null)
        {
            // fabricate players for input
            InputProxy.AddInput(new KeyInput(Keyboard.current, "a"));
            InputProxy.AddInput(new KeyInput(Keyboard.current, "b"));
            InputProxy.AddInput(new KeyInput(Keyboard.current, "c"));
            blueTeam = new Team(new List<int> { 0, 1 }, ai);
            redTeam = new Team(new List<int> { 2 }, ai);
        }
        else
        {
            // use real input
            blueTeam = new Team(MenuSelections.teams[0], ai);
            redTeam = new Team(MenuSelections.teams[1], ai);
        }
    }

    public void OnTurnStart(bool turn)
    {
        blueTurn = turn;

        if (blueTurn)
            blueTeam.Next(2);
        else
            redTeam.Next(2);

        inputIconHUDManager.Index = CurrentPlayer;
        ai.StartTimer();

        characterManager.OnTurnStart(CurrentPlayer, NextPlayer);
    }

    public void OnThrow()
    {
        if (blueTurn)
            blueTeam.Next();
        else
            redTeam.Next();
        inputIconHUDManager.Index = ((blueTurn ? blueTeam : redTeam).CurrentPlayer);
    }

    public bool GetInput()
    {
        if (!isHost)
            return false;

        if (blueTurn)
            return blueTeam.Input;
        else
            return redTeam.Input;
    }
    
    public bool GetToggledInput()
    {
        if (!isHost)
            return false;

        if (blueTurn)
            return blueTeam.ToggledInput;
        else
            return redTeam.ToggledInput;
    }

    public class Team
    {
        public bool Input => aiTeam? ai.brushing : InputProxy.P(members[currentIndex]);
        public bool ToggledInput => aiTeam ? ai.brushing : InputProxy.GetToggledInput(members[currentIndex]);
        public int CurrentPlayer => aiTeam ? -1 : members[currentIndex];
        public int NextPlayer => aiTeam ? -1 : members[GetNextIndex()];

        AIScript ai;

        int[] members;
        int count;
        int currentIndex;
        public bool aiTeam;

        public Team(List<int> players, AIScript aiScript = null)
        {
            ai = aiScript;
            count = players.Count;
            members = players.ToArray();

            currentIndex = 0;
            if (count == 0)
                aiTeam = true;
        }

        public void Next(int increase = 1)
        {
            if (aiTeam)
                return;
            currentIndex = (currentIndex + increase) % count;
        }

        private int GetNextIndex(int increase = 1) => (currentIndex + increase) % count;

        public override string ToString()
        {
            string res = "Team: [";
            for (int i = 0; i < count; i++)
                res += members[i] + ",";
            res += $"], active player: {CurrentPlayer}, aiTeam? {aiTeam}";
            return res;
        }
    }
}
