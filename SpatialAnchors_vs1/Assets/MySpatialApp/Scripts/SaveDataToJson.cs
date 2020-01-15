using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

namespace Microsoft.Azure.SpatialAnchors.Unity {
    public class SaveDataToJson : MonoBehaviour {

        public Vector3 positionVector, rotationVector, positionVectorLocalize, rotationVectorLocalize;

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

        public void SaveData (GameObject dataObject, string seconds, string progress, int maxFeaturePoints, string testPhase) {
            AnchorData data = dataObject.GetComponent<AnchorData> ();

            JSONObject dataJson = new JSONObject ();
            dataJson.Add ("Name", data.AnchorName);
            dataJson.Add ("ID", data.AnchorId);
            dataJson.Add ("Key", data.AnchorKey);
            dataJson.Add ("Date", data.AnchorDate);
            dataJson.Add ("Info", data.AnchorInfo);
            dataJson.Add ("GenerateSeconds", data.AnchorGenerateMilliseconds);  
            dataJson.Add ("GenerateProgress", data.AnchorProgress);
            dataJson.Add ("GenerateFeaturePoints", data.AnchorFeaturePoints);
            
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

            dataJson.Add ("GeneratePosition", position);
            dataJson.Add ("GenerateRotation", rotation);

            dataJson.Add("TestPhase", testPhase);

            dataJson.Add ("LocalizeSeconds", seconds);
            dataJson.Add ("LocalizeProgress", progress);
            dataJson.Add ("LocalizeFeaturePoints", maxFeaturePoints);

            positionVectorLocalize = StringToVector3(data.AnchorPositionLocalization);
            rotationVectorLocalize = StringToVector3(data.AnchorRotationLocalization);

            JSONArray localizePositionArray = new JSONArray ();
            localizePositionArray.Add (positionVectorLocalize.x);
            localizePositionArray.Add (positionVectorLocalize.y);
            localizePositionArray.Add (positionVectorLocalize.z);

            JSONArray localizeRotationArray = new JSONArray ();
            localizeRotationArray.Add (rotationVectorLocalize.x);
            localizeRotationArray.Add (rotationVectorLocalize.y);
            localizeRotationArray.Add (rotationVectorLocalize.z);

            dataJson.Add ("LocalizePosition", localizePositionArray);
            dataJson.Add ("LocalizeRotation", localizeRotationArray);

            string date = System.DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH'-'mm'-'ss");

            string path = Application.persistentDataPath + "/DataSave" + data.AnchorId + "_" + date + ".json";
            File.WriteAllText (path, dataJson.ToString ());
        }

        public void SaveDataGenerate (CloudSpatialAnchor dataObject) {
            //AnchorData data = dataObject.GetComponent<AnchorData> ();

            JSONObject dataJson = new JSONObject ();
            dataJson.Add ("Name", dataObject.AppProperties[@"name"]);
            dataJson.Add ("ID", dataObject.AppProperties[@"id"]);
            dataJson.Add ("Key", dataObject.Identifier);
            dataJson.Add ("Date", dataObject.AppProperties[@"date"]);
            dataJson.Add ("Info", dataObject.AppProperties[@"info"]);
            dataJson.Add ("GenerateSeconds", dataObject.AppProperties[@"generateMilliseconds"]);
            dataJson.Add ("GenerateProgress", dataObject.AppProperties[@"progress"]);
            dataJson.Add ("GenerateFeaturePoints", dataObject.AppProperties[@"featurePoints"]);
            
            positionVector = StringToVector3 (dataObject.AppProperties[@"position"]);
            rotationVector = StringToVector3 (dataObject.AppProperties[@"rotation"]);

            JSONArray position = new JSONArray ();
            position.Add (positionVector.x);
            position.Add (positionVector.y);
            position.Add (positionVector.z);

            JSONArray rotation = new JSONArray ();
            rotation.Add (rotationVector.x);
            rotation.Add (rotationVector.y);
            rotation.Add (rotationVector.z);

            dataJson.Add ("GeneratePosition", position);
            dataJson.Add ("GenerateRotation", rotation);

            string date = System.DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH'-'mm'-'ss");

            string path = Application.persistentDataPath + "/DataSaveGenerate" + dataObject.AppProperties[@"id"] + "_" + date + ".json";
            File.WriteAllText (path, dataJson.ToString ());
        }
    }
}