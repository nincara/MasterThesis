using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.Azure.SpatialAnchors.Unity
{


    public class AnchorData : MonoBehaviour
    {
        private string anchorId, anchorName, anchorDescription;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public string AnchorId { get => anchorId; set => anchorId = value; }
        public string AnchorName { get => anchorName; set => anchorName = value; }
        public string AnchorDescription { get => anchorDescription; set => anchorDescription = value; }
    }
}