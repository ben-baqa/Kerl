using Newtonsoft.Json.Linq;
using System.Collections;
using System;
using UnityEngine;

namespace EmotivUnityPlugin
{
    public class Headset
    {
        private string headsetID;
        private string status;
        private string serialId;
        private string firmwareVersion;
        private string dongleSerial;
        private ArrayList sensors;
        private ArrayList motionSensors;
        private JObject settings;
        private ConnectionType connectedBy;
        private HeadsetTypes headsetType;
        private string mode;

        // Contructor
        public Headset()
        {
        }
        public Headset(JObject jHeadset)
        {
            headsetID = (string)jHeadset["id"];

            if (headsetID.Contains(HeadsetNames.epoc_plus))
            {
                headsetType = HeadsetTypes.HEADSET_TYPE_EPOC_PLUS;
            }
            else if (headsetID.Contains(HeadsetNames.epoc_flex))
            {
                headsetType = HeadsetTypes.HEADSET_TYPE_EPOC_FLEX;
            }
            else if (headsetID.Contains(HeadsetNames.epoc_x))
            {
                headsetType = HeadsetTypes.HEADSET_TYPE_EPOC_X;
            }
            else if (headsetID.Contains(HeadsetNames.insight2))
            {
                headsetType = HeadsetTypes.HEADSET_TYPE_INSIGHT2;
            }
            else if (headsetID.Contains(HeadsetNames.insight))
            {
                headsetType = HeadsetTypes.HEADSET_TYPE_INSIGHT;
            }
            else if (headsetID.Contains(HeadsetNames.mn8))
            {
                headsetType = HeadsetTypes.HEADSET_TYPE_MN8;
            }
            else if (headsetID.Contains(HeadsetNames.epoc))
            {
                headsetType = HeadsetTypes.HEADSET_TYPE_EPOC_STD;
            }

            status = (string)jHeadset["status"];
            firmwareVersion = (string)jHeadset["firmware"];
            dongleSerial = (string)jHeadset["dongle"];
            sensors = new ArrayList();

            foreach (JToken sensor in (JArray)jHeadset["sensors"])
            {
                sensors.Add(sensor.ToString());
            }
            motionSensors = new ArrayList();
            foreach (JToken sensor in (JArray)jHeadset["motionSensors"])
            {
                motionSensors.Add(sensor.ToString());
            }
            mode = (string)jHeadset["mode"];
            string cnnBy = (string)jHeadset["connectedBy"];
            if (cnnBy == "dongle")
            {
                connectedBy = ConnectionType.CONN_TYPE_DONGLE;
            }
            else if (cnnBy == "bluetooth")
            {
                connectedBy = ConnectionType.CONN_TYPE_BTLE;
            }
            else if (cnnBy == "extender")
            {
                connectedBy = ConnectionType.CONN_TYPE_EXTENDER;
            }
            else if (cnnBy == "usb cable")
            {
                connectedBy = ConnectionType.CONN_TYPE_USB_CABLE;
            }
            else
            {
                connectedBy = ConnectionType.CONN_TYPE_UNKNOWN;
            }
            settings = (JObject)jHeadset["settings"];
        }
    }
}
