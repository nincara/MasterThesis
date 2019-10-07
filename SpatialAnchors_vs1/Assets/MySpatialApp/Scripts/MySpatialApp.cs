using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.Azure.SpatialAnchors {

    public class MySpatialApp : MonoBehaviour {
        CloudSpatialAnchorSession cloudSession;
        // Start is called before the first frame update
        void Start () {
            this.cloudSession = new CloudSpatialAnchorSession ();
            Debug.Log (cloudSession);
            this.cloudSession.Configuration.AccountKey = @"MyAccountKey";
            this.cloudSession.Configuration.AccessToken = @"MyAccessToken";
        }

        // Update is called once per frame
        void Update () {

        }
    }
}