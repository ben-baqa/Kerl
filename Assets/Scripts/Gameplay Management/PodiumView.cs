using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumView : MonoBehaviour
{
    public Placement[] winningPlacements;
    public Placement[] losingPlacements;

    private void OnDrawGizmos()
    {
        if (winningPlacements == null || losingPlacements == null)
            return;
        Gizmos.color = Color.green;
        foreach (Placement p in winningPlacements)
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.matrix *= Matrix4x4.Translate(transform.position + p.position);
            Gizmos.matrix *= Matrix4x4.Rotate(Quaternion.Euler(0, p.rotation, 0));
            Gizmos.DrawCube(Vector3.up * 0.5f, new Vector3(0.2f, 1, 0.5f));
        }
        Gizmos.color = Color.red;
        foreach (Placement p in losingPlacements)
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.matrix *= Matrix4x4.Translate(transform.position + p.position);
            Gizmos.matrix *= Matrix4x4.Rotate(Quaternion.Euler(0, p.rotation, 0));
            Gizmos.DrawCube(Vector3.up * 0.5f, new Vector3(0.2f, 1, 0.5f));
        }
    }

    public void Apply(TurnManager turnManager, CharacterManager characterManager)
    {
        int score = FindObjectOfType<Scorekeeper>().score;

        int[] winningTeam = turnManager.GetBlueTeam();
        int[] losingTeam = turnManager.GetRedTeam();

        foreach (Placement p in winningPlacements)
            p.position += transform.position;
        foreach (Placement p in losingPlacements)
            p.position += transform.position;

        // red team won
        if(score < 0)
        {
            winningTeam = turnManager.GetRedTeam();
            losingTeam = turnManager.GetBlueTeam();
        }

        for(int i = 0; i < winningPlacements.Length; i++)
        {
            Character target = characterManager.GetCharacter(winningTeam[i % winningTeam.Length]);
            target.OnWin();
            (winningPlacements[i] + target.podiumPlacement).Apply(target.transform);
        }
        for(int i = 0; i < losingPlacements.Length; i++)
        {
            Character target = characterManager.GetCharacter(losingTeam[i % losingTeam.Length]);
            target.OnLose();
            (losingPlacements[i] + target.podiumPlacement).Apply(target.transform);
        }
    }
}
