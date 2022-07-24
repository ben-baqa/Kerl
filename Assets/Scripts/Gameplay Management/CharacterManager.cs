using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    public GameObject fallbackCharacter;

    public Material blueTeam, redTeam;

    public float throwSpeedMultiplier = 2;

    [Header("Broom Defaults")]
    public Mesh broomHeadDefaultMesh;
    public Material[] broomHeadDefaultMaterials;
    public Mesh broomstickDefaultMesh;
    public Material[] broomstickDefaultMaterials;

    CharacterSet characterSet;

    Sweeper sweeper;

    Character throwerCharacter;
    Character sweeperCharacter;

    void Start()
    {
        Dictionary<int, GameObject> selectedCharacters = MenuSelections.characterSelections;
        if (selectedCharacters == null)
        {
            selectedCharacters = new Dictionary<int, GameObject>();
            selectedCharacters.Add(0, fallbackCharacter);
            selectedCharacters.Add(1, fallbackCharacter);
            selectedCharacters.Add(2, fallbackCharacter);
        }

        List<List<int>> teams = MenuSelections.teams;
        if (teams == null)
            teams = MenuSelections.debugTeams;

        characterSet = new CharacterSet(selectedCharacters, teams);
        characterSet.SetTeamMaterials(blueTeam, redTeam, teams);
        characterSet.SetBrushMeshes(broomHeadDefaultMesh, broomHeadDefaultMaterials,
            broomstickDefaultMesh, broomstickDefaultMaterials);

        sweeper = FindObjectOfType<Sweeper>();

        ApplyIntroPlacements(FindObjectOfType<TeamIntro>().GetPlayerPlacments());
    }

    public void ApplyIntroPlacements(Placement[] characterPlacements)
    {
        TurnManager turnManager = GetComponent<TurnManager>();

        int[] team1 = turnManager.GetBlueTeam();
        int[] team2 = turnManager.GetRedTeam();

        int halfLength = characterPlacements.Length / 2;
        for(int i = 0; i < characterPlacements.Length; i++)
        {
            int playerIndex = -1;
            if(i < halfLength)
            {
                if (team1.Length > 0)
                    playerIndex = team1[i % team1.Length];
            }
            else
            {
                if (team2.Length > 0)
                    playerIndex = team2[(i - halfLength) % team2.Length];
            }

            Character character = characterSet[playerIndex];
            (characterPlacements[i] + character.introPlacement).Apply(character.transform);
            character.OnTeamIntro();
        }
    }

    public void OnTurnStart(int throwerIndex, int sweeperIndex)
    {
        characterSet.HideAll();

        throwerCharacter = characterSet[throwerIndex];
        throwerCharacter.SetUpThrowing();

        sweeperCharacter = characterSet[sweeperIndex];
        sweeperCharacter.SetUpBrushing();
        sweeper.SetCharacter(sweeperCharacter);
    }

    public void OnThrow()
    {
        throwerCharacter.OnThrow(throwSpeedMultiplier / sweeper.BrushingTime);
        sweeperCharacter.OnThrow();
    }

    public Character GetCharacter(int playerIndex) => characterSet[playerIndex];

    class CharacterSet
    {
        List<IndexedCharacter> characters;
        int lastFetched;

        public CharacterSet(Dictionary<int, GameObject> selectedCharacters, List<List<int>> teams)
        {
            characters = new List<IndexedCharacter>();

            foreach(var kvp in selectedCharacters)
            {
                characters.Add(InstantiateCharacter(kvp.Value, kvp.Key));
                if(IsPlayerOnSoloTeam(kvp.Key, teams))
                    characters.Add(InstantiateCharacter(kvp.Value, kvp.Key));
            }
        }

        IndexedCharacter InstantiateCharacter(GameObject target, int index)
        {
            GameObject characterObject = Instantiate(target);
            characterObject.name = $"P{index} character ({target.name})";
            Character instantiatedCharacter = characterObject.GetComponent<Character>();
            instantiatedCharacter.Init();

            return new IndexedCharacter(index, instantiatedCharacter);
        }

        private bool IsPlayerOnSoloTeam(int playerIndex, List<List<int>> teams)
        {
            if (playerIndex < 0)
                return true;

            foreach (List<int> team in teams)
                if (team.Count == 1 && team[0] == playerIndex)
                    return true;
            return false;
        }

        public Character this[int index]
        {
            get
            {
                // search for an indexed character that was not fetched last time
                // this allows for one player index to use multiple characters
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i].playerIndex == index && i != lastFetched)
                    {
                        lastFetched = i;
                        return characters[i].character;
                    }
                }

                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i].playerIndex == index)
                    {
                        lastFetched = i;
                        return characters[i].character;
                    }
                }

                print($"Character not found for index {index}");
                return null;
            }
        }

        public Character[] GetCharacters()
        {
            Character[] characterArray = new Character[characters.Count];
            for (int i = 0; i < characters.Count; i++)
                characterArray[i] = characters[i].character;

            return characterArray;
        }

        public void HideAll()
        {
            foreach (IndexedCharacter ic in characters)
                ic.character.Hide();
        }

        public void SetBrushMeshes(Mesh head, Material[] headMaterials, Mesh stick, Material[] stickMaterials)
        {
            foreach (IndexedCharacter ic in characters)
                ic.character.SetBrushMeshes(head, headMaterials, stick, stickMaterials);
        }

        public void SetTeamMaterials(Material team1Mat, Material team2Mat, List<List<int>> teams)
        {
            foreach (IndexedCharacter ic in characters)
            {
                if (teams[0].Contains(ic.playerIndex))
                    ic.character.SetTeamMaterial(team1Mat);
                if (teams[1].Contains(ic.playerIndex))
                    ic.character.SetTeamMaterial(team2Mat);
            }

            Material aiMat = team1Mat;
            if (teams[1].Count == 0)
                aiMat = team2Mat;

            foreach (IndexedCharacter ic in characters)
                if (ic.playerIndex == -1)
                    ic.character.SetTeamMaterial(aiMat);
        }

        struct IndexedCharacter
        {
            public int playerIndex;
            public Character character;

            public IndexedCharacter(int index, Character c)
            {
                playerIndex = index;
                character = c;
            }
        }
    }

    public class PlayerCharacter
    {
        public int playerIndex;
        public Character character;

        public PlayerCharacter(int index, Character c)
        {
            playerIndex = index;
            character = c;
        }

        public void Hide() => character.Hide();
        public void Init() => character.Init();
    }
}
