using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public GameObject fallbackCharacter;

    List<GameObject> selectedCharacters;

    // Start is called before the first frame update
    void Start()
    {
        selectedCharacters = MenuSelections.characterSelections;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTurnStart(int skipper, int sweeper)
    {

    }
}
