# SpatialAnchorsManager
Das Skript *"SpatialAnchorManager"* besteht aus 9 Regionen.

0. Zu Beginn wird eine **enum-Liste** erzeugt mit dem Modus der Authentifikation.  
*(Das Schlüsselwort enum wird zum Deklarieren einer Enumeration verwendet. 
Dies ist ein eigener Typ, der aus einer Gruppe benannter Konstanten besteht, 
die Enumeratorliste genannt wird.)*
Diese Liste besteht aus den Modi *"ApiKey"* (Account ID and Account Key) 
und *"AAD"* (Azure Active Directory)

1. Member Variables
    * Variablen für die Session
      * **bool**: *isSessionStarted* = false
      * **CloudSpatialAnchorSession**: *session* = null
      * **SessionStatus**: *sessionStatus* = null
     

2. Unity Inspector Variables
  * Variablen, welche im Unity-Inspector eingegeben und eingestellt werden können, dazu zählen:
    * **AuthenticationMode**: authenticationMode = AuthenticationMode.ApiKey
    * **string**: spatialAnchorsAccountId
    * **string**: spatialAnchorsAccountKey
    * **string**: clientId
    * **string**: tenantId
    * **SessionLogLevel**: logLevel = SessionLogLevel.All
    
3. Internal Methods
  * Interne Methoden 
    * **void**: EnsureSessionStarted()
    * **async Task<bool>**: EnsureValidConfiguration(**bool** disable, **bool** exception)
    * **void**: ProcessLatestFrame():
    * **internal static ARReferencePoint**: ReferencePointFromPointer(**IntPtr** intPtr)
    * **void**: ProcessPendingEventArgs()

4. Overridables
  * Diese Methoden sind alle virtual und können überschrieben werden. 
    * **virtual async Task<string>**: GetAADTokenAsync()  
    _Diese Methode ist auskommentiert, da sie nur bei der Verwendung von AAD Anwendung findet._
    * **virtual void**: LoadConfiguration  
    _Diese Methode lädt die ID und den Key aus der Config Datei im Resources Ordner. Dabei setzt die Methode die Werte aus der Config in die Inspector Varialen aus 2._  
    _Aufgerufen: Line 618!_
    * **async virtual Task<bool>**: IsValidateConfiguration()
    _Dieses Skript überprüft, ob der Manager richtig konfiguriert wurde und bereit zum laufen ist. Wenn ja, gibt die Methode true zurück, andernfalls false._
    * **virtual void**: OnAnchorLocated(**object** sender, **AnchorsLocatedEventArgs** args)
    * **virtual void**: OnError(**object** sender, **SessionErrorEventArgs** args)
    * **virtual void**: OnLocateAnchorsCompleted(**object** sender, **LocateAnchorsCompletedEventArgs** args)
    * **virtual void**: OnLogDebug(**object** sender, **OnLogDebugEventArgs** args)
    * **virtual void**: OnSessionChanged()
    * **virtual void** OnSessionCreated()
    * **virtual void** OnSessionDestroyed()
    * **virtual void**: OnSessionStarted()
    * **virtual void**: OnSessionStopped()
    * **virtual void**: OnSessionUpdated(**object** sender, **SessionUpdatedEventArgs** args)

5. Event Handlers
  * 
    * **async void**: Session_TokenRequired(**object** sender, **TokenRequiredEventArgs** args)
    * **void**: ARReferencePointManager_referendePointsChanged (**ARReferencePointsChangedEventArgs** obj)
    * **void**: ArCameraManager_frameReceived(**ARCameraFrameEventArgs** obj)

6. Unity Overrides
  * Unity Methoden, welche vom Nutzer überschrieben werden können.
    * **virtual void**: Awake()  
    _Wird beim Laden des Skripts aufgerufen. Hier wird die Funktion LoadConfiguration() aufgerufen._
    * **virtual void**: OnDestroy()  
    _Ruft die Methode DestroySession() auf. Wird ausgeführt wenn die Szene oder die Anwendung beendet ist._
    * **async virtual void**: Start()
      * mainCamera = Camera.main
      * arCameraManager = FindObjectOfType<ARCameraManager>()
      * arSession = FindObjectOfType<ARSession>()
      * arReferencePointManager = FindObjectOfType<ARReferencePointManager>()
    _Die start-Methode wird vor der ersten Update-Methode ausgeführt. Sie überprüft mit **EnsureValidConfiguration()** ob der Manager richtig configuriert wurde. Dies wird mit einer await-Methode durchgeführt._
    * **virtual void**: Update()
    _In der Update wird die Methode **ProcessPendingEventArgs()** jeden Frame ausgeführt._
    
7. Public Methods
  * Wichtige Methoden zur Erstellung von Session und Anker!
    * **async Task**: CreateSessionAsync()
    * **void**: DestroySession()
    * **async Task**: ResetSessionAsync()

    * **asynch Task**: CreateAnchorAsync(**CloudSpatialAnchor** anchor, **CancellationToken** canellationToken)
    * **async Task**: CreateAnchorAsync(**CloudSpatialAnchor** anchor)
    * **async Task**: DeleteAnchorAsync(**CloudSpatialAnchor** anchor)
    * **async Task**: StartSessionAsync()
    * **async Task**: StopSession()

8. Public Properties
  * Get und Set Methoden für Properties
    * **AuthenticationMode** AuthenticationMode {get {} set{}}
    * **string** ClientId {get {} set{}}
    * **bool** IsLocating {get {}}
    * **bool** IsReadyForCreate {get {}}
    * **bool** IsSessionStarted {get{}}
    * **SessionLogLevel** LogLevel {get {} set{}}
    * **CloudSpatialAnchorSession** Session {get {} set{}}
    * **SessionStatus** SessionStatus {get {}}
    * **string** SpatialAnchorsAccountId {get {} set{}}
    * **string** SpatialAnchorsAccountKey {get {} set{}}
    * **string** TenantId {get {} set{}}

9. Public Events
  * Events?
    * **event AnchorsLocatedDelegate** AnchorsLocated
    * **event LocateAnchorsCompletedDelegate** LocateAnchorsCompleted
    * **event SessionErrorDelegate** Error
    * **event EventHandler** SessionChanged
    * **event EventHandler** SessionCreated
    * **event EventHandler** SessionDestroyed
    * **event EventHandler** SessionStopped
    * **event SessionUpdatedDelegate** SessionUpdated
    * **event OnLogDebugDelegate** LogDebug