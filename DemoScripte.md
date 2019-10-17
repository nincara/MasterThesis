# Demos: Basic Demos
Die Demos bestehen aus zwei Basis-Scripten, welche im Hintergrund arbeiten. Als Basis fungiert das InputInteractionBase.cs Script, welches die Eingabe handhabt. Dieses Skript hat eine abstrakte Klasse _InputInteractionBase_, welche von MonoBehavior erbt. Die Klasse befindet sich im **namespace** _Microsoft.Azure.SpatialAnchors.Unity.Examples_. Abstrakt bedeutet, dass in der Klasse Funktionen definiert sind, welche nicht vollständig sind, diese müssen von einer erbenden Klasse ergänzt werden.
Das Script DemoScriptBase.cs ist ebenfalls abstract und erbt die Klasse _InputInteractionBase_. 
Diese beiden Skripte sind die Grundlage für die 3 von Azure bereitgestellten Demos. Alle 3 Demos erben die DemoScriptBase Klasse. 

## InputInteractionBase
* public **virtual** void OnDestroy()
  * _Wird in DemoScriptBase.cs überschrieben._
* public **virtual** void Start()
  * _Wird in DemoScriptBase.cs überschrieben._
* public **virtual** void Update()

* private void TriggerInteractions()

* protected **virtual** void OnGazeInteraction()
  * Wird aufgerufen, wenn eine Gaze-Interaktion auftritt. 
  * _Wird in DemoScriptBase.cs überschrieben._
* protected **virtual** void OnGazeObjectInteraction(Vector3 hitPoint, Vector3 hitNormal)
  * Wird aufgerufen, wenn eine Gaze-Interaktion beginnt. Dabei werden zwei Vector3 Objekte übergeben.
  * _Wird in DemoScriptBase.cs überschrieben._ 
* protected **virtual** void OnTouchInteraction(Touch touch)
  * Wird aufgerufen, wenn eine Touch-Interaktion auftritt.
  * _Wird in DemoScriptBase.cs überschrieben._
* protected **virtual** void OnTouchInteractionEnded(Touch touch)
  * Wird aufgerufen, wenn eine Touch-Interaktion endet.
* protected **virtual** void OnSelectInteraction()
  * Wird aufgerufen, wenn eine Auswahl-Interkation auftritt. Ausschließlich für HoloLens.
  * _Wird in DemoScriptBase.cs überschrieben._
* protected **virtual** void OnSelectObjectInteraction(Vector3 hitPoint, object target)
  * Wird aufgerufen, wenn eine Touch-Interaktion mit einem Objekt auftritt. 
  * _Wird in DemoScriptBase.cs überschrieben._

* private bool TryGazeHitTest(out RaycastHit target)
* private void InteractionManager_InteractionSourcePressed(UnityEngine.XR.WSA.Input.InteractionSourcePressedEventArgs obj)

## DemoScriptBase
* public **_override_** void OnDestroy()
* public **virtual** bool SanityCheckAccessConfiguration()
  * Überprüft, ob eine ID und ein Key in der Confoguration eingegeben wurden. 
* public **_override_** void Start()

* public abstract Task AdvanceDemoAsync();
  * Diese Methode muss in der Demo gefüllt werden. Hier werden die Operationen der Methode aufgeführt. Im Falle der Azure Demo sind die Operationen durch verschiedene AppStates und einem einzelnen Button definiert. Nach jedem Button-Klick wird der App-State gewechselt. In der Methode befindet sich ein switch-case, welche den App-State abfängt und dementsprechend in einen Case springt, welcher dann die gewünschte Operation ausführt. 
* public async void AdvanceDemo()
  * Die Methode exsitiert nur, falls die Methode AdvanceDemoAsync() über einen Button aufgerufen wird. Es werden mögliche Fehler mit einem try-catch abgefangen. Die Methode ist async. 

* public async void ReturnToLauncher()
  * Da es drei Demos gibt, ist diese Methode dazu da, zurück zur Demo-Auswahl zu kommen. 

* protected **virtual** void CleanupSpawnedObjects()
  * Zerstört alle gespawnte Objekte der Szene (Prefabs). 

* protected CloudSpatialAnchorWatcher CreateWatcher()
  * Erstellt einen CloudSpatialAnchorWatcher, wenn ein CloudManager und eine Session vorhanden sind. Der Watcher beobachtet und hält ausschau nach Spatial Anchors. 
* protected void SetAnchorIdsToLocate(IEnumerable<string> anchorIds)
  * Diese Methode setzt eine Liste, welche IDs von Spatial Anchors enthält. Diese IDs sollen lokalisiert werden. 
* protected void ResetAnchorIdsToLocate()
  * Diese Methode leert die Liste mit zu lokalisierenden IDs. Das Lokalisierungskriterium des Identifizierers der Klasse AnchorLocateCriteria wird auf 0 gesetzt, was bedeutet, dass nur nach dem Anchor mit der ID 0 gesucht wird. (???)
* protected void SetNearbyAnchor(CloudSpatialAnchor nearbyAnchor, float DistanceInMeters, int MaxNearAnchorsToFind)
  * Setzt als AnchorLocateCriteria einen Anchor in der Nähe. Dabei werden der Methode der spezifische Anchor, die Distanz in welcher gesucht wird und die maximale Anzahl an Anchors, welche gefunden werden sollen übergeben. 
* protected void SetGraphEnabled(bool UseGraph, bool JustGraph = false)
  * ???
  * Setzt als AnchorLocateCriteria eine bestimmte Strategie. 
* public void SetBypassCache(bool BypassCache)
  * AnchorLocateCriteria, soll bei der Lokalisierung der Lokale Cache des Ankers umganzen werden?

* protected abstract Color GetStepColor();
* protected abstract bool IsPlacingObject();

* protected **virtual** void MoveAnchoredObject(GameObject objectToMove, Vector3 worldPos, Quaternion worldRot, CloudSpatialAnchor cloudSpatialAnchor = null)
  * 
* protected **virtual** void OnCloudAnchorLocated(AnchorLocatedEventArgs args)
  * Wird aufgerufen, wenn ein CloudAnchor lokalisiert wurde. 
  * _Muss überschrieben werden!_
* protected **virtual** void OnCloudLocateAnchorsCompleted(LocateAnchorsCompletedEventArgs args)
  * Wird aufgerufen, wenn die Lokalisierung vollständig ist. 
  * Event
* protected **virtual** void OnCloudSessionUpdated()
  * Wird aufgerufen, wenn die CloudSession aktualisiert wurde.
  * _Muss überschrieben werden!_
* protected **_override_** void OnGazeInteraction()
  * Führt die Methode der Basisklasse aus, sofern die Methode IsPlacingObject() true zurück gibt. Dies bedeutet, dass eine Interaktion mit der Hololens nur möglich ist, wenn gerade die Platzierung eines Objekts erlaubt wird. Die Methode IsPlacingObject() muss in der erbenden Klasse definiert werden. 
* protected **_override_** void OnGazeObjectInteraction(Vector3 hitPoint, Vector3 hitNormal)
  * Die Methode führt die Basis-Methode der Vererbten Klasse aus mit einem zusatz für WINDOWS_UWP oder UNITY_WSA
* protected **virtual** void OnSaveCloudAnchorFailed(Exception exception)
  * Wird aufgerufen, wenn ein Clound Anchor nicht richtig gespeichert wurde. 
* protected **virtual** Task OnSaveCloudAnchorSuccessfulAsync()
  * Wird aufgerufen, wenn ein Cloud Anchors erfolgreicht gespeichert wurde.

* protected **_override_** void OnSelectInteraction()
  * Die Basismethode wird ausgeführt und um eine Funktion für WINDOWS_UWP || UNITY_WSA erweitert.  
* protected **_override_** void OnSelectObjectInteraction(Vector3 hitPoint, object target)
  * Wird ausgeführt, sobald eine Touch-Interaktion stattfindet! 
  * Gibt die Methode IsPlacingObject() true zurück, wird ein neues Anchor gespawned, oder ein bestehender verschobnen (Anpassung)
* protected **_override_** void OnTouchInteraction(Touch touch)
  * Auch hier wird die Basis-Funktion der vererbten Klasse ausgeführt, sofern die Methode IsPlacingObject() true zurück gibt. 

* protected **virtual** async Task SaveCurrentObjectAnchorToCloudAsync()
  * Diese Methode speichert das aktuellen Anchor in die Cloud. Hier wird auch der Process zum speichern der Umgebungsdaten gestartet. 
* protected **virtual** GameObject SpawnNewAnchoredObject(Vector3 worldPos, Quaternion worldRot)
  * Die Methode spawned ein neues verankertes Objekt und gibt dieses zurück. Dabei werden die World Position und die World Rotiation übergeben. 
* protected **virtual** GameObject SpawnNewAnchoredObject(Vector3 worldPos, Quaternion worldRot, CloudSpatialAnchor cloudSpatialAnchor)
  * Die Methode spawned ein neues Objekt an einem vorhandenen Anker und gibt dieses zurück.  Dabei werden die World Position, die World Rotiation und der Cloud Anchor übergeben. 
* protected **virtual** void SpawnOrMoveCurrentAnchoredObject(Vector3 worldPos, Quaternion worldRot)
  * Spawned ein neues verankertes Objekt mit SpawnNewAnchordObject() und setzt es als das aktuelle Objekt. Exsitiert bereits ein verankertes Objekt, so wird es nur bewegt mit MoveAnchoredObject().  

* private void CloudManager_AnchorLocated(object sender, AnchorLocatedEventArgs args)
* private void CloudManager_LocateAnchorsCompleted(object sender, LocateAnchorsCompletedEventArgs args)
* private void CloudManager_SessionUpdated(object sender, SessionUpdatedEventArgs args)
* private void CloudManager_Error(object sender, SessionErrorEventArgs args)
* private void CloudManager_LogDebug(object sender, OnLogDebugEventArgs args)
* protected struct DemoStepParams

* public GameObject AnchoredObjectPrefab { get { return anchoredObjectPrefab; } }
* public SpatialAnchorManager CloudManager { get { return cloudManager; } }

## CloudNativeAnchors.cs
Das Skript ermöglicht, einen nativen Anchor in einen CloudAnchor zu wandeln und umgekehrt.