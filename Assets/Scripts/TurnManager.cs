using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Keeps track of which player has active input, and the flow of game state
/// </summary>
public class TurnManager : MonoBehaviour
{
    // Identifies this instance as the authoritative Network Host
    public static bool isHost = true;

    private ScoreHUD score;
    private AIScript ai;
    Team blueTeam, redTeam;

    private bool blueTurn = true, skipper = true;

    // Start is called before the first frame update
    void Start()
    {
        score = FindObjectOfType<ScoreHUD>();
        ai = GetComponent<AIScript>();

        blueTeam = new Team(MenuSelections.teams[0], ai);
        redTeam = new Team(MenuSelections.teams[1], ai);
        print(blueTeam);
        print(redTeam);
    }

    public void OnTurn()
    {
        blueTurn = !blueTurn;

        if (blueTurn)
            blueTeam.Next(2);
        else
            redTeam.Next(2);

        score.UpdateTurn((blueTurn ? blueTeam : redTeam).currentPlayer);
        ai.StartTimer();
    }

    public void OnThrow()
    {
        if (blueTurn)
            blueTeam.Next();
        else
            redTeam.Next();
        score.UpdateTurn((blueTurn ? blueTeam : redTeam).currentPlayer);
    }

    public bool GetInput()
    {
        if (!isHost)
            return false;

        if (blueTurn)
            return blueTeam.input;
        else
            return redTeam.input;
    }

    public class Team
    {
        public bool input => aiTeam? ai.brushing : InputProxy.P(members[currentIndex]);
        public int currentPlayer => aiTeam ? -1 : members[currentIndex];

        AIScript ai;

        int[] members;
        int count;
        int currentIndex;
        bool aiTeam;

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

        public override string ToString()
        {
            string res = "Team: [";
            for (int i = 0; i < count; i++)
                res += members[i] + ",";
            res += $"], active player: {currentPlayer}, aiTeam? {aiTeam}";
            return res;
        }
    }
}
