# Demos: Basic Demos
Die Demos bestehen aus zwei Basis-Scripten, welche im Hintergrund arbeiten. Als Basis fungiert das InputInteractionBase.cs Script, welches die Eingabe handhabt. Dieses Skript hat eine abstrakte Klasse _InputInteractionBase_, welche von MonoBehavior erbt. Die Klasse befindet sich im **namespace** _Microsoft.Azure.SpatialAnchors.Unity.Examples_. Abstrakt bedeutet, dass in der Klasse Funktionen definiert sind, welche nicht vollständig sind, diese müssen von einer erbenden Klasse ergänzt werden.
Das Script DemoScriptBase.cs ist ebenfalls abstract und erbt die Klasse _InputInteractionBase_. 
Diese beiden Skripte sind die Grundlage für die 3 von Azure bereitgestellten Demos. Alle 3 Demos erben die DemoScriptBase Klasse. 

## InputInteractionBase
* public **virtual** void OnDestroy()
* public **virtual** void Start()
* public **virtual** void Update()

* private void TriggerInteractions()

* protected **virtual** void OnGazeInteraction()
  * Wird aufgerufen, wenn eine Gaze-Interaktion auftritt
* protected **virtual** void OnGazeObjectInteraction(Vector3 hitPoint, Vector3 hitNormal)
  * Wird aufgerufen, wenn eine Gaze-Interaktion beginnt. Dabei werden zwei Vector3 Objekte übergeben. 
* protected **virtual** void OnTouchInteraction(Touch touch)
  * Wird aufgerufen, wenn eine Touch-Interaktion auftritt.
* protected **virtual** void OnTouchInteractionEnded(Touch touch)
  * Wird aufgerufen, wenn eine Touch-Interaktion endet.
* protected **virtual** void OnSelectInteraction()
  * Wird aufgerufen, wenn eine Auswahl-Interkation auftritt. Ausschließlich für HoloLens.
* protected **virtual** void OnSelectObjectInteraction(Vector3 hitPoint, object target)
  * Wird aufgerufen, wenn eine Touch-Interaktion mit einem Objekt auftritt. 

* private bool TryGazeHitTest(out RaycastHit target)
* private void InteractionManager_InteractionSourcePressed(UnityEngine.XR.WSA.Input.InteractionSourcePressedEventArgs obj)

## DemoScriptBase
* public **_override_** void OnDestroy()
* public **virtual** bool SanityCheckAccessConfiguration()
* public **_override_** void Start()

* public abstract Task AdvanceDemoAsync();
* public async void AdvanceDemo()
* public async void ReturnToLauncher()

* protected **virtual** void CleanupSpawnedObjects()

* protected CloudSpatialAnchorWatcher CreateWatcher()
* protected void SetAnchorIdsToLocate(IEnumerable<string> anchorIds)
* protected void ResetAnchorIdsToLocate()
* protected void SetNearbyAnchor(CloudSpatialAnchor nearbyAnchor, float DistanceInMeters, int MaxNearAnchorsToFind)
* protected void SetGraphEnabled(bool UseGraph, bool JustGraph = false)
* public void SetBypassCache(bool BypassCache)
* protected abstract Color GetStepColor();
* protected abstract bool IsPlacingObject();

* protected **virtual** void MoveAnchoredObject(GameObject objectToMove, Vector3 worldPos, Quaternion worldRot, CloudSpatialAnchor cloudSpatialAnchor = null)
* protected **virtual** void OnCloudAnchorLocated(AnchorLocatedEventArgs args)
* protected **virtual** void OnCloudLocateAnchorsCompleted(LocateAnchorsCompletedEventArgs args)
* protected **virtual** void OnCloudSessionUpdated()
* protected **_override_** void OnGazeInteraction()
* protected **_override_** void OnGazeObjectInteraction(Vector3 hitPoint, Vector3 hitNormal)
* protected **virtual** void OnSaveCloudAnchorFailed(Exception exception)
* protected **virtual** Task OnSaveCloudAnchorSuccessfulAsync()
* protected **_override_** void OnSelectInteraction()
* protected **_override_** void OnSelectObjectInteraction(Vector3 hitPoint, object target)
* protected **_override_** void OnTouchInteraction(Touch touch)
* protected **virtual** async Task SaveCurrentObjectAnchorToCloudAsync()
* protected **virtual** GameObject SpawnNewAnchoredObject(Vector3 worldPos, Quaternion worldRot)
* protected **virtual** GameObject SpawnNewAnchoredObject(Vector3 worldPos, Quaternion worldRot, CloudSpatialAnchor cloudSpatialAnchor)
* protected **virtual** void SpawnOrMoveCurrentAnchoredObject(Vector3 worldPos, Quaternion worldRot)

* private void CloudManager_AnchorLocated(object sender, AnchorLocatedEventArgs args)
* private void CloudManager_LocateAnchorsCompleted(object sender, LocateAnchorsCompletedEventArgs args)
* private void CloudManager_SessionUpdated(object sender, SessionUpdatedEventArgs args)
* private void CloudManager_Error(object sender, SessionErrorEventArgs args)
* private void CloudManager_LogDebug(object sender, OnLogDebugEventArgs args)
* protected struct DemoStepParams

* public GameObject AnchoredObjectPrefab { get { return anchoredObjectPrefab; } }
* public SpatialAnchorManager CloudManager { get { return cloudManager; } }