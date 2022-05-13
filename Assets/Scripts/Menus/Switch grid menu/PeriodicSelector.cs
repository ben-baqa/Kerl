using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicSelector : MonoBehaviour
{
    public float waitTime;

    public string currentTag;

    public GameObject[] currentList;

    private float waitTimer;

    private int currentIndex;

    private GameObject currentObject;

    private void Start()
    {
        waitTimer = 0f;
    }

    void Update()
    {
        if (currentList.Length == 0)
        {
            UpdateTag();
        }
        else {
            if (Input.GetButtonDown("Submit")) {
                currentObject.GetComponent<Selectable>().OnSelected();
            }

            if (waitTimer >= waitTime)
            {
                waitTimer = 0f;
                SwitchObject();
            }
            else
            {
                waitTimer += Time.deltaTime;
            }
        }
    }
    void UpdateTag() {
        currentList = GameObject.FindGameObjectsWithTag(currentTag);
        currentIndex = 0;
        currentObject = currentList[currentIndex];
        currentObject.GetComponent<Selectable>().selecting = true;
    }

    public void ChangeTag(string newTag) {
        for (int i = 0; i < currentList.Length; i++) {
            currentList[i].GetComponent<Selectable>().selecting = false;
        }
        currentTag = newTag;
        UpdateTag();
    }

    void SwitchObject() {
        currentObject.GetComponent<Selectable>().selecting = false;
        currentIndex++;
        if (currentIndex >= currentList.Length) {
            currentIndex = 0;
        }
        currentObject = currentList[currentIndex];
        currentObject.GetComponent<Selectable>().selecting = true;
    }
}
