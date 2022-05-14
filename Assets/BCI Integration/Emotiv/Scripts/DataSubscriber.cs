using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;

/// <summary>
/// In progress - subscribes to data streams from the data stream manager
/// </summary>
public class DataSubscriber : MonoBehaviour
{
    DataStreamManager dataStream = DataStreamManager.Instance;

    float dataUpdateTimer = 0;
    const float DATA_UPDATE_INTERVAL = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActiveAndEnabled)
            return;

        dataUpdateTimer += Time.deltaTime;
        if (dataUpdateTimer < DATA_UPDATE_INTERVAL)
            return;

        dataUpdateTimer -= DATA_UPDATE_INTERVAL;

        // update data

    }
}
