#Einrichten für Azure Spatial Anchors:

##Erstellen eines Accounts bei Azure
	1. Erstellen einer Ressourcengruppe: SpatialResourceGroupCB
	2. Erstellen Spatial Anchors Ressource: SpatialResourceCB - Spatial Anchors Account
		Standort: USA Ost 2
		Account-ID: 9925e31a-01af-478d-9be2-f5610d6ab3b1
		Key: 2NBDrkDMB6CL/0Vg4iPuPSi3R1zozO6nSww5O/XY0VM=
		Sharing URL: https://spatialanchorswebappcb.azurewebsites.net/index.html
	3. Erstellen eines App-Service (Web App): SpatialAnchorsWebAppCB
		Referenz zur Ressourcengruppe und zum App Service-Plan

##Voraussetzung:
	Unity Version 2019.1 oder höher
	Git für Windows
	Android Studio mit Android ADK und NDK, CMaker, ...
	.NET Core 2.2 SDK
	Windows Computer mit ASP.NET- und Webentwicklungs-Workload, mit Visual Studio 2017 oder höher

##Herunterladen des Beispiels von Azure
	Im Zielordner mit "Git BASH here" 
	--> "git clone https://github.com/Azure/azure-spatial-anchors-samples.git"

##Veröffentlichen der SharingService Solution
	Öffnen des SharingServiceProjekt in Visual Studio
	Wichtig: Es muss die SharingService.sln geöffnet werden in Visual Studio
	In der Projektmappe "SharingService" befindet sich das Objekt "SharingService"
	Dies mit Doppelclick öffnen und > Erstellen > "SharingService" veröffentlichen clicken
	Danach bei 'App Service' auf "Vorhandenes Element auswählen" und die zuvor erstelle WebApp auswählen
	Danach sollte sich im Browser die Webapp öffnen mit eine index.html 

..* In Unity muss dann das Projekt geöffnet werden: Version Höher als Unity 2019.1.10f !!!

..* Einfügen der Account ID und Key in AzureSpatialAnchors.SDK > Resources > SpatialAnchorConfig

..* Einfügen der Sharing URL in AzureSpatialAnchors.Examples > Resources > SpatialAnchorsSamplesConfig
	Achtung: Bei der URL muss die Endung /index.html durch /api/anchors ersetzt werden
..* In den Build Settings muss die Plattform zu Android gewechselt werden
	Evtl müssen alles Assets neu Importiert werden: Assets > Re-import all 

..* Mit Samsung S9+ funktioniert alles, mit Samsung Tablet A nicht! --> Gerät muss AR Core Fähig sein!

#Demo

1. Basic Demo
	- Create Azure Spatial Anchors Session
	- Config Azure Spatial Anchors Session 
	- Start Azure Spatial Anchors Session 
	- Create Local Anchors and save to cloud
	- Moving Device to capture enviroment data
	- Stop Azure Spatial Anchors Session 
	- Create Azure Spatial Anchors Session for query
	- Start Azure Spatial Anchors Session for query
	- Looking for Anchors
	- Delete Anchors
	- Stop Azure Spatial Anchors Session for query
	- Restart Demo

2. Local Shared Demo: Create & Share / Locate Anchors
	- Create Cloud Spatial Anchors Session
	- Config Cloud Spatial Anchors Session
	- Start Cloud Spatial Anchors Session
	- Create Local Anchors and save to cloud
	- Moving Device to capture enviroment data
	- Stop Cloud Spatial Anchors Session

	- Input Anchors Number: X
	- Create Cloud Spatial Anchors Session
	- Create Cloud Spatial Anchors Session for query
	- Start Cloud Spatial Anchors Session for query
	- Looking for anchors
	- Stop Create Cloud Spatial Anchors Session for query

Version Control in Unity: 
Mit den #define Operator können if-Statements für bestimmte Versionen oder OS bestimmt werden
https://docs.unity3d.com/Manual/PlatformDependentCompilation.html

*#if UNITY_ANDROID
	debug.Log("Das wird bei einem Android-Gerät ausgegeben");
*#endif

*#if UNITY_IOS
	debug.Log("Das wird bei einem iOS-Gerät ausgegeben");
*#endif

Das ist auch für Unity-Versionen möglich:
*#if UNITY_5_0_1
	debug.Log("Das wird nur ausgegeben bei Unity Version 5.0.1");
*#endif