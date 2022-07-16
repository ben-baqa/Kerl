using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public void ToggleActive(GameObject target) => target.SetActive(!target.activeSelf);
    public void ToggleEnabled(MonoBehaviour target) => target.enabled = !target.enabled;
}
