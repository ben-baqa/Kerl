using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace EmotivUnityPlugin
{
    public class TrainingHandler
    {
        CortexClient ctxClient = CortexClient.Instance;
        Authorizer auth = Authorizer.Instance;

        // event buffers to enable engine synchronous callbacks
        public EventBuffer<DetectionInfo> GetDetectionInfoResult;
        public EventBuffer<List<string>> ProfileQueryResult;
        public EventBuffer<string> GetCurrentProfileResult;
        public EventBuffer<string> ProfileCreated;
        public EventBuffer<string> ProfileLoaded;
        public EventBuffer<bool> ProfileUnloaded;
        public EventBuffer<string> ProfileSaved;

        public EventBuffer<JObject> TrainingRequestResult;

        public EventBuffer<double> TrainingTimeResult;
        public EventBuffer<TrainedActions> GetTrainedActionsResult;
        public EventBuffer<TrainingThreshold> TrainingThresholdResult;

        public string sessionID
        {
            get
            {
                if (string.IsNullOrEmpty(_sid))
                    Debug.LogWarning("Attempted to train BCI without specifying a session");
                return _sid;
            }
            set { _sid = value; }
        }
        string _sid;

        string token { get => auth.CortexToken; }

        // profile management
        public void QueryProfiles() { try { ctxClient.QueryProfile(token); } catch (System.Exception e) { Debug.LogWarning(e); } }
        public void GetCurrentProfile(string headsetID) => ctxClient.GetCurrentProfile(token, headsetID);
        public void CreateProfile(string profileName) => ctxClient.SetupProfile(token, profileName, "create");
        public void DeleteProfile(string profileName) => ctxClient.SetupProfile(token, profileName, "delete");
        public void RenameProfile(string oldName, string newName) => ctxClient.SetupProfile(token, oldName, "rename", newProfileName: newName);
        public void LoadProfile(string profileName, string headsetID) => ctxClient.SetupProfile(token, profileName, "load", headsetID);
        public void UnloadProfile(string profileName, string headsetID) => ctxClient.SetupProfile(token, profileName, "unload", headsetID);
        public void SaveProfile(string profileName, string headsetID) => ctxClient.SetupProfile(token, profileName, "save", headsetID);

        // training management
        public void StartTraining(string action) => Training("start", action);
        public void AcceptTraining(string action) => Training("accept", action);
        public void RejectTraining(string action) => Training("reject", action);
        public void EraseTraining(string action) => Training("erase", action);
        public void CancelTraining(string action) => Training("reset", action);
        void Training(string status, string action) => ctxClient.Training(token, sessionID, status, "mentalCommand", action);

        public void GetMentalCommandInfo() => GetDetectionInfo("mentalCommand");
        public void GetDetectionInfo(string detection) => ctxClient.GetDetectionInfo(detection);
        public void GetTrainingTime() => ctxClient.GetTrainingTime(token, "mentalCommand", sessionID);
        public void GetTrainedActions(string profileName) => ctxClient.GetTrainedSignatureActions(token, "mentalCommand", profileName);
        public void GetTrainingThreshold() => ctxClient.MentalCommandTrainingThreshold(token, sessionId: sessionID);


        /// <summary>
        /// Instaite all available event buffers to allow engine
        /// synchronous callbacks, called by Cortex in Start
        /// </summary>
        /// <param name="host">gameobject to attach event buffers to</param>
        public void InstantiateEventBuffers(EventBufferInstance host)
        {
            GetDetectionInfoResult = new EventBuffer<DetectionInfo>();
            ctxClient.GetDetectionInfoDone += ParseDetectionInfo;
            //host.AddComponent<EventBufferInstance>().buffer = GetDetectionInfoResult;

            ProfileQueryResult = new EventBuffer<List<string>>();
            ctxClient.QueryProfileOK += ParseProfileList;
            //host.AddComponent<EventBufferInstance>().buffer = ProfileQueryResult;

            GetCurrentProfileResult = new EventBuffer<string>();
            ctxClient.GetCurrentProfileDone += OnGetCurrentProfileOK;
            //host.AddComponent<EventBufferInstance>().buffer = GetDetectionInfoResult;

            ProfileCreated = new EventBuffer<string>();
            ctxClient.CreateProfileOK += ProfileCreated.OnParentEvent;
            //host.AddComponent<EventBufferInstance>().buffer = ProfileCreated;

            ProfileLoaded = new EventBuffer<string>();
            ctxClient.LoadProfileOK += ProfileLoaded.OnParentEvent;
            //host.AddComponent<EventBufferInstance>().buffer = ProfileLoaded;

            ProfileUnloaded = new EventBuffer<bool>();
            ctxClient.UnloadProfileDone += ProfileUnloaded.OnParentEvent;
            //host.AddComponent<EventBufferInstance>().buffer = ProfileUnloaded;

            ProfileSaved = new EventBuffer<string>();
            ctxClient.SaveProfileOK += ProfileSaved.OnParentEvent;
            //host.AddComponent<EventBufferInstance>().buffer = ProfileSaved;

            TrainingRequestResult = new EventBuffer<JObject>();
            ctxClient.TrainingOK += OnTrainingOK;
            //host.AddComponent<EventBufferInstance>().buffer = TrainingCompleted;

            TrainingTimeResult = new EventBuffer<double>();
            ctxClient.GetTrainingTimeDone += TrainingTimeResult.OnParentEvent;
            //host.AddComponent<EventBufferInstance>().buffer = TrainingTimeResult;

            GetTrainedActionsResult = new EventBuffer<TrainedActions>();
            ctxClient.GetTrainedSignatureActionsOK += GetTrainedActionsResult.OnParentEvent;

            TrainingThresholdResult = new EventBuffer<TrainingThreshold>();
            ctxClient.MentalCommandTrainingThresholdOK += TrainingThresholdResult.OnParentEvent;

            var buffers = new EventBufferBase[]
            {
                GetDetectionInfoResult,
                ProfileQueryResult,
                GetCurrentProfileResult,
                ProfileCreated,
                ProfileLoaded,
                ProfileUnloaded,
                ProfileSaved,
                TrainingRequestResult,
                TrainingTimeResult,
                GetTrainedActionsResult,
                TrainingThresholdResult
            };
            foreach (EventBufferBase b in buffers) host.AddBuffer(b);
        }

        /// <summary>
        /// Wraps the get detection info event callback with a useful data type
        /// </summary>
        /// <param name="data">data to be parsed (raw from websocket)</param>
        void ParseDetectionInfo(object sender, JObject data)
        {
            try
            {
                DetectionInfo detectioninfo = new DetectionInfo("mentalCommand");

                JArray actions = (JArray)data["actions"];
                foreach (var ele in actions)
                {
                    detectioninfo.Actions.Add(ele.ToString());
                }
                JArray controls = (JArray)data["controls"];
                foreach (var ele in actions)
                {
                    detectioninfo.Controls.Add(ele.ToString());
                }
                JArray events = (JArray)data["events"];
                foreach (var ele in actions)
                {
                    detectioninfo.Events.Add(ele.ToString());
                }
                JArray signature = (JArray)data["signature"];
                foreach (var ele in actions)
                {
                    detectioninfo.Signature.Add(ele.ToString());
                }
                GetDetectionInfoResult.OnParentEvent(sender, detectioninfo);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        /// <summary>
        /// Wraps the get profile list event callback with a readable type
        /// </summary>
        /// <param name="profiles">data to be parsed into a list of profiles</param>
        void ParseProfileList(object sender, JArray profiles)
        {
            try
            {
                List<string> profileLists = new List<string>();
                foreach (JObject ele in profiles)
                {
                    string name = (string)ele["name"];
                    profileLists.Add(name);
                }
                ProfileQueryResult.OnParentEvent(this, profileLists);
            }catch (System.Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        /// <summary>
        /// Wraps the get current profile event callback with a readable type and extra functionality
        /// </summary>
        void OnGetCurrentProfileOK(object sender, JObject data)
        {
            if (data["name"].Type == JTokenType.Null)
            {
                Debug.Log("OnGetCurrentProfileDone: no profile loaded with the headset");
            }
            else
            {
                string profileName = data["name"].ToString();
                bool loadByThisApp = (bool)data["loadedByThisApp"];

                if (!loadByThisApp)
                    Debug.LogWarning($"Profile: {profileName} is loaded, but by another app");

                GetCurrentProfileResult.OnParentEvent(sender, profileName);
            }
        }

        /// <summary>
        /// Wraps the training request response event callback
        /// </summary>
        void OnTrainingOK(object sender, JObject data)
        {
            TrainingRequestResult.OnParentEvent(sender, data);
        }
    }
}
