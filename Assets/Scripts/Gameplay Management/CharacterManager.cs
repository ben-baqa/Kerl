using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public GameObject fallbackCharacter;

    public Material blueTeam, redTeam;


    [Header("Broom Defaults")]
    public Mesh broomHeadDefaultMesh;
    public Material[] broomHeadDefaultMaterials;
    public Mesh broomstickDefaultMesh;
    public Material[] broomstickDefaultMaterials;

    List<Character> characters = new List<Character>();

    //FakeSkipper skipper;
    FakeSweeper sweeper;

    Character skipperCharacter;
    Character sweeperCharacter;

    void Start()
    {
        List<GameObject> selectedCharacters;
        if(MenuSelections.characterSelections != null)
        {
            selectedCharacters = new List<GameObject>(MenuSelections.characterSelections);
        }
        else
        {
            selectedCharacters = new List<GameObject> { fallbackCharacter, 
                fallbackCharacter, fallbackCharacter, fallbackCharacter };
        }

        foreach(GameObject characterPrefab in selectedCharacters)
            characters.Add(Instantiate(characterPrefab).GetComponent<Character>());

        foreach (Character character in characters)
        {
            character.Init();
            character.Hide();
            //assign default brush
            character.SetBrushMeshes(broomHeadDefaultMesh, broomHeadDefaultMaterials,
                broomstickDefaultMesh, broomstickDefaultMaterials);
        }

        //skipper = FindObjectOfType<FakeSkipper>();
        sweeper = FindObjectOfType<FakeSweeper>();
    }

    public void OnTurnStart(int skipperIndex, int sweeperIndex)
    {
        foreach (Character c in characters)
            c.Hide();

        skipperCharacter = characters[skipperIndex];
        skipperCharacter.SetUpThrowing();

        sweeperCharacter = characters[sweeperIndex];
        sweeperCharacter.SetUpBrushing();
        sweeper.SetCharacter(sweeperCharacter);
    }

    public void OnThrow()
    {
        skipperCharacter.OnThrow();
        sweeperCharacter.OnThrow();
    }
}
