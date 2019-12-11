using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

namespace Microsoft.Azure.SpatialAnchors.Unity {
    public class SaveDataToJson : MonoBehaviour {

        public string _name;
        public string _id;
        public string _date;
        public string _info;
        public int _seconds;
        public float _progress;
        public string _key;
        public Vector3 positionVector, rotationVector;

        // Start is called before the first frame update
        void Start () {

        }

        // Update is called once per frame
        void Update () {

        }

        public static Vector3 StringToVector3 (string sVector) {
            // Remove the parentheses
            if (sVector.StartsWith ("(") && sVector.EndsWith (")")) {
                sVector = sVector.Substring (1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split (',');

            // store as a Vector3
            Vector3 result = new Vector3 (
                float.Parse (sArray[0]),
                float.Parse (sArray[1]),
                float.Parse (sArray[2]));

            return result;
        }

        public void SaveData (GameObject dataObject, string seconds, float progress) {
            AnchorData data = dataObject.GetComponent<AnchorData> ();

            JSONObject dataJson = new JSONObject ();
            dataJson.Add ("Name", data.AnchorName);
            dataJson.Add ("ID", data.AnchorId);
            dataJson.Add ("Date", data.AnchorDate);
            dataJson.Add ("Info", data.AnchorInfo);
            dataJson.Add ("Seconds", seconds);
            dataJson.Add ("Progress", data.AnchorProgress);
            dataJson.Add ("Key", data.AnchorKey);
            dataJson.Add ("LookingProgress", progress);

            positionVector = StringToVector3 (data.AnchorPosition);
            rotationVector = StringToVector3 (data.AnchorRotation);

            JSONArray position = new JSONArray ();
            position.Add (positionVector.x);
            position.Add (positionVector.y);
            position.Add (positionVector.z);

            JSONArray rotation = new JSONArray ();
            rotation.Add (rotationVector.x);
            rotation.Add (rotationVector.y);
            rotation.Add (rotationVector.z);

            dataJson.Add ("Position", position);
            dataJson.Add ("Rotation", rotation);

            string date = System.DateTime.Now.ToString("'yyyy’-‘MM’-‘dd’_’HH’-’mm’-’ss");

            string path = Application.persistentDataPath + "/DataSave" + data.AnchorId + "_" + date + ".json";
            File.WriteAllText (path, dataJson.ToString ());
        }
    }
}