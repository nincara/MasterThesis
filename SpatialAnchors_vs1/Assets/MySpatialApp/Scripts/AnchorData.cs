using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.Azure.SpatialAnchors.Unity
{


    public class AnchorData : MonoBehaviour
    {
        private string anchorKey, anchorName, anchorId, anchorDate, anchorDescription, anchorInfo, anchorProgress;
        // Start is called before the first frame update

        public string AnchorKey { get => anchorKey; set => anchorKey = value; }
        public string AnchorName { get => anchorName; set => anchorName = value; }
        public string AnchorId { get => anchorId; set => anchorId = value; }
        public string AnchorDate { get => anchorDate; set => anchorDate = value; }
        public string AnchorInfo { get => anchorInfo; set => anchorInfo = value; }
        public string AnchorProgress{ get => anchorProgress; set => anchorProgress = value; }
        public string AnchorDescription { get => anchorDescription; set => anchorDescription = value; }
    }
}