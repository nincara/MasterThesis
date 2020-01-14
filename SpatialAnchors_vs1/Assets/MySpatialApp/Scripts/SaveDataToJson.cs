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

        public void SaveData (GameObject dataObject, string seconds, string progress, int maxFeaturePoints) {
            AnchorData data = dataObject.GetComponent<AnchorData> ();

            JSONObject dataJson = new JSONObject ();
            dataJson.Add ("Name", data.AnchorName);
            dataJson.Add ("ID", data.AnchorId);
            dataJson.Add ("Key", data.AnchorKey);
            dataJson.Add ("Date", data.AnchorDate);
            dataJson.Add ("Info", data.AnchorInfo);
            dataJson.Add ("GenerateSeconds", data.AnchorGenerateMilliseconds);
            dataJson.Add ("LocalizeSeconds", seconds);
            dataJson.Add ("Progress", data.AnchorProgress);
            dataJson.Add ("LookingProgress", progress);
            dataJson.Add("GenerateFeaturePoints", data.AnchorFeaturePoints);
            dataJson.Add("LocalizeFeaturePoints", maxFeaturePoints);
            
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

            dataJson.Add ("PositionGenerate", position);
            dataJson.Add ("RotationGenerate", rotation);

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

            dataJson.Add ("PositionLocalize", localizePositionArray);
            dataJson.Add ("RotationLocalize", localizeRotationArray);

            string date = System.DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH'-'mm'-'ss");

            string path = Application.persistentDataPath + "/DataSave" + data.AnchorId + "_" + date + ".json";
            File.WriteAllText (path, dataJson.ToString ());
        }
    }
}