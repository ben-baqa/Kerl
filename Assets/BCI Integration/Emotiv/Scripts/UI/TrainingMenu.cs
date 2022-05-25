using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using Newtonsoft.Json.Linq;

public class TrainingMenu : MonoBehaviour
{

    private void start()
    {
        Cortex.training.TrainingCompleted += OnTrainingComplete;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTrainingComplete(JObject result)
    {
        Debug.Log("============== Training completed!");
    }
}
