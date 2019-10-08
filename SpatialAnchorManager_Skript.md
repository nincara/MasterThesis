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
    * AuthenticationMode: authenticationMode = AuthenticationMode.ApiKey
    * String: spatialAnchorsAccountId
    * String: spatialAnchorsAccountKey
    * String: clientId
    * String: tenantId
    * SessionLogLevel: logLevel = SessionLogLevel.All
    
3. Internal Methods
4. Overridables
5. Event Handlers
6. Unity Overrides 
7. Public Methods
8. Public Properties
9. Public Events