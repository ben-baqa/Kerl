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
            blueTeam = new Team(MenuSelections.debugTeams[0], ai);
            redTeam = new Team(MenuSelections.debugTeams[1], ai);

            // fabricate players for input
            foreach (List<int> team in MenuSelections.debugTeams)
                foreach (int i in team)
                    InputProxy.AddInput(new KeyInput(Keyboard.current, "" + (char)('a' + i)));
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

        if (CurrentPlayer >= 0)
            inputIconHUDManager.Index = CurrentPlayer;
        else
            inputIconHUDManager.SetAI();

        characterManager.OnTurnStart(CurrentPlayer, NextPlayer);
    }

    public void OnThrow()
    {
        if (blueTurn)
            blueTeam.Next();
        else
            redTeam.Next();

        int currentplayer = (blueTurn ? blueTeam : redTeam).CurrentPlayer;
        if (currentplayer >= 0)
            inputIconHUDManager.Index = currentplayer;
        else
            inputIconHUDManager.SetAI();
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

    public int[] GetBlueTeam() => blueTeam.members;
    public int[] GetRedTeam() => redTeam.members;

    public class Team
    {
        public bool Input => aiTeam? ai.input : InputProxy.P(members[currentIndex]);
        public bool ToggledInput => aiTeam ? ai.input : InputProxy.GetToggledInput(members[currentIndex]);
        public int CurrentPlayer => aiTeam ? -1 : members[currentIndex];
        public int NextPlayer => aiTeam ? -1 : members[GetNextIndex()];

        AIScript ai;

        public int[] members;
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
