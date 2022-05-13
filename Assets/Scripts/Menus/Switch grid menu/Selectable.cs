using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool isParent = false;
    public string childTag;

    public bool selecting = false;
    public bool selected = false;

    public GameObject[] children;

    void Update() {
        if (isParent && !selected)
        {
            if (children.Length == 0)
            {
                children = GameObject.FindGameObjectsWithTag(childTag);
            }
            else
            {
                for (int i = 0; i < children.Length; i++)
                {
                    children[i].GetComponent<Selectable>().selecting = selecting;
                }
            }
        }
    }

    public void OnSelected()
    {
        selected = true;
        if (isParent)
        {
            for (int i = 0; i < children.Length; i++)
            {
                children[i].GetComponent<Selectable>().selecting = false;
            }
            PeriodicSelector ps = GameObject.FindGameObjectWithTag("PeriodicSelector").GetComponent<PeriodicSelector>();
            ps.ChangeTag(childTag);
        }
        else {
            PeriodicSelector ps = GameObject.FindGameObjectWithTag("PeriodicSelector").GetComponent<PeriodicSelector>();
            ps.enabled = false;
        }
    }
}
