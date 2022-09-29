using Facepunch;
using Network;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using ProtoBuf;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Time = UnityEngine.Time;

namespace Oxide.Plugins
{
	[Info("PirateNPC", "bmgjet", "0.0.8")]
	[Description("Creates NPC To Patrol Ocean,Skies and Land")]
	class PirateNPC : RustPlugin
	{
		#region Variables

		[PluginReference]
		private Plugin Kits;
		private int piratePlane = 0;
		private int pirateRHIB = 0;
		private int pirateRowBoat = 0;
		private int pirateSub = 0;
		private int piratesMini = 0;
		private int pirateScrap = 0;
		private int pirateRail = 0;
		private int pirateHorse = 0;
		private int pirateSnowMobile = 0;
		private int pirateBase = 0;
		private List<uint> PirateUintList = new List<uint>();
		private List<uint> BaseParts = new List<uint>();
		private List<uint> GroundedNPCs = new List<uint>();
		private Dictionary<ScientistNPC, BaseEntity> ActiveNPCs = new Dictionary<ScientistNPC, BaseEntity>();
		private List<uint> NPCsUintList = new List<uint>();
		private List<BaseEntity> TrainsBaseEntityList = new List<BaseEntity>();
		private List<uint> VoiceUintList = new List<uint>();
		private List<uint> CoolDownUintList = new List<uint>();
		private List<Vector3> PointsOfIntrest = new List<Vector3>();
		private List<BasePlayer> downed = new List<BasePlayer>();
		private List<MapMarkerGenericRadius> mapmarkersList = new List<MapMarkerGenericRadius>();
		private Timer spawner = null;
		private List<Coroutine> Threads = new List<Coroutine>();
		private List<Vector3> seanodes = new List<Vector3>();
		private List<Vector3> airnodes = new List<Vector3>();
		private List<Vector3> groundnodes = new List<Vector3>();
		private List<Vector3> roadnodes = new List<Vector3>();
		private List<Vector3> railnodes = new List<Vector3>();
		private Dictionary<Vector3, bool> dropnodes = new Dictionary<Vector3, bool>();
		private List<PrefabData> footnodes = new List<PrefabData>();
		private List<PrefabData> Alarmtriggers = new List<PrefabData>();
		private List<uint> alarmcooldown = new List<uint>();
		private bool sealoaded = false;
		private bool airloaded = false;
		private bool landloaded = false;
		private bool railloaded = false;
		private bool footloaded = false;
		private readonly List<BasePlayer> _openProfiles = new List<BasePlayer>();
		public static PirateNPC plugin;
		public static ServerSettings _ServerSettings;
		private List<List<Dictionary<string, string>>> loottableDict = new List<List<Dictionary<string, string>>>();
		private const string wallprefab = "assets/prefabs/building core/wall/wall.prefab";
		private const string foundationprefab = "assets/prefabs/building core/foundation/foundation.prefab";
		private const string floorprefab = "assets/prefabs/building core/floor/floor.prefab";
		private const string windowprefab = "assets/prefabs/building/wall.window.reinforcedglass/wall.window.glass.reinforced.prefab";
		private const string wallframeprefab = "assets/prefabs/building core/wall.frame/wall.frame.prefab";
		private const string garagedoorprefab = "assets/prefabs/building/wall.frame.garagedoor/wall.frame.garagedoor.prefab";
		private const string workcartprefab = "assets/content/vehicles/workcart/workcart_aboveground.entity.prefab";
		private const string wagonentsprefab = "assets/content/vehicles/train/trainwagon$.entity.prefab";
		private const string codelockprefab = "assets/prefabs/locks/keypad/lock.code.prefab";
		private const string autoturretprefab = "assets/prefabs/npc/autoturret/autoturret_deployed.prefab";
		private const string samsiteprefab = "assets/prefabs/npc/sam_site_turret/sam_site_turret_deployed.prefab";
		private const string rhibentityprefab = "assets/content/vehicles/boats/rhib/rhib.prefab";
		private const string rowboatentityprefab = "assets/content/vehicles/boats/rowboat/rowboat.prefab";
		private const string minientityprefab = "assets/content/vehicles/minicopter/minicopter.entity.prefab";
		private const string scrapentityprefab = "assets/content/vehicles/scrap heli carrier/scraptransporthelicopter.prefab";
		private const string horseentityprefab = "assets/rust.ai/nextai/testridablehorse.prefab";
		private const string snowmobileentityprefab = "assets/content/vehicles/snowmobiles/snowmobile.prefab";
		private const string subentityprefab = "assets/content/vehicles/submarine/submarineduo.entity.prefab";
		private const string hoboentityprefab = "assets/prefabs/misc/twitch/hobobarrel/hobobarrel.deployed.prefab";
		private const string sphereentityprefab = "assets/prefabs/visualization/sphere.prefab";
		private const string buoyantentityprefab = "assets/prefabs/misc/item drop/item_drop_buoyant.prefab";
		private const string eliteentityprefab = "assets/bundled/prefabs/radtown/crate_elite.prefab";
		private const string basicentityprefab = "assets/bundled/prefabs/radtown/crate_basic.prefab";
		private const string normalentityprefab = "assets/bundled/prefabs/radtown/crate_normal.prefab";
		private const string helicrateentityprefab = "assets/prefabs/npc/patrol helicopter/heli_crate.prefab";
		private const string cargoplaneentityprefab = "assets/prefabs/npc/cargo plane/cargo_plane.prefab";
		private const string lanternentityprefab = "assets/prefabs/deployable/lantern/lantern.deployed.prefab";
		private const string woodboxentityprefab = "assets/prefabs/deployable/large wood storage/box.wooden.large.prefab";
		private const string rhibstorageentityprefab = "assets/content/vehicles/boats/rhib/subents/rhib_storage.prefab";
		private const string playerentityprefab = "assets/prefabs/player/player.prefab";
		private const string mapmarkerentityprefab = "assets/prefabs/tools/map/genericradiusmarker.prefab";
		private const string parachuteentityprefab = "assets/prefabs/misc/parachute/parachute.prefab";
		private const string groundfallentityprefab = "assets/bundled/prefabs/fx/player/groundfall.prefab";
		private const string c4_explosionentityprefab = "assets/prefabs/tools/c4/effects/c4_explosion.prefab";
		private const string damage_effect_debrisprefab = "assets/prefabs/npc/patrol helicopter/damage_effect_debris.prefab";
		private const string searchlightentityprefab = "assets/prefabs/deployable/search light/searchlight.deployed.prefab";
		private const string oilfireballsmallprefab = "assets/bundled/prefabs/oilfireballsmall.prefab";

		private int TopologyBaseBlock = TerrainTopology.MONUMENT;
		#endregion

		#region Configuration

		public class ServerSettings
		{
			public int SpawnEvery;
			public int LeaveAfter;
			public bool ShowMapMarkers;
			public float MapMarkerSize;
			public float MapMarkerAlpha;
			public float StartFireHealth;
			public Color AirMapMarkerColour;
			public Color WaterMapMarkerColour;
			public Color LandMapMarkerColour;
			public Color BaseMapMarkerColour;
			public Color RailMapMarkerColour;
			public float StopDelayFloat;
			public float CrashDelayFloat;
			public bool DisableScrapHeliGibs;
			public bool DisableHeliFireball;
			public bool DisableHelilights;
			public bool DisableBoatlights;
			public bool DisableLandlights;
			public float FireDelay;
			public float ExplodeDelay;
			public int RHIBNPCAmount;
			public int RHIBHealth;
			public int RHIBNPCHealth;
			public int RHIBBoatAmount;
			public int RHIBBoatMaxA;
			public float RHIBBoatMaxSpeed;
			public float RHIBBoatTurnSpeed;
			public float RHIBAIDistance;
			public string RHIBKit;
			public string RHIBLootString;
			public int RowBoatNPCAmount;
			public int RowBoatHealth;
			public int RowBoatNPCHealth;
			public int RowBoatBoatAmount;
			public int RowBoatBoatMaxA;
			public float RowBoatBoatMaxSpeed;
			public float RowBoatBoatTurnSpeed;
			public float RowBoatAIDistance;
			public string RowBoatKit;
			public string RowBoatLootString;
			public int SubmarineHealth;
			public int SubmarineAmount;
			public int SubmarineMaxA;
			public float SubmarineMaxSpeed;
			public float SubmarineTurnSpeed;
			public float SubmarineTargetDepth;
			public float SubmarineDamage;
			public string SubmarineLootString;
			public int MiniHealth;
			public int MiniNPCHealth;
			public int MiniNPCAmount;
			public int MiniAmount;
			public int MiniMaxA;
			public float MiniMaxSpeed;
			public float MiniOrbitDistance;
			public float MiniTurnSpeed;
			public float MiniHoverHeight;
			public float MiniAIDistance;
			public float MiniThreatLevel;
			public string MiniKit;
			public string MiniLootString;
			public int ScrapHealth;
			public int ScrapNPCHealth;
			public int ScrapNPCAmount;
			public int ScrapAmount;
			public int ScrapMaxA;
			public float ScrapMaxSpeed;
			public float ScrapOrbitDistance;
			public float ScrapTurnSpeed;
			public float ScrapHoverHeight;
			public float ScrapAIDistance;
			public float ScrapThreatLevel;
			public string ScrapKit;
			public string ScrapLootString;
			public bool CargoPathBool;
			public bool MonumentPathBool;
			public bool SamSitesBool0;
			public bool SamSitesBoat;
			public bool StaticSamSitesBool;
			public float SamSitesDamageFloat;
			public bool NPCParachuteBool;
			public float NPCTick;
			public float NPCParachuteTrigger;
			public float NPCParachuteDecay;
			public float NPCParachuteDie;
			public float HeliTargetDistance;
			public float BoatTargetDistance;
			public float LandTargetDistance;
			public int SnowMobileAmount;
			public int SnowMobileMax;
			public float SnowMobileNPCHealth;
			public float SnowMobileAimMulti;
			public float SnowMobileHealth;
			public float SnowMobileRomeRange;
			public bool SnowMobileCanRoam;
			public float SnowMobileMoveSpeed;
			public string SnowMobileKit;
			public string SnowMobileLootString;
			public int HorseAmount;
			public int HorseMax;
			public float HorseNPCHealth;
			public float HorseAimMulti;
			public float HorseHealth;
			public float HorseRomeRange;
			public bool HorseCanRoam;
			public float HorseMoveSpeed;
			public string HorseKit;
			public string HorseLootString;
			public string LootProfiles;
			public bool NPCChatter;
			public int NPCMaxVoicesAtOnce;
			public string NPCCustomChatter0;
			public int NPCCustomChatterCD;
			public string NPCCustomDeath0;
			public int NPCCustomDeathCD;
			public string NPCCustomKill0;
			public int NPCCustomKillCD;
			public string NPCCustomWarningString;
			public float NPCCustomWarningDistance;
			public int NPCCustomWarningCD;
			public bool NPCCAttackSupplyDrops;
			public bool NPCAttackCreate;
			public bool NPCCountRaids;
			public bool NPCsCanTargetNPCsBool;
			public float NPCDamageScaler;
			public float NPCAimScaler;
			public string NPCNameTag;
			public float PiratesAirPOIRadius;
			public float PiratesWaterPOIRadius;
			public float PiratesLandPOIRadius;
			public bool NPCPeaceKeeperBool;
			public int POIExpires;
			public float NPCPeaceKeeperToggle;
			public bool RaidableBasesAT;
			public int HorseLootMulti;
			public int SnowMobileLootMulti;
			public int ScrapLootMulti;
			public int MiniLootMulti;
			public int SubmarineLootMulti;
			public int RowBoatLootMulti;
			public int RHIBLootMulti;
			public int PirateBaseAmount;
			public int PirateBaseWallRadius;
			public int PirateBaseNPCAmount;
			public float PirateBaseAimMulti;
			public float PirateBaseNPCHealth;
			public string PirateBaseNPCKit;
			public string PirateBaseLootString;
			public int PirateBaseLootMulti;
			public float PirateBaseDoorHealth;
			public int PirateRailAmount;
			public float PirateRailTrainSpeed;
			public int PirateRailHealth;
			public int PirateRailNPCAmount;
			public float PirateRailAimMulti;
			public float PirateRailNPCHealth;
			public float PirateRailNPCDriverHealth;
			public string PirateRailNPCKit;
			public string PirateRailLootString;
			public int PirateRailLootMulti;
			public int PirateRailWagonAmount;
			public float PirateRailWagonSamRange;
			public float PirateRailWagonSamHealth;
			public float PirateRailWagonTurretHealth;
			public string PirateRailWagonTurretGun;
			public float PirateRailWagonhealth;
			public float PirateRailWagondoorhealth;
			public ulong PirateRailwagondoorskinid;
			public float PirateFootNPCHealth;
			public float PirateFootNPCAimMulti;
			public string PirateFootNPCKit;
			public float PirateFootNPCRoam;
			public bool PirateCargoPlane;
			public float PiratePlaneflyheight;
			public float PiratePlaneflyspeed;
			public int PiratePlaneNPCAmount;
			public int PiratePlaneNPCDrops;
			public bool PirateEventEnabled;
			public bool PirateEventAnnounce;
			public float PirateEventRadius;
			public int PirateEventNPCAmount;
			public string PirateEventNPCKit;
			public float PirateEventNPCHealth;
			public float PirateEventNPCAimMulti;
			public float PirateEventCoolDownDelay;
			public float PirateEventSpawnRadius;
			public float PirateEventStationaryRadius;
			public List<string> PiratePlaneAllowedDrops;

			public void LoadSettings()
			{
				RaidableBasesAT = bool.Parse(plugin.Config["(RaidableBases) Disable Auto Turrets Attacking Pirates"].ToString());
				SpawnEvery = int.Parse(plugin.Config["Spawn Pirates Every (sec)"].ToString());
				LeaveAfter = int.Parse(plugin.Config["Leave After (sec)"].ToString());
				ShowMapMarkers = bool.Parse(plugin.Config["Map Marker Show"].ToString());
				MapMarkerSize = float.Parse(plugin.Config["Map Marker Size"].ToString());
				MapMarkerAlpha = float.Parse(plugin.Config["Map Marker Alpha"].ToString());
				StartFireHealth = float.Parse(plugin.Config["Start Fire When Health Below"].ToString());
				AirMapMarkerColour = plugin.Hex2Colour(plugin.Config["Map Marker Colour (Air) (Hex)"].ToString());
				WaterMapMarkerColour = plugin.Hex2Colour(plugin.Config["Map Marker Colour (Water) (Hex)"].ToString());
				LandMapMarkerColour = plugin.Hex2Colour(plugin.Config["Map Marker Colour (Land) (Hex)"].ToString());
				BaseMapMarkerColour = plugin.Hex2Colour(plugin.Config["Map Marker Colour (Base) (Hex)"].ToString());
				RailMapMarkerColour = plugin.Hex2Colour(plugin.Config["Map Marker Colour (Rail) (Hex)"].ToString());
				StopDelayFloat = float.Parse(plugin.Config["Stop Delay (sec)"].ToString());
				CrashDelayFloat = float.Parse(plugin.Config["Crash Delay (sec)"].ToString());
				DisableScrapHeliGibs = bool.Parse(plugin.Config["Disable ScrapHeli Gibs"].ToString());
				DisableHeliFireball = bool.Parse(plugin.Config["Disable Heli Fireball"].ToString());
				DisableHelilights = bool.Parse(plugin.Config["Disable Heli Lights"].ToString());
				DisableBoatlights = bool.Parse(plugin.Config["Disable Boat Lights"].ToString());
				DisableLandlights = bool.Parse(plugin.Config["Disable Land Lights"].ToString());
				FireDelay = float.Parse(plugin.Config["Fire After Crash/Sink (sec)"].ToString());
				ExplodeDelay = float.Parse(plugin.Config["Explode After Fire (sec)"].ToString());
				RHIBNPCAmount = int.Parse(plugin.Config["RHIB NPC Amount Per Boat"].ToString());
				RHIBHealth = int.Parse(plugin.Config["RHIB Health"].ToString());
				RHIBNPCHealth = int.Parse(plugin.Config["RHIB NPC Health"].ToString());
				RHIBBoatAmount = int.Parse(plugin.Config["RHIB Boat Amount Per Spawn"].ToString());
				RHIBBoatMaxA = int.Parse(plugin.Config["RHIB Boat Max At Once"].ToString());
				RHIBBoatMaxSpeed = float.Parse(plugin.Config["RHIB Boat Max Speed"].ToString());
				RHIBBoatTurnSpeed = float.Parse(plugin.Config["RHIB Boat Turn Speed"].ToString());
				RHIBAIDistance = float.Parse(plugin.Config["RHIB AI Target Distance"].ToString());
				RHIBKit = plugin.Config["RHIB Kit Bots"].ToString();
				RHIBLootString = plugin.Config["RHIB Loot Spawner (Leave Blank For Profile Loot)"].ToString();
				RHIBLootMulti = int.Parse(plugin.Config["RHIB Loot Spawner Multiplier"].ToString());
				RowBoatNPCAmount = int.Parse(plugin.Config["RowBoat NPC Amount Per Boat"].ToString());
				RowBoatHealth = int.Parse(plugin.Config["RowBoat Health"].ToString());
				RowBoatNPCHealth = int.Parse(plugin.Config["RowBoat NPC Health"].ToString());
				RowBoatBoatAmount = int.Parse(plugin.Config["RowBoat Boat Amount Per Spawn"].ToString());
				RowBoatBoatMaxA = int.Parse(plugin.Config["RowBoat Boat Max At Once"].ToString());
				RowBoatBoatMaxSpeed = float.Parse(plugin.Config["RowBoat Boat Max Speed"].ToString());
				RowBoatBoatTurnSpeed = float.Parse(plugin.Config["RowBoat Boat Turn Speed"].ToString());
				RowBoatAIDistance = float.Parse(plugin.Config["RowBoat AI Target Distance"].ToString());
				RowBoatKit = plugin.Config["RowBoat Kit Bots"].ToString();
				RowBoatLootString = plugin.Config["RowBoat Loot Spawner (Leave Blank For Profile Loot)"].ToString();
				RowBoatLootMulti = int.Parse(plugin.Config["RowBoat Loot Spawner Multiplier"].ToString());
				SubmarineHealth = int.Parse(plugin.Config["Submarine Health"].ToString());
				SubmarineAmount = int.Parse(plugin.Config["Submarine Amount Per Spawn"].ToString());
				SubmarineMaxA = int.Parse(plugin.Config["Submarine Max At Once"].ToString());
				SubmarineMaxSpeed = float.Parse(plugin.Config["Submarine Max Speed"].ToString());
				SubmarineTurnSpeed = float.Parse(plugin.Config["Submarine Turn Speed"].ToString());
				SubmarineTargetDepth = float.Parse(plugin.Config["Submarine Target Depth"].ToString());
				SubmarineDamage = float.Parse(plugin.Config["Submarine Take Damage"].ToString());
				SubmarineLootString = plugin.Config["Submarine Loot Spawner (Leave Blank For Profile Loot)"].ToString();
				SubmarineLootMulti = int.Parse(plugin.Config["Submarine Loot Spawner Multiplier"].ToString());
				MiniHealth = int.Parse(plugin.Config["Minicopter Health"].ToString());
				MiniNPCHealth = int.Parse(plugin.Config["Minicopter NPC Health"].ToString());
				MiniNPCAmount = int.Parse(plugin.Config["Minicopter NPC Amount"].ToString());
				MiniAmount = int.Parse(plugin.Config["Minicopter Amount Per Spawn"].ToString());
				MiniMaxA = int.Parse(plugin.Config["Minicopter Max At Once"].ToString());
				MiniMaxSpeed = float.Parse(plugin.Config["Minicopter Max Speed"].ToString());
				MiniOrbitDistance = float.Parse(plugin.Config["Minicopter Orbit Distance"].ToString());
				MiniTurnSpeed = float.Parse(plugin.Config["Minicopter Turn Speed"].ToString());
				MiniHoverHeight = float.Parse(plugin.Config["Minicopter Hover Height"].ToString());
				MiniAIDistance = float.Parse(plugin.Config["Minicopter AI Target Distance"].ToString());
				MiniThreatLevel = float.Parse(plugin.Config["Minicopter Target Threat Level"].ToString());
				MiniKit = plugin.Config["Minicopter Kit Bots"].ToString();
				MiniLootString = plugin.Config["Minicopter Loot Spawner (Leave Blank For Profile Loot)"].ToString();
				MiniLootMulti = int.Parse(plugin.Config["Minicopter Loot Spawner Multiplier"].ToString());
				ScrapHealth = int.Parse(plugin.Config["Scrapcopter Health"].ToString());
				ScrapNPCHealth = int.Parse(plugin.Config["Scrapcopter NPC Health"].ToString());
				ScrapNPCAmount = int.Parse(plugin.Config["Scrapcopter NPC Amount"].ToString());
				ScrapAmount = int.Parse(plugin.Config["Scrapcopter Amount Per Spawn"].ToString());
				ScrapMaxA = int.Parse(plugin.Config["Scrapcopter Max At Once"].ToString());
				ScrapMaxSpeed = float.Parse(plugin.Config["Scrapcopter Max Speed"].ToString());
				ScrapOrbitDistance = float.Parse(plugin.Config["Scrapcopter Orbit Distance"].ToString());
				ScrapTurnSpeed = float.Parse(plugin.Config["Scrapcopter Turn Speed"].ToString());
				ScrapHoverHeight = float.Parse(plugin.Config["Scrapcopter Hover Height"].ToString());
				ScrapAIDistance = float.Parse(plugin.Config["Scrapcopter AI Target Distance"].ToString());
				ScrapThreatLevel = float.Parse(plugin.Config["Scrapcopter Target Threat Level"].ToString());
				ScrapKit = plugin.Config["Scrapcopter Kit Bots"].ToString();
				ScrapLootString = plugin.Config["Scrapcopter Loot Spawner (Leave Blank For Profile Loot)"].ToString();
				ScrapLootMulti = int.Parse(plugin.Config["Scrapcopter Loot Spawner Multiplier"].ToString());
				CargoPathBool = bool.Parse(plugin.Config["Use Cargoship Path For Ocean"].ToString());
				MonumentPathBool = bool.Parse(plugin.Config["Use Monument Path For Air"].ToString());
				SamSitesBool0 = bool.Parse(plugin.Config["Samsites Attack Pirates"].ToString());
				SamSitesBoat = bool.Parse(plugin.Config["Samsites Attack Pirate Boats"].ToString());
				StaticSamSitesBool = bool.Parse(plugin.Config["Static(Server) Samsites Attack Pirates"].ToString());
				SamSitesDamageFloat = float.Parse(plugin.Config["Samsites Damage Multiplyer (Pirates)"].ToString());
				NPCParachuteBool = bool.Parse(plugin.Config["NPCs Parachute"].ToString());
				NPCTick = float.Parse(plugin.Config["NPCs Tick Rate (sec)"].ToString());
				NPCParachuteTrigger = float.Parse(plugin.Config["NPCs Parachute When Heli Health Below"].ToString());
				NPCParachuteDecay = float.Parse(plugin.Config["NPCs Parachute Decay (sec)"].ToString());
				NPCParachuteDie = float.Parse(plugin.Config["NPCs Parachute Suicide (sec)"].ToString());
				HeliTargetDistance = float.Parse(plugin.Config["(Air) Pirates Target Distance"].ToString());
				BoatTargetDistance = float.Parse(plugin.Config["(Sea) Pirates Target Distance"].ToString());
				LandTargetDistance = float.Parse(plugin.Config["(Land) Pirates Target Distance"].ToString());
				SnowMobileAmount = int.Parse(plugin.Config["SnowMobile Amount Per Spawn"].ToString());
				SnowMobileMax = int.Parse(plugin.Config["SnowMobile Max At Once"].ToString());
				SnowMobileNPCHealth = float.Parse(plugin.Config["SnowMobile NPC Health"].ToString());
				SnowMobileAimMulti = float.Parse(plugin.Config["SnowMobile NPC Aim Distance"].ToString());
				SnowMobileHealth = float.Parse(plugin.Config["SnowMobile Health"].ToString());
				SnowMobileRomeRange = float.Parse(plugin.Config["SnowMobile Roam Range"].ToString());
				SnowMobileCanRoam = bool.Parse(plugin.Config["SnowMobile Roam"].ToString());
				SnowMobileMoveSpeed = float.Parse(plugin.Config["SnowMobile Speed"].ToString());
				SnowMobileKit = plugin.Config["SnowMobile Kit Bots"].ToString();
				SnowMobileLootString = plugin.Config["SnowMobile Loot Spawner (Leave Blank For Profile Loot)"].ToString();
				SnowMobileLootMulti = int.Parse(plugin.Config["SnowMobile Loot Spawner Multiplier"].ToString());
				HorseAmount = int.Parse(plugin.Config["Horse Amount Per Spawn"].ToString());
				HorseMax = int.Parse(plugin.Config["Horse Max At Once"].ToString());
				HorseNPCHealth = float.Parse(plugin.Config["Horse NPC Health"].ToString());
				HorseAimMulti = float.Parse(plugin.Config["Horse NPC Aim Distance"].ToString());
				HorseHealth = float.Parse(plugin.Config["Horse Health"].ToString());
				HorseRomeRange = float.Parse(plugin.Config["Horse Roam Range"].ToString());
				HorseCanRoam = bool.Parse(plugin.Config["Horse Roam"].ToString());
				HorseMoveSpeed = float.Parse(plugin.Config["Horse Speed"].ToString());
				HorseKit = plugin.Config["Horse Kit Bots"].ToString();
				HorseLootString = plugin.Config["Horse Loot Spawner (Leave Blank For Profile Loot)"].ToString();
				HorseLootMulti = int.Parse(plugin.Config["Horse Loot Spawner Multiplier"].ToString());
				LootProfiles = plugin.Config["XLootCustomProfiles"].ToString();
				NPCChatter = bool.Parse(plugin.Config["NPCs Radio Disable"].ToString());
				NPCCustomChatter0 = plugin.Config["NPCs Radio Custom"].ToString();
				NPCCustomChatterCD = int.Parse(plugin.Config["NPCs Radio Custom Cooldown"].ToString());
				NPCCustomDeath0 = plugin.Config["NPCs Custom Death Voice"].ToString();
				NPCCustomDeathCD = int.Parse(plugin.Config["NPCs Custom Death VoiceCooldown"].ToString());
				NPCCustomKill0 = plugin.Config["NPCs Custom Kill Voice"].ToString();
				NPCCustomKillCD = int.Parse(plugin.Config["NPCs Custom Kill Voice Cooldown"].ToString());
				NPCNameTag = plugin.Config["NPCs Name Tag"].ToString();
				NPCCustomWarningString = plugin.Config["NPCs Custom Warning Voice"].ToString();
				NPCCustomWarningDistance = float.Parse(plugin.Config["NPCs Custom Warning Voice Trigger Distance"].ToString());
				NPCCustomWarningCD = int.Parse(plugin.Config["NPCs Custom Warning Voice Cooldown"].ToString());
				NPCCAttackSupplyDrops = bool.Parse(plugin.Config["NPCs Pirates Attack Supply Drops"].ToString());
				NPCAttackCreate = bool.Parse(plugin.Config["NPCs Pirates Attack Creates Being Hacked"].ToString());
				NPCsCanTargetNPCsBool = bool.Parse(plugin.Config["NPCs Can Target Other NPCs"].ToString());
				NPCCountRaids = bool.Parse(plugin.Config["NPCs Pirates Counter Raids"].ToString());
				NPCMaxVoicesAtOnce = int.Parse(plugin.Config["NPCs Max Voices To Play At Once On Server"].ToString());
				NPCDamageScaler = float.Parse(plugin.Config["NPC Damage Scaler"].ToString());
				NPCAimScaler = float.Parse(plugin.Config["NPC Aim Scaler"].ToString());
				PiratesAirPOIRadius = float.Parse(plugin.Config["Pirates Distance From POI Air"].ToString());
				PiratesWaterPOIRadius = float.Parse(plugin.Config["Pirates Distance From POI Water"].ToString());
				PiratesLandPOIRadius = float.Parse(plugin.Config["Pirates Distance From POI Land"].ToString());
				NPCPeaceKeeperBool = bool.Parse(plugin.Config["NPCs Pirates PeaceKeeper Mode"].ToString());
				POIExpires = int.Parse(plugin.Config["Pirates POI Expires After (sec)"].ToString());
				NPCPeaceKeeperToggle = float.Parse(plugin.Config["NPCs Pirates PeaceKeeper Toggle Hostile Distance"].ToString());
				PirateBaseAmount = int.Parse(plugin.Config["Pirate Base Amount To Spawn"].ToString());
				PirateBaseNPCAmount = int.Parse(plugin.Config["Pirate Base Amount Of NPCs"].ToString());
				PirateBaseWallRadius = int.Parse(plugin.Config["Pirate Base Wall Radius"].ToString());
				PirateBaseNPCHealth = float.Parse(plugin.Config["Pirate Base NPC Health"].ToString());
				PirateBaseAimMulti = float.Parse(plugin.Config["Pirate Base NPC Aim Distance"].ToString());
				PirateBaseNPCKit = plugin.Config["Pirate Base NPC Kit"].ToString();
				PirateBaseLootString = plugin.Config["Pirate Base Loot Spawner (Leave Blank For Profile Loot)"].ToString();
				PirateBaseDoorHealth = float.Parse(plugin.Config["Pirate Base Door Health"].ToString());
				PirateBaseLootMulti = int.Parse(plugin.Config["Pirate Base Loot Spawner Multiplyer"].ToString());
				PirateRailAmount = int.Parse(plugin.Config["Pirate Rail Amount To Spawn"].ToString());
				PirateRailHealth = int.Parse(plugin.Config["Pirate Rail Train Health"].ToString());
				PirateRailNPCAmount = int.Parse(plugin.Config["Pirate Rail Amount Of NPCs"].ToString());
				PirateRailNPCHealth = float.Parse(plugin.Config["Pirate Rail NPC Health"].ToString());
				PirateRailNPCDriverHealth = float.Parse(plugin.Config["Pirate Rail NPC Driver Health"].ToString());
				PirateRailAimMulti = float.Parse(plugin.Config["Pirate Rail NPC Aim Distance"].ToString());
				PirateRailNPCKit = plugin.Config["Pirate Rail NPC Kit"].ToString();
				PirateRailLootString = plugin.Config["Pirate Rail Loot Spawner (Leave Blank For Profile Loot)"].ToString();
				PirateRailLootMulti = int.Parse(plugin.Config["Pirate Rail Loot Spawner Multiplyer"].ToString());
				PirateRailWagonSamRange = float.Parse(plugin.Config["Pirate Rail Samsite Range"].ToString());
				PirateRailWagonSamHealth = float.Parse(plugin.Config["Pirate Rail Samsite Health"].ToString());
				PirateRailWagonTurretHealth = float.Parse(plugin.Config["Pirate Rail Turret Health"].ToString());
				PirateRailWagonTurretGun = plugin.Config["Pirate Rail Turret Gun"].ToString();
				PirateRailWagonhealth = float.Parse(plugin.Config["Pirate Rail Wagon Health"].ToString());
				PirateRailWagondoorhealth = float.Parse(plugin.Config["Pirate Rail Wagon Door Health"].ToString());
				PirateRailwagondoorskinid = ulong.Parse(plugin.Config["Pirate Rail Wagon Door SkinID"].ToString());
				PirateRailWagonAmount = int.Parse(plugin.Config["Pirate Rail Wagon Spawn Ammount"].ToString());
				PirateRailTrainSpeed = float.Parse(plugin.Config["Pirate Rail Train Speed"].ToString());
				PirateFootNPCHealth = float.Parse(plugin.Config["Pirate Foot NPC Health"].ToString());
				PirateFootNPCAimMulti = float.Parse(plugin.Config["Pirate Foot Aim Distance"].ToString());
				PirateFootNPCKit = plugin.Config["Pirate Foot NPC Kit"].ToString();
				PirateFootNPCRoam = float.Parse(plugin.Config["Pirate Foot NPC Roam Distance"].ToString());
				PirateCargoPlane = bool.Parse(plugin.Config["Pirate Plane Event Enabled"].ToString());
				PiratePlaneflyheight = float.Parse(plugin.Config["Pirate Plane Fly Height"].ToString());
				PiratePlaneflyspeed = float.Parse(plugin.Config["Pirate Plane Fly Speed"].ToString());
				PiratePlaneNPCAmount = int.Parse(plugin.Config["Pirate Plane NPCs Per Drop"].ToString());
				PiratePlaneNPCDrops = int.Parse(plugin.Config["Pirate Plane Number Of Drops"].ToString());
				PiratePlaneAllowedDrops = plugin.AllowedMonumentList(plugin.Config["Pirate Plane Allowed Monuments"].ToString());
				PirateEventEnabled = bool.Parse(plugin.Config["Pirates Events Enabled"].ToString());
				PirateEventCoolDownDelay = float.Parse(plugin.Config["Pirate Events CoolDown Delay"].ToString());
				PirateEventRadius = float.Parse(plugin.Config["Pirate Events Player Radius"].ToString());
				PirateEventNPCAmount = int.Parse(plugin.Config["Pirate Events NPC Amount"].ToString());
				PirateEventNPCKit = plugin.Config["Pirate Events NPC Kit"].ToString();
				PirateEventNPCHealth = float.Parse(plugin.Config["Pirate Events NPC Health"].ToString());
				PirateEventNPCAimMulti = float.Parse(plugin.Config["Pirate Events NPC AimMulti"].ToString());
				PirateEventAnnounce = bool.Parse(plugin.Config["Pirate Events Annoucements"].ToString());
				PirateEventSpawnRadius = float.Parse(plugin.Config["Pirate Events NPC Spawn Radius"].ToString());
				PirateEventStationaryRadius = float.Parse(plugin.Config["Pirate Events NPC Stationary Radius"].ToString());
			}
		}

		protected override void LoadDefaultConfig()
		{
			Puts("Creating a new configuration file");
			Config["Spawn Pirates Every (sec)"] = 900;
			Config["Leave After (sec)"] = 1300;
			Config["(Air) Pirates Target Distance"] = 200f;
			Config["(Sea) Pirates Target Distance"] = 30f;
			Config["(Land) Pirates Target Distance"] = 40f;
			Config["(RaidableBases) Disable Auto Turrets Attacking Pirates"] = true;
			Config["Map Marker Show"] = true;
			Config["Map Marker Colour (Air) (Hex)"] = "#ada500";
			Config["Map Marker Colour (Water) (Hex)"] = "#001dad";
			Config["Map Marker Colour (Land) (Hex)"] = "#00ad00";
			Config["Map Marker Colour (Base) (Hex)"] = "#ad0000";
			Config["Map Marker Colour (Rail) (Hex)"] = "#ad00a0";
			Config["Map Marker Alpha"] = 0.2f;
			Config["Map Marker Size"] = 0.3f;
			Config["Start Fire When Health Below"] = 80f;
			Config["Stop Delay (sec)"] = 3;
			Config["Crash Delay (sec)"] = 0.5;
			Config["Disable ScrapHeli Gibs"] = true;
			Config["Disable Heli Fireball"] = true;
			Config["Disable Heli Lights"] = false;
			Config["Disable Boat Lights"] = false;
			Config["Disable Land Lights"] = false;
			Config["Fire After Crash/Sink (sec)"] = 60;
			Config["Explode After Fire (sec)"] = 10;
			Config["RHIB Kit Bots"] = "";
			Config["RHIB Loot Spawner (Leave Blank For Profile Loot)"] = eliteentityprefab;
			Config["RHIB Loot Spawner Multiplier"] = 2;
			Config["RHIB NPC Amount Per Boat"] = 4;
			Config["RHIB Health"] = 500;
			Config["RHIB NPC Health"] = 120;
			Config["RHIB Boat Amount Per Spawn"] = 1;
			Config["RHIB Boat Max Speed"] = 14f;
			Config["RHIB Boat Turn Speed"] = 7.5f;
			Config["RHIB AI Target Distance"] = 5f;
			Config["RHIB Boat Max At Once"] = 1;
			Config["RowBoat Kit Bots"] = "";
			Config["RowBoat Loot Spawner (Leave Blank For Profile Loot)"] = basicentityprefab;
			Config["RowBoat Loot Spawner Multiplier"] = 1;
			Config["RowBoat NPC Amount Per Boat"] = 2;
			Config["RowBoat Health"] = 400;
			Config["RowBoat NPC Health"] = 100;
			Config["RowBoat Boat Amount Per Spawn"] = 2;
			Config["RowBoat Boat Max Speed"] = 11f;
			Config["RowBoat Boat Turn Speed"] = 7.5f;
			Config["RowBoat AI Target Distance"] = 10f;
			Config["RowBoat Boat Max At Once"] = 4;
			Config["Submarine Loot Spawner (Leave Blank For Profile Loot)"] = eliteentityprefab;
			Config["Submarine Loot Spawner Multiplier"] = 1;
			Config["Submarine Health"] = 700;
			Config["Submarine Amount Per Spawn"] = 1;
			Config["Submarine Max Speed"] = 6f;
			Config["Submarine Turn Speed"] = 18f;
			Config["Submarine Target Depth"] = 6f;
			Config["Submarine Max At Once"] = 2;
			Config["Submarine Take Damage"] = 2500;
			Config["Minicopter Kit Bots"] = "";
			Config["Minicopter Loot Spawner (Leave Blank For Profile Loot)"] = normalentityprefab;
			Config["Minicopter Loot Spawner Multiplier"] = 1;
			Config["Minicopter Health"] = 300;
			Config["Minicopter NPC Health"] = 600;
			Config["Minicopter NPC Amount"] = 3;
			Config["Minicopter Amount Per Spawn"] = 2;
			Config["Minicopter Max Speed"] = 25f;
			Config["Minicopter Turn Speed"] = 4.5f;
			Config["Minicopter Hover Height"] = 45f;
			Config["Minicopter AI Target Distance"] = 4.5f;
			Config["Minicopter Max At Once"] = 4;
			Config["Minicopter Orbit Distance"] = 60f;
			Config["Minicopter Target Threat Level"] = 0.9f;
			Config["Scrapcopter Kit Bots"] = "";
			Config["Scrapcopter Loot Spawner (Leave Blank For Profile Loot)"] = helicrateentityprefab;
			Config["Scrapcopter Loot Spawner Multiplier"] = 3;
			Config["Scrapcopter Health"] = 850;
			Config["Scrapcopter NPC Health"] = 100;
			Config["Scrapcopter NPC Amount"] = 11;
			Config["Scrapcopter Amount Per Spawn"] = 1;
			Config["Scrapcopter Max Speed"] = 20f;
			Config["Scrapcopter Turn Speed"] = 2.5f;
			Config["Scrapcopter Hover Height"] = 45f;
			Config["Scrapcopter AI Target Distance"] = 4.5f;
			Config["Scrapcopter Max At Once"] = 2;
			Config["Scrapcopter Orbit Distance"] = 65f;
			Config["Scrapcopter Target Threat Level"] = 1f;
			Config["SnowMobile Amount Per Spawn"] = 3;
			Config["SnowMobile Max At Once"] = 6;
			Config["SnowMobile NPC Health"] = 15f;
			Config["SnowMobile NPC Aim Distance"] = 0.5f;
			Config["SnowMobile Health"] = 200f;
			Config["SnowMobile Roam Range"] = 200f;
			Config["SnowMobile Roam"] = true;
			Config["SnowMobile Speed"] = 10f;
			Config["SnowMobile Kit Bots"] = "";
			Config["SnowMobile Loot Spawner (Leave Blank For Profile Loot)"] = normalentityprefab;
			Config["SnowMobile Loot Spawner Multiplier"] = 1;
			Config["Horse Amount Per Spawn"] = 3;
			Config["Horse Max At Once"] = 6;
			Config["Horse NPC Health"] = 15f;
			Config["Horse Health"] = 350f;
			Config["Horse NPC Aim Distance"] = 0.5f;
			Config["Horse Roam Range"] = 280f;
			Config["Horse Roam"] = true;
			Config["Horse Speed"] = 8f;
			Config["Horse Kit Bots"] = "";
			Config["Horse Loot Spawner (Leave Blank For Profile Loot)"] = normalentityprefab;
			Config["Horse Loot Spawner Multiplier"] = 1;
			Config["Use Cargoship Path For Ocean"] = true;
			Config["Use Monument Path For Air"] = false;
			Config["Samsites Attack Pirates"] = false;
			Config["Static(Server) Samsites Attack Pirates"] = false;
			Config["Samsites Attack Pirate Boats"] = false;
			Config["Samsites Damage Multiplyer (Pirates)"] = 2f;
			Config["NPCs Tick Rate (sec)"] = 1.5f;
			Config["NPCs Parachute"] = true;
			Config["NPCs Parachute When Heli Health Below"] = 30f;
			Config["NPCs Parachute Decay (sec)"] = 30f;
			Config["NPCs Parachute Suicide (sec)"] = 600f;
			Config["NPCs Radio Disable"] = false;
			Config["NPCs Radio Custom"] = "";
			Config["NPCs Custom Kill Voice"] = "";
			Config["NPCs Custom Death Voice"] = "";
			Config["NPCs Custom Warning Voice"] = "";
			Config["NPCs Radio Custom Cooldown"] = 10;
			Config["NPCs Custom Death VoiceCooldown"] = 8;
			Config["NPCs Custom Kill Voice Cooldown"] = 8;
			Config["NPCs Custom Warning Voice Cooldown"] = 15;
			Config["NPCs Custom Warning Voice Trigger Distance"] = 15;
			Config["NPCs Max Voices To Play At Once On Server"] = 20;
			Config["NPCs Pirates Attack Supply Drops"] = true;
			Config["NPCs Pirates Attack Creates Being Hacked"] = true;
			Config["NPCs Pirates Counter Raids"] = true;
			Config["NPCs Pirates PeaceKeeper Mode"] = true;
			Config["NPCs Pirates PeaceKeeper Toggle Hostile Distance"] = 4;
			Config["NPC Damage Scaler"] = 0.8f;
			Config["NPC Aim Scaler"] = 1;
			Config["NPCs Name Tag"] = "[Pirate]";
			Config["NPCs Can Target Other NPCs"] = false;
			Config["Pirates Distance From POI Air"] = 5000f;
			Config["Pirates Distance From POI Water"] = 400f;
			Config["Pirates Distance From POI Land"] = 800f;
			Config["Pirates POI Expires After (sec)"] = 60f;
			Config["Pirate Base Amount To Spawn"] = 3;
			Config["Pirate Base Amount Of NPCs"] = 3;
			Config["Pirate Base NPC Aim Distance"] = 0.6f;
			Config["Pirate Base NPC Health"] = 150f;
			Config["Pirate Base NPC Kit"] = "";
			Config["Pirate Base Wall Radius"] = 12;
			Config["Pirate Base Loot Spawner (Leave Blank For Profile Loot)"] = eliteentityprefab;
			Config["Pirate Base Loot Spawner Multiplyer"] = 3;
			Config["Pirate Base Door Health"] = 600f;
			Config["Pirate Rail Amount To Spawn"] = 1;
			Config["Pirate Rail Train Health"] = 1000;
			Config["Pirate Rail Amount Of NPCs"] = 6;
			Config["Pirate Rail NPC Aim Distance"] = 0.6f;
			Config["Pirate Rail NPC Health"] = 150f;
			Config["Pirate Rail NPC Driver Health"] = 2f;
			Config["Pirate Rail NPC Kit"] = "";
			Config["Pirate Rail Loot Spawner (Leave Blank For Profile Loot)"] = eliteentityprefab;
			Config["Pirate Rail Loot Spawner Multiplyer"] = 4;
			Config["Pirate Rail Samsite Range"] = 20f;
			Config["Pirate Rail Samsite Health"] = 200f;
			Config["Pirate Rail Turret Health"] = 50f;
			Config["Pirate Rail Turret Gun"] = "pistol.eoka";
			Config["Pirate Rail Wagon Health"] = 2500f;
			Config["Pirate Rail Wagon Door Health"] = 55f;
			Config["Pirate Rail Wagon Door SkinID"] = 2488843636;
			Config["Pirate Rail Wagon Spawn Ammount"] = 3;
			Config["Pirate Rail Train Speed"] = 8;
			Config["Pirate Foot NPC Health"] = 120;
			Config["Pirate Foot Aim Distance"] = 0.9f;
			Config["Pirate Foot NPC Kit"] = "";
			Config["Pirate Foot NPC Roam Distance"] = 50f;
			Config["Pirates Events Enabled"] = true;
			Config["Pirate Events CoolDown Delay"] = 600f;
			Config["Pirate Events Annoucements"] = true;
			Config["Pirate Events Player Radius"] = 50;
			Config["Pirate Events NPC Amount"] = 5;
			Config["Pirate Events NPC Kit"] = "";
			Config["Pirate Events NPC Health"] = 120f;
			Config["Pirate Events NPC AimMulti"] = 0.8f;
			Config["Pirate Plane Event Enabled"] = true;
			Config["Pirate Plane Fly Height"] = 120f;
			Config["Pirate Plane Fly Speed"] = 40f;
			Config["Pirate Plane NPCs Per Drop"] = 3f;
			Config["Pirate Plane Number Of Drops"] = 3f;
			Config["Pirate Events NPC Spawn Radius"] = 12f;
			Config["Pirate Events NPC Stationary Radius"] = 4f;
			Config["Pirate Plane Allowed Monuments"] = "assets/bundled/prefabs/autospawn/monument/harbor/harbor_2.prefab|assets/bundled/prefabs/autospawn/monument/harbor/harbor_1.prefab|assets/bundled/prefabs/autospawn/monument/military_bases/desert_military_base_d.prefab|assets/bundled/prefabs/autospawn/monument/arctic_bases/arctic_research_base_a.prefab|assets/bundled/prefabs/autospawn/monument/xlarge/launch_site_1.prefab|assets/bundled/prefabs/autospawn/monument/large/excavator_1.prefab|assets/bundled/prefabs/autospawn/monument/medium/junkyard_1.prefab|assets/bundled/prefabs/autospawn/monument/large/trainyard_1.prefab|assets/bundled/prefabs/autospawn/monument/large/powerplant_1.prefab|assets/bundled/prefabs/autospawn/monument/large/military_tunnel_1.prefab|assets/bundled/prefabs/autospawn/monument/large/airfield_1.prefab|assets/bundled/prefabs/autospawn/monument/large/water_treatment_plant_1.prefab|assets/bundled/prefabs/autospawn/monument/medium/radtown_small_3.prefab|assets/bundled/prefabs/autospawn/monument/small/mining_quarry_a.prefab|assets/bundled/prefabs/autospawn/monument/small/satellite_dish.prefab|assets/bundled/prefabs/autospawn/monument/small/sphere_tank.prefab|assets/bundled/prefabs/autospawn/monument/small/mining_quarry_b.prefab|assets/bundled/prefabs/autospawn/monument/small/mining_quarry_c.prefab|assets/bundled/prefabs/autospawn/monument/tiny/water_well_d.prefab|assets/bundled/prefabs/autospawn/monument/tiny/water_well_e.prefab|assets/bundled/prefabs/autospawn/monument/tiny/water_well_c.prefab|assets/bundled/prefabs/autospawn/tunnel-entrance/entrance_bunker_a.prefab|assets/bundled/prefabs/autospawn/tunnel-entrance/entrance_bunker_c.prefab|assets/bundled/prefabs/autospawn/monument/roadside/warehouse.prefab|assets/bundled/prefabs/autospawn/monument/roadside/gas_station_1.prefab|assets/bundled/prefabs/autospawn/monument/roadside/supermarket_1.prefab|assets/bundled/prefabs/autospawn/power substations/small/power_sub_small_2.prefab|assets/bundled/prefabs/autospawn/power substations/small/power_sub_small_1.prefab|assets/bundled/prefabs/autospawn/power substations/big/power_sub_big_2.prefab|assets/bundled/prefabs/autospawn/power substations/big/power_sub_big_1.prefab";
			Config["XLootCustomProfiles"] = "<Profile>jumpsuit.suit.blue,1,0|maxhealthtea.pure,1,0|ammo.rifle,30,0|riflebody,2,0|gunpowder,150,0|can.beans,1,0";
		}

		#endregion

		#region Commands
		[ChatCommand("pirates.count")]
		private void ActivePiratesCount(BasePlayer player) { if (player.IsAdmin) { Counterfunction(player); } }
		[ChatCommand("pirates.cargoplane")]
		private void ManualSpawncargoplane(BasePlayer player) { if (player.IsAdmin) { SpawnPirates(player.transform.position, 9); player.ChatMessage("Calling pirate cargo drop spawn."); } }
		[ChatCommand("pirates.drop")]
		private void ManualSpawnFootNPCDrop(BasePlayer player)
		{
			if (player.IsAdmin)
			{
				FootPirate pf = SpawnPirates(player.transform.position, 11).GetComponent<FootPirate>();
				if (pf != null) { pf.movetopoint = player.transform.position; pf.movetopoint.y = TerrainMeta.HeightMap.GetHeight(player.transform.position) + _ServerSettings.PiratePlaneflyheight; player.ChatMessage("Calling pirate cargo drop on your location."); }
			}
		}
		[ChatCommand("pirates.foot")]
		private void ManualSpawnFootNPCGround(BasePlayer player) { if (player.IsAdmin) { SpawnPirates(player.transform.position, 10); player.ChatMessage("Spawning foot pirate on your location."); } }
		[ChatCommand("pirates.base")]
		private void ManualSpawnBase(BasePlayer player) { if (player.IsAdmin) { SpawnPirates(player.transform.position, 7); player.ChatMessage("Spawning pirate base on your location."); } }
		[ChatCommand("pirates.showroad")]
		private void ShowRoadCmd(BasePlayer player) { if (!player.IsAdmin) { return; } foreach (Vector3 v in roadnodes) { player.SendConsoleCommand("ddraw.sphere", 8f, Color.blue, v, 1f); } }
		[ConsoleCommand("pirates.addpoi")]
		private void CmdAddPOI(ConsoleSystem.Arg arg) { BasePlayer player = arg?.Connection?.player as BasePlayer; if ((player != null && player?.net?.connection.authLevel < 2) || arg?.Args?.Length != 2) { return; } Manuallyaddpoifunction(arg.Args, null); }
		[ChatCommand("pirates.poi")]
		private void ManualAddPOI(BasePlayer player, string command, string[] Args) { if (!player.IsAdmin) { return; } Manuallyaddpoifunction(Args, player); }
		[ChatCommand("pirates.clearpoi")]
		private void ManualClearPoi(BasePlayer player, string command, string[] Args) { if (!player.IsAdmin) { return; } PointsOfIntrest.Clear(); }
		[ChatCommand("pirates.rhib")]
		private void ManualSpawnRhib(BasePlayer player) { if (player.IsAdmin) { SpawnPirates(player.transform.position); player.ChatMessage("Spawning pirate rhib on your location."); } }
		[ChatCommand("pirates.rowboat")]
		private void ManualSpawnRowBoat(BasePlayer player) { if (player.IsAdmin) { SpawnPirates(player.transform.position, 1); player.ChatMessage("Spawning pirate rowboat on your location."); } }
		[ChatCommand("pirates.sub")]
		private void ManualSpawnsub(BasePlayer player) { if (player.IsAdmin) { SpawnPirates(player.transform.position, 6); player.ChatMessage("Spawning pirate sub on your location."); } }
		[ChatCommand("pirates.mini")]
		private void ManualSpawnMini(BasePlayer player) { if (player.IsAdmin) { SpawnPirates(player.transform.position, 2); player.ChatMessage("Spawning pirate mini on your location."); } }
		[ChatCommand("pirates.scrap")]
		private void ManualSpawnScrap(BasePlayer player) { if (player.IsAdmin) { SpawnPirates(player.transform.position, 3); player.ChatMessage("Spawning pirate scrappy on your location."); } }
		[ChatCommand("pirates.horse")]
		private void ManualSpawnHorse(BasePlayer player) { if (player.IsAdmin) { SpawnPirates(player.transform.position, 4); player.ChatMessage("Spawning pirate horse on your location."); } }
		[ChatCommand("pirates.snowmobile")]
		private void ManualSpawnSnowMobile(BasePlayer player) { if (player.IsAdmin) { SpawnPirates(player.transform.position, 5); player.ChatMessage("Spawning pirate snowmobile on your location."); } }
		[ChatCommand("pirates.train")]
		private void ManualSpawnTrain(BasePlayer player) { if (player.IsAdmin) { SpawnPirates(player.transform.position, 8); player.ChatMessage("Spawning pirate train on your location."); } }
		[ChatCommand("pirates.reload")]
		private void ManualLoadSettings(BasePlayer player) { if (player.IsAdmin) { _ServerSettings = new ServerSettings(); BuildLootTablefunction(); } }
		[ChatCommand("pirates.showpath")]
		private void DrawPathCMD(BasePlayer player) { if (player.IsAdmin) { foreach (Vector3 vector in seanodes) { player.SendConsoleCommand("ddraw.sphere", 8f, Color.blue, vector, 2f); } foreach (Vector3 vector in airnodes) { player.SendConsoleCommand("ddraw.sphere", 8f, Color.red, vector, 2f); } foreach (Vector3 vector in groundnodes) { player.SendConsoleCommand("ddraw.sphere", 8f, Color.red, vector, 2f); } } }
		[ChatCommand("pirates.cargopath")]
		private void SwitchPathCargocmd(BasePlayer player) { if (player.IsAdmin) { player.ChatMessage("Switching to Cargo Pathing"); seanodes = TerrainMeta.Path.OceanPatrolFar; } }
		[ChatCommand("pirates.customseapath")]
		private void SwitchPathCustom(BasePlayer player) { if (player.IsAdmin) { player.ChatMessage("Switching to Custom Pathing"); SetupOceanPatrolPath(); } }
		[ChatCommand("pirates.monumentpath")]
		private void SwitchMonumentPath(BasePlayer player) { if (player.IsAdmin) { player.ChatMessage("Switching to Monument Pathing"); airnodes = MonumentPathList(); } }
		[ChatCommand("pirates.customairpath")]
		private void SwitchMonumentCustom(BasePlayer player) { if (player.IsAdmin) { player.ChatMessage("Switching to Custom Air Pathing"); SetupAirPatrolPath(); } }
		[ChatCommand("pirates.topologytest")]
		private void topologytestcmd(BasePlayer player)
		{
			if (player.IsAdmin) { player.ChatMessage(TerrainMeta.TopologyMap.GetTopology(player.transform.position, TopologyBaseBlock).ToString()); }
		}
		[ChatCommand("pirates.loot")]
		private void SetupLootcmd(BasePlayer player, string command, string[] args)
		{
			if (player.IsAdmin)
			{
				try
				{
					if (args.Length < 1)
					{
						ShowHelpfunction(player);
						return;
					}
					if (args.Length == 1 && args[0].Contains("add"))
					{
						ItemContainer profile = new ItemContainer();
						profile.isServer = true;
						profile.entityOwner = player;
						profile.allowedContents = ItemContainer.ContentsType.Generic;
						profile.GiveUID();
						profile.capacity = 30;
						if (!_openProfiles.Contains(player)) { _openProfiles.Add(player); }
						timer.Once(0.5f, () => { LootContainerfunction(player, profile); });
						player.ChatMessage("Created New Loot Profile.");
						return;
					}
					if (args.Length == 2 && args[0].Contains("edit"))
					{
						if (args[1].IsNumeric())
						{
							ItemContainer profile = new ItemContainer();
							profile.isServer = true;
							profile.entityOwner = player;
							profile.allowedContents = ItemContainer.ContentsType.Generic;
							profile.GiveUID();
							profile.capacity = 30;
							if (!_openProfiles.Contains(player)) { _openProfiles.Add(player); }
							filllootfunction(profile, int.Parse(args[1]), 0);
							plugin.loottableDict.RemoveAt(int.Parse(args[1]));
							timer.Once(0.5f, () => { LootContainerfunction(player, profile); });
							player.ChatMessage("Editing Loot Profile.");
						}
						else { player.ChatMessage("Must profile profile index number."); }
						return;
					}
				}
				catch { ShowHelpfunction(player); }
			}
		}

		#endregion

		#region Hooks
		private void Init()
		{
			plugin = this;
			try
			{
				_ServerSettings = new ServerSettings();
				_ServerSettings.LoadSettings();
			}
			catch
			{
				Puts("Failed to load settings, Resetting to default");
				LoadDefaultConfig();
				SaveConfig();
				_ServerSettings = new ServerSettings();
				_ServerSettings.LoadSettings();
			}
			if (!_ServerSettings.NPCCAttackSupplyDrops) { Unsubscribe("OnSupplyDropLanded"); }
			if (!_ServerSettings.NPCAttackCreate) { Unsubscribe("OnCrateHack"); }
			if (!_ServerSettings.SamSitesBool0) { Unsubscribe("OnSamSiteTargetScan"); }
			if (!_ServerSettings.RaidableBasesAT) { Unsubscribe("CanBeTargeted"); }
		}

		private void OnServerInitialized(bool initial)
		{
			BuildLootTablefunction();
			if (!_ServerSettings.CargoPathBool) { SetupOceanPatrolPath(); } else { seanodes = TerrainMeta.Path.OceanPatrolFar; sealoaded = true; Puts("Loaded " + seanodes.Count.ToString() + " Sea Nodes."); }
			if (!_ServerSettings.MonumentPathBool) { SetupAirPatrolPath(); } else { airnodes = MonumentPathList(); airloaded = true; }
			Threads.Add(ServerMgr.Instance.StartCoroutine(GenerateRoadGrid()));
			timer.Once(2f, () => { SetupPlanePath(); });
			SetupLandPatrolPath();
			SetupRailPatrolPath();
			if (_ServerSettings.ShowMapMarkers) { Threads.Add(ServerMgr.Instance.StartCoroutine(MapRoutine())); };
			PirateSpawner();
			spawner = timer.Every(_ServerSettings.SpawnEvery, () => { PirateSpawner(); });
			if (!initial) { ClearUpVectorZerofunction(Vector3.zero); }
			Threads.Add(ServerMgr.Instance.StartCoroutine(GenerateFootGrid()));
			timer.Once(30f, () => { Threads.Add(ServerMgr.Instance.StartCoroutine(NPCAIFunction())); });
		}

		private void Unload()
		{
			if (spawner != null) { spawner.Destroy(); spawner = null; }
			KillTrainsfunction();
			ClearLootfunction(PirateUintList);
			KillListfunction(GroundedNPCs);
			KillListfunction(NPCsUintList);
			KillListfunction(BaseParts);
			KillListfunction(PirateUintList);
			if (Threads != null && Threads.Count != 0)
			{
				foreach (Coroutine co in Threads) { if (co != null) ServerMgr.Instance.StopCoroutine(co); }
				Threads = null;
				MarkerDisplayingDelete();
			}
			_ServerSettings = null;
			plugin = null;
		}

		private object CanLootPlayer(BasePlayer looted, BasePlayer looter) { if (_openProfiles.Contains(looter)) { return true; } return null; }

		private void OnPlayerLootEnd(PlayerLoot playerLoot)
		{
			var player = (BasePlayer)playerLoot.gameObject.ToBaseEntity();
			if (_openProfiles.Contains(player))
			{
				if (playerLoot.containers[0].itemList == null) { _openProfiles.Clear(); return; }
				if (playerLoot.containers[0].itemList.Count == 0)
				{
					savelootfunction();
					_openProfiles.Clear();
					player.ChatMessage("Profile Deleted.");
					return;
				}
				var newitems = new List<Dictionary<string, string>>();
				foreach (Item pl in playerLoot.containers[0].itemList)
				{
					Dictionary<string, string> items = new Dictionary<string, string>();
					items.Add(pl.info.shortname, pl.amount + "," + pl.skin);
					newitems.Add(items);
				}
				loottableDict.Add(newitems);
				savelootfunction();
				_openProfiles.Clear();
				player.ChatMessage("Saved Loot Profile Changes.");
			}
		}

		private object OnEntityFlagsNetworkUpdate(BaseEntity be)
		{
			if (be.prefabID == 500822506 && _ServerSettings.PirateEventEnabled)
			{
				if (Rust.Application.isLoading || !sealoaded || !airloaded || !landloaded || alarmcooldown.Contains(be.net.ID)) { return null; }
				foreach (PrefabData alarm in Alarmtriggers)
				{
					if (Vector3.Distance(be.transform.position, alarm.position) < 1f && PlayersNearbyfunction(be.transform.position, _ServerSettings.PirateEventRadius))
					{
						uint ID = be.net.ID;
						alarmcooldown.Add(ID);
						timer.Once(_ServerSettings.PirateEventCoolDownDelay, () => { alarmcooldown.Remove(ID); });
						if (alarm.category.Contains("callpirateplane")) { EventCallfunction(0, alarm); }
						else if (alarm.category.Contains("callpiratestationary")) { EventCallfunction(1, alarm); }
						else if (alarm.category.Contains("callpiratefoot")) { EventCallfunction(2, alarm); }
						break;
					}
				}
			}
			return null;
		}

		private object CanBeTargeted(BaseCombatEntity target, AutoTurret at)
		{
			if (target == null || at == null) { return null; }
			if (at.OwnerID != 0) { return null; }
			if (target.net != null) { if (NPCsUintList.Contains(target.net.ID) || GroundedNPCs.Contains(target.net.ID)) { return false; } }
			return null;
		}

		private object CanMountEntity(BasePlayer player, BaseMountable mount)
		{
			if (player == null | mount == null) { return null; }
			if (player.IsConnected == false) { return null; }
			BaseVehicle bv = mount.VehicleParent();
			if (bv != null) { if (PirateUintList.Contains(bv.net.ID)) { return false; } }
			return null;
		}

		private void OnEntitySpawned(BaseEntity spawned)
		{
			HelicopterDebris debris = spawned as HelicopterDebris;
			if (debris != null)
			{
				NextTick(() => { if (debris != null && _ServerSettings.DisableScrapHeliGibs && debris.ShortPrefabName == "servergibs_scraptransport") { debris.Kill(); } });
			}
		}

		private object CanDismountEntity(BasePlayer player, BaseMountable mount) { if (player == null && mount == null || player.net == null) { return null; } if (NPCsUintList.Contains(player.net.ID)) { return false; } return null; }

		private object OnNpcRadioChatter(ScientistNPC bot)
		{
			if (_ServerSettings.NPCCustomChatter0 != "")
			{
				uint ID = bot.net.ID;
				if (VoiceUintList.Count > _ServerSettings.NPCMaxVoicesAtOnce) { return false; }
				bool flag = false;
				foreach (BasePlayer bp in BasePlayer.activePlayerList)
				{
					if (bot.Distance(bp.transform.position) < 90)
					{
						flag = true;
						break;
					}
				}
				if (!CoolDownUintList.Contains(ID) && flag)
				{
					CoolDownUintList.Add(ID);
					WebPlayback(_ServerSettings.NPCCustomChatter0, bot);
					timer.Once(_ServerSettings.NPCCustomChatterCD, () => { CoolDownUintList.Remove(ID); });
					return false;
				}
			}
			if (_ServerSettings.NPCChatter) { return false; }
			return null;
		}

		private void OnSupplyDropLanded(SupplyDrop supplyDrop) { if (_ServerSettings.NPCCAttackSupplyDrops) { assignPOIfunction(new Vector3(supplyDrop.transform.position.x, supplyDrop.transform.position.y, supplyDrop.transform.position.z)); } }

		private void OnCrateHack(HackableLockedCrate hackableLockedCrate) { if (_ServerSettings.NPCAttackCreate) { assignPOIfunction(new Vector3(hackableLockedCrate.transform.position.x, hackableLockedCrate.transform.position.y, hackableLockedCrate.transform.position.z)); } }

		private object OnSamSiteTarget(SamSite samsite, SamSite.ISamSiteTarget target)
		{
			if (!_ServerSettings.StaticSamSitesBool) { foreach (uint id in PirateUintList) { if (target.ToString().Contains(id.ToString())) { return false; } } }
			return null;
		}

		private object OnSamSiteTargetScan(SamSite samSite)
		{
			if (samSite == null) { return null; }
			if (!_ServerSettings.SamSitesBool0) { return null; }
			if (samSite.HasAmmo() && PirateUintList.Count != 0)
			{
				foreach (uint p in PirateUintList)
				{
					if (!BaseNetworkable.serverEntities.entityList.Contains(p)) { continue; }
					BaseEntity be = BaseNetworkable.serverEntities.entityList[p] as BaseEntity;
					if (be == null) { continue; }
					if (samSite.Distance(be) < samSite.vehicleScanRadius)
					{
						if (!_ServerSettings.StaticSamSitesBool && samSite.staticRespawn) { samSite.ClearTarget(); return true; }
						AirPirates AP = be.GetComponent<AirPirates>();
						if (AP != null)
						{
							for (int i = 0; i < 3; i++) { timer.Once(i * 0.5f, () => { if (AP == null) { return; } SamLogicfunction(samSite, be, AP.currentVelocity, AP.moveSpeed); }); }
							return true;
						}
						if (_ServerSettings.SamSitesBoat)
						{
							WaterPirate WP = be.GetComponent<WaterPirate>();
							if (WP != null)
							{
								for (int i = 0; i < 3; i++) { timer.Once(i * 0.5f, () => { if (WP == null) { return; } SamLogicfunction(samSite, be, WP.currentVelocity, 0); }); }
								return true;
							}
						}
					}
				}
			}
			return null;
		}

		private void OnPlayerDeath(BasePlayer player, HitInfo info)
		{
			if (player == null || player.net == null) { return; }
			if (NPCsUintList.Contains(player.net.ID) || GroundedNPCs.Contains(player.net.ID)) { CoolDownUintList.Remove(player.net.ID); foreach (BaseEntity be in player.children.ToArray()) { if (be == null) { continue; } be.SetParent(null, true, true); if (be.ToString().Contains("parachute")) { be.Kill(); } if (be is SearchLight) { be.Kill(); } } }
		}

		private void OnEntityDeath(BaseCombatEntity entity, HitInfo info)
		{
			if (entity != null && info != null)
			{
				bool messaged = false;
				if (_ServerSettings.NPCCustomKill0 != "")
				{
					if (info.InitiatorPlayer != null && info.InitiatorPlayer.net != null)
					{
						uint ID = info.InitiatorPlayer.net.ID;
						if (NPCsUintList.Contains(ID) || GroundedNPCs.Contains(ID))
						{
							messaged = true;
							if (!CoolDownUintList.Contains(ID) && VoiceUintList.Count < _ServerSettings.NPCMaxVoicesAtOnce)
							{
								CoolDownUintList.Add(ID);
								WebPlayback(_ServerSettings.NPCCustomKill0, info.InitiatorPlayer);
								timer.Once(_ServerSettings.NPCCustomKillCD, () => { CoolDownUintList.Remove(ID); });
							}
						}
					}
				}
				if (_ServerSettings.NPCCustomDeath0 != "" && !messaged && entity.net != null)
				{
					uint ID = entity.net.ID;
					if (NPCsUintList.Contains(ID) || GroundedNPCs.Contains(ID))
					{
						if (!CoolDownUintList.Contains(ID) && VoiceUintList.Count < _ServerSettings.NPCMaxVoicesAtOnce)
						{
							CoolDownUintList.Add(ID);
							BasePlayer p = entity.ToPlayer();
							if (p != null)
							{
								WebPlayback(_ServerSettings.NPCCustomDeath0, p);
								timer.Once(_ServerSettings.NPCCustomDeathCD, () => { CoolDownUintList.Remove(ID); });
							}
						}
					}
				}
			}
			FreeLists(entity);
			if (_ServerSettings.NPCCountRaids)
			{
				if (info == null) { return; }
				if (!BaseDamagefunction(entity)) { return; }
				if (info.InitiatorPlayer == null) { return; }
				if (entity.OwnerID >= 10000000 && entity.OwnerID <= 20000000)
				{
					assignPOIfunction(new Vector3(entity.transform.position.x, entity.transform.position.y, entity.transform.position.z));
					return;
				}
				BuildingPrivlidge buildingPrivilege = entity.GetBuildingPrivilege();
				if (buildingPrivilege == null || buildingPrivilege.authorizedPlayers.IsEmpty()) { return; }
				foreach (PlayerNameID attacked in buildingPrivilege.authorizedPlayers)
				{
					if (attacked == null) continue;
					if (attacked.userid == info.InitiatorPlayer.userID) return;
					assignPOIfunction(new Vector3(entity.transform.position.x, entity.transform.position.y, entity.transform.position.z));
				}
			}
		}

		private object CanEntityBeTargeted(BasePlayer player, BaseEntity turret)
		{
			if (turret == null || player == null) { return null; }
			if (BaseParts.Contains(turret.net.ID) && !player.IsNpc)
			{
				return true;
			}
			return null;
		}

		private object CanEntityTakeDamage(BaseCombatEntity entity, HitInfo hitInfo)
		{
			if (entity == null || PirateUintList == null || hitInfo == null || entity.net == null) { return null; }
			uint ID = entity.net.ID;
			if (PirateUintList.Contains(ID) || GroundedNPCs.Contains(ID) || NPCsUintList.Contains(ID) || BaseParts.Contains(ID)) { return true; }
			if (TrainsBaseEntityList != null || TrainsBaseEntityList.Count != 0) { foreach (BaseEntity be in TrainsBaseEntityList) { if (be != null) { if (be.net.ID == ID) { return true; } } } }
			if (hitInfo.Initiator != null)
			{
				uint ID2 = hitInfo.Initiator.net.ID;
				if (PirateUintList.Contains(ID2) || GroundedNPCs.Contains(ID2) || NPCsUintList.Contains(ID2) || BaseParts.Contains(ID2)) { return true; }
			}
			return null;
		}

		private object OnEntityTakeDamage(BaseCombatEntity bce, HitInfo info)
		{
			if (bce == null || PirateUintList == null || info == null || bce.net == null) { return null; }
			uint ID = bce.net.ID;
			if (TrainsBaseEntityList.Contains(bce))
			{
				if (info.damageTypes == null) { return null; }
				if (info.damageTypes.GetMajorityDamageType() == DamageType.Decay) { return true; }
				if (info.damageTypes.GetMajorityDamageType() == DamageType.Collision) { return true; }
			}
			if (NPCsUintList.Contains(ID) || GroundedNPCs.Contains(ID))
			{
				if (info.InitiatorPlayer != null)
				{
					BasePlayer attackingplayer = info.InitiatorPlayer;
					if (attackingplayer == null) { return null; }
					if (BasePlayer.activePlayerList.Contains(attackingplayer))
					{
						ScientistNPC npc = bce as ScientistNPC;
						if (npc != null && npc.Brain != null)
						{
							npc.Brain.Senses.Memory.Targets.Clear();
							npc.Brain.Senses.Memory.Players.Clear();
							npc.Brain.Senses.Memory.Threats.Clear();
							npc.Brain.Senses.Memory.Targets.Add(attackingplayer);
							npc.Brain.Senses.Memory.Players.Add(attackingplayer);
							npc.Brain.Senses.Memory.Threats.Add(attackingplayer);
							if (npc.Brain.AttackRangeMultiplier != 10)
							{
								float oldTLR = npc.Brain.TargetLostRange;
								float oldARM = npc.Brain.AttackRangeMultiplier;
								float oldAIM = npc.aimConeScale;
								npc.aimConeScale = 0.2f;
								npc.Brain.TargetLostRange = 800;
								npc.Brain.AttackRangeMultiplier = 10;
								npc.Invoke(() =>
								{
									if (npc != null)
									{
										npc.aimConeScale = oldAIM;
										npc.Brain.TargetLostRange = oldTLR;
										npc.Brain.AttackRangeMultiplier = oldARM;
										npc.Brain.Senses.Memory.Targets.Clear();
										npc.Brain.Senses.Memory.Players.Clear();
										npc.Brain.Senses.Memory.Threats.Clear();
									}
								}, 8f);
							}
						}
						return null;
					}
				}
				return null;
			}
			if (PirateUintList.Contains(ID))
			{
				if (info.damageTypes == null) { return null; }
				if (info.damageTypes.GetMajorityDamageType() == DamageType.Decay) { return true; }
				if ((bce is Snowmobile) && info.damageTypes.GetMajorityDamageType() == DamageType.Collision) { return true; }
				if (info.Initiator != null)
				{
					if (info.damageTypes.GetMajorityDamageType() == DamageType.Explosion && (info.Initiator.ShortPrefabName == "scraptransporthelicopter" || info.Initiator.ShortPrefabName == "minicopter.entity")) { return true; }
					if (info.Initiator is SamSite) { return null; }
					if (info.Initiator is AutoTurret) { return null; }
					if (info.Initiator is BaseSubmarine) { return null; }
				}
				if (info.InitiatorPlayer != null)
				{
					BasePlayer attackingplayer = info.InitiatorPlayer;
					if (attackingplayer == null) { return null; }
					if (BasePlayer.activePlayerList.Contains(attackingplayer))
					{
						PirateAttackedfunction(bce, attackingplayer);
						if (bce is BaseSubmarine)
						{
							if (info.damageTypes != null)
							{
								if (info.damageTypes.GetMajorityDamageType() == DamageType.AntiVehicle) { bce.Hurt(_ServerSettings.SubmarineDamage); return null; }
								if (info.damageTypes.GetMajorityDamageType() == DamageType.Bullet) { bce.Hurt(50); return null; }
							}
							if (info.WeaponPrefab != null && info.WeaponPrefab._name != null) { if (info.WeaponPrefab._name.ToString().Contains("explosive.timed.deployed")) { bce.Hurt(_ServerSettings.SubmarineDamage); } }
						}
						return null;
					}
				}
				return true;
			}
			return null;
		}

		private object OnNpcTarget(HumanNPC bot, BaseEntity baseEntity)
		{
			if (bot == null || bot.net == null || baseEntity == null || baseEntity.net == null) { return null; }
			uint ID = bot.net.ID;
			if (VoiceUintList.Contains(baseEntity.net.ID)) { return true; }
			if (!NPCsUintList.Contains(ID) && !GroundedNPCs.Contains(ID)) { return null; }
			BasePlayer target = baseEntity.ToPlayer();
			if (!_ServerSettings.NPCsCanTargetNPCsBool) { if (target != null) { if (target.IsNpc) { return true; } } }
			if (_ServerSettings.NPCPeaceKeeperBool)
			{
				if (target != null)
				{
					float distance = bot.Distance(target);
					if (distance < _ServerSettings.NPCPeaceKeeperToggle) { target.MarkHostileFor(10); }
					if (_ServerSettings.NPCCustomWarningString != "")
					{
						if (!CoolDownUintList.Contains(ID) && VoiceUintList.Count < _ServerSettings.NPCMaxVoicesAtOnce)
						{
							if (distance > _ServerSettings.NPCPeaceKeeperToggle && distance < _ServerSettings.NPCCustomWarningDistance)
							{
								CoolDownUintList.Add(ID);
								WebPlayback(_ServerSettings.NPCCustomWarningString, bot);
								timer.Once(_ServerSettings.NPCCustomWarningCD, () => { CoolDownUintList.Remove(ID); });
							}
						}
					}
					if (target.IsHostile()) { return null; }
				}
				return true;
			}
			return null;
		}

		#endregion

		#region Functions

		private void ShowHelpfunction(BasePlayer player)
		{
			player.ChatMessage("How to setup loot:");
			player.ChatMessage("Type /pirates.loot add to create a new loot profile.");
			player.ChatMessage("Type /pirates.loot edit int to view loot of that index");
			player.ChatMessage("Type /pirates.loot remove int to remove profile loot of that index");
		}

		private void EventCallfunction(int eventtype, PrefabData alarm)
		{
			if (_ServerSettings.PirateEventAnnounce)
			{
				foreach (BasePlayer bp in BasePlayer.activePlayerList)
				{
					switch (eventtype)
					{
						case 0:
							bp.ChatMessage("Pirate Event Cargo Plane Inbound!");
							break;
						case 1:
							bp.ChatMessage("Pirate Event NPCs Spawning Stationary!");
							break;
						case 2:
							bp.ChatMessage("Pirate Event NPCs Spawning!");
							break;
					}
				}
			}
			timer.Once(5f, () =>
			{
				Vector3 pos = Vector3.zero;
				List<Vector3> addpos = new List<Vector3>();
				List<ScientistNPC> npcs = new List<ScientistNPC>();
				switch (eventtype)
				{
					case 0:
						FootPirate pf = SpawnPirates(seanodes.GetRandom(), 11).GetComponent<FootPirate>();
						if (pf != null) { pf.movetopoint = new Vector3(alarm.position.x, TerrainMeta.HeightMap.GetHeight(alarm.position) + _ServerSettings.PiratePlaneflyheight, alarm.position.z); }
						break;
					case 1:
						pos = new Vector3(alarm.position.x, alarm.position.y, alarm.position.z);
						for (int i = 0; i < _ServerSettings.PirateEventNPCAmount; i++)
						{
							Vector3 vector = UnityEngine.Random.insideUnitCircle * _ServerSettings.PirateEventStationaryRadius;
							addpos.Add(pos + new Vector3(vector.x, 0, vector.y));
						}
						npcs = CreateNPCs(_ServerSettings.PirateEventNPCAmount, 10, _ServerSettings.PirateBaseNPCHealth, _ServerSettings.PirateEventNPCKit, _ServerSettings.PirateEventNPCAimMulti, null, addpos);
						if (npcs != null) { foreach (ScientistNPC npc in npcs) { npc.NavAgent.isStopped = true; } }
						break;
					case 2:
						pos = new Vector3(alarm.position.x, alarm.position.y, alarm.position.z);
						for (int i = 0; i < _ServerSettings.PirateEventNPCAmount; i++) { addpos.Add(pos); }
						npcs = CreateNPCs(_ServerSettings.PirateEventNPCAmount, 10, _ServerSettings.PirateBaseNPCHealth, _ServerSettings.PirateEventNPCKit, _ServerSettings.PirateEventNPCAimMulti, null, addpos);
						timer.Once(2f, () =>
						{
							if (npcs == null) { return; }
							foreach (ScientistNPC npc in npcs) { BailOutfunction(alarm.position, npc, alarm.position, (int)_ServerSettings.PirateEventSpawnRadius, 0, false); }
						});
						break;
				}
			});
		}

		private List<string> AllowedMonumentList(string settings)
		{
			List<string> list = new List<string>();
			list.AddRange(settings.Split('|'));
			return list;
		}

		private void Counterfunction(BasePlayer p) { p.ChatMessage("Pirates: RHIB:" + pirateRHIB + " RowBoat:" + pirateRowBoat + " Sub:" + pirateSub + " Mini:" + piratesMini + " Scrap:" + pirateScrap + " Horse:" + pirateHorse + " SnowMobile:" + pirateSnowMobile + " Train:" + pirateRail + " Bases:" + pirateBase + " ActiveNPCs:" + ActiveNPCs.Count); }

		private void KillWagonsfunction(List<BaseEntity> wagons)
		{
			if (wagons == null || wagons.Count == 0) { return; }
			foreach (BaseEntity w in wagons)
			{
				if (w != null && !w.IsDestroyed)
				{
					if ((w as TrainCar).platformParentTrigger != null && (w as TrainCar).platformParentTrigger.HasAnyEntityContents)
					{
						using (HashSet<BaseEntity>.Enumerator enumerator = (w as TrainCar).platformParentTrigger.entityContents.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.ToPlayer() != null)
								{
									enumerator.Current.ToPlayer().SetParent(null, true, true);
									enumerator.Current.ToPlayer().transform.position = enumerator.Current.ToPlayer().transform.position + new Vector3(0, 1.5f, 0);
									enumerator.Current.ToPlayer().SendNetworkUpdateImmediate();
								}
							}
						}
					}
					NextFrame(() =>
					{
						if (w != null && !w.IsDestroyed)
						{
							w.transform.position = new Vector3(0, -501, 0);
							w.Kill();
						}
					});
				}
			}
		}

		private void KillTrainsfunction()
		{
			foreach (BaseEntity be in TrainsBaseEntityList)
			{
				if (be != null && !be.IsDestroyed)
				{
					foreach (BaseEntity child in be.children)
					{
						if (child is BasePlayer && !child.IsNpc)
						{
							child.SetParent(null, true, true);
						}
					}
					RailPirate RP = be.GetComponent<RailPirate>();
					KillWagonsfunction(RP.wagons.ToList());
					NextFrame(() =>
					{
						be.transform.position = new Vector3(0, -501, 0); be.Kill();
					});
				}
			}
		}

		public char GetLetterfunction() { return (char)('a' + (ulong)UnityEngine.Random.Range(0, 4)); }

		private bool HasSpaceToSpawnfunction(TrainTrackSpline trainTrackSpline, Vector3 position)
		{
			foreach (TrainTrackSpline.ITrainTrackUser trainTrackUser in trainTrackSpline.trackUsers)
			{
				if (Vector3.SqrMagnitude(trainTrackUser.Position - position) < 145f)
					return false;
			}
			return true;
		}

		private Vector3 GetPositionOnFromfunction(ref TrainTrackSpline trainTrackSpline, Vector3 forward, Vector3 position, float distance, out Vector3 tangent)
		{
			float minSplineDistance;
			trainTrackSpline.GetDistance(position, 1f, out minSplineDistance);
			bool facingForward = trainTrackSpline.IsForward(-forward, minSplineDistance);
			bool isAtEndOfLine;
			float newDistance = trainTrackSpline.GetSplineDistAfterMove(minSplineDistance, distance, 0, facingForward, out trainTrackSpline, out isAtEndOfLine, null, null);
			tangent = trainTrackSpline.GetTangentCubicHermiteWorld(newDistance);
			return trainTrackSpline.GetPosition(newDistance);
		}

		public bool PlayersNearbyfunction(Vector3 _base, float Distance)
		{
			if (_base == null) { return false; }
			List<BasePlayer> obj = Pool.GetList<BasePlayer>();
			Vis.Entities(_base, Distance, obj, 131072);
			bool result = false;
			if (obj == null) { return false; }
			foreach (BasePlayer item in obj)
			{
				if (item == null) { continue; }
				if (!item.IsSleeping() && item.IsAlive() && !item.IsNpc)
				{
					result = true;
					break;
				}
			}
			Pool.FreeList(ref obj);
			return result;
		}

		private void KillListfunction(List<uint> EntIds)
		{
			if (EntIds == null) { return; }
			foreach (uint p in EntIds)
			{
				if (BaseEntity.serverEntities.entityList.Contains(p))
				{
					BaseEntity be = BaseEntity.serverEntities.entityList[p] as BaseEntity;
					if (TrainsBaseEntityList.Contains(be)) { continue; }
					if (be != null && !be.IsDestroyed) { be.transform.position = new Vector3(0, -501, 0); be.Kill(); }
				}
			}
		}

		private void ClearLootfunction(List<uint> EntIds)
		{
			if (EntIds == null) { return; }
			foreach (uint p in EntIds)
			{
				if (BaseEntity.serverEntities.entityList.Contains(p))
				{
					BaseEntity be = BaseEntity.serverEntities.entityList[p] as BaseEntity;
					if (be != null && !be.IsDestroyed)
					{
						AirPirates ap = be.GetComponent<AirPirates>();
						if (ap != null && ap.itembox != null) { ap.itembox.inventory.Clear(); }
						LandPirate lp = be.GetComponent<LandPirate>();
						if (lp != null && lp.itembox != null) { lp.itembox.Clear(); }
						WaterPirate wp = be.GetComponent<WaterPirate>();
						if (wp != null && wp.itembox != null) { wp.itembox.inventory.Clear(); }
						RailPirate rp = be.GetComponent<RailPirate>();
						if (rp != null && rp.itembox != null) { rp.itembox.inventory.Clear(); }
						BasePirate pb = be.GetComponent<BasePirate>();
						if (pb != null && pb.itembox != null)
						{
							pb.itembox.inventory.Clear(); pb.itembox.Kill();
							foreach (BaseEntity b in pb.entitys) { if (b != null && !b.IsDestroyed) { b.Kill(); } }
						}
					}
				}
			}
		}

		private void ClearAreafunction(Vector3 Location, float ProtectionRadius)
		{
			int hits = Physics.OverlapSphereNonAlloc(Location, ProtectionRadius * 1.25f, Vis.colBuffer, -1, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < hits; i++)
			{
				var e = Vis.colBuffer[i].ToBaseEntity();
				if (e is TreeEntity || e is BaseVehicle) { e.Kill(); }
				Vis.colBuffer[i] = null;
			}
		}

		private StorageContainer CreateStoragefunction(Vector3 offset, BaseEntity parent)
		{
			StorageContainer itembox = GameManager.server.CreateEntity(rhibstorageentityprefab) as StorageContainer;
			plugin.DestroyMeshCollider(itembox);
			plugin.DestroyGroundComp(itembox);
			itembox.Spawn();
			itembox.enableSaving = false;
			itembox.SetParent(parent);
			itembox.transform.localPosition = offset;
			itembox.inventory.capacity = 30;
			itembox.SendNetworkUpdateImmediate();
			return itembox;
		}

		public bool SpawnCheckfunctioon(Vector3 position, float radius, int layers)
		{
			bool flag = true;
			if (TerrainMeta.TopologyMap.GetTopology(position, TopologyBaseBlock)) { return false; }
			var colliders = Pool.GetList<Collider>();
			Vis.Colliders(position, radius, colliders, layers, QueryTriggerInteraction.Collide);
			foreach (var collider in colliders)
			{
				if (collider.name.Contains("SafeZone", CompareOptions.OrdinalIgnoreCase) || collider.name.Contains("rock_", CompareOptions.OrdinalIgnoreCase) || collider.name.Contains("modding", CompareOptions.OrdinalIgnoreCase) || collider.name.Contains("structures", CompareOptions.OrdinalIgnoreCase) || collider.name.Contains("formation_", CompareOptions.OrdinalIgnoreCase)) { flag = false; break; }
				var e = collider.ToBaseEntity();
				if (e.IsValid())
				{
					if (e is BasePlayer)
					{
						var player = e as BasePlayer;
						if (!player.IsConnected || player.IsFlying) { continue; }
						else { flag = false; break; }
					}
					if (e is JunkPile) { flag = false; break; }
					else if (e is PlayerCorpse)
					{
						var corpse = e as PlayerCorpse;
						if (corpse.playerSteamID == 0 || corpse.playerSteamID.IsSteamId()) { flag = false; break; }
					}
					else if (e is DroppedItemContainer && e.ShortPrefabName != "item_drop")
					{
						var backpack = e as DroppedItemContainer;
						if (backpack.playerSteamID == 0 || backpack.playerSteamID.IsSteamId()) { flag = false; break; }
					}
					else if (e.OwnerID == 0)
					{
						if (e is BuildingBlock) { flag = false; break; }
						else if (e is MiningQuarry) { flag = false; break; }
					}
				}
			}
			Pool.FreeList(ref colliders);
			return flag;
		}

		private void ClearUpVectorZerofunction(Vector3 pos)
		{
			List<BaseNetworkable> ScanEntitys = new List<BaseNetworkable>();
			Vis.Entities<BaseNetworkable>(pos, 5f, ScanEntitys);
			foreach (BaseNetworkable bn in ScanEntitys) { bn.Kill(); }
		}

		void AddLockfunction(BaseEntity ent)
		{
			CodeLock alock = GameManager.server.CreateEntity(codelockprefab) as CodeLock;
			alock.Spawn();
			alock.OwnerID = ent.OwnerID;
			alock.code = UnityEngine.Random.Range(1111, 9999).ToString();
			alock.SetParent(ent, ent.GetSlotAnchorName(BaseEntity.Slot.Lock));
			ent.SetSlot(BaseEntity.Slot.Lock, alock);
			alock.SetFlag(BaseEntity.Flags.Locked, true);
			alock.enableSaving = false;
			alock.SendNetworkUpdateImmediate(true);
		}

		private StabilityEntity CreateWindowfunction(Vector3 pos, Vector3 rot, BaseEntity parent)
		{
			StabilityEntity Glasswindow = GameManager.server.CreateEntity(windowprefab, parent.transform.position, Quaternion.Euler(rot)) as StabilityEntity;
			DestroyMeshCollider(Glasswindow);
			DestroyGroundComp(Glasswindow);
			Glasswindow.Spawn();
			Glasswindow.enableSaving = false;
			Glasswindow.pickup.enabled = false;
			Glasswindow.grounded = true;
			Glasswindow.SetParent(parent);
			Glasswindow.transform.localPosition = pos;
			return Glasswindow;
		}

		private StabilityEntity CreateGarageDoorfunction(Vector3 pos, Vector3 rot, BaseEntity parent, ulong skinid = 0, ulong ID = 0, float health = 400)
		{
			StabilityEntity gdoor = GameManager.server.CreateEntity(garagedoorprefab, parent.transform.position, Quaternion.Euler(rot)) as StabilityEntity;
			DestroyMeshCollider(gdoor);
			DestroyGroundComp(gdoor);
			gdoor.Spawn();
			gdoor.enableSaving = false;
			gdoor.skinID = skinid;
			gdoor.OwnerID = ID;
			gdoor.InitializeHealth(health, health);
			AddLockfunction(gdoor);
			gdoor.grounded = true;
			gdoor.pickup.enabled = false;
			gdoor.SetParent(parent);
			gdoor.transform.localPosition = pos;
			gdoor.SendNetworkUpdateImmediate(true);
			return gdoor;
		}

		private StorageContainer CreateLootSpawnerfunction(Vector3 pos, Vector3 rot, BaseEntity parent, string CreatePrefab)
		{
			StorageContainer lootHOLDER = GameManager.server.CreateEntity(CreatePrefab, parent.transform.position, Quaternion.Euler(rot)) as StorageContainer;
			if (lootHOLDER)
			{
				lootHOLDER.enableSaving = false;
				lootHOLDER.SendMessage("SetWasDropped", SendMessageOptions.DontRequireReceiver);
				lootHOLDER.Spawn();
				lootHOLDER.SetParent(parent);
				lootHOLDER.enableSaving = false;
				lootHOLDER.transform.localPosition = pos;
			}
			return lootHOLDER;
		}

		private AutoTurret CreateTurretfunction(Vector3 pos, Vector3 rot, BaseEntity parent)
		{
			AutoTurret createdturret = GameManager.server.CreateEntity(autoturretprefab, parent.transform.position, Quaternion.Euler(rot)) as AutoTurret;
			createdturret.Spawn();
			createdturret.enableSaving = false;
			createdturret.pickup.enabled = false;
			createdturret.SetParent(parent);
			createdturret.transform.localPosition = pos;
			createdturret.InitializeHealth(_ServerSettings.PirateRailWagonTurretHealth, _ServerSettings.PirateRailWagonTurretHealth);
			BaseParts.Add(createdturret.net.ID);
			createdturret.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
			NextFrame(() =>
			{
				if (createdturret.AttachedWeapon == null)
				{
					var shortname = _ServerSettings.PirateRailWagonTurretGun;
					var itemToCreate = ItemManager.FindItemDefinition(shortname);

					if (itemToCreate != null)
					{
						Item item = ItemManager.Create(itemToCreate, 1, (ulong)itemToCreate.skins.GetRandom().id);
						if (!item.MoveToContainer(createdturret.inventory, 0, false)) { item.Remove(); }
					}
				}
			});
			createdturret.Invoke(() => FillAmmoTurret(createdturret), 2.5f);
			createdturret.Invoke(createdturret.InitiateStartup, 3f);
			createdturret.inventory.onPreItemRemove += new Action<Item>(OnWeaponItemPreRemove);
			return createdturret;
		}

		private SamSite CreateSamsitefunction(Vector3 pos, Vector3 rot, BaseEntity parent)
		{
			SamSite createdturret = GameManager.server.CreateEntity(samsiteprefab, parent.transform.position, Quaternion.Euler(rot)) as SamSite;
			createdturret.Spawn();
			createdturret.enableSaving = false;
			createdturret.pickup.enabled = false;
			createdturret.SetParent(parent);
			createdturret.transform.localPosition = pos;
			createdturret.InitializeHealth(_ServerSettings.PirateRailWagonSamHealth, _ServerSettings.PirateRailWagonSamHealth);
			createdturret.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
			createdturret.vehicleScanRadius = createdturret.missileScanRadius = _ServerSettings.PirateRailWagonSamRange;
			FillAmmoSamSite(createdturret);
			createdturret.inventory.onPreItemRemove += new Action<Item>(OnWeaponItemPreRemove);
			BaseParts.Add(createdturret.net.ID);
			return createdturret;
		}

		public Color Hex2Colour(string text)
		{
			Color Colour;
			if (ColorUtility.TryParseHtmlString(text, out Colour)) { return Colour; }
			return Color.black;
		}

		private void FillAmmoTurret(AutoTurret turret)
		{
			var attachedWeaponvar = turret.GetAttachedWeapon();
			if (attachedWeaponvar == null)
			{
				turret.Invoke(() => FillAmmoTurret(turret), 0.2f);
				return;
			}
			attachedWeaponvar.primaryMagazine.contents = 999;
			attachedWeaponvar.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			turret.Invoke(turret.UpdateTotalAmmo, 0.25f);
		}

		private void FillAmmoSamSite(SamSite ss)
		{
			if (ss.ammoItem == null || !ss.HasAmmo())
			{
				Item item = ItemManager.Create(ss.ammoType, 5);

				if (!item.MoveToContainer(ss.inventory))
				{
					item.Remove();
				}
				else ss.ammoItem = item;
			}
			else if (ss.ammoItem.amount < 5) { ss.ammoItem.amount = 5; }
		}

		private void OnWeaponItemPreRemove(Item item)
		{
			var weaponvar = item.parent?.entityOwner;

			if (weaponvar is AutoTurret)
			{
				weaponvar.Invoke(() => FillAmmoTurret(weaponvar as AutoTurret), 0.1f);
			}
			else if (weaponvar is SamSite)
			{
				weaponvar.Invoke(() => FillAmmoSamSite(weaponvar as SamSite), 0.1f);
			}
		}

		private void Manuallyaddpoifunction(string[] Args, BasePlayer player)
		{
			if (Args.Length != 2)
			{
				Vector3 POI = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
				PointsOfIntrest.Add(POI);
				timer.Once(_ServerSettings.POIExpires, () => { PointsOfIntrest.Remove(POI); });
				if (player != null) { player.ChatMessage("Set a point of intrest at your location."); }
			}
			else if (Args.Length == 2)
			{
				string sVector = Args[1];
				if (sVector.StartsWith("(") && sVector.EndsWith(")")) { sVector = sVector.Substring(1, sVector.Length - 2); }
				string[] sArray = sVector.Split(',');
				Vector3 result = new Vector3(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]));
				PointsOfIntrest.Add(result);
				timer.Once(_ServerSettings.POIExpires, () => { PointsOfIntrest.Remove(result); });
				if (player != null) { player.ChatMessage("Set a point of intrest at " + result.ToString()); }
			}
		}

		private void PirateAttackedfunction(BaseCombatEntity bce, BasePlayer basePlayer)
		{
			if (bce == null || basePlayer == null) { return; }
			AirPirates ap = bce.GetComponent<AirPirates>();
			if (ap != null) { ap._targetList.Add(new targetinfo(basePlayer, basePlayer)); return; }
			LandPirate lp = bce.GetComponent<LandPirate>();
			if (lp != null) { lp.PlayerTarget = basePlayer; return; }
			WaterPirate wp = bce.GetComponent<WaterPirate>();
			if (wp != null) { if (TerrainMeta.HeightMap.GetHeight(basePlayer.transform.position) < -2f) { wp.PlayerTarget = basePlayer; } return; }
		}

		private bool BaseDamagefunction(BaseCombatEntity entity)
		{
			if (entity is Door) return true;
			if (!(entity is BuildingBlock)) return false;
			return ((BuildingBlock)entity).grade != BuildingGrade.Enum.Twigs;
		}

		private void SamLogicfunction(SamSite samSite, BaseEntity be, Vector3 currentVelocity, float currentspeed)
		{
			try
			{
				if (!samSite.HasAmmo() || currentVelocity == null || be == null) { return; }
				if (be.HasFlag(BaseEntity.Flags.On) || be.HasFlag(BaseEntity.Flags.Reserved2))
				{
					Vector3 estimatedPoint = PredictedPosfunction(be, samSite, currentVelocity);
					estimatedPoint += be.transform.forward * (currentspeed + 3f);
					samSite.currentAimDir = (estimatedPoint - samSite.eyePoint.transform.position).normalized;
					Vector3 vector = Quaternion.LookRotation(samSite.currentAimDir, samSite.transform.up).eulerAngles;
					vector = BaseMountable.ConvertVector(vector);
					float t = Mathf.InverseLerp(0f, 90f, -vector.x);
					float z = Mathf.Lerp(15f, -75f, t);
					Quaternion localRotationq = Quaternion.Euler(0f, vector.y, 0f);
					samSite.yaw.transform.localRotation = localRotationq;
					Quaternion localRotation2 = Quaternion.Euler(samSite.pitch.transform.localRotation.eulerAngles.x, samSite.pitch.transform.localRotation.eulerAngles.y, z);
					samSite.pitch.transform.localRotation = localRotation2;
					samSite.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
					FireProjectilefunction(samSite, samSite.currentAimDir, 1);
					Effect.server.Run(samSite.muzzleFlashTest.resourcePath, samSite, StringPool.Get("Tube " + (samSite.currentTubeIndex + 1).ToString()), Vector3.zero, Vector3.up, null, false);
					samSite.ammoItem.UseItem(1);
					samSite.currentTubeIndex++;
					if (samSite.currentTubeIndex >= samSite.tubes.Length) { samSite.currentTubeIndex = 0; }
				}
			}
			catch { }
		}

		private void FireProjectilefunction(SamSite origin, Vector3 direction, float speedMultiplier)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(origin.projectileTest.resourcePath, origin.tubes[origin.currentTubeIndex].position, Quaternion.LookRotation(direction, Vector3.up));
			if (!(baseEntity == null))
			{
				baseEntity.creatorEntity = origin;
				ServerProjectile component = baseEntity.GetComponent<ServerProjectile>();
				TimedExplosive rocket = baseEntity.GetComponent<TimedExplosive>();
				rocket.SetDamageScale(_ServerSettings.SamSitesDamageFloat);
				if ((bool)component) { component.InitializeVelocity(origin.GetInheritedProjectileVelocity() + direction * component.speed * speedMultiplier); }
				baseEntity.Spawn();
				baseEntity.enableSaving = false;
			}
		}

		private Vector3 PredictedPosfunction(BaseEntity target, SamSite samSite, Vector3 targetVelocity)
		{
			Vector3 targetposv = target.transform.TransformPoint(target.transform.GetBounds().center);
			Vector3 displacement0 = targetposv - samSite.eyePoint.transform.position;
			float projectileSpeed = samSite.projectileTest.Get().GetComponent<ServerProjectile>().speed;
			float targetMoveAngle = Vector3.Angle(-displacement0, targetVelocity) * Mathf.Deg2Rad;
			if (targetVelocity.magnitude == 0 || targetVelocity.magnitude > projectileSpeed && Mathf.Sin(targetMoveAngle) / projectileSpeed > Mathf.Cos(targetMoveAngle) / targetVelocity.magnitude) { return targetposv; }
			float shootAngle = Mathf.Asin(Mathf.Sin(targetMoveAngle) * targetVelocity.magnitude / projectileSpeed);
			return targetposv + targetVelocity * displacement0.magnitude / Mathf.Sin(Mathf.PI - targetMoveAngle - shootAngle) * Mathf.Sin(shootAngle) / targetVelocity.magnitude;
		}

		private List<BaseEntity> CreateWallsfunction(Vector3 center, float radius)
		{
			List<BaseEntity> WallsList = new List<BaseEntity>();
			string prefabstring = StringPool.Get(1745077396);
			bool gate = true;
			float maxHeightfloat = -499f;
			float minHeightfloat = 999f;
			int raycasts = Mathf.CeilToInt(360 / radius * 0.1375f);
			foreach (var position in GetCircumferencePositions(center, radius, raycasts, false))
			{
				maxHeightfloat = Mathf.Max(position.y, maxHeightfloat, TerrainMeta.WaterMap.GetHeight(position));
				minHeightfloat = Mathf.Min(position.y, minHeightfloat);
				center.y = minHeightfloat;
			}
			float gap = 0.10f;
			float next = 360 / radius - gap;
			float j = 1 * 6f + 6f;
			float groundHeightFLOAT = 0f;
			BaseEntity e;
			List<Vector3> wallpos = GetCircumferencePositions(center, radius, next, false, center.y);
			int halfway = (int)(wallpos.Count / 2);
			int count = 0;
			foreach (var position in wallpos)
			{
				if (center.y - position.y > 48f) { continue; }
				Quaternion Rotation = Quaternion.Euler(0, 180, 0);
				groundHeightFLOAT = TerrainMeta.HeightMap.GetHeight(new Vector3(position.x, position.y + 6f, position.z));
				if (groundHeightFLOAT > position.y + 9f) { continue; }
				if (position.y - groundHeightFLOAT > j && position.y < TerrainMeta.HeightMap.GetHeight(position)) { continue; }
				if (!gate && count != halfway) { e = GameManager.server.CreateEntity(prefabstring, new Vector3(position.x, TerrainMeta.HeightMap.GetHeight(position) - 0.2f, position.z), default(Quaternion), false); }
				else { e = GameManager.server.CreateEntity(StringPool.Get(95147612), new Vector3(position.x, TerrainMeta.HeightMap.GetHeight(position) - 0.25f, position.z), default(Quaternion), false); gate = false; Rotation = Quaternion.Euler(0, 0, 0); }
				if (e == null) { continue; }
				e.OwnerID = (ulong)UnityEngine.Random.Range(10000000, 20000000);
				e.transform.LookAt(center, Vector3.up);
				e.transform.rotation *= Rotation;
				e.enableSaving = false;
				e.Spawn();
				e.enableSaving = false;
				WallsList.Add(e);
				e.gameObject.SetActive(true);
				count++;
			}
			return WallsList;
		}

		private List<byte[]> ArrayToByteArrayList(byte[] bytes, int offset = 0)
		{
			List<byte[]> packetbytes = new List<byte[]>();
			List<int> packetchecksum = new List<int>();
			for (int checksum = 0; checksum < bytes.Length - offset;)
			{
				packetchecksum.Add(BitConverter.ToInt32(bytes, offset));
				offset += 4;
				checksum = packetchecksum.Sum();
				if (checksum > bytes.Length - offset)
				{
					Puts("Unpacking Stream Failed!");
					return null;
				}
			}
			foreach (int size in packetchecksum) { packetbytes.Add(bytes.Skip(offset).Take(size).ToArray()); offset += size; }
			return packetbytes;
		}

		public bool IsBase64String(string s) { s = s.Trim(); return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None); }

		public void WebPlayback(string URL, BasePlayer Target)
		{
			if (URL == "" || Target == null || Target.net == null) { return; }
			uint ID = Target.net.ID;
			if (!GroundedNPCs.Contains(ID) && !NPCsUintList.Contains(ID)) { return; }
			if (Target.transform.position.y > TerrainMeta.HeightMap.GetHeight(Target.transform.position) + 20f) { return; }
			foreach (Connection c in BaseNetworkable.GetConnectionsWithin(Target.transform.position, 50))
			{
				BasePlayer p = c.player as BasePlayer;
				if (p != null) { if (GamePhysics.LineOfSight(p.eyes.transform.position, Target.transform.position + new Vector3(0, -4, 0), -1)) { return; } }
			}
			Vector3 backuppos = new Vector3(Target.transform.position.x, Target.transform.position.y, Target.transform.position.z);
			webrequest.Enqueue(URL, null, (code, response) =>
			{
				if (code != 200 || response == null) { Puts($"Error: {code}"); return; }
				if (IsBase64String(response))
				{
					List<byte[]> VD = ArrayToByteArrayList(Convert.FromBase64String(response));
					Vector3 GroundPos = backuppos + new Vector3(0, -4, 0);
					BasePlayer newPlayer = GameManager.server.CreateEntity(playerentityprefab, GroundPos).ToPlayer();
					newPlayer.Spawn();
					newPlayer.enableSaving = false;
					Interface.Oxide.CallHook("OnNPCRespawn", newPlayer);
					CreateCloths(newPlayer, "burlap.gloves", 2488609451);
					CreateCloths(newPlayer, "hoodie", 2488607577);
					CreateCloths(newPlayer, "pants", 2488608085);
					CreateCloths(newPlayer, "shoes.boots", 2488606012);
					newPlayer.modelState.ducked = true;
					if (Target != null && newPlayer != null) { newPlayer.SetParent(Target, true, true); }
					newPlayer.transform.localScale = Vector3.zero;
					BaseEntity.Query.Server.RemovePlayer(newPlayer);
					uint IDnum = newPlayer.net.ID;
					VoiceUintList.Add(IDnum);
					timer.Once((VD.Count / 10) + 4f, () => { VoiceUintList.Remove(IDnum); newPlayer.Kill(); });
					NextFrame(() => { Threads.Add(InvokeHandler.Instance.StartCoroutine(PlayVoicefunction(VD, newPlayer))); });
				}
				else { Puts("WebVoice URL Fault!"); }
			}, this, RequestMethod.GET);
		}

		private IEnumerator PlayVoicefunction(List<byte[]> VD, BasePlayer bot)
		{
			if (VD == null || bot == null) yield break;
			foreach (byte[] data in VD)
			{
				if (Network.Net.sv.write.Start())
				{
					Network.Net.sv.write.PacketID(Message.Type.VoiceData);
					Network.Net.sv.write.UInt32(bot.net.ID);
					Network.Net.sv.write.BytesWithSize(data);
					Network.Net.sv.write.Send(new SendInfo(BaseNetworkable.GetConnectionsWithin(bot.transform.position, 100)) { priority = Priority.Immediate });
				}
				yield return new WaitForSeconds(0.1f);
			}
		}

		private IEnumerator MapRoutine()
		{
			do
			{
				try { DrawMapfunction(); } catch { }
				yield return CoroutineEx.waitForSeconds(30);
			} while (true);
		}

		private IEnumerator Playpacket(byte[] VD, Vector3 position, BasePlayer newPlayer)
		{
			if (Network.Net.sv.write.Start())
			{
				Network.Net.sv.write.PacketID(Message.Type.VoiceData);
				Network.Net.sv.write.UInt32(newPlayer.net.ID);
				Network.Net.sv.write.BytesWithSize(VD);
				Network.Net.sv.write.Send(new SendInfo(global::BaseNetworkable.GetConnectionsWithin(position, 100f)) { priority = Priority.Immediate });
			}
			yield return new WaitForSeconds(0.01f);
		}

		private void MarkerDisplayingDelete()
		{
			foreach (var m in mapmarkersList)
			{
				if (m != null && !m.IsDestroyed)
				{
					m.Kill();
					m.SendUpdate();
				}
			}
			mapmarkersList.Clear();
		}

		private void DrawMapfunction()
		{
			MarkerDisplayingDelete();
			if (PirateUintList.Count == 0) { return; }
			foreach (uint p in PirateUintList.ToArray())
			{
				try
				{
					if (BaseNetworkable.serverEntities.entityList.Contains(p))
					{
						BaseEntity be = BaseNetworkable.serverEntities.entityList[p] as BaseEntity;
						if (be != null && !be.IsDestroyed)
						{
							MapMarkerGenericRadius MapMarkerCustom;
							MapMarkerCustom = GameManager.server.CreateEntity(mapmarkerentityprefab) as MapMarkerGenericRadius;
							MapMarkerCustom.alpha = _ServerSettings.MapMarkerAlpha;
							MapMarkerCustom.radius = _ServerSettings.MapMarkerSize;
							if (be.HasComponent<AirPirates>())
							{
								MapMarkerCustom.color1 = _ServerSettings.AirMapMarkerColour;
								MapMarkerCustom.color2 = _ServerSettings.AirMapMarkerColour;
							}
							else if (be.HasComponent<WaterPirate>())
							{
								MapMarkerCustom.color1 = _ServerSettings.WaterMapMarkerColour;
								MapMarkerCustom.color2 = _ServerSettings.WaterMapMarkerColour;
							}
							else if (be.HasComponent<LandPirate>())
							{
								MapMarkerCustom.color1 = _ServerSettings.LandMapMarkerColour;
								MapMarkerCustom.color2 = _ServerSettings.LandMapMarkerColour;
							}
							else if (be.HasComponent<LandPirate>())
							{
								MapMarkerCustom.color1 = _ServerSettings.LandMapMarkerColour;
								MapMarkerCustom.color2 = _ServerSettings.LandMapMarkerColour;
							}
							else if (be.HasComponent<BasePirate>())
							{
								MapMarkerCustom.color1 = _ServerSettings.BaseMapMarkerColour;
								MapMarkerCustom.color2 = _ServerSettings.BaseMapMarkerColour;
							}
							else if (be.HasComponent<RailPirate>())
							{
								MapMarkerCustom.color1 = _ServerSettings.RailMapMarkerColour;
								MapMarkerCustom.color2 = _ServerSettings.RailMapMarkerColour;
							}
							else if (be.HasComponent<FootPirate>())
							{
								MapMarkerCustom.color1 = Color.white;
								MapMarkerCustom.color2 = Color.white;
								MapMarkerCustom.alpha = 1;
								MapMarkerCustom.radius *= 0.5f;
							}

							MapMarkerCustom.SetParent(be, false, true);
							if (MapMarkerCustom.transform.position != new Vector3(0, 0, 0)) { mapmarkersList.Add(MapMarkerCustom); }
						}
					}
				}
				catch { }
			}
			foreach (var m in mapmarkersList)
			{
				m.Spawn();
				MapMarker.serverMapMarkers.Remove(m);
				m.SendUpdate();
			}
		}

		private void BuildLootTablefunction()
		{
			_ServerSettings.LootProfiles = plugin.Config["XLootCustomProfiles"].ToString();
			if (_ServerSettings.LootProfiles == "") { Puts("No Loot Table"); return; }
			loottableDict.Clear();
			try
			{
				if (_ServerSettings.LootProfiles.Contains("<Profile>"))
				{
					string[] profiles = _ServerSettings.LootProfiles.Split(new string[] { "<Profile>" }, System.StringSplitOptions.RemoveEmptyEntries);
					foreach (string p in profiles)
					{
						string[] items = p.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries);
						var profiledata = new List<Dictionary<string, string>>();
						foreach (string ent in items)
						{
							Dictionary<string, string> data = new Dictionary<string, string>();
							string[] iteminfo = ent.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
							if (iteminfo.Length < 2) { continue; }
							data.Add(iteminfo[0], ent.Replace(iteminfo[0] + ",", ""));
							profiledata.Add(data);
						}
						loottableDict.Add(profiledata);
					}
					Puts("Rebuilt Loot Table, " + loottableDict.Count.ToString() + " profiles loaded");
					return;
				}
			}
			catch { }
			Puts("Invalid Loot Table");
			return;
		}

		private void LootContainerfunction(BasePlayer playerVAR, ItemContainer containerVAR)
		{
			playerVAR.inventory.loot.Clear();
			playerVAR.inventory.loot.PositionChecks = false;
			playerVAR.inventory.loot.entitySource = containerVAR.entityOwner ?? playerVAR;
			playerVAR.inventory.loot.itemSource = null;
			playerVAR.inventory.loot.MarkDirty();
			playerVAR.inventory.loot.AddContainer(containerVAR);
			playerVAR.inventory.loot.SendImmediate();
			playerVAR.ClientRPCPlayer(null, playerVAR, "RPC_OpenLootPanel", "generic_resizable");
		}

		private void filllootfunction(ItemContainer itemb, int selectINT, int multi, string CreatePrefab = "")
		{
			int selectionINT = selectINT;
			if (itemb == null) { return; }
			if (multi == 0)
			{
				if (loottableDict == null || loottableDict.Count == 0) { return; }
				if (selectionINT > loottableDict.Count) { return; }
				foreach (Dictionary<string, string> i in loottableDict[selectionINT].ToArray())
				{
					foreach (KeyValuePair<string, string> it in i.ToArray())
					{
						try
						{
							string[] iteminfo = it.Value.Split(',');
							Item item = ItemManager.CreateByName(it.Key, int.Parse(iteminfo[0]), ulong.Parse(iteminfo[1]));
							if (item == null) { continue; }
							if (!item.MoveToContainer(itemb, -1, false)) { item.Remove(); }
						}
						catch { }
					}
				}
			}

			for (int m = 0; m <= multi; m++)
			{
				if (selectINT == -1) { selectionINT = Core.Random.Range(0, plugin.loottableDict.Count); }
				if (CreatePrefab == "")
				{
					if (loottableDict.Count == 0) { return; }
					foreach (Dictionary<string, string> i in loottableDict[selectionINT].ToArray())
					{
						foreach (KeyValuePair<string, string> it in i.ToArray())
						{
							try
							{
								string[] iteminfo = it.Value.Split(',');
								Item item = ItemManager.CreateByName(it.Key, int.Parse(iteminfo[0]), ulong.Parse(iteminfo[1]));
								if (item == null) { continue; }
								if (!item.MoveToContainer(itemb, -1, false)) { item.Remove(); }
							}
							catch { }
						}
					}
				}
				else
				{
					StorageContainer baseEntity = GameManager.server.CreateEntity(CreatePrefab) as StorageContainer;
					if (baseEntity != null && itemb != null)
					{
						baseEntity.enableSaving = false;
						baseEntity.SendMessage("SetWasDropped", SendMessageOptions.DontRequireReceiver);
						baseEntity.Spawn();
						baseEntity.enableSaving = false;
						foreach (Item i in baseEntity.inventory.itemList.ToList()) { if (i != null) { i.MoveToContainer(itemb); } }
						NextFrame(() => { if (baseEntity != null && !baseEntity.IsDestroyed) { baseEntity.Kill(); } });
					}
				}
			}
		}

		private void savelootfunction()
		{
			string newloottable = "";
			if (loottableDict.Count != 0)
			{
				foreach (List<Dictionary<string, string>> profiles in loottableDict)
				{
					newloottable += "<Profile>";
					foreach (Dictionary<string, string> i in profiles) { foreach (KeyValuePair<string, string> it in i) { newloottable += it.Key + "," + it.Value + "|"; } }
				}
				Config["XLootCustomProfiles"] = newloottable;
				NextFrame(() => { Config.Save(); });
				timer.Once(1f, () => { BuildLootTablefunction(); });
			}
		}

		private bool IsKitbool(string kit)
		{
			var success = Kits?.Call("isKit", kit);
			if (success == null || !(success is bool)) { return false; }
			return (bool)success;
		}

		private void GiveKitfunction(NPCPlayer npc, string NPCKit)
		{
			if (Kits == null) { return; };
			if (NPCKit == "" || !IsKitbool(NPCKit)) { return; };
			object success = Kits?.Call("GiveKit", npc, NPCKit);
			if (success == null || !(success is bool))
			{
				Puts("Failed to give NPC Kit");
				return;
			}
			Item projectileItem = null;
			foreach (var item in npc.inventory.containerBelt.itemList.ToList())
			{
				if (item.GetHeldEntity() is BaseProjectile)
				{
					projectileItem = item;
					break;
				}
				if (item.GetHeldEntity() is MedicalTool)
				{
					item.MoveToContainer(npc.inventory.containerMain);
					continue;
				}
			}
			if (projectileItem == null)
			{
				foreach (var item in npc.inventory.containerBelt.itemList.ToList())
				{
					if (item.GetHeldEntity() is BaseMelee)
					{
						projectileItem = item;
						break;
					}
				}
			}
			if (projectileItem != null)
			{
				npc.UpdateActiveItem(projectileItem.uid);
				npc.inventory.UpdatedVisibleHolsteredItems();
				timer.Once(1f, () => { npc.AttemptReload(); });
			}
		}

		private void BailOutfunction(Vector3 pos, ScientistNPC npc, Vector3 home, int radius, float distance = 0, bool die = true)
		{
			if (npc != null)
			{
				NPCsUintList.Remove(npc.net.ID);
				npc.EnsureDismounted();
				npc.SetParent(null, true, true);
				npc.transform.position = pos;
				if (!GroundedNPCs.Contains(npc.net.ID))
				{
					float d = radius;
					Vector2 vector = Vector3.zero;
					Vector3 newPos = Vector3.zero;
					for (int scan = 0; scan < 50; scan++)
					{
						vector = UnityEngine.Random.insideUnitCircle * d;
						newPos = pos + new Vector3(vector.x, 0, vector.y);
						if (Vector3.Distance(newPos, pos) < distance) { continue; }
						if (TerrainMeta.HeightMap.GetHeight(newPos) > -0.4f) { break; }
					}
					npc.modelState.mounted = false;
					if (die) { plugin.timer.Once(_ServerSettings.NPCParachuteDie, () => { if (npc != null && npc.IsAlive() && !npc.IsDestroyed) { GroundedNPCs.Remove(npc.net.ID); npc.Kill(); } }); }
					newPos.y = TerrainMeta.HeightMap.GetHeight(newPos) + 4f;
					npc.transform.position = newPos;
					var rb = npc.gameObject.GetComponent<Rigidbody>();
					if (rb != null)
					{
						rb.drag = 0f;
						rb.useGravity = false;
						rb.isKinematic = false;
						rb.velocity = new Vector3(npc.transform.forward.x * 0, 0, npc.transform.forward.z * 0) - new Vector3(0, 2, 0);
					}
					npc.SendNetworkUpdateImmediate();
					plugin.NextFrame(() => { if (npc != null) { GroundCheckfunction(npc, home); } });
				}
			}
		}

		private void AttachParachute(ScientistNPC npc, Vector3 pos)
		{
			if (npc == null || npc.IsDead()) { return; }
			if (TerrainMeta.HeightMap.GetHeight(npc.transform.position) < -0.4f) { NPCsUintList.Remove(npc.net.ID); ActiveNPCs.Remove(npc); npc.Die(); }
			npc.transform.localPosition = Vector3.zero;
			npc.transform.localRotation = Quaternion.Euler(Vector3.zero);
			npc.SetParent(null, true, true);
			npc.transform.position = pos;
			npc.transform.rotation = Quaternion.Euler(0, 0, 0);
			npc.SendNetworkUpdateImmediate();
			var rb = npc.gameObject.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.drag = 0f;
				rb.useGravity = false;
				rb.isKinematic = false;
				rb.velocity = new Vector3(npc.transform.forward.x * 0, 0, npc.transform.forward.z * 0) - new Vector3(0, 5, 0);
			}
			timer.Once(1f, () =>
			{
				if (npc == null) { return; }
				timer.Once(_ServerSettings.NPCParachuteDie, () => { if (npc == null) { return; } if (npc.IsAlive() && !npc.IsDestroyed) { GroundedNPCs.Remove(npc.net.ID); npc.Kill(); } });
				var Chute = GameManager.server.CreateEntity(parachuteentityprefab, npc.transform.position, Quaternion.Euler(0, 0, 0));
				if (Chute != null)
				{
					Chute.SetParent(npc, 0, true, true);
					Vector3 movepos = Vector3.zero;
					movepos += npc.transform.forward * -0.10f;
					movepos += npc.transform.up * 1.5f;
					Chute.transform.localPosition = movepos;
					Chute.transform.rotation = Quaternion.FromToRotation(Chute.transform.up, Vector3.up) * Chute.transform.rotation;
					Chute.SendNetworkUpdateImmediate();
					DestroyMeshCollider(Chute);
					Chute.Spawn();
					Chute.enableSaving = false;
					timer.Once(_ServerSettings.NPCParachuteDecay, () =>
					{
						if (Chute != null)
						{
							Chute.SetParent(null, true, true);
							Effect.server.Run(groundfallentityprefab, Chute.transform.position);
							NextFrame(() => { Chute.Kill(); });
						}
					});
					NextFrame(() => { GroundCheckfunction(npc, Vector3.zero, Chute); });
				}
			});
		}

		private bool collidersbool(Vector3 position)
		{
			List<BuildingBlock> obj = Pool.GetList<BuildingBlock>();
			Vis.Entities(position, 10, obj, -1);
			if (obj != null && obj.Count != 0) { return true; }
			return false;
		}

		public void GroundCheckfunction(ScientistNPC npc, Vector3 home, BaseEntity Chute = null)
		{
			if (npc == null || npc.IsDead() || npc.IsDestroyed) { if (Chute != null) { Chute.SetParent(null, true, true); Chute.Kill(); } return; }
			if (npc.transform.position.y > TerrainMeta.HeightMap.GetHeight(npc.transform.position) + 8f)
			{
				timer.Once(0.1f, () => { if (npc != null) { GroundCheckfunction(npc, home, Chute); } });
				return;
			}
			if (collidersbool(npc.transform.position)) { npc.Die(); }
			RaycastHit raycastHit;
			if (Physics.SphereCast(npc.transform.position, 3f, Vector3.down, out raycastHit, 5f, -1) || Physics.SphereCast(npc.transform.position, 3f, Vector3.up, out raycastHit, 5f, -1))
			{
				var rb = npc.gameObject.GetComponent<Rigidbody>();
				if (rb != null)
				{
					rb.isKinematic = true;
					rb.useGravity = false;
					npc.gameObject.layer = 17;
					if (Chute != null)
					{
						Chute.SetParent(null, true, true);
						Effect.server.Run(groundfallentityprefab, Chute.transform.position);
						NextFrame(() => { Chute.Kill(); });
					}
				}
				npc.transform.position = raycastHit.point;
				npc.Brain.Navigator.MaxRoamDistanceFromHome = _ServerSettings.PirateFootNPCRoam;
				npc.Brain.Navigator.BestMovementPointMaxDistance = 30f;
				if (home != Vector3.zero)
				{
					npc.Brain.Events.Memory.Position.Set(home, 4);
				}
				else
				{
					npc.Brain.Events.Memory.Position.Set(npc.transform.position, 4);
				}
				npc.Brain.Navigator.BestRoamPointMaxDistance = 40f;
				npc.Brain.Navigator.DefaultArea = "Walkable";
				npc.Brain.Navigator.Agent.agentTypeID = -1372625422;
				npc.Brain.Navigator.MaxWaterDepth = 1f;
				npc.Brain.Navigator.CanUseNavMesh = true;
				npc.Brain.Navigator.Init(npc, npc.Brain.Navigator.Agent);
				npc.Brain.Navigator.SetCurrentNavigationType(BaseNavigator.NavigationType.None);
				npc.SendNetworkUpdateImmediate();
				NPCsUintList.Remove(npc.net.ID);
				GroundedNPCs.Add(npc.net.ID);
				return;
			}
			if (npc.transform.position.y + 0.5f < TerrainMeta.HeightMap.GetHeight(npc.transform.position))
			{
				Vector2 vector = UnityEngine.Random.insideUnitCircle * 4;
				Vector3 newpos = npc.transform.position + new Vector3(vector.x, 0, vector.y);
				newpos.y = TerrainMeta.HeightMap.GetHeight(newpos);
				npc.transform.position = newpos;
				npc.TransformChanged();
				npc.Brain.Navigator.CanUseNavMesh = false;
				var rb = npc.gameObject.GetComponent<Rigidbody>();
				if (rb != null)
				{
					rb.isKinematic = true;
					rb.useGravity = false;
					rb.velocity = new Vector3(npc.transform.forward.x * 0, 0, npc.transform.forward.z * 0) - new Vector3(0, 0, 0);
					npc.gameObject.layer = 17;
				}
				return;
			}
			if (npc.transform.position.y < -1f) { npc.Kill(); return; }
			timer.Once(0.1f, () => { if (npc != null) { GroundCheckfunction(npc, home, Chute); } });
		}

		private void Explodefunction(BaseEntity be)
		{
			if (be == null) { return; }
			plugin.RunEffect(c4_explosionentityprefab, be, be.transform.position);
			plugin.RunEffect(damage_effect_debrisprefab, be, be.transform.position);
			BasePirate pb = be.GetComponent<BasePirate>();
			if (pb != null) { pb.Die(); }
			AirPirates ap = be.GetComponent<AirPirates>();
			if (ap != null) { DropLootfunction(ap.npcs, ap.itembox); }
			WaterPirate wp = be.GetComponent<WaterPirate>();
			if (wp != null) { DropLootfunction(wp.npcs, wp.itembox); }
			RailPirate rp = be.GetComponent<RailPirate>();
			if (rp != null) { DropLootfunction(rp.npcs, rp.itembox); KillWagonsfunction(rp.wagons.ToList()); }
			foreach (uint id in NPCsUintList.ToArray()) { if (!BaseEntity.serverEntities.Contains(id)) { NPCsUintList.Remove(id); } }
			NextFrame(() => { if (!be.IsDestroyed) { be.AdminKill(); } });
		}

		private void DropLootfunction(List<ScientistNPC> npcs, StorageContainer itembox)
		{
			if (npcs != null) { foreach (ScientistNPC npc in npcs) { if (npc != null && !npc.IsDestroyed && !npc.IsDead()) npc.Die(); } }
			if (itembox != null) { itembox.dropFloats = true; itembox.DropItems(); }
		}

		private NightLight CreateLightfunction(BaseEntity be, Vector3 LocationPos, Vector3 Rot)
		{
			SphereEntity sph = (SphereEntity)GameManager.server.CreateEntity(sphereentityprefab, be.transform.position, new Quaternion(0, 0, 0, 0), true);
			plugin.DestroyMeshCollider(sph);
			plugin.DestroyGroundComp(sph);
			sph.Spawn();
			sph.SetParent(be);
			sph.transform.localPosition = new Vector3(0, -150, 0);
			SearchLight searchLightE0 = GameManager.server.CreateEntity(searchlightentityprefab, sph.transform.position) as SearchLight;
			plugin.DestroyMeshCollider(searchLightE0);
			plugin.DestroyGroundComp(searchLightE0);
			searchLightE0.Spawn();
			searchLightE0.SetFlag(BaseEntity.Flags.Reserved5, true, false, true);
			searchLightE0.SetFlag(BaseEntity.Flags.Busy, true);
			searchLightE0.SetParent(sph);
			searchLightE0.transform.localPosition = new Vector3(0, 0, 0);
			searchLightE0.transform.localRotation = Quaternion.Euler(Rot);
			searchLightE0.pickup.enabled = false;
			searchLightE0.SendNetworkUpdate();
			searchLightE0.enableSaving = false;
			sph.transform.localScale += new Vector3(0.9f, 0, 0);
			sph.LerpRadiusTo(0.01f, 10f);
			searchLightE0.Invoke(() => { if (sph != null) { sph.transform.localPosition = LocationPos; sph.SendNetworkUpdateImmediate(); } }, 3f);
			NightLight nl = new NightLight();
			nl._light = searchLightE0;
			nl.resizer = sph;
			return nl;
		}

		private List<ScientistNPC> CreateNPCs(int NPCAmountint, int Type, float NPCHealth, string Kit, float AIShootMultiplyer, BaseEntity baseEntity, List<Vector3> Seats)
		{
			List<ScientistNPC> npcs = new List<ScientistNPC>();
			foreach (Vector3 position in Seats)
			{
				if (npcs.Count >= NPCAmountint) { return npcs; }
				var prefabName = StringPool.Get(3763080634);
				var prefab = GameManager.server.FindPrefab(prefabName);
				var go = Facepunch.Instantiate.GameObject(prefab);
				go.SetActive(false);
				go.name = prefabName;
				go.transform.position = new Vector3(0, 3, 0);
				ScientistNPC npc = go.GetComponent<ScientistNPC>();
				SceneManager.MoveGameObjectToScene(go, Rust.Server.EntityScene);
				npc.userID = (ulong)UnityEngine.Random.Range(0, 10000000);
				npc.UserIDString = npc.userID.ToString();
				npc.displayName = _ServerSettings.NPCNameTag + RandomUsernames.Get(npc.userID);
				npc.startHealth = NPCHealth;
				ScientistBrain brain = npc.GetComponent<ScientistBrain>();
				if (brain != null) { brain.AttackRangeMultiplier = AIShootMultiplyer; brain.SenseRange = 100; brain.CheckVisionCone = false; brain.CheckLOS = false; }
				go.SetActive(true);
				npc.Spawn();
				if (npc == null) { continue; }
				npcs.Add(npc);
				NPCsUintList.Add(npc.net.ID);
				npc.enableSaving = false;
				npc.isSpawned = false;
				timer.Once(5, () => { if (npc != null) { npc.isSpawned = true; } });
				npc.inventory.Strip();
				npc.InitializeHealth(NPCHealth, NPCHealth);
				npc.aimConeScale *= _ServerSettings.NPCAimScaler;
				npc.damageScale *= _ServerSettings.NPCDamageScaler;
				if (baseEntity != null) { npc.transform.position = baseEntity.transform.position; }
				BaseNavigator baseNavigator = npc.GetComponent<BaseNavigator>();
				if (baseNavigator != null) { baseNavigator.CanUseNavMesh = false; }
				switch (Type)
				{
					case 0:
						if (npcs.Count >= 4 && baseEntity != null) { NPCSit(npc, baseEntity); }
						plugin.timer.Once(Core.Random.Range(1, 3), () => { DressNPC(npc, Kit, "jumpsuit.suit.blue", true, false); });
						break;
					case 1:
						if ((npcs.Count == 0 || npcs.Count == 1) && baseEntity != null) { NPCSit(npc, baseEntity); }
						plugin.timer.Once(Core.Random.Range(1, 3), () => { DressNPC(npc, Kit, "jumpsuit.suit.blue", true, false); });
						break;
					case 2:
						if (baseEntity != null) { NPCSit(npc, baseEntity); }
						plugin.timer.Once(Core.Random.Range(1, 3), () => { DressNPC(npc, Kit, "jumpsuit.suit", true, false); });
						break;
					case 3:
						if ((npcs.Count == 0 || npcs.Count == 1 || npcs.Count >= 8) && baseEntity != null) { NPCSit(npc, baseEntity); }
						plugin.timer.Once(Core.Random.Range(1, 3), () => { DressNPC(npc, Kit, "jumpsuit.suit", true, false); });
						break;
					case 4:
						plugin.timer.Once(Core.Random.Range(1, 3), () => { DressNPC(npc, Kit, "attire.ninja.suit", false, true); });
						break;
					case 5:
						plugin.timer.Once(Core.Random.Range(1, 3), () => { DressNPC(npc, Kit, "hazmatsuit_scientist_arctic", false, false); });
						break;
					case 7:
						plugin.timer.Once(Core.Random.Range(1, 3), () => { DressNPC(npc, Kit, "hazmatsuit.nomadsuit", false, false); });
						break;
					case 8:
						plugin.timer.Once(Core.Random.Range(1, 3), () => { DressNPC(npc, Kit, "attire.banditguard", false, false, true); });
						break;
					case 9:
						plugin.timer.Once(Core.Random.Range(1, 3), () => { DressNPC(npc, Kit, "attire.banditguard", false, false, true); });
						break;
					case 10:
						plugin.timer.Once(Core.Random.Range(1, 3), () => { DressNPC(npc, Kit, "scientistsuit_heavy", false, false, false); });
						break;
				}
				plugin.NextFrame(() =>
				{
					if (npc == null) { return; }
					if (baseEntity != null)
					{
						npc.SetParent(baseEntity);
						npc.transform.localPosition = position;
					}
					else
					{
						npc.transform.localPosition = new Vector3(0, 0, 0);
						npc.transform.position = position;
						npc.SendNetworkUpdateImmediate();
						plugin.timer.Once(_ServerSettings.LeaveAfter + 30, () => { if (npc != null) { DieInPeace(npc); } });
					}
					ActiveNPCs.Add(npc, null);
				});
			}
			return npcs;
		}

		private void DieInPeace(ScientistNPC npc)
		{
			if (npc == null) { return; }
			BaseEntity target = ActiveNPCs[npc];
			if (PlayersNearbyfunction(npc.transform.position, 40) || target != null)
			{
				timer.Once(60, () => { if (npc != null) { DieInPeace(npc); } });
				return;
			}
			npc.Kill();
		}

		private void DressNPC(HumanNPC npc, string Kit, string outfit, bool headset = false, bool assassin = false, bool mask = false)
		{
			if (npc == null) { return; }
			if (Kit != "" && plugin.Kits != null) { plugin.GiveKitfunction(npc, Kit); }
			else
			{
				CreateCloths(npc, outfit);
				if (mask) { CreateCloths(npc, "hat.tigermask"); }
				if (headset) { CreateCloths(npc, "twitch.headset"); }
				if (assassin) { CreateGun(npc, "rifle.lr300", "weapon.mod.silencer"); }
				else { CreateGun(npc, "rifle.semiauto", "weapon.mod.flashlight"); }
			}
		}

		private void CreateCloths(BasePlayer npc, string ItemName, ulong skin = 0)
		{
			if (npc == null) { return; }
			Item item = ItemManager.CreateByName(ItemName, 1, skin);
			if (item == null) { return; }
			if (!item.MoveToContainer(npc.inventory.containerWear, -1, false)) { item.Remove(); }
		}

		private void CreateGun(HumanNPC npc, string ItemName, string Attachment)
		{
			Item item = ItemManager.CreateByName(ItemName, 1, 0);
			if (item == null) { return; }
			BaseEntity be = item.GetHeldEntity();
			if (be != null) { be.isSpawned = false; timer.Once(2f, () => { if (be != null) { be.isSpawned = true; } }); }
			BaseEntity we = item.GetWorldEntity();
			if (we != null) { we.isSpawned = false; timer.Once(2f, () => { if (we != null) { we.isSpawned = true; } }); }
			if (!item.MoveToContainer(npc.inventory.containerBelt, -1, false)) { item.Remove(); return; }
			if (be != null && be is BaseProjectile)
			{
				if (Attachment != "")
				{
					Item moditem = ItemManager.CreateByName(Attachment, 1, 0);
					if (moditem != null && item.contents != null)
					{
						BaseEntity bemi = moditem.GetHeldEntity();
						if (bemi != null) { bemi.isSpawned = false; timer.Once(2f, () => { if (bemi != null) { bemi.isSpawned = true; } }); }
						BaseEntity wemi = moditem.GetWorldEntity();
						if (wemi != null) { wemi.isSpawned = false; timer.Once(2f, () => { if (wemi != null) { wemi.isSpawned = true; } }); }
						if (!moditem.MoveToContainer(item.contents)) { item.contents.Insert(moditem); }
					}
				}
				timer.Once(4f, () => { if (npc != null && item != null) { npc.UpdateActiveItem(item.uid); } });
			}
		}

		private void NPCSit(ScientistNPC npc, BaseEntity baseEntity)
		{
			if (npc == null) { return; }
			npc.modelState.mounted = true;
			npc.transform.rotation = baseEntity.transform.rotation;
			npc.ServerRotation = baseEntity.transform.rotation;
			npc.OverrideViewAngles(baseEntity.transform.rotation.eulerAngles);
			npc.eyes.NetworkUpdate(baseEntity.transform.rotation);
		}

		private void assignPOIfunction(Vector3 AttackVector)
		{
			if (PointsOfIntrest.Count > 5) { return; }
			if (PointsOfIntrest.Count == 0)
			{
				PointsOfIntrest.Add(AttackVector);
				timer.Once(_ServerSettings.POIExpires, () => { PointsOfIntrest.Remove(AttackVector); });
			}
			else if (!PointsOfIntrest.Contains(AttackVector))
			{
				bool Close = false;
				foreach (Vector3 poi in PointsOfIntrest.ToArray())
				{
					if (Vector3.Distance(poi, AttackVector) < 800)
					{
						Close = true;
						break;
					}
				}
				if (!Close)
				{
					PointsOfIntrest.Add(AttackVector);
					timer.Once(_ServerSettings.POIExpires, () => { PointsOfIntrest.Remove(AttackVector); });
				}
			}
		}

		private Vector3 getPOIfunction(Vector3 orig, float maxDistance)
		{
			if (PointsOfIntrest.Count != 0) { foreach (Vector3 poi in PointsOfIntrest.ToArray()) { if (Vector3.Distance(poi, orig) < maxDistance) { return (poi); } } }
			return Vector3.zero;
		}

		private float GetSpawnHeight(Vector3 target)
		{
			float y = TerrainMeta.HeightMap.GetHeight(target);
			float w = TerrainMeta.WaterMap.GetHeight(target);
			float p = TerrainMeta.HighestPoint.y + 250f;
			RaycastHit hit;
			if (Physics.Raycast(new Vector3(target.x, w, target.z), Vector3.up, out hit, p, Layers.Mask.World))
			{
				y = Mathf.Max(y, hit.point.y);
				if (Physics.Raycast(new Vector3(target.x, hit.point.y + 0.5f, target.z), Vector3.up, out hit, p, Layers.Mask.World)) { y = Mathf.Max(y, hit.point.y); }
			}
			return Mathf.Max(y, w);
		}

		private List<Vector3> GetCircumferencePositions(Vector3 center, float radius, float next, bool spawnHeight, float y = 0f)
		{
			var positions = new List<Vector3>();
			if (next < 1f) { next = 1f; }
			float angle = 0f;
			float angleInRadians = 2 * (float)Math.PI;
			while (angle < 360)
			{
				float radian = (angleInRadians / 360) * angle;
				float x = center.x + radius * (float)Math.Cos(radian);
				float z = center.z + radius * (float)Math.Sin(radian);
				var a = new Vector3(x, 0f, z);
				a.y = y == 0f ? spawnHeight ? GetSpawnHeight(a) : TerrainMeta.HeightMap.GetHeight(a) : y;
				if (a.y < -48f) { a.y = -48f; }
				positions.Add(a);
				angle += next;
			}
			positions.RemoveAt(0);
			return positions;
		}

		private Elevation GetTerrainElevation(Vector3 center)
		{
			float maxY = -1000;
			float minY = 1000;
			foreach (var position in GetCircumferencePositions(center, 20f, 30f, true))
			{
				if (position.y > maxY) maxY = position.y;
				if (position.y < minY) minY = position.y;
			}
			return new Elevation { Min = minY, Max = maxY };
		}

		private IEnumerator GenerateAirGrid()
		{
			int minPos = (int)(World.Size / -2f);
			int maxPos = (int)(World.Size / 2f);
			int checks = 0;
			var _instruction = ConVar.FPS.limit > 80 ? CoroutineEx.waitForSeconds(0.01f) : null;
			for (float x = minPos; x < maxPos; x += 25f)
			{
				for (float z = minPos; z < maxPos; z += 25f)
				{
					var pos = new Vector3(x, 0f, z);
					if (TerrainMeta.HeightMap.GetHeight(pos) < 0.5) { continue; }
					pos.y = GetSpawnHeight(pos) + 50f;
					var elevation = GetTerrainElevation(pos);
					if (IsFlatTerrain(pos, elevation, 3)) { airnodes.Add(pos); }
					if (++checks >= 50)
					{
						checks = 0;
						yield return _instruction;
					}
				}
			}
			airloaded = true;
			Puts("Created " + airnodes.Count.ToString() + " Air Nodes.");
			yield break;
		}

		private IEnumerator GenerateLandGrid()
		{
			int minPos = (int)(World.Size / -2f);
			int maxPos = (int)(World.Size / 2f);
			var checks = 0;
			var _instruction = ConVar.FPS.limit > 80 ? CoroutineEx.waitForSeconds(0.01f) : null;
			for (float x = minPos; x < maxPos; x += 50f)
			{
				for (float z = minPos; z < maxPos; z += 50f)
				{
					if (++checks >= 50)
					{
						checks = 0;
						yield return _instruction;
					}
					Vector3 original = new Vector3(x, 0, z);
					original.y = TerrainMeta.HeightMap.GetHeight(original);
					var position = FindPointOnNavmesh(original);
					if (position != null && position is Vector3 && (Vector3)position != Vector3.zero)
					{
						if (TerrainMeta.HeightMap.GetHeight((Vector3)position) > TerrainMeta.WaterMap.GetHeight((Vector3)position)) { groundnodes.Add((Vector3)position); }
					}
				}
			}
			landloaded = true;
			Puts("Created " + groundnodes.Count.ToString() + " Land Nodes.");
			yield break;
		}

		private IEnumerator GenerateRoadGrid()
		{
			var checks = 0;
			var _instruction = ConVar.FPS.limit > 80 ? CoroutineEx.waitForSeconds(0.01f) : null;
			foreach (PathList roadnode in TerrainMeta.Path.Roads)
			{
				foreach (Vector3 v in roadnode.Path.Points)
				{
					if (++checks >= 1000)
					{
						checks = 0;
						yield return _instruction;
					}
					var elevation = GetTerrainElevation(v);
					if (IsFlatTerrain(v, elevation, 4))
					{
						bool flag = false;
						foreach (Vector3 n in roadnodes) { if (Vector3.Distance(v, n) < 20) { flag = true; break; } }
						if (!flag && SpawnCheckfunctioon(v, _ServerSettings.PirateBaseWallRadius + 1, -1)) { roadnodes.Add(v); }
					}
				}
			}
			Puts("Created " + roadnodes.Count.ToString() + " Road Nodes.");
			yield break;
		}

		private IEnumerator GenerateFootGrid()
		{
			var checks = 0;
			var _instruction = ConVar.FPS.limit > 30 ? CoroutineEx.waitForSeconds(0.01f) : null;
			for (int i = World.Serialization.world.prefabs.Count - 1; i >= 0; i--)
			{
				if (++checks >= 500)
				{
					checks = 0;
					yield return _instruction;
				}
				PrefabData prefabdata0 = World.Serialization.world.prefabs[i];
				if (prefabdata0.id == 1519652535 && prefabdata0.category.Contains("piratenpcspawner")) { footnodes.Add(prefabdata0); }
				if (prefabdata0.id == 500822506 && prefabdata0.category.Contains("callpirate")) { Alarmtriggers.Add(prefabdata0); }
			}
			if (footnodes.Count != 0) { footloaded = true; }

			Puts("Found " + footnodes.Count.ToString() + " NPC Spawner Nodes. & " + Alarmtriggers.Count.ToString() + " triggers");
			yield break;
		}

		private IEnumerator NPCAIFunction()
		{
			var checks = 0;
			var _instruction = ConVar.FPS.limit > 30 ? CoroutineEx.waitForSeconds(0.01f) : null;
			Puts("Starting AI Thread");
			while (plugin != null)
			{
				for (int i = 0; i < ActiveNPCs.Count; i++)
				{
					if (++checks >= 50)
					{
						checks = 0;
						yield return _instruction;
					}
					try
					{
						if (ActiveNPCs.ElementAt(i).Key == null || ActiveNPCs.ElementAt(i).Key.IsDestroyed)
						{
							ActiveNPCs.Remove(ActiveNPCs.ElementAt(i).Key);
							continue;
						}
						KeyValuePair<ScientistNPC, BaseEntity> npc = ActiveNPCs.ElementAt(i);
						if (npc.Key == null || npc.Key.net == null) { continue; }
						if (GroundedNPCs.Contains(npc.Key.net.ID))
						{
							if (npc.Key == null) continue;
							if (NPCsUintList.Contains(npc.Key.net.ID)) { NPCsUintList.Remove(npc.Key.net.ID); }
							if (npc.Key.IsWounded()) { if (!downed.Contains(npc.Key)) { downed.Add(npc.Key); } }
							if (!npc.Key.IsWounded() && downed.Contains(npc.Key))
							{
								downed.Remove(npc.Key);
								npc.Key.EquipWeapon();
							}
							float groundheightFLOAT = TerrainMeta.HeightMap.GetHeight(npc.Key.transform.position);
							if (groundheightFLOAT < -2f || npc.Key.transform.position.y < groundheightFLOAT - 2f) { GroundedNPCs.Remove(npc.Key.net.ID); npc.Key.Die(); continue; }
							if (npc.Value != null) { if (npc.Value.Distance(npc.Key) > 50f) { ActiveNPCs[npc.Key] = null; } }
							if (npc.Value == null) { ActiveNPCs[npc.Key] = npc.Key.Brain.Senses.GetNearestPlayer(50); }
							if (npc.Value != null && !VoiceUintList.Contains(npc.Value.net.ID)) { npc.Key.Brain.Navigator.SetDestination(ActiveNPCs[npc.Key].transform.position, BaseNavigator.NavigationSpeed.Normal); }
							else
							{
								if (!npc.Key.Brain.Navigator.Moving)
								{
									float d = npc.Key.Brain.Navigator.MaxRoamDistanceFromHome;
									Vector2 vector = Vector3.zero;
									Vector3 newPos = npc.Key.Brain.Events.Memory.Position.Get(4);
									for (int scan = 0; scan < 10; scan++)
									{
										vector = UnityEngine.Random.insideUnitCircle * d;
										newPos += new Vector3(vector.x, 0, vector.y);
										newPos.y = TerrainMeta.HeightMap.GetHeight(newPos);
										if (newPos.y > -0.4f) { break; }
										newPos = npc.Key.transform.position;
									}
									npc.Key.Brain.Navigator.SetDestination(newPos, BaseNavigator.NavigationSpeed.Normal);
								}
								if (Vector3.Distance(npc.Key.transform.position, npc.Key.Brain.Events.Memory.Position.Get(4)) > npc.Key.Brain.Navigator.MaxRoamDistanceFromHome) { npc.Key.Brain.Navigator.SetDestination(npc.Key.Brain.Events.Memory.Position.Get(4), BaseNavigator.NavigationSpeed.Fast); }
							}
							try { if (npc.Key.GetGun().primaryMagazine.contents < 1) { npc.Key.AttemptReload(); } } catch { }
							if (npc.Value != null)
							{
								float Distance = npc.Value.Distance2D(npc.Key);
								if (Distance <= 10f && !npc.Key.IsWounded() && npc.Key.IsAlive())
								{
									AttackEntity attackEntity = npc.Key.GetHeldEntity() as AttackEntity;
									if (attackEntity == null)
									{
										bool givennewgun = true;
										foreach (Item it in npc.Key.inventory.AllItems())
										{
											if (it.name.Contains("eoka"))
											{
												givennewgun = false;
												break;
											}
										}
										if (givennewgun)
										{
											CreateGun(npc.Key, "pistol.eoka", "");
										}
									}
									npc.Key.SetAimDirection(npc.Value.transform.position);
									npc.Key.ShotTest(Distance);
								}
							}
						}
						else if (NPCsUintList.Contains(npc.Key.net.ID))
						{
							if (npc.Key.IsAlive())
							{
								float senserange = 30f;
								float terrainheight = TerrainMeta.HeightMap.GetHeight(npc.Key.transform.position);
								if (npc.Key.transform.position.y < 10 && terrainheight < 0)
								{
									senserange = _ServerSettings.BoatTargetDistance;
								}
								else if (npc.Key.transform.position.y + 20f > terrainheight)
								{
									senserange = _ServerSettings.HeliTargetDistance;
								}
								else
								{
									senserange = _ServerSettings.LandTargetDistance;
								}
								if (npc.Value == null) { ActiveNPCs[npc.Key] = npc.Key.Brain.Senses.GetNearestTarget(senserange); }
								if (npc.Value != null)
								{
									BasePlayer p = npc.Value.ToPlayer();
									if (p != null)
									{
										if (p.IsSleeping() || p.IsDead() || plugin.VoiceUintList.Contains(p.net.ID)) { ActiveNPCs[npc.Key] = null; }
										else if (senserange == _ServerSettings.BoatTargetDistance && p.transform.position.y > 505) { ActiveNPCs[npc.Key] = null; }
									}
								}
							}
							else { NPCsUintList.Remove(npc.Key.net.ID); }
						}
					}
					catch { }
				}
				yield return CoroutineEx.waitForSeconds(_ServerSettings.NPCTick);
			}
			Puts("Stopped AI Thread");
			yield break;
		}

		private IEnumerator GenerateSeaGrid()
		{
			float x = TerrainMeta.Size.x;
			int Sections = Mathf.CeilToInt(TerrainMeta.Size.x * 2f * 3.14159274f / 50);
			seanodes = new List<Vector3>();
			var checks = 0;
			var _instruction = ConVar.FPS.limit > 30 ? CoroutineEx.waitForSeconds(0.01f) : null;
			for (int i = 0; i < Sections; i++)
			{
				float num5 = (float)i / (float)Sections * 360f;
				seanodes.Add(new Vector3(Mathf.Sin(num5 * 0.0174532924f) * x, 0, Mathf.Cos(num5 * 0.0174532924f) * x));
			}
			bool flag = true;
			int num8 = 1;
			Vector3[] array = new Vector3[]
			{
			new Vector3(0f, 0f, 0f),
			new Vector3(30, 0f, 0f),
			new Vector3(-30, 0f, 0f),
			new Vector3(0f, 0f, 30),
			new Vector3(0f, 0f, -30)
			};
			while (flag)
			{
				num8++;
				flag = false;
				for (int j = 0; j < Sections; j++)
				{
					Vector3 vector = seanodes[j];
					int index = (j == 0) ? (Sections - 1) : (j - 1);
					int index2 = (j == Sections - 1) ? 0 : (j + 1);
					Vector3 b = seanodes[index2];
					Vector3 b2 = seanodes[index];
					Vector3 normalized = (Vector3.zero - vector).normalized;
					Vector3 vector3 = vector + normalized * 2;
					if (Vector3.Distance(vector3, b) <= 100 && Vector3.Distance(vector3, b2) <= 100)
					{
						bool flag2 = true;
						for (int k = 0; k < array.Length; k++)
						{
							Vector3 vector4 = vector3 + array[k];
							if (GetWaterDepthfunction(vector4) < 5) { flag2 = false; break; }
						}
						if (flag2) { flag = true; seanodes[j] = vector3; }
					}
					if (++checks >= 1000)
					{
						checks = 0;
						yield return _instruction;
					}
				}
			}
			var list = Facepunch.Pool.GetList<int>();
			LineUtility.Simplify(seanodes, 10f, list);
			List<Vector3> list2 = seanodes;
			seanodes = new List<Vector3>();
			foreach (int index3 in list) { seanodes.Add(list2[index3]); }
			sealoaded = true;
			Puts("Created " + seanodes.Count.ToString() + " Sea Nodes.");
			Facepunch.Pool.FreeList(ref list);
			yield break;
		}

		private IEnumerator GeneratRailGrid()
		{
			var checks = 0;
			var _instruction = ConVar.FPS.limit > 30 ? CoroutineEx.waitForSeconds(0.01f) : null;
			foreach (PathList pathList in World.GetPaths("Rail").AsEnumerable<PathList>())
			{
				if (railnodes.Count != 0) { break; }
				foreach (Vector3 v in pathList.Path.Points)
				{
					if (++checks >= 1000)
					{
						checks = 0;
						yield return _instruction;
					}
					railnodes.Add(v);
				}
			}
			railloaded = true;
			Puts("Created " + railnodes.Count.ToString() + " Rail Nodes.");
			yield break;
		}

		private IEnumerator GeneratPlaneGrid()
		{
			int droppable = 0;
			foreach (MonumentInfo pathList in TerrainMeta.Path.Monuments)
			{
				if (!pathList.IsSafeZone)
				{
					if (dropnodes.ContainsKey(pathList.transform.position)) { continue; }
					if (_ServerSettings.PiratePlaneAllowedDrops.Contains(pathList.name))
					{
						droppable++;
						dropnodes.Add(pathList.transform.position, true);
					}
					else { dropnodes.Add(pathList.transform.position, false); }
				}
			}
			Puts("Loaded " + dropnodes.Count.ToString() + " Monument nodes. " + droppable.ToString() + " are drop points.");
			yield break;
		}

		private object FindPointOnNavmesh(Vector3 pos)
		{
			NavMeshHit navmeshHit;
			for (int i = 0; i < 10; i++)
			{
				Vector3 position = i == 0 ? pos : pos + (UnityEngine.Random.onUnitSphere * 30);
				if (NavMesh.SamplePosition(position, out navmeshHit, 30, 1))
				{
					if (navmeshHit.mask != 1)
						continue;
					if (!(Physics.OverlapSphere(navmeshHit.position, 15f, 65536).Length == 0))
						continue;
					return navmeshHit.position;
				}
			}
			return null;
		}

		private List<Vector3> FindNavPoints(Vector3 scanpoint, float radius)
		{
			List<Vector3> mesh = new List<Vector3>();
			mesh.Add(scanpoint);
			for (int i = 0; i < 25; i++)
			{
				float d = radius;
				Vector2 vector = UnityEngine.Random.insideUnitCircle * d;
				Vector3 point = scanpoint + new Vector3(vector.x, 0, vector.y);
				point.y = TerrainMeta.HeightMap.GetHeight(point);
				if (point.y < 0) { continue; }
				RaycastHit raycastHit;
				bool add = true;
				if (Physics.Raycast(point, Vector3.up, out raycastHit, 10, 1218511105)) { add = false; }
				if (add)
				{
					foreach (Vector3 check in mesh)
					{
						if (Vector3.Distance(point, check) < 8)
						{
							add = false;
							break;
						}
					}
					if (Physics.OverlapSphere(point, 5f, 65536).Length != 0) { add = false; }
				}
				if (add) { mesh.Add(point); }
			}
			return mesh;
		}

		private List<Vector3> MonumentPathList()
		{
			List<Vector3> path = new List<Vector3>();
			if (TerrainMeta.Path != null && TerrainMeta.Path.Monuments != null && TerrainMeta.Path.Monuments.Count > 0)
			{
				foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
				{
					Vector3 pos = monumentInfo.transform.position;
					pos.y += 50;
					path.Add(pos);
				}
			}
			else { path.Add(new Vector3(0, 50, 0)); }
			if (path.Count == 0) { path.Add(new Vector3(0, 0, 0)); }
			Puts("Loaded " + path.Count.ToString() + " Air Nodes.");
			return path;
		}

		private bool IsFlatTerrain(Vector3 center, Elevation elevation, float value) { return elevation.Max - elevation.Min <= value && elevation.Max - center.y <= value; }

		private void SetupOceanPatrolPath() { Threads.Add(ServerMgr.Instance.StartCoroutine(GenerateSeaGrid())); }

		private void SetupAirPatrolPath() { Threads.Add(ServerMgr.Instance.StartCoroutine(GenerateAirGrid())); }

		private void SetupLandPatrolPath() { Threads.Add(ServerMgr.Instance.StartCoroutine(GenerateLandGrid())); }

		private void SetupRailPatrolPath() { Threads.Add(ServerMgr.Instance.StartCoroutine(GeneratRailGrid())); }

		private void SetupPlanePath() { Threads.Add(ServerMgr.Instance.StartCoroutine(GeneratPlaneGrid())); }

		private float GetWaterDepthfunction(Vector3 pos)
		{
			RaycastHit raycastHit;
			if (!Physics.Raycast(pos, Vector3.down, out raycastHit, 100f, 8388608)) { return 100f; }
			return raycastHit.distance;
		}

		private void RunEffect(string name, BaseEntity entity = null, Vector3 position = new Vector3(), Vector3 offset = new Vector3())
		{
			if (entity != null) { Effect.server.Run(name, entity, 0, offset, position, null, true); return; }
			Effect.server.Run(name, position, Vector3.up, null, true);
		}

		private void CreateFire(float lifetime, Vector3 FireOffset, float FireDelay, BaseEntity parant)
		{
			timer.Once(FireDelay, () =>
			{
				if (parant == null) { return; }
				FireBall fireBallSPAWN = GameManager.server.CreateEntity(oilfireballsmallprefab, parant.transform.position) as FireBall;
				fireBallSPAWN.Spawn();
				fireBallSPAWN.enableSaving = false;
				fireBallSPAWN.SetParent(parant, true, true);
				fireBallSPAWN.transform.localPosition = FireOffset;
				Rigidbody rb = fireBallSPAWN.GetComponent<Rigidbody>();
				rb.isKinematic = true;
				fireBallSPAWN.CancelInvoke(fireBallSPAWN.Extinguish);
				fireBallSPAWN.Invoke(fireBallSPAWN.Extinguish, lifetime);
			});
		}

		private void FreeLists(BaseEntity baseEntity)
		{
			if (baseEntity == null || baseEntity.net == null) { return; }
			BaseParts.Remove(baseEntity.net.ID);
			if (PirateUintList.Contains(baseEntity.net.ID))
			{
				foreach (BaseEntity be in baseEntity.children.ToArray()) { if (be == null) { continue; } if (be is SphereEntity || be is MapMarkerGenericRadius || be is SearchLight || be is FireBall || be is HorseCorpse || be.ShortPrefabName.Contains("parachute")) { be.SetParent(null, true, true); be.Kill(); } }
				PirateUintList.Remove(baseEntity.net.ID);
			}
			NPCsUintList.Remove(baseEntity.net.ID);
			GroundedNPCs.Remove(baseEntity.net.ID);
			if (TrainsBaseEntityList.Contains(baseEntity))
			{
				RailPirate RP = baseEntity.GetComponent<RailPirate>();
				if (RP != null) { KillWagonsfunction(RP.wagons.ToList()); }
				TrainsBaseEntityList.Remove(baseEntity);
			}
		}

		private void PirateSpawner()
		{
			if (Rust.Application.isLoading || !sealoaded || !airloaded || !landloaded)
			{
				timer.Once(10f, () => { PirateSpawner(); });
				return;
			}
			for (int s = 0; s < _ServerSettings.RHIBBoatAmount; s++)
			{
				if (pirateRHIB >= _ServerSettings.RHIBBoatMaxA) { break; }
				SpawnPirates(seanodes.GetRandom());
			}
			for (int s = 0; s < _ServerSettings.RowBoatBoatAmount; s++)
			{
				if (pirateRowBoat >= _ServerSettings.RowBoatBoatMaxA) { break; }
				SpawnPirates(seanodes.GetRandom(), 1);
			}
			for (int s = 0; s < _ServerSettings.MiniAmount; s++)
			{
				if (piratesMini >= _ServerSettings.MiniMaxA) { break; }
				SpawnPirates(seanodes.GetRandom(), 2);
			}
			for (int s = 0; s < _ServerSettings.ScrapAmount; s++)
			{
				if (pirateScrap >= _ServerSettings.ScrapMaxA) { break; }
				SpawnPirates(seanodes.GetRandom(), 3);
			}
			for (int s = 0; s < _ServerSettings.HorseAmount; s++)
			{
				if (pirateHorse >= _ServerSettings.HorseMax) { break; }
				SpawnPirates(groundnodes.GetRandom(), 4);
			}
			for (int s = 0; s < _ServerSettings.SnowMobileAmount; s++)
			{
				if (pirateSnowMobile >= _ServerSettings.SnowMobileMax) { break; }
				SpawnPirates(groundnodes.GetRandom(), 5);
			}
			for (int s = 0; s < _ServerSettings.SubmarineAmount; s++)
			{
				if (pirateSub >= _ServerSettings.SubmarineMaxA) { break; }
				SpawnPirates(seanodes.GetRandom(), 6);
			}
			if (roadnodes.Count > 5)
			{
				List<Vector3> BasePositions = new List<Vector3>();
				for (int s = 0; s < _ServerSettings.PirateBaseAmount; s++)
				{
					if (pirateBase >= _ServerSettings.PirateBaseAmount) { break; }
					for (int i = 0; i < 100; i++)
					{
						Vector3 SpawnPos = roadnodes.GetRandom();
						bool valid = true;
						foreach (Vector3 b in BasePositions) { if (Vector3.Distance(SpawnPos, b) < 200) { valid = false; break; } }
						foreach (PathList pathList in World.GetPaths("Rail").AsEnumerable<PathList>())
						{
							foreach (Vector3 v in pathList.Path.Points)
							{
								if (Vector3.Distance(SpawnPos, v) < 20)
								{
									valid = false;
									break;
								}
							}
						}
						if (valid && SpawnCheckfunctioon(SpawnPos, _ServerSettings.PirateBaseWallRadius + 2, -1))
						{
							SpawnPirates(SpawnPos, 7);
							BasePositions.Add(SpawnPos);
							break;
						}
					}
				}
			}
			if (railnodes.Count > 5 || !railloaded)
			{
				List<Vector3> BasePositions = new List<Vector3>();
				for (int s = 0; s < _ServerSettings.PirateRailAmount; s++)
				{
					if (pirateRail >= _ServerSettings.PirateRailAmount) { break; }
					for (int i = 0; i < 100; i++)
					{
						Vector3 SpawnPos = railnodes.GetRandom();
						bool valid = true;
						foreach (Vector3 b in BasePositions)
						{
							if (Vector3.Distance(SpawnPos, b) < 500) { valid = false; break; }
						}
						if (valid && SpawnCheckfunctioon(SpawnPos, 10, -1))
						{
							SpawnPirates(SpawnPos, 8);
							BasePositions.Add(SpawnPos);
							break;
						}
					}
				}
			}
			if (footloaded)
			{
				foreach (PrefabData pd in footnodes)
				{
					if (pd != null)
					{
						Vector3 pos = new Vector3(pd.position.x, pd.position.y, pd.position.z);
						if (PlayersNearbyfunction(pos, 20)) { continue; }
						bool missingnpc = true;
						foreach (ScientistNPC npc in ActiveNPCs.Keys)
						{
							if (npc.Brain.Events.Memory.Position.Get(4) == pos)
							{
								missingnpc = false;
								break;
							}
						}
						if (missingnpc)
						{
							List<Vector3> addpos = new List<Vector3>();
							addpos.Add(pos);
							var NPZ = CreateNPCs(1, 9, _ServerSettings.PirateFootNPCHealth, _ServerSettings.PirateFootNPCKit, _ServerSettings.PirateFootNPCAimMulti, null, addpos);
							if (NPZ != null && NPZ.Count == 1) { NPZ[0].SetAimDirection(pd.rotation); }
						}
					}
				}
			}
			if (_ServerSettings.PirateCargoPlane && piratePlane == 0)
			{
				SpawnPirates(seanodes.GetRandom(), 9);
			}
		}

		private void DestroyGroundComp(BaseEntity ent)
		{
			UnityEngine.Object.DestroyImmediate(ent.GetComponent<DestroyOnGroundMissing>());
			UnityEngine.Object.DestroyImmediate(ent.GetComponent<GroundWatch>());
		}

		private void DestroyMeshCollider(BaseEntity ent) { foreach (var mesh in ent.GetComponentsInChildren<MeshCollider>()) { UnityEngine.Object.DestroyImmediate(mesh); } }

		private BaseEntity spawnersetupfunction(BaseEntity baseEntity)
		{
			baseEntity.syncPosition = true;
			baseEntity.globalBroadcast = true;
			baseEntity.Spawn();
			baseEntity.enableSaving = false;
			if (baseEntity is RidableHorse)
			{
				RidableHorse horse = baseEntity as RidableHorse;
				Item item = ItemManager.CreateByName("horse.armor.wood", 1, 0);
				if (item != null)
				{
					if (!item.MoveToContainer(horse.inventory, -1, false)) { item.Remove(); }
					ItemModAnimalEquipment storage = horse.inventory.itemList[0].info.GetComponent<ItemModAnimalEquipment>();
					if (storage) { storage.additionalInventorySlots = 30; }
					horse.SendNetworkUpdateImmediate();
				}
			}
			return baseEntity;
		}

		private void EdgeDelete(List<ScientistNPC> npcz)
		{
			if (npcz == null) { return; }
			foreach (ScientistNPC npc in npcz) { if (npc != null && !npc.IsDestroyed) { npc.Kill(); } }
		}

		private void InvalidateCheck(BaseEntity baseentitybe)
		{
			if (baseentitybe == null) { return; }
			if (!ValidBounds.Test(baseentitybe.transform.position + baseentitybe.transform.forward * 200f))
			{
				AirPirates ap = baseentitybe.GetComponent<AirPirates>();
				if (ap != null)
				{
					EdgeDelete(ap.npcs);
					if (ap.itembox != null && !ap.itembox.IsDestroyed) { ap.itembox.Kill(); }
					if (ap._heli != null && !ap._heli.IsDestroyed) { ap._heli.Kill(); }
				}
				WaterPirate wp = baseentitybe.GetComponent<WaterPirate>();
				if (wp != null)
				{
					EdgeDelete(wp.npcs);
					if (wp.itembox != null && !wp.itembox.IsDestroyed) { wp.itembox.Kill(); }
					if (wp._boat != null && !wp._boat.IsDestroyed) { wp._boat.Kill(); }
				}
				LandPirate lp = baseentitybe.GetComponent<LandPirate>();
				if (lp != null)
				{
					if (lp.npc != null && !lp.npc.IsDestroyed) { lp.npc.Kill(); }
					if (lp.itembox != null) { lp.itembox.Kill(); }
					if (wp._boat != null && !wp._boat.IsDestroyed) { wp._boat.Kill(); }
				}
				foreach (uint id in NPCsUintList.ToArray()) { if (!BaseEntity.serverEntities.entityList.Contains(id)) { NPCsUintList.Remove(id); } }
				FootPirate pf = baseentitybe.GetComponent<FootPirate>();
				if (pf != null) { if (pf.baseentitybe != null && !pf.baseentitybe.IsDestroyed) { pf.baseentitybe.Kill(); } }
				return;
			}
			timer.Once(1f, () => { InvalidateCheck(baseentitybe); });
		}

		#endregion

		#region Classes

		private BaseEntity SpawnPirates(Vector3 dropPosition, int Type = 0)
		{
			string prefab = "";
			float baseheightFLOAT;
			float groundheightFLOAT = TerrainMeta.HeightMap.GetHeight(dropPosition);
			float waterheightFLOAT = TerrainMeta.WaterMap.GetHeight(dropPosition);
			if (groundheightFLOAT > waterheightFLOAT) { baseheightFLOAT = groundheightFLOAT; }
			else { baseheightFLOAT = waterheightFLOAT; }

			switch (Type)
			{
				case 0:
					prefab = rhibentityprefab;
					dropPosition.y = baseheightFLOAT;
					break;
				case 1:
					prefab = rowboatentityprefab;
					dropPosition.y = baseheightFLOAT;
					break;
				case 2:
					prefab = minientityprefab;
					dropPosition.y = baseheightFLOAT + 60;
					break;
				case 3:
					prefab = scrapentityprefab;
					dropPosition.y = baseheightFLOAT + 60;
					break;
				case 4:
					prefab = horseentityprefab;
					dropPosition.y = baseheightFLOAT + 1;
					break;
				case 5:
					prefab = snowmobileentityprefab;
					dropPosition.y = baseheightFLOAT + 1;
					break;
				case 6:
					prefab = subentityprefab;
					dropPosition.y = baseheightFLOAT;
					break;
				case 7:
					prefab = hoboentityprefab;
					dropPosition.y = baseheightFLOAT - 5.5f;
					break;
				case 8:
					prefab = workcartprefab;
					break;
				case 9:
					dropPosition.y = TerrainMeta.HighestPoint.y + 50f;
					prefab = sphereentityprefab;
					break;
				case 10:
					List<Vector3> addpos = new List<Vector3>();
					addpos.Add(dropPosition);
					List<ScientistNPC> npc = CreateNPCs(1, 9, _ServerSettings.PirateFootNPCHealth, _ServerSettings.PirateFootNPCKit, _ServerSettings.PirateFootNPCAimMulti, null, addpos);
					timer.Once(5f, () =>
					{
						if (npc != null && npc.Count == 1)
						{
							if (npc[0] != null)
							{
								BailOutfunction(dropPosition, npc[0], dropPosition, 3, 0, false);
							}
						}
					});
					return null;
				case 11:
					dropPosition = seanodes.GetRandom();
					dropPosition.y = TerrainMeta.HighestPoint.y + 50f;
					prefab = sphereentityprefab;
					break;
			}
			BaseEntity baseentitybe = GameManager.server.CreateEntity(prefab, dropPosition, Quaternion.identity, true);
			switch (Type)
			{
				case 0:
				case 1:
				case 6:
					WaterPirate _Pirate = baseentitybe.gameObject.AddComponent<WaterPirate>();
					_Pirate.Type = Type;
					break;
				case 2:
				case 3:
					AirPirates _PirateAir = baseentitybe.gameObject.AddComponent<AirPirates>();
					_PirateAir.Type = Type;
					if (_ServerSettings.DisableHeliFireball) { (baseentitybe as MiniCopter).fireBall.guid = null; }
					break;
				case 4:
				case 5:
					LandPirate _PirateLand = baseentitybe.gameObject.AddComponent<LandPirate>();
					_PirateLand.Type = Type;
					break;
				case 7:
					BasePirate _PirateBase = baseentitybe.gameObject.AddComponent<BasePirate>();
					_PirateBase.Type = Type;
					break;
				case 8:
					RailPirate _PirateRail = baseentitybe.gameObject.AddComponent<RailPirate>();
					_PirateRail.Type = Type;
					break;
				case 9:
					FootPirate _PirateFoot = baseentitybe.gameObject.AddComponent<FootPirate>();
					_PirateFoot.Type = Type;
					break;
				case 11:
					FootPirate _PirateFoot2 = baseentitybe.gameObject.AddComponent<FootPirate>();
					_PirateFoot2.Type = 91;
					break;
			}
			return spawnersetupfunction(baseentitybe);
		}

		private class NightLight { public SearchLight _light; public SphereEntity resizer; }

		private class Elevation { public float Min { get; set; } public float Max { get; set; } }

		private class targetinfo
		{
			public targetinfo(BaseEntity initEnt, BasePlayer initPly = null)
			{
				ply = initPly;
				ent = initEnt;
				lastSeenTime = float.PositiveInfinity;
				nextLOSCheck = UnityEngine.Time.realtimeSinceStartup + 1.5f;
			}

			public BasePlayer ply;
			public BaseEntity ent;
			public float lastSeenTime = float.PositiveInfinity;
			public float visibleFor;
			public float nextLOSCheck;
		}

		private class WaterPirate : MonoBehaviour
		{
			public int Type = 0;
			public BaseEntity _boat;
			public BaseSubmarine _sub;
			private bool burning;
			private bool isDead;
			public List<ScientistNPC> npcs;
			public StorageContainer itembox;
			private int targetNodeIndex = -1;
			public Vector3 currentVelocity = Vector3.zero;
			private float currentThrottle;
			private float currentTurnSpeed;
			private float turnScale = 10;
			private bool egressing;
			private bool stopping;
			private float BoatTurnSpeedfloat;
			private float BoatMaxSpeedfloat;
			private NightLight searchLight;
			private int NPCAmountint;
			private string RandomLoot = "";
			private int LootMulti = 1;
			private List<Vector3> Position = new List<Vector3>();
			private float NPCHealthfloat;
			private string Kit;
			private float AIShootMultiplyer;
			private Vector3 FireOffset;
			public BaseEntity PlayerTarget;
			private Vector3 TargetDest0;
			private bool halftickrate;
			private bool loading = true;

			private void Awake()
			{
				plugin.NextFrame(() =>
				{
					_boat = GetComponent<BaseEntity>();
					plugin.timer.Once(10, () => { loading = false; });
					TargetDest0 = _boat.transform.position;
					switch (Type)
					{
						case 0:
							plugin.pirateRHIB++;
							BoatTurnSpeedfloat = _ServerSettings.RHIBBoatTurnSpeed;
							BoatMaxSpeedfloat = _ServerSettings.RHIBBoatMaxSpeed;
							NPCAmountint = _ServerSettings.RHIBNPCAmount;
							NPCHealthfloat = _ServerSettings.RHIBNPCHealth;
							_boat.gameObject.GetComponent<BaseCombatEntity>().InitializeHealth(_ServerSettings.RHIBHealth, _ServerSettings.RHIBHealth);
							Kit = _ServerSettings.RHIBKit;
							RandomLoot = _ServerSettings.RHIBLootString;
							LootMulti = _ServerSettings.RHIBLootMulti;
							AIShootMultiplyer = _ServerSettings.RHIBAIDistance;
							Position.Add(new Vector3(0f, 1f, 0f));
							Position.Add(new Vector3(0f, 1f, 3f));
							Position.Add(new Vector3(0f, 1f, -1f));
							Position.Add(new Vector3(0f, 1f, -1.9f));
							Position.Add(new Vector3(0f, 1f, -2.8f));
							Position.Add(new Vector3(0.9f, 1f, -2.7f));
							Position.Add(new Vector3(-0.9f, 1f, -2.7f));
							Position.Add(new Vector3(0.9f, 1f, -1f));
							Position.Add(new Vector3(-0.9f, 1f, -1f));
							Position.Add(new Vector3(0.9f, 1f, 0f));
							Position.Add(new Vector3(-0.9f, 1f, 0f));
							Position.Add(new Vector3(0.9f, 1f, 1f));
							Position.Add(new Vector3(-0.9f, 1f, 1f));
							Position.Add(new Vector3(0.9f, 1f, 2f));
							Position.Add(new Vector3(-0.9f, 1f, 2f));
							FireOffset = new Vector3(0, 1.3f, 0);
							if (!_ServerSettings.DisableBoatlights) { searchLight = plugin.CreateLightfunction(_boat, new Vector3(0, 1.8f, 4.9f), new Vector3(-5, 180, 0)); }
							break;
						case 1:
							plugin.pirateRowBoat++;
							BoatTurnSpeedfloat = _ServerSettings.RowBoatBoatTurnSpeed;
							BoatMaxSpeedfloat = _ServerSettings.RowBoatBoatMaxSpeed;
							NPCAmountint = _ServerSettings.RowBoatNPCAmount;
							NPCHealthfloat = _ServerSettings.RowBoatNPCHealth;
							_boat.gameObject.GetComponent<BaseCombatEntity>().InitializeHealth(_ServerSettings.RowBoatHealth, _ServerSettings.RowBoatHealth);
							Kit = _ServerSettings.RowBoatKit;
							RandomLoot = _ServerSettings.RowBoatLootString;
							LootMulti = _ServerSettings.RowBoatLootMulti;
							AIShootMultiplyer = _ServerSettings.RowBoatAIDistance;
							Position.Add(new Vector3(0f, 0f, -1.3f));
							Position.Add(new Vector3(0f, 0f, 0f));
							Position.Add(new Vector3(0f, 0f, 1f));
							FireOffset = new Vector3(0, 0.8f, 0);
							if (!_ServerSettings.DisableBoatlights) { searchLight = plugin.CreateLightfunction(_boat, new Vector3(0, 0.8f, 2.2f), new Vector3(-5, 180, 0)); }
							break;
						case 6:
							plugin.pirateSub++;
							BoatTurnSpeedfloat = _ServerSettings.SubmarineTurnSpeed;
							BoatMaxSpeedfloat = _ServerSettings.SubmarineMaxSpeed;
							NPCAmountint = 2;
							NPCHealthfloat = 100;
							_boat.gameObject.GetComponent<BaseCombatEntity>().InitializeHealth(_ServerSettings.SubmarineHealth, _ServerSettings.SubmarineHealth);
							Kit = "";
							RandomLoot = _ServerSettings.SubmarineLootString;
							LootMulti = _ServerSettings.SubmarineLootMulti;
							AIShootMultiplyer = 1;
							Position.Add(new Vector3(0f, 0f, 0f));
							Position.Add(new Vector3(0f, 0f, 0f));
							_sub = (_boat as BaseSubmarine);
							_sub.idleFuelPerSec = 0;
							_sub.maxFuelPerSec = 0;
							_boat.SetFlag(BaseVehicle.Flags.Locked, true);
							break;
					}
					plugin.timer.Once(_ServerSettings.LeaveAfter, () => { leaveing(); });
					try { foreach (StorageContainer container in _boat.children) { if (container != null && container.ToString().Contains("rhib_storage") || container.ToString().Contains("rowboat_storage") || container.ToString().Contains("itemstorage")) { itembox = container; itembox.inventory.capacity = 30; break; } } } catch { }
					if (itembox != null) { plugin.filllootfunction(itembox.inventory, -1, LootMulti, RandomLoot); }
					FindInitialNode();
					npcs = plugin.CreateNPCs(NPCAmountint, Type, NPCHealthfloat, Kit, AIShootMultiplyer, _boat, Position);
					if (npcs == null) { _boat.Kill(); }
					plugin.PirateUintList.Add(_boat.net.ID);
					plugin.timer.Once(Core.Random.Range(2, 10), NPCsAlive);
					if (_sub != null)
					{
						foreach (ScientistNPC n in npcs)
						{
							_sub.AttemptMount(n, false);
							n.inventory.Strip();
						}
						_sub.SetupOwner(npcs[0], Vector3.zero, 1);
						_sub.engineController.TryStartEngine(npcs[0]);
					}
					StorageContainer fuelContainer = (_boat as BaseVehicle).GetFuelSystem().GetFuelContainer();
					if (fuelContainer != null) { fuelContainer.SetFlag(BaseEntity.Flags.Locked, true); }
				});
			}

			private void leaveing()
			{
				if (_boat == null) { return; }
				if (PlayerTarget != null)
				{
					plugin.timer.Once(10f, leaveing);
					return;
				}
				egressing = true;
				plugin.InvalidateCheck(_boat);
				plugin.timer.Once(60f, Die);
			}

			private void OnDestroy()
			{
				try
				{
					switch (Type)
					{
						case 0:
							plugin.pirateRHIB--;
							break;
						case 1:
							plugin.pirateRowBoat--;
							break;
						case 6:
							plugin.pirateSub--;
							break;
					}
					enabled = false;
					CancelInvoke();
					if (npcs != null) { foreach (ScientistNPC npc in npcs) { if (npc != null && !npc.IsDestroyed && !npc.IsDead()) { npc.transform.position = new Vector3(0, -501, 0); npc.Kill(); } } }
					if (searchLight != null) { if (searchLight.resizer != null && !searchLight.resizer.IsDestroyed) { searchLight.resizer.Kill(); } if (searchLight._light != null && !searchLight._light.IsDestroyed) { searchLight._light.Kill(); } }
					if (_boat != null && !_boat.IsDestroyed) { _boat.Kill(); }
				}
				catch { }
			}

			public void Die() { if (this != null) { Destroy(this); } }

			private void Explodevoid() { if (_boat != null) { plugin.Explodefunction(_boat); } }

			private void SubAttack()
			{
				if (_sub == null || PlayerTarget == null) { return; }
				if (_sub.timeSinceTorpedoFired >= _sub.maxFireRate)
				{
					if (PlayerTarget.Distance(_sub) < 50 && PlayerTarget.Distance(_sub) > 5)
					{
						if (!GamePhysics.LineOfSight(_sub.torpedoFiringPoint.position, _sub.torpedoFiringPoint.position + _sub.torpedoFiringPoint.forward * 30f, (int)Layers.Server.Players | 1 << (int)Layers.Server.Buildings | 1 << (int)Layers.Server.Deployed | 1 << (int)Layers.Server.VehiclesSimple))
						{
							float minSpeed = _sub.GetSpeed() + 10f;
							ServerProjectile serverProjectile;
							Item item = ItemManager.CreateByName("submarine.torpedo.straight", 1, 0);
							if (item == null) { return; }
							if (!item.MoveToContainer(_sub.GetTorpedoContainer().inventory, -1, false)) { item.Remove(); }
							if (BaseMountable.TryFireProjectile(_sub.GetTorpedoContainer(), AmmoTypes.TORPEDO, _sub.torpedoFiringPoint.position + _sub.torpedoFiringPoint.forward * 2, _sub.torpedoFiringPoint.forward, npcs[0], 1f, minSpeed, out serverProjectile))
							{
								_sub.timeSinceTorpedoFired = 0f;
								_sub.ClientRPC(null, "TorpedoFired");
							}
						}
					}
				}
			}

			public void FixedUpdate()
			{
				if (_boat == null || targetNodeIndex == -1 || plugin.seanodes == null || loading) { return; }
				try
				{
					if (_sub == null) { _boat.transform.position = new Vector3(_boat.transform.position.x, TerrainMeta.WaterMap.GetHeight(_boat.transform.position), _boat.transform.position.z); }
					_boat.transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
					if (!stopping) { _boat.SetFlag(BaseVehicle.Flags.Reserved1, true); }
					if (!_ServerSettings.DisableBoatlights)
					{
						if (searchLight != null)
						{
							searchLight._light.SetFlag(BaseEntity.Flags.On, TOD_Sky.Instance.IsNight);
							searchLight._light.UpdateHasPower(searchLight._light.IsOn() ? 10 : 0, 1);
						}
						else if (_sub != null) { _sub.SetFlag(BaseSubmarine.Flag_Headlights, TOD_Sky.Instance.IsNight); }
					}
					if (!isDead)
					{
						_boat.SetFlag(BaseVehicle.Flags.Reserved2, true);
						if (!halftickrate) { halftickrate = true; }
						else
						{
							if (_boat.Health() < 10) { Explodevoid(); return; }
							if (_sub != null)
							{
								SubAttack();
								Vector3 IdealHeightvec = new Vector3(_boat.transform.position.x, _ServerSettings.SubmarineTargetDepth * -1, _boat.transform.position.z);
								float GroundHeightFLOAT = TerrainMeta.HeightMap.GetHeight(IdealHeightvec);
								if (IdealHeightvec.y < GroundHeightFLOAT) { IdealHeightvec.y = GroundHeightFLOAT + 0.6f; }
								_boat.transform.position = IdealHeightvec;
							}
							if (!burning)
							{
								if (_boat.Health() < _ServerSettings.StartFireHealth)
								{
									burning = true;
									plugin.CreateFire(60, new Vector3(0, 0.9f, -1.8f), 0.1f, _boat);
								}
							}
							halftickrate = false;
							if (TerrainMeta.HeightMap.GetHeight(_boat.transform.position) > 0.01)
							{
								isDead = true;
								_boat.Invoke(() =>
								{
									BailOutcall();
									plugin.CreateFire(_ServerSettings.ExplodeDelay + 2f, FireOffset, 0, _boat);
									Invoke("Explodevoid", _ServerSettings.ExplodeDelay);
								}, _ServerSettings.CrashDelayFloat);
								return;
							}
						}
						TargetDest0 = plugin.seanodes[targetNodeIndex];
						if (plugin.PointsOfIntrest.Count != 0)
						{
							Vector3 PointOfIntrest = plugin.getPOIfunction(_boat.transform.position, _ServerSettings.PiratesWaterPOIRadius);
							if (PointOfIntrest != Vector3.zero && TerrainMeta.HeightMap.GetHeight(PointOfIntrest) < -5) { TargetDest0 = PointOfIntrest; }
						}
						if (PlayerTarget != null) { TargetDest0 = PlayerTarget.transform.position; }
						if (egressing) { TargetDest0 = base.transform.position + (base.transform.position - Vector3.zero).normalized * 10000f; }
						Vector3 normalized = (TargetDest0 - base.transform.position).normalized;
						float value = Vector3.Dot(base.transform.forward, normalized);
						float b = Mathf.InverseLerp(0f, 1f, value);
						float num = Vector3.Dot(base.transform.right, normalized);
						float b2 = Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(num));
						turnScale = Mathf.Lerp(turnScale, b2, Time.deltaTime * 0.2f);
						float num3 = (float)((num < 0f) ? -1 : 1);
						currentTurnSpeed = BoatTurnSpeedfloat * turnScale * num3;
						base.transform.Rotate(Vector3.up, Time.deltaTime * currentTurnSpeed, Space.World);
						currentThrottle = Mathf.Lerp(currentThrottle, b, Time.deltaTime * 0.2f);
						currentVelocity = base.transform.forward * (BoatMaxSpeedfloat * currentThrottle);
						base.transform.position += currentVelocity * Time.deltaTime;
						if (Vector3.Distance(base.transform.position, TargetDest0) < 80f) { targetNodeIndex++; if (targetNodeIndex >= plugin.seanodes.Count) { targetNodeIndex = 0; } }
					}
				}
				catch { FindInitialNode(); }
			}

			private void BailOutcall()
			{
				if (itembox != null) { itembox.dropFloats = true; itembox.DropItems(); }
				foreach (ScientistNPC npc in npcs.ToArray())
				{
					if (_sub != null) { plugin.CreateGun(npc, "rifle.lr300", "weapon.mod.silencer"); }
					npcs.Remove(npc);
					plugin.BailOutfunction(_boat.transform.position, npc, Vector3.zero, 8);
				}
			}

			private int GetClosestNodeToUs()
			{
				if (plugin.seanodes == null || plugin.seanodes.Count == 0) { plugin.seanodes = TerrainMeta.Path.OceanPatrolFar; }
				int result = 0;
				float num = float.PositiveInfinity;
				for (int i = 0; i < plugin.seanodes.Count; i++)
				{
					Vector3 b = plugin.seanodes[i];
					float num2 = Vector3.Distance(base.transform.position, b);
					if (num2 < num)
					{
						result = i;
						num = num2;
					}
				}
				return result;
			}

			private void FindInitialNode() { targetNodeIndex = GetClosestNodeToUs(); }

			private void NPCsAlive()
			{
				if (npcs == null || _boat == null) { return; }
				int alive = 0;
				foreach (ScientistNPC npc in npcs.ToArray())
				{
					if (npc == null || !npc.IsAlive() || !plugin.ActiveNPCs.ContainsKey(npc)) { continue; }
					if (PlayerTarget == null) { PlayerTarget = plugin.ActiveNPCs[npc]; }
					alive++;
				}
				if (alive == 0)
				{
					plugin.timer.Once(10f, () => { if (_boat != null) { _boat.SetFlag(BaseVehicle.Flags.Reserved1, false); stopping = true; } });
					isDead = true;
					plugin.timer.Once(_ServerSettings.StopDelayFloat, Stop);
					return;
				}
				plugin.timer.Once(_ServerSettings.NPCTick, NPCsAlive);
			}

			private void Stop()
			{
				if (_boat == null) { return; }
				plugin.timer.Once(_ServerSettings.FireDelay + _ServerSettings.ExplodeDelay, Explodevoid);
				plugin.CreateFire(_ServerSettings.ExplodeDelay, FireOffset, _ServerSettings.FireDelay, _boat);
			}
		}

		private class AirPirates : MonoBehaviour
		{
			private enum aiState
			{
				IDLE,
				MOVE,
				ORBIT,
				STRAFE,
				PATROL,
			}

			public List<ScientistNPC> npcs;
			private Dictionary<ScientistNPC, Vector3> NPCOffSet = new Dictionary<ScientistNPC, Vector3>();
			public StorageContainer itembox;
			public BaseEntity _heli;
			private float AiAltitudeForce = 20000f;
			private Vector3 _moveTarget = Vector3.zero;
			public Vector3 currentVelocity = Vector3.zero;
			private int lastAltitudeCheckFrame;
			private float altOverride;
			private float currentDesiredAltitude;
			private bool altitudeProtection = true;
			private float hoverHeight;
			private Vector3 interestZoneOrigin;
			private Vector3 destination;
			public float moveSpeed;
			private float maxSpeed;
			private float courseAdjustLerpTime = 4f;
			private Quaternion targetRotation;
			private float targetThrottleSpeed;
			private float throttleSpeed;
			private float maxRotationSpeed;
			private float rotationSpeed;
			private aiState _currentState;
			private Vector3 _lastPos;
			private Vector3 _lastMoveDir;
			private bool isDead;
			private bool isRetiring;
			private bool burning;
			private float destination_min_dist = 2f;
			private float currentOrbitDistance;
			private float currentOrbitTime;
			private bool hasEnteredOrbit;
			private float orbitStartTime;
			private float maxOrbitDuration = 10f;
			private bool breakingOrbit;
			private Vector3 strafe_target_position;
			private bool puttingDistance;
			private float lastStrafeTime = float.NegativeInfinity;
			private float _lastThinkTime;
			public List<targetinfo> _targetList = new List<targetinfo>();
			private bool stuckcheck = false;
			private Vector3 SpotCheck = new Vector3(0, 0, 0);
			private NightLight searchLight;
			public int Type = 0;
			private Rigidbody _rb;
			private float NPCHealth;
			private string Kit;
			private Vector3 FireOffset;
			private float orbitDistance;
			private float ThreatTarget;
			private float TargetDistance;
			private float AIShootMultiplyer;
			private List<Vector3> Position = new List<Vector3>();
			private int NPCAmountint;
			private string RandomLoot = "";
			private int LootMulti = 1;
			private float NextPOITick;
			private bool halftickrate;

			private void Awake()
			{
				plugin.NextFrame(() =>
				{
					_heli = GetComponent<BaseEntity>();
					_heli.SendNetworkUpdateImmediate();
					NextPOITick = Time.time;
					switch (Type)
					{
						case 2:
							plugin.piratesMini++;
							maxRotationSpeed = _ServerSettings.MiniTurnSpeed;
							maxSpeed = _ServerSettings.MiniMaxSpeed;
							hoverHeight = _ServerSettings.MiniHoverHeight;
							NPCHealth = _ServerSettings.MiniNPCHealth;
							Kit = _ServerSettings.MiniKit;
							orbitDistance = _ServerSettings.MiniOrbitDistance;
							AIShootMultiplyer = _ServerSettings.MiniAIDistance;
							ThreatTarget = _ServerSettings.MiniThreatLevel;
							TargetDistance = _ServerSettings.HeliTargetDistance;
							NPCAmountint = _ServerSettings.MiniNPCAmount;
							gameObject.GetComponent<BaseCombatEntity>().InitializeHealth(_ServerSettings.MiniHealth, _ServerSettings.MiniHealth);
							FireOffset = new Vector3(0, 0.2f, 0);
							itembox = plugin.CreateStoragefunction(new Vector3(0, 0, 0), _heli);
							RandomLoot = _ServerSettings.MiniLootString;
							LootMulti = _ServerSettings.MiniLootMulti;
							Position.Add(new Vector3(0.0f, 0.0f, 0.5f));
							Position.Add(new Vector3(1f, -0.2f, -0.6f));
							Position.Add(new Vector3(-1f, -0.2f, -0.6f));
							Position.Add(new Vector3(0.0f, 0.0f, 1.3f));
							if (!_ServerSettings.DisableHelilights) { searchLight = plugin.CreateLightfunction(_heli, new Vector3(0, 0.24f, 1.8f), new Vector3(-20, 180, 180)); }
							break;
						case 3:
							plugin.pirateScrap++;
							maxRotationSpeed = _ServerSettings.ScrapTurnSpeed;
							maxSpeed = _ServerSettings.ScrapMaxSpeed;
							hoverHeight = _ServerSettings.ScrapHoverHeight;
							NPCHealth = _ServerSettings.ScrapNPCHealth;
							Kit = _ServerSettings.ScrapKit;
							orbitDistance = _ServerSettings.ScrapOrbitDistance;
							AIShootMultiplyer = _ServerSettings.ScrapAIDistance;
							ThreatTarget = _ServerSettings.ScrapThreatLevel;
							TargetDistance = _ServerSettings.HeliTargetDistance;
							NPCAmountint = _ServerSettings.ScrapNPCAmount;
							gameObject.GetComponent<BaseCombatEntity>().InitializeHealth(_ServerSettings.ScrapHealth, _ServerSettings.ScrapHealth);
							FireOffset = new Vector3(0, 0.2f, 0);
							itembox = plugin.CreateStoragefunction(new Vector3(0, 0.8f, 0), _heli);
							RandomLoot = _ServerSettings.ScrapLootString;
							LootMulti = _ServerSettings.ScrapLootMulti;
							Position.Add(new Vector3(0.8f, 0.8f, 2.6f));
							Position.Add(new Vector3(-0.8f, 0.8f, 2.6f));
							Position.Add(new Vector3(-1f, 0.8f, -1.5f));
							Position.Add(new Vector3(1f, 0.8f, -1.5f));
							Position.Add(new Vector3(-1f, 0.8f, -0.5f));
							Position.Add(new Vector3(1f, 0.8f, -0.5f));
							Position.Add(new Vector3(-1f, 0.8f, 0.5f));
							Position.Add(new Vector3(1f, 0.8f, 0.5f));
							Position.Add(new Vector3(-0.9f, 0.4f, -2.7f));
							Position.Add(new Vector3(0.9f, 0.4f, -2.7f));
							Position.Add(new Vector3(0f, 0.4f, -2.7f));
							break;
					}
					_lastPos = transform.position;
					plugin.timer.Once(_ServerSettings.LeaveAfter, () => { Retire(); });
					_rb = GetComponent<Rigidbody>();
					if (itembox != null) { plugin.filllootfunction(itembox.inventory, -1, LootMulti, RandomLoot); ; }
					npcs = plugin.CreateNPCs(NPCAmountint, Type, NPCHealth, Kit, AIShootMultiplyer, _heli, Position);
					if (npcs == null) { _heli.Kill(); }
					foreach (ScientistNPC n in npcs) { NPCOffSet.Add(n, n.transform.localPosition); }
					plugin.PirateUintList.Add(_heli.net.ID);
					StorageContainer fuelContainer = (_heli as MiniCopter).GetFuelSystem().GetFuelContainer();
					fuelContainer.SetFlag(BaseEntity.Flags.Locked, true);
					plugin.timer.Once(Core.Random.Range(5, 10), NPCsAlive);
				});
			}

			public void FixedUpdate()
			{
				if (_heli == null || isDead) { return; }
				MaintainAIAltutide();
				if (!halftickrate) { halftickrate = true; }
				else
				{
					halftickrate = false;
					if (_heli.WaterFactor() > 0.4f) { Explodevoid(); return; }
					if (_heli.transform.position.y < TerrainMeta.HeightMap.GetHeight(_heli.transform.position) + 3f && !isDead)
					{
						isDead = true;
						plugin.CreateFire(_ServerSettings.ExplodeDelay, FireOffset, 0, _heli);
						Invoke("Explodevoid", _ServerSettings.CrashDelayFloat);
					}

					if (_heli.Health() < _ServerSettings.NPCParachuteTrigger)
					{
						if (itembox != null) { itembox.dropFloats = true; itembox.DropItems(); }
						foreach (ScientistNPC npc in npcs.ToArray())
						{
							npcs.Remove(npc);
							if (npc != null && npc.IsAlive() && _heli != null)
							{
								npc.modelState.mounted = false;
								if (_ServerSettings.NPCParachuteBool)
								{
									Vector3 pos = UnityEngine.Random.insideUnitCircle * 8;
									plugin.AttachParachute(npc, _heli.transform.position + new Vector3(pos.x, 0, pos.y));
								}
								else
								{
									if (npc != null && !npc.IsDestroyed) { npc.Die(); }
								}
							}
						}
						Invoke("Explodevoid", 2f);
					}
					if (!burning && Type == 2)
					{
						if (_heli.Health() < _ServerSettings.StartFireHealth)
						{
							burning = true;
							plugin.CreateFire(99, new Vector3(1, 1.9f, 0f), 0.1f, _heli);
						}
					}
					if (npcs != null) { foreach (ScientistNPC npc in npcs) { if (npc == null) { continue; } if (npc.GetParentEntity() == null) { npc.SetParent(_heli, true, true); npc.transform.localPosition = NPCOffSet[npc]; } } }
					CalculateDesiredAltitude();
					UpdateTargetList();
				}

				_heli.SetFlag(BaseEntity.Flags.On, true);
				if (_ServerSettings.DisableHelilights) { _heli.SetFlag(BaseEntity.Flags.Reserved5, false); }
				else { if (searchLight == null) { _heli.SetFlag(BaseEntity.Flags.Reserved5, TOD_Sky.Instance.IsNight); } else { searchLight._light.SetFlag(BaseEntity.Flags.On, TOD_Sky.Instance.IsNight); searchLight._light.UpdateHasPower(searchLight._light.IsOn() ? 10 : 0, 1); } }
				Vector3 vector = Vector3.Lerp(_lastMoveDir, (destination - transform.position).normalized, UnityEngine.Time.deltaTime / courseAdjustLerpTime);
				_lastMoveDir = vector;
				throttleSpeed = Mathf.Lerp(throttleSpeed, targetThrottleSpeed, UnityEngine.Time.deltaTime / 3f);
				float d = throttleSpeed * maxSpeed;
				transform.position += vector * d * UnityEngine.Time.deltaTime;
				moveSpeed = Mathf.Lerp(moveSpeed, Vector3.Distance(_lastPos, transform.position) / UnityEngine.Time.deltaTime, UnityEngine.Time.deltaTime * 2f);
				_lastPos = transform.position;
				AIThink();
				if (plugin.PointsOfIntrest.Count != 0)
				{
					if (NextPOITick < Time.time)
					{
						NextPOITick = Time.time + 10;
						Vector3 POI = plugin.getPOIfunction(_heli.transform.position, _ServerSettings.PiratesAirPOIRadius);
						if (POI != Vector3.zero)
						{
							float groundheightFLOAT = TerrainMeta.HeightMap.GetHeight(_heli.transform.position);
							if (groundheightFLOAT < 20) { groundheightFLOAT = 30; }
							POI.y = groundheightFLOAT;
							SetTargetDestination(POI, 10f, 30f);
							SetIdealRotation(GetYawRotationTo(POI), -1f);
							interestZoneOrigin = POI;
							currentVelocity = _lastMoveDir * (throttleSpeed * maxSpeed);
						}
					}
				}
				rotationSpeed = Mathf.Lerp(rotationSpeed, maxRotationSpeed, UnityEngine.Time.deltaTime / 2f);
				transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * UnityEngine.Time.deltaTime);
				transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
			}

			private void OnDestroy()
			{
				try
				{
					switch (Type)
					{
						case 2:
							plugin.piratesMini--;
							break;
						case 3:
							plugin.pirateScrap--;
							break;
					}
					enabled = false;
					CancelInvoke();
					if (npcs != null) { foreach (ScientistNPC npc in npcs) { if (npc != null && !npc.IsDestroyed && !npc.IsDead()) { npc.transform.position = new Vector3(0, -501, 0); npc.Kill(); } } }
					if (searchLight != null) { if (searchLight.resizer != null && !searchLight.resizer.IsDestroyed) { searchLight.resizer.Kill(); } if (searchLight._light != null && !searchLight._light.IsDestroyed) { searchLight._light.Kill(); } }
					if (_heli != null && !_heli.IsDestroyed) { _heli.Kill(); }
				}
				catch { }
			}

			public void Die() { if (this != null) { Destroy(this); } }

			private void Explodevoid() { plugin.Explodefunction(_heli); }

			private void NPCsAlive()
			{
				if (npcs == null) { if (_heli != null) { plugin.Explodefunction(_heli); } return; }
				int alive = 0;
				foreach (ScientistNPC npc in npcs.ToArray())
				{
					if (npc == null || !npc.IsAlive() || !plugin.ActiveNPCs.ContainsKey(npc)) { continue; }
					if (plugin.ActiveNPCs[npc] != null) { _targetList.Add(new targetinfo(plugin.ActiveNPCs[npc], plugin.ActiveNPCs[npc].ToPlayer())); }
					alive++;
				}
				if (alive == 0 && !isDead)
				{
					isDead = true;
					if (itembox != null) { itembox.dropFloats = true; itembox.DropItems(); plugin.NextFrame(() => { if (!itembox.IsDestroyed) { itembox.Kill(); } }); }
					plugin.timer.Once(_ServerSettings.FireDelay + _ServerSettings.ExplodeDelay, Explodevoid);
					plugin.CreateFire(_ServerSettings.ExplodeDelay, FireOffset, _ServerSettings.FireDelay, _heli);
					return;
				}
				plugin.timer.Once(_ServerSettings.NPCTick, NPCsAlive);
			}


			private float GetThrottleForDistance(float distToTarget)
			{
				float result;
				if (distToTarget >= 75f) { result = 1f; }
				else if (distToTarget >= 50f) { result = 0.75f; }
				else if (distToTarget >= 25f) { result = 0.33f; }
				else if (distToTarget >= 5f) { result = 0.05f; }
				else { result = 0.05f * (1f - distToTarget / 5f); }
				return result;
			}

			private void CalculateDesiredAltitude()
			{
				CalculateOverrideAltitude();
				if (altOverride > currentDesiredAltitude) { currentDesiredAltitude = altOverride; return; }
				currentDesiredAltitude = Mathf.MoveTowards(currentDesiredAltitude, altOverride, Time.fixedDeltaTime * 5f);
			}

			private void OnCurrentStateExit()
			{
				switch (_currentState)
				{
					default:
						return;
					case aiState.MOVE:
						return;
					case aiState.ORBIT:
						breakingOrbit = false;
						hasEnteredOrbit = false;
						currentOrbitTime = 0f;
						return;
					case aiState.STRAFE:
						lastStrafeTime = UnityEngine.Time.realtimeSinceStartup;
						return;
					case aiState.PATROL:
						return;
				}
			}

			private void ExitCurrentState()
			{
				OnCurrentStateExit();
				_currentState = aiState.IDLE;
			}


			private void Retire()
			{
				if (_heli == null) { return; }
				if (isRetiring) { return; }
				plugin.InvalidateCheck(_heli);
				isRetiring = true;
				float x = TerrainMeta.Size.x;
				float y = 200f;
				Vector3 vector = Vector3Ex.Range(-1f, 1f);
				vector.y = 0f;
				vector.Normalize();
				vector *= x * 20f;
				vector.y = y;
				ExitCurrentState();
				_currentState = aiState.MOVE;
				destination_min_dist = 5f;
				SetTargetDestination(vector, 5f, 30f);
				float distToTarget = Vector3.Distance(base.transform.position, destination);
				targetThrottleSpeed = GetThrottleForDistance(distToTarget);
			}

			private void SetIdealRotation(Quaternion newTargetRot, float rotationSpeedOverride = -1f)
			{
				float num = (rotationSpeedOverride == -1f) ? Mathf.Clamp01(moveSpeed / (maxSpeed * 0.5f)) : rotationSpeedOverride;
				rotationSpeed = num * maxRotationSpeed;
				targetRotation = newTargetRot;
			}

			private Quaternion GetYawRotationTo(Vector3 targetDest)
			{
				Vector3 a = targetDest;
				a.y = 0f;
				Vector3 position = base.transform.position;
				position.y = 0f;
				return Quaternion.LookRotation((a - position).normalized);
			}

			private void SetTargetDestination(Vector3 targetDest, float minDist = 5f, float minDistForFacingRotation = 30f)
			{
				destination = targetDest;
				destination_min_dist = minDist;
				float num = Vector3.Distance(targetDest, base.transform.position);
				if (num > minDistForFacingRotation) { SetIdealRotation(GetYawRotationTo(destination), -1f); }
				targetThrottleSpeed = GetThrottleForDistance(num);
			}

			private bool AtDestination() { return Vector3.Distance(base.transform.position, destination) < destination_min_dist; }

			private float CalculateOverrideAltitude()
			{
				if (Time.frameCount == lastAltitudeCheckFrame) { return altOverride; }
				lastAltitudeCheckFrame = Time.frameCount;
				float y = _moveTarget.y;
				float num = Mathf.Max(TerrainMeta.WaterMap.GetHeight(new Vector3(_moveTarget.x, 0, _moveTarget.z)), TerrainMeta.HeightMap.GetHeight(new Vector3(_moveTarget.x, 0, _moveTarget.z)));
				float num2 = Mathf.Max(y, num + hoverHeight);
				if (altitudeProtection)
				{
					Vector3 rhs = (_rb.velocity.magnitude < 0.1f) ? base.transform.forward : _rb.velocity.normalized;
					Vector3 normalized = (Vector3.Cross(Vector3.Cross(base.transform.up, rhs), Vector3.up) + Vector3.down * 0.3f).normalized;
					RaycastHit raycastHit;
					RaycastHit raycastHit2;
					if (Physics.SphereCast(base.transform.position - normalized * 20f, 50f, normalized, out raycastHit, 200f, 1134625043) && Physics.SphereCast(raycastHit.point + Vector3.up * 200f, 50f, Vector3.down, out raycastHit2, 200f, 1134625043)) { num2 = raycastHit2.point.y + hoverHeight; }
				}
				altOverride = num2;
				return altOverride;
			}

			private void MaintainAIAltutide()
			{
				float groundheightFLOAT = TerrainMeta.HeightMap.GetHeight(_heli.transform.position);
				if (currentDesiredAltitude < groundheightFLOAT + 10f) { currentDesiredAltitude = groundheightFLOAT + 15f; }
				Vector3 ptr = transform.position + _rb.velocity;
				float value = Mathf.Abs(currentDesiredAltitude - ptr.y);
				bool flag = currentDesiredAltitude > ptr.y;
				float d = Mathf.InverseLerp(0f, 12f, value) * AiAltitudeForce * (flag ? 1f : -1f);
				_rb.AddForce(Vector3.up * d, ForceMode.Force);
			}

			private bool PlayerVisible(BasePlayer ply)
			{
				Vector3 position = ply.eyes.position;
				if (TOD_Sky.Instance.IsNight && Vector3.Distance(position, interestZoneOrigin) > 40f) { return false; }
				Vector3 vector = base.transform.position - Vector3.up * 6f;
				float num = Vector3.Distance(position, vector);
				Vector3 normalized = (position - vector).normalized;
				RaycastHit raycastHit;
				return GamePhysics.Trace(new Ray(vector + normalized * 5f, normalized), 0f, out raycastHit, num * 1.1f, 1218652417, QueryTriggerInteraction.UseGlobal) && raycastHit.collider.gameObject.ToBaseEntity() == ply;
			}

			private void UpdateTargetList()
			{
				Vector3 strafePos = Vector3.zero;
				bool attacktarget = false;
				for (int i = _targetList.Count - 1; i >= 0; i--)
				{
					targetinfo targetinfo = _targetList[i];
					if (targetinfo == null || targetinfo.ent == null) { _targetList.Remove(targetinfo); }
					else
					{
						if (UnityEngine.Time.realtimeSinceStartup > targetinfo.nextLOSCheck)
						{
							targetinfo.nextLOSCheck = UnityEngine.Time.realtimeSinceStartup + 1f;
							if (PlayerVisible(targetinfo.ply))
							{
								targetinfo.lastSeenTime = UnityEngine.Time.realtimeSinceStartup;
								targetinfo.visibleFor += 1f;
							}
							else { targetinfo.visibleFor = 0f; }
						}
						bool targetdead = targetinfo.ply ? targetinfo.ply.IsDead() : (targetinfo.ent.Health() <= 0f);
						if (UnityEngine.Time.realtimeSinceStartup - targetinfo.lastSeenTime >= 6f || targetdead)
						{
							if (UnityEngine.Time.realtimeSinceStartup - lastStrafeTime >= 5f && _currentState != aiState.STRAFE && !isDead && !attacktarget && !targetdead && UnityEngine.Random.Range(0f, 1f) >= 0f)
							{
								attacktarget = true;
								strafePos = targetinfo.ply.transform.position;
							}
							_targetList.Remove(targetinfo);
						}
					}
				}
				foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
				{
					if (!basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.SafeZone) && Vector3Ex.Distance2D(base.transform.position, basePlayer.transform.position) <= TargetDistance)
					{
						bool Target = false;
						using (List<targetinfo>.Enumerator enumerator2 = _targetList.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.ply == basePlayer)
								{
									Target = true;
									break;
								}
							}
						}
						if (!Target && basePlayer.GetThreatLevel() > ThreatTarget && PlayerVisible(basePlayer)) { _targetList.Add(new targetinfo(basePlayer, basePlayer)); }
					}
				}
				if (attacktarget)
				{
					ExitCurrentState();
					lastStrafeTime = UnityEngine.Time.realtimeSinceStartup;
					_currentState = aiState.STRAFE;
					Vector3 vector;
					Vector3 vector2;
					if (TransformUtil.GetGroundInfo(strafePos, out vector, out vector2, 100f, 1134625043, base.transform)) { strafe_target_position = vector; }
					else { strafe_target_position = strafePos; }
					Vector3 randomOffset = GetRandomOffset(strafePos, 100f, 192.5f, 20f, 30f);
					SetTargetDestination(randomOffset, 10f, 30f);
					SetIdealRotation(GetYawRotationTo(randomOffset), -1f);
					puttingDistance = true;
				}
			}

			private Vector3 GetRandomOffset(Vector3 origin, float minRange, float maxRange = 0f, float minHeight = 20f, float maxHeight = 30f)
			{
				Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
				onUnitSphere.y = 0f;
				onUnitSphere.Normalize();
				maxRange = Mathf.Max(minRange, maxRange);
				Vector3 origin2 = origin + onUnitSphere * UnityEngine.Random.Range(minRange, maxRange);
				return GetAppropriatePosition(origin2, minHeight, maxHeight);
			}

			private Vector3 GetAppropriatePosition(Vector3 origin, float minHeight = 20f, float maxHeight = 30f)
			{
				float num = 100f;
				Ray ray = new Ray(origin + new Vector3(0f, num, 0f), Vector3.down);
				float num2 = 5f;
				RaycastHit raycastHit;
				if (UnityEngine.Physics.SphereCast(ray, num2, out raycastHit, num * 2f - num2, 1134625043)) { origin = raycastHit.point; }
				origin.y += UnityEngine.Random.Range(minHeight, maxHeight);
				return origin;
			}

			private void stuck()
			{
				Vector3 Checkpos = _heli.transform.position;
				Checkpos.y = 0;
				SpotCheck.y = 0;
				if (Vector3.Distance(Checkpos, SpotCheck) < 5f)
				{
					if (plugin.airnodes.Count <= 1) { SetTargetDestination(plugin.airnodes.GetRandom() + new Vector3(0f, 40f, 0f), 10f, 30f); }
					else { SetTargetDestination(plugin.airnodes[UnityEngine.Random.Range(0, plugin.airnodes.Count)], 10f, 30f); }
					_currentState = aiState.MOVE;
				}
				stuckcheck = false;
			}

			private void AIThink()
			{
				float time = UnityEngine.Time.realtimeSinceStartup;
				float timePassed = time - _lastThinkTime;
				_lastThinkTime = time;
				if (!stuckcheck)
				{
					SpotCheck = _heli.transform.position;
					stuckcheck = true;
					Invoke("stuck", 10f);
				}
				switch (_currentState)
				{
					default:
						ExitCurrentState();
						_currentState = aiState.PATROL;
						destination = plugin.airnodes.GetRandom();
						SetIdealRotation(GetYawRotationTo(destination), -1f);
						return;
					case aiState.MOVE:
						float distToTarget = Vector3.Distance(base.transform.position, destination);
						targetThrottleSpeed = GetThrottleForDistance(distToTarget);
						if (AtDestination())
						{
							ExitCurrentState();
							_currentState = aiState.IDLE;
						}
						return;
					case aiState.ORBIT:
						State_Orbit_Think(timePassed);
						return;
					case aiState.STRAFE:
						if (puttingDistance)
						{
							if (AtDestination())
							{
								puttingDistance = false;
								SetTargetDestination(strafe_target_position + new Vector3(0f, 40f, 0f), 10f, 30f);
								SetIdealRotation(GetYawRotationTo(strafe_target_position), -1f);
								return;
							}
						}
						else { SetIdealRotation(GetYawRotationTo(strafe_target_position), -1f); }
						return;
					case aiState.PATROL:
						float num = Vector3.Distance(base.transform.position, destination);
						if (num <= 25f) { targetThrottleSpeed = GetThrottleForDistance(num); }
						else { targetThrottleSpeed = 0.5f; }
						if (AtDestination())
						{
							ExitCurrentState();
							State_Orbit_Enter();
						}
						if (_targetList.Count > 0)
						{
							interestZoneOrigin = _targetList[0].ply.transform.position + new Vector3(0f, 20f, 0f);
							ExitCurrentState();
							State_Orbit_Enter();
						}
						return;
				}
			}

			private void State_Orbit_Think(float timePassed)
			{
				if (breakingOrbit)
				{
					if (AtDestination())
					{
						ExitCurrentState();
						_currentState = aiState.IDLE;
					}
				}
				else
				{
					if (Vector3Ex.Distance2D(base.transform.position, destination) > 15f) { return; }
					if (!hasEnteredOrbit)
					{
						hasEnteredOrbit = true;
						orbitStartTime = UnityEngine.Time.realtimeSinceStartup;
					}
					float num = 6.28318548f * currentOrbitDistance;
					float num2 = 0.5f * maxSpeed;
					float num3 = num / num2;
					currentOrbitTime += timePassed / (num3 * 1.01f);
					float rate = currentOrbitTime;
					Vector3 orbitPosition = GetOrbitPosition(rate);
					SetTargetDestination(orbitPosition, 0f, 1f);
					targetThrottleSpeed = 0.5f;
				}
				if (UnityEngine.Time.realtimeSinceStartup - orbitStartTime > maxOrbitDuration && !breakingOrbit)
				{
					breakingOrbit = true;
					Vector3 appropriatePosition = GetAppropriatePosition(base.transform.position + base.transform.forward * 75f, 40f, 50f);
					SetTargetDestination(appropriatePosition, 10f, 0f);
				}
			}

			private Vector3 GetOrbitPosition(float rate)
			{
				float x = Mathf.Sin(6.28318548f * rate) * currentOrbitDistance;
				float z = Mathf.Cos(6.28318548f * rate) * currentOrbitDistance;
				Vector3 vector = new Vector3(x, 20f, z);
				vector = interestZoneOrigin + vector;
				return vector;
			}

			private void State_Orbit_Enter()
			{
				_currentState = aiState.ORBIT;
				breakingOrbit = false;
				hasEnteredOrbit = false;
				orbitStartTime = UnityEngine.Time.realtimeSinceStartup;
				Vector3 vector = base.transform.position - interestZoneOrigin;
				currentOrbitTime = Mathf.Atan2(vector.x, vector.z);
				currentOrbitDistance = orbitDistance;
				SetTargetDestination(GetOrbitPosition(currentOrbitTime), 20f, 0f);
			}
		}

		public class LandPirate : MonoBehaviour
		{
			private BaseVehicle _rideable;
			private Snowmobile sm;
			private NavMeshAgent agent;
			public ScientistNPC npc;
			public ItemContainer itembox;
			public Rigidbody rb;
			private bool isleaving = false;
			private float RomeRange;
			private bool CanRoam;
			private float MoveSpeed;
			private float NextRoamTick;
			private float MoveDelay;
			private float FindingPoint;
			private bool PutDistance;
			private float npchealth;
			private float ridablehealth;
			private string RandomLoot = "";
			private int LootMulti = 1;
			private string Kit = "";
			private float AimMulti;
			private NightLight searchLight;
			private List<Vector3> NavMeshPoints;
			public BaseEntity PlayerTarget;
			public int Type;
			private Rigidbody _rb;
			private InputState input = new InputState();
			private InputMessage im = new InputMessage();
			private bool IsMoving = false;
			private Vector3 lastpos;
			private bool halftickrate;

			private void Awake()
			{
				plugin.NextTick(() =>
				{
					try { _rideable = GetComponent<BaseVehicle>(); } catch { Die(); }
					try { rb = GetComponent<Rigidbody>(); } catch { }
					agent = _rideable.gameObject.AddComponent<NavMeshAgent>();
					_rideable.enableSaving = false;
					MoveDelay = Time.time;
					NextRoamTick = Time.time + 5;
					switch (Type)
					{
						case 4:
							plugin.pirateHorse++;
							npchealth = _ServerSettings.HorseNPCHealth;
							ridablehealth = _ServerSettings.HorseHealth;
							RomeRange = _ServerSettings.HorseRomeRange;
							CanRoam = _ServerSettings.HorseCanRoam;
							MoveSpeed = _ServerSettings.HorseMoveSpeed;
							Kit = _ServerSettings.HorseKit;
							AimMulti = _ServerSettings.HorseAimMulti;
							RandomLoot = _ServerSettings.HorseLootString;
							LootMulti = _ServerSettings.HorseLootMulti;
							RidableHorse horse = _rideable as RidableHorse;
							itembox = horse.inventory;
							AttachLight();
							break;
						case 5:
							plugin.pirateSnowMobile++;
							npchealth = _ServerSettings.SnowMobileNPCHealth;
							ridablehealth = _ServerSettings.SnowMobileHealth;
							RomeRange = _ServerSettings.SnowMobileRomeRange;
							CanRoam = _ServerSettings.SnowMobileCanRoam;
							MoveSpeed = _ServerSettings.SnowMobileMoveSpeed;
							Kit = _ServerSettings.SnowMobileKit;
							AimMulti = _ServerSettings.SnowMobileAimMulti;
							RandomLoot = _ServerSettings.SnowMobileLootString;
							LootMulti = _ServerSettings.SnowMobileLootMulti;
							sm = _rideable as Snowmobile;
							im.buttons = 2;
							input.current = im;
							sm.engineKW = 50;
							sm.collisionEffect.guid = null;
							sm.badTerrainDrag = 0.5f;
							sm.idleFuelPerSec = 0f;
							sm.maxFuelPerSec = 0f;
							StorageContainer fuelContainer = sm.GetFuelSystem()?.GetFuelContainer();
							if (fuelContainer != null)
							{
								fuelContainer.inventory.AddItem(fuelContainer.allowedItem, 1);
								fuelContainer.SetFlag(BaseEntity.Flags.Locked, true);
							}
							itembox = sm.GetItemContainer().inventory;
							break;
					}
					_rideable.InitializeHealth(ridablehealth, ridablehealth);
					npc = plugin.CreateNPCs(1, Type, npchealth, Kit, AimMulti, _rideable, new List<Vector3> { new Vector3(0f, 0f, 0f) })[0];
					_rideable.AttemptMount(npc, false);
					NavMeshPoints = plugin.FindNavPoints(_rideable.transform.position, RomeRange);
					plugin.PirateUintList.Add(_rideable.net.ID);
					if (itembox != null) { itembox.capacity = 30; plugin.filllootfunction(itembox, -1, LootMulti, RandomLoot); }
					plugin.timer.Once(Core.Random.Range(2, 10), NPCsAlive);
					plugin.timer.Once(_ServerSettings.LeaveAfter, () => { Leave(); });
					agent.autoTraverseOffMeshLink = true;
					agent.autoRepath = true;
					agent.autoBraking = true;
					_rb = GetComponent<Rigidbody>();
					RaycastHit raycastHit;
					if (Physics.SphereCast(agent.transform.position + new Vector3(0, 5, 0), 10f, Vector3.down, out raycastHit, 10f, 1134625043) || Physics.SphereCast(agent.transform.position + new Vector3(0, 5, 0), 10f, Vector3.up, out raycastHit, 10f, 1134625043))
					{
						_rideable.transform.position = raycastHit.point;
						agent.transform.position = raycastHit.point;
					}
				});
			}

			private void Leave()
			{
				if (_rideable == null) { return; }
				if (PlayerTarget != null)
				{
					plugin.timer.Once(10f, Leave);
					return;
				}
				if (agent != null)
				{
					agent.isStopped = true;
				}
				if (npc != null) { plugin.NPCsUintList.Remove(npc.net.ID); }
				plugin.timer.Once(30f, () =>
				{
					if (_rideable == null) { return; }
					plugin.FreeLists(_rideable);
					if (npc != null && !npc.IsDestroyed && !npc.IsDead()) { npc.Kill(); }
					if (!_rideable.IsDestroyed) { if (itembox != null) { itembox.Clear(); } _rideable.transform.position = Vector3.zero; plugin.NextFrame(() => { if (_rideable != null && !_rideable.IsDestroyed) { _rideable.Kill(); } }); }
				});
			}

			private void AttachLight() { plugin.timer.Once(8f, () => { if (!_ServerSettings.DisableLandlights && _rideable != null) { searchLight = plugin.CreateLightfunction(_rideable, new Vector3(0, 1.8f, 1.2f), new Vector3(-20, 180, 180)); } }); }

			private void NPCsAlive()
			{
				if (_rideable == null || isleaving) { return; }
				if (npc != null && npc.IsAlive() && plugin.ActiveNPCs.ContainsKey(npc))
				{
					if (!npc.IsMounted() && _rideable._health > 20) { _rideable.AttemptMount(npc, false); }
					if (PlayerTarget == null) { PlayerTarget = plugin.ActiveNPCs[npc]; }
					plugin.timer.Once(_ServerSettings.NPCTick, NPCsAlive);
					return;
				}
				PlayerTarget = null;
				DropLoot();
				plugin.timer.Once(_ServerSettings.StopDelayFloat, Stop);
			}

			private void Stop()
			{
				IsMoving = false;
				plugin.timer.Once(_ServerSettings.FireDelay + _ServerSettings.ExplodeDelay, () => { try { Explodevoid(); } catch { } });
				plugin.CreateFire(_ServerSettings.ExplodeDelay, new Vector3(0, 0, 0), _ServerSettings.FireDelay, _rideable);
			}

			private void Explodevoid() { if (_rideable == null) { return; } if (sm != null) { plugin.Explodefunction(sm); } else { if (searchLight != null) { searchLight._light.SetParent(null, true, true); searchLight.resizer.SetParent(null, true, true); if (!searchLight._light.IsDestroyed) { searchLight._light.Kill(); } if (!searchLight.resizer.IsDestroyed) { searchLight.resizer.Kill(); } } plugin.NextFrame(() => { _rideable.Die(); }); } }

			private void setdestination(Vector3 movePoint, float speed = 8f)
			{
				if (agent.isOnNavMesh)
				{
					if (agent.isStopped) agent.isStopped = false;
					if (agent.speed != speed) agent.speed = speed;
					IsMoving = true;
					if (TerrainMeta.HeightMap.GetHeight(new Vector3(movePoint.x, 0, movePoint.z)) > -0.6f) { agent.SetDestination(movePoint); }
					return;
				}
				RaycastHit raycastHit;
				if (Physics.SphereCast(agent.transform.position + new Vector3(0, 5, 0), 10f, Vector3.down, out raycastHit, 10f, 1134625043) || Physics.SphereCast(agent.transform.position + new Vector3(0, 5, 0), 10f, Vector3.up, out raycastHit, 10f, 1134625043))
				{
					agent.transform.position = raycastHit.point;
					agent.areaMask = 1134625043;
					agent.agentTypeID = -1372625422;
					return;
				}
			}

			private void DropLoot() { if (sm != null) { if (itembox != null) { itembox.Drop(buoyantentityprefab, _rideable.transform.position + new Vector3(0, 5, 0), _rideable.ServerRotation); itembox = null; } } }

			void BailOutcall()
			{
				DropLoot();
				plugin.BailOutfunction(_rideable.transform.position, npc, Vector3.zero, 3);
				npc = null;
			}

			public void FixedUpdate()
			{
				if (_rideable == null || rb == null || isleaving) { return; }
				try
				{
					if (!halftickrate) { halftickrate = true; }
					else
					{
						if (npc == null || npc.IsDestroyed || !_rideable.IsMounted()) { if (_rideable != null) { _rideable.SetFlag(BaseVehicle.Flags.On, false); } return; }
						halftickrate = false;
						float GroundH = TerrainMeta.HeightMap.GetHeight(_rideable.transform.position);
						if (_rideable.transform.position.y + 0.5f < GroundH) { _rideable.transform.position = new Vector3(_rideable.transform.position.x, GroundH + 0.5f, _rideable.transform.position.z); }
						if (_rideable.WaterFactor() > 0.4f) { _rb.velocity *= -1; }
						if (_rideable.WaterFactor() > 0.6f) { Explodevoid(); return; }
						if (_rideable.Health() <= 40) { BailOutcall(); }
						if (_rideable.Health() <= 10) { Explodevoid(); }
						if (sm != null)
						{
							if (agent == null) { return; }
							if (Vector3.Distance(agent.destination, npc.transform.position) > 8 && IsMoving) { im.buttons = 2; }
							else { im.buttons = 0; IsMoving = false; }
							sm.PlayerServerInput(input, npc);
							sm.SetFlag(Snowmobile.Flags.On, true);
						}
						if (PlayerTarget != null)
						{
							float distance = Vector3.Distance(_rideable.transform.position, PlayerTarget.transform.position);
							if (!PutDistance && distance < 10)
							{
								PutDistance = true;
								plugin.timer.Once(8f, () => { PutDistance = false; });
							}
							if (PutDistance)
							{
								if (FindingPoint < Time.time)
								{
									Vector3 point = plugin.FindNavPoints(PlayerTarget.transform.position, 20).GetRandom();
									setdestination(point, 6f);
									FindingPoint = Time.time + 5;
								}
								return;
							}
							if (MoveDelay < Time.time)
							{
								MoveDelay = Time.time + 5;
								if (Vector3.Distance(_rideable.transform.position, PlayerTarget.transform.position) > 20)
								{
									if (npc != null) { npc.SetAimDirection(PlayerTarget.transform.position - npc.transform.position); npc.SendNetworkUpdateImmediate(); }
									setdestination(PlayerTarget.transform.position, 8f);
								}
							}
						}
						else
						{
							if (CanRoam && npc != null && NextRoamTick < Time.time)
							{
								if (Vector3.Distance(lastpos, _rideable.transform.position) < 5f) { IsMoving = true; }
								lastpos = _rideable.transform.position;
								NextRoamTick = Time.time + UnityEngine.Random.Range(10, 30);
								if (plugin.PointsOfIntrest.Count != 0)
								{
									Vector3 PointOfIntrest = plugin.getPOIfunction(lastpos, _ServerSettings.PiratesLandPOIRadius);
									if (PointOfIntrest != Vector3.zero && TerrainMeta.HeightMap.GetHeight(PointOfIntrest) > 5f)
									{
										if (npc != null) { npc.SetAimDirection(PointOfIntrest - npc.transform.position); npc.SendNetworkUpdateImmediate(); }
										setdestination(PointOfIntrest, MoveSpeed);
										return;
									}
								}
								if (NavMeshPoints != null && NavMeshPoints.Count > 0)
								{
									Vector3 point = NavMeshPoints.GetRandom();
									if (npc != null) { npc.SetAimDirection(point - npc.transform.position); npc.SendNetworkUpdateImmediate(); }
									setdestination(point, MoveSpeed);
								}
								else { NavMeshPoints = plugin.FindNavPoints(_rideable.transform.position, RomeRange); }
							}
						}
					}
					if (sm != null) { if (!_ServerSettings.DisableLandlights) { sm.SetFlag(Snowmobile.Flag_Headlights, TOD_Sky.Instance.IsNight); } }
					else { if (!_ServerSettings.DisableLandlights) { if (searchLight != null) { searchLight._light.SetFlag(BaseEntity.Flags.On, TOD_Sky.Instance.IsNight); searchLight._light.UpdateHasPower(searchLight._light.IsOn() ? 10 : 0, 1); } } }
				}
				catch { }
			}

			private void OnDestroy()
			{
				try
				{
					switch (Type)
					{
						case 4:
							plugin.pirateHorse--;
							break;
						case 5:
							plugin.pirateSnowMobile--;
							break;
					}
					enabled = false;
					if (npc != null && !npc.IsDestroyed && !npc.IsDead()) { npc.transform.position = new Vector3(0, -501, 0); npc.Kill(); }
					if (searchLight != null) { if (searchLight.resizer != null && !searchLight.resizer.IsDestroyed) { searchLight.resizer.Kill(); } if (searchLight._light != null && !searchLight._light.IsDestroyed) { searchLight._light.Kill(); } }
					if (_rideable != null && !_rideable.IsDestroyed) { _rideable.transform.position = new Vector3(0, -501, 0); _rideable.Kill(); }
					CancelInvoke();
				}
				catch { }
			}

			private void Die() { if (this != null) { Destroy(this); } }
		}

		private class RailPirate : MonoBehaviour
		{
			public int Type = 8;
			public BaseEntity _trainbe;
			public TrainEngine _trainEngine;
			public TrainCar _trainCar;
			public List<BaseEntity> wagons = new List<BaseEntity>();
			private bool burning;
			private bool isDead;
			private bool stopping;
			private bool loading = true;
			public List<ScientistNPC> npcs;
			public StorageContainer itembox;
			private int NPCAmountint;
			private string RandomLoot = "";
			private int LootMulti = 1;
			private List<Vector3> Position = new List<Vector3>();
			private float NPCHealth;
			private string Kit;
			private float AIShootMultiplyer = 1;
			private Vector3 FireOffset;
			public BaseEntity PlayerTarget;
			private bool halftickrate;
			private float TargetSpeed = -8;
			private float CollisionCheck;
			private bool exploding = false;
			private Vector3 curretpos;
			private ScientistNPC Driver;

			private void Awake()
			{
				plugin.NextFrame(() =>
				{
					_trainbe = GetComponent<BaseEntity>();
					if (_trainbe == null) { return; }
					plugin.pirateRail++;
					plugin.TrainsBaseEntityList.Add(_trainbe);
					if (_trainbe.transform == null) { _trainbe.Kill(); return; }
					curretpos = _trainbe.transform.position;
					_trainEngine = _trainbe as TrainEngine;
					_trainCar = _trainbe as TrainCar;
					if (_trainEngine == null || _trainCar == null) { _trainbe.Kill(); return; }
					_trainCar.frontCollisionTrigger.interestLayers = Layers.Mask.Vehicle_World | Layers.Mask.Construction | Layers.Mask.Deployed;
					_trainCar.rearCollisionTrigger.interestLayers = Layers.Mask.Vehicle_World | Layers.Mask.Construction | Layers.Mask.Deployed;
					plugin.NextFrame(() => { _trainEngine.CancelInvoke("DecayTick"); });
					TargetSpeed = _ServerSettings.PirateRailTrainSpeed * -1f;
					_trainCar.completeTrain.trackSpeed = TargetSpeed;
					CollisionCheck = Time.time;
					AIShootMultiplyer = _ServerSettings.PirateRailAimMulti;
					NPCAmountint = _ServerSettings.PirateRailNPCAmount;
					NPCHealth = _ServerSettings.PirateBaseNPCHealth;
					plugin.timer.Once(10, () => { loading = false; });
					(_trainbe as BaseCombatEntity).InitializeHealth(_ServerSettings.PirateRailHealth, _ServerSettings.PirateRailHealth);
					Kit = _ServerSettings.PirateRailNPCKit;
					RandomLoot = _ServerSettings.PirateRailLootString;
					LootMulti = _ServerSettings.PirateRailLootMulti;
					Position.Add(new Vector3(0.9f, 1.5f, 2.5f));
					Position.Add(new Vector3(0f, 1.5f, -1.9f));
					Position.Add(new Vector3(0f, 1.5f, -3.8f));
					Position.Add(new Vector3(0.9f, 1.5f, -3.7f));
					Position.Add(new Vector3(-0.9f, 1.5f, -3.7f));
					Position.Add(new Vector3(0f, 1.5f, -2.8f));
					Position.Add(new Vector3(0.9f, 1.5f, -2.7f));
					Position.Add(new Vector3(-0.9f, 1.5f, -2.7f));
					Position.Add(new Vector3(0.9f, 1.5f, -1f));
					Position.Add(new Vector3(-0.9f, 1.5f, -1f));
					FireOffset = new Vector3(0, 1.3f, 0);
					plugin.CreateWindowfunction(new Vector3(1.393f, 2.321f, 3.374f), new Vector3(90.000f, 0, 0), _trainbe);
					plugin.CreateWindowfunction(new Vector3(1.531f, 2.334f, 4.584f), new Vector3(90.000f, 270.000f, 0), _trainbe);
					plugin.CreateWindowfunction(new Vector3(0.961f, 2.339f, 4.584f), new Vector3(90.000f, 270.000f, 0), _trainbe);
					plugin.CreateWindowfunction(new Vector3(-0.195f, 2.341f, 4.584f), new Vector3(90.0000f, 270.000f, 0), _trainbe);
					plugin.CreateWindowfunction(new Vector3(1.393f, 2.321f, 2.203f), new Vector3(90.000f, 0, 0), _trainbe);
					plugin.CreateWindowfunction(new Vector3(1.393f, 2.321f, 1.024f), new Vector3(90.000f, 0, 0), _trainbe);
					plugin.CreateWindowfunction(new Vector3(-1.415f, 2.321f, 1.024f), new Vector3(90.000f, 0, 0), _trainbe);
					plugin.CreateWindowfunction(new Vector3(-1.415f, 2.321f, 2.203f), new Vector3(90.000f, 0, 0), _trainbe);
					plugin.CreateWindowfunction(new Vector3(-1.415f, 2.321f, 3.374f), new Vector3(90.000f, 0, 0), _trainbe);
					plugin.CreateWindowfunction(new Vector3(-1.415f, 2.321f, 3.374f), new Vector3(90.000f, 0, 0), _trainbe);
					plugin.timer.Once(_ServerSettings.LeaveAfter, () => { leaveing(); });
					itembox = plugin.CreateStoragefunction(new Vector3(0, 1.45f, 0), _trainbe);
					if (itembox != null) { plugin.filllootfunction(itembox.inventory, -1, LootMulti, RandomLoot); }
					npcs = plugin.CreateNPCs(NPCAmountint, Type, NPCHealth, Kit, AIShootMultiplyer, _trainbe, Position);
					if (npcs == null) { _trainbe.Kill(); }
					Driver = npcs[0];
					if (npcs.Count == 0) { plugin.NPCSit(Driver, _trainEngine); }
					plugin.timer.Once(2, () => { Driver.InitializeHealth(_ServerSettings.PirateRailNPCDriverHealth, _ServerSettings.PirateRailNPCDriverHealth); });
					plugin.PirateUintList.Add(_trainbe.net.ID);
					plugin.timer.Once(11f, () =>
					{
						_trainEngine.AttemptMount(Driver, false);
						_trainEngine.engineController.TryStartEngine(Driver);
					});
					plugin.timer.Once(Core.Random.Range(5, 10), NPCsAlive);
					StorageContainer fuelContainer = (_trainbe as BaseVehicle).GetFuelSystem().GetFuelContainer();
					if (fuelContainer != null) { fuelContainer.SetFlag(BaseEntity.Flags.Locked, true); }
					TrainTrackSpline trainTrack;
					float splineDistance;
					if (TrainTrackSpline.TryFindTrackNear(_trainCar.transform.position, 10f, out trainTrack, out splineDistance))
					{
						TrainTrackSpline currentTrack = trainTrack;
						if (currentTrack == null) { return; }
						Vector3 lastCheckPosition = currentTrack.GetPosition(splineDistance);
						Vector3 checkForward = currentTrack.GetTangentCubicHermiteWorld(splineDistance);
						TrainCar lastSpawnedCar = _trainCar;
						TrainCar activeTrain;
						for (int i = 0; i < _ServerSettings.PirateRailWagonAmount; i++)
						{
							try
							{
								lastCheckPosition = plugin.GetPositionOnFromfunction(ref currentTrack, checkForward, lastCheckPosition, 21, out checkForward);
								if (!plugin.HasSpaceToSpawnfunction(currentTrack, lastCheckPosition)) { break; }
								Action<TrainCar> disableCoupling = i == _ServerSettings.PirateRailWagonAmount ? new Action<TrainCar>((TrainCar t) => t.rearCoupling = null) : null;
								char Letter = plugin.GetLetterfunction();
								if (Letter == 'd') { Letter = 'a'; }
								if (SpawnEntity(wagonentsprefab.Replace('$', Letter), lastCheckPosition, Quaternion.LookRotation(_trainCar.transform.forward), out activeTrain, disableCoupling))
								{
									plugin.BaseParts.Add(activeTrain.net.ID);
									wagons.Add(activeTrain);
									float distToMove = Vector3Ex.Distance2D(lastSpawnedCar.rearCoupling.position, activeTrain.frontCoupling.position);
									activeTrain.MoveFrontWheelsAlongTrackSpline(activeTrain.FrontTrackSection, activeTrain.FrontWheelSplineDist, distToMove, null, 0);
									activeTrain.coupling.frontCoupling.TryCouple(lastSpawnedCar.coupling.rearCoupling, true);
									lastSpawnedCar = activeTrain;
									Vector3[] WagonPos;
									switch (Letter)
									{
										case 'a':
											WagonPos = new Vector3[] { new Vector3(0.0f, 1.5f, 2.5f), new Vector3(0.0f, 1.5f, 4.5f) };
											npcs.AddRange(plugin.CreateNPCs(WagonPos.Length, 8, _ServerSettings.PirateRailNPCHealth, _ServerSettings.PirateRailNPCKit, AIShootMultiplyer, activeTrain, WagonPos.ToList()));
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(1.674f, 0.857f, 3.30f), Vector3.zero, activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(-1.619f, 0.857f, 3.142f), new Vector3(0, 180f, 0), activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(-1.619f, 0.857f, -3.226f), new Vector3(0, 180f, 0), activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(0f, 0.857f, -4.781f), new Vector3(0, 90f, 0), activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(0f, 0.857f, 6.097f), new Vector3(0, 90f, 0), activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(1.486f, 3.889f, -3.079f), new Vector3(0, 0, 90f), activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(1.486f, 3.889f, -0.378f), new Vector3(0, 0, 90f), activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(1.486f, 3.889f, 2.318f), new Vector3(0, 0, 90f), activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(1.486f, 3.889f, 5.014f), new Vector3(0, 0, 90f), activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateTurretfunction(new Vector3(-0.808f, 1.546f, -5.566f), new Vector3(0, 90f, 0), activeTrain).net.ID);
											plugin.CreateLootSpawnerfunction(new Vector3(-0.927f, 1.546f, -0.117f), new Vector3(0, 270f, 0), activeTrain, eliteentityprefab);
											break;
										case 'b':
											WagonPos = new Vector3[] { new Vector3(0.0f, 1.5f, 2.5f), new Vector3(0.0f, 1.5f, 4.5f) };
											npcs.AddRange(plugin.CreateNPCs(WagonPos.Length, 8, _ServerSettings.PirateRailNPCHealth, _ServerSettings.PirateRailNPCKit, AIShootMultiplyer, activeTrain, WagonPos.ToList()));
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(1.674f, 0.857f, 3.30f), Vector3.zero, activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(-1.619f, 0.857f, 3.142f), new Vector3(0, 180f, 0), activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(-1.619f, 0.857f, -3.226f), new Vector3(0, 180f, 0), activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(0f, 0.857f, -4.781f), new Vector3(0, 90f, 0), activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(new Vector3(0f, 0.857f, 6.097f), new Vector3(0, 90f, 0), activeTrain, _ServerSettings.PirateRailwagondoorskinid, activeTrain.OwnerID, _ServerSettings.PirateRailWagondoorhealth).net.ID);
											plugin.BaseParts.Add(plugin.CreateTurretfunction(new Vector3(-0.808f, 1.546f, -5.566f), new Vector3(0, 90f, 0), activeTrain).net.ID);
											plugin.CreateLootSpawnerfunction(new Vector3(-0.927f, 1.546f, -0.117f), new Vector3(0, 270f, 0), activeTrain, eliteentityprefab);
											break;
										//case 'd':
										//lugin.BaseParts.Add(plugin.CreateTurretfunction(new Vector3(-0f, 4.85f, -0f), new Vector3(0, 180f, 0), activeTrain).net.ID);
										//	break;
										case 'c':
											plugin.BaseParts.Add(plugin.CreateSamsitefunction(new Vector3(0f, 1.4f, 0f), new Vector3(0, 0, 0), activeTrain).net.ID);
											WagonPos = new Vector3[] { new Vector3(0.9f, 1.5f, 2.5f), };
											npcs.AddRange(plugin.CreateNPCs(WagonPos.Length, 8, _ServerSettings.PirateRailNPCHealth, _ServerSettings.PirateRailNPCKit, AIShootMultiplyer, activeTrain, WagonPos.ToList()));
											break;
									}
									activeTrain.InitializeHealth(_ServerSettings.PirateRailWagonhealth, _ServerSettings.PirateRailWagonhealth);
								}
							}
							catch { }
						}
					}
				});
			}

			private bool SpawnEntity(string prefab, Vector3 position, Quaternion rotation, out TrainCar trainCar, Action<TrainCar> preSpawn)
			{
				trainCar = GameManager.server.CreateEntity(prefab, position, rotation) as TrainCar;
				trainCar.enableSaving = false;
				if (preSpawn != null) { preSpawn(trainCar); }
				trainCar.platformParentTrigger.ParentNPCPlayers = true;
				trainCar.Spawn();
				trainCar.enableSaving = false;
				trainCar.transform.position = position;
				if (trainCar == null || trainCar.IsDestroyed) { return false; }
				return true;
			}

			private void leaveing()
			{
				if (_trainbe == null) { return; }
				if (PlayerTarget != null || plugin.PlayersNearbyfunction(_trainbe.transform.position, 60))
				{
					plugin.timer.Once(10f, leaveing);
					return;
				}
				Die();
			}

			private void OnDestroy()
			{
				try
				{
					plugin.pirateRail--;
					isDead = true;
					enabled = false;
					CancelInvoke();
					if (npcs != null) { foreach (ScientistNPC npc in npcs) { if (npc != null && !npc.IsDestroyed && !npc.IsDead()) { npc.transform.position = new Vector3(0, -501, 0); npc.Kill(); } } }
					if (_trainbe != null && !_trainbe.IsDestroyed) { _trainbe.transform.position = new Vector3(0, -501, 0); _trainbe.Kill(); }

				}
				catch { }
			}

			public void Die() { if (this != null) { Destroy(this); } }

			private void Explodevoid() { if (_trainbe != null) { plugin.Explodefunction(_trainbe); } }

			public void FixedUpdate()
			{
				try
				{
					if (_trainbe == null || isDead || loading || _trainbe.IsDestroyed || _trainCar.completeTrain == null) { return; }
					if (stopping) { _trainCar.completeTrain.trackSpeed = 0; return; }
					if (CollisionCheck < Time.time)
					{
						if (_trainEngine.HasFlag(BaseEntity.Flags.Reserved6) || Vector3.Distance(_trainbe.transform.position, curretpos) < 5f) { TargetSpeed *= -1f; }
						CollisionCheck = Time.time + 4;
						curretpos = _trainbe.transform.position;
					}
					_trainbe.SetFlag(BaseVehicle.Flags.Reserved1, true);
					_trainbe.SetFlag(BaseEntity.Flags.Reserved5, true);
					_trainbe.SetFlag(TrainEngine.Flags.On, true);
					if (!halftickrate) { halftickrate = true; if (Driver == null || Driver.IsDead()) { stopping = true; } }
					else
					{
						_trainCar.completeTrain.trackSpeed = TargetSpeed;
						if (!burning)
						{
							if (_trainbe.Health() < _ServerSettings.StartFireHealth)
							{
								burning = true;
								plugin.CreateFire(60, new Vector3(0, 0.9f, -1.8f), 0.1f, _trainbe);
								BailOutcall();
							}
						}
						if (_trainbe.Health() < 10 && !exploding) { exploding = true; Explodevoid(); return; }
						halftickrate = false;
					}
				}
				catch { }
			}

			private void BailOutcall()
			{
				if (itembox != null) { itembox.dropFloats = true; itembox.DropItems(); }
				foreach (ScientistNPC npc in npcs.ToArray())
				{
					npcs.Remove(npc);
					plugin.BailOutfunction(_trainbe.transform.position, npc, Vector3.zero, 8);
				}
			}

			private void NPCsAlive()
			{
				if (npcs == null || _trainbe == null) { return; }
				int alive = 0;

				foreach (ScientistNPC npc in npcs.ToArray())
				{
					if (npc == null || !npc.IsAlive() || !plugin.ActiveNPCs.ContainsKey(npc)) { continue; }
					if (PlayerTarget == null) { PlayerTarget = plugin.ActiveNPCs[npc]; }
					alive++;
				}
				if (alive == 0)
				{
					plugin.timer.Once(10f, () => { if (_trainbe != null) { _trainbe.SetFlag(BaseVehicle.Flags.Reserved1, false); stopping = true; } });
					isDead = true;
					plugin.timer.Once(_ServerSettings.StopDelayFloat, Stop);
					return;
				}
				plugin.timer.Once(_ServerSettings.NPCTick, NPCsAlive);
			}

			private void Stop()
			{
				if (_trainbe == null) { return; }
				plugin.timer.Once(_ServerSettings.FireDelay + _ServerSettings.ExplodeDelay, Explodevoid);
				plugin.CreateFire(_ServerSettings.ExplodeDelay, FireOffset, _ServerSettings.FireDelay, _trainbe);
			}
		}

		public class FootPirate : MonoBehaviour
		{
			public int Type = 9;
			private float terraincheck;
			public float flyheightfloat = 150;
			public float flyspeedfloat = 60f;
			public int NPCAmountint = 5;
			public int NPCDropsint = 3;
			public int NPCDropped = 0;
			public BaseEntity baseentitybe;
			public BaseEntity plane;
			public Vector3 movetopoint = Vector3.zero;
			private bool isRetiring = false;
			public List<ScientistNPC> npcs = new List<ScientistNPC>();
			public Dictionary<Vector3, bool> droppoints = plugin.dropnodes;
			private float NextPOITick;
			List<Vector3> DropPos = new List<Vector3>();
			public bool CanDropNext = false;

			void Awake()
			{
				plugin.NextFrame(() =>
				{
					baseentitybe = GetComponent<BaseEntity>();
					if (baseentitybe == null) { OnDestroy(); return; }
					flyheightfloat = _ServerSettings.PiratePlaneflyheight;
					flyspeedfloat = _ServerSettings.PiratePlaneflyspeed;
					NPCAmountint = _ServerSettings.PiratePlaneNPCAmount;
					NPCDropsint = _ServerSettings.PiratePlaneNPCDrops;
					terraincheck = Time.time;
					plane = GameManager.server.CreateEntity(cargoplaneentityprefab, baseentitybe.transform.position, Quaternion.identity, true);
					if (plane != null)
					{
						plane.enabled = false;
						plane.Spawn();
						plane.enableSaving = false;
						plane.SetParent(baseentitybe);
						plane.transform.localPosition += new Vector3(0, -15, 0);

					}
					for (int i = 0; i <= NPCAmountint; i++) { DropPos.Add(new Vector3(0f, 2.1f, -13.1f)); }
					movetopoint = FindPOIvector();
					NextPOITick = Time.time;
					if (Type == 91) { NextPOITick = Time.time + 9999; }
					plugin.timer.Once(_ServerSettings.LeaveAfter, () => { Retire(); });
					plugin.piratePlane++;
					plugin.PirateUintList.Add(baseentitybe.net.ID);
				});
			}

			private Vector3 Retire()
			{
				if (plane == null) { return Vector3.zero; }
				if (isRetiring) { return Vector3.zero; }
				plugin.InvalidateCheck(plane);
				isRetiring = true;
				float x = TerrainMeta.Size.x;
				float y = plane.transform.position.y;
				Vector3 vector = Vector3Ex.Range(-1f, 1f);
				vector.y = 0f;
				vector.Normalize();
				vector *= (x * 20f);
				vector.y = y;
				movetopoint = vector;
				isRetiring = true;
				return vector;
			}

			private bool DropHere(Vector3 position)
			{
				if (npcs == null || npcs.Count == 0) { return false; }
				float groundheightFLOAT = TerrainMeta.HeightMap.GetHeight(position);
				if (groundheightFLOAT < 1f) { return false; }
				if (CanDropNext) { return true; }
				return false;
			}

			Vector3 FindPOIvector()
			{
				if (Type == 91)
				{
					if (npcs.Count == 0)
					{
						plugin.NextFrame(() =>
						{
							if (baseentitybe != null)
							{
								List<Vector3> addpos = new List<Vector3>();
								for (int i = 0; i < _ServerSettings.PirateEventNPCAmount; i++) { addpos.Add(new Vector3(0, 2.1f, -13.1f)); }
								npcs = plugin.CreateNPCs(_ServerSettings.PirateEventNPCAmount, 10, _ServerSettings.PirateBaseNPCHealth, _ServerSettings.PirateEventNPCKit, _ServerSettings.PirateEventNPCAimMulti, baseentitybe, addpos);
							}
						});
					}
					if (Vector3.Distance(movetopoint, plane.transform.position) < 10)
					{
						plugin.NextFrame(() =>
						{
							if (npcs != null && npcs.Count != 0 && plane != null)
							{
								foreach (ScientistNPC npc in npcs)
								{
									Vector3 pos = UnityEngine.Random.insideUnitCircle * 12;
									plugin.AttachParachute(npc, baseentitybe.transform.position + new Vector3(pos.x, -10f, pos.y));
								}
								npcs.Clear();
							}
						});
						return Retire();
					}
					return movetopoint;
				}
				if (DropHere(baseentitybe.transform.position) && movetopoint != Vector3.zero)
				{
					foreach (ScientistNPC npc in npcs)
					{
						Vector3 pos = UnityEngine.Random.insideUnitCircle * 12;
						plugin.AttachParachute(npc, plane.transform.position + new Vector3(pos.x, -10f, pos.y));
					}
					npcs.Clear();
				}
				plugin.NextFrame(() =>
				{
					if (NPCDropsint > NPCDropped && npcs.Count == 0)
					{
						NPCDropped++;
						npcs = plugin.CreateNPCs(NPCAmountint, 9, _ServerSettings.PirateFootNPCHealth, _ServerSettings.PirateFootNPCKit, _ServerSettings.PirateFootNPCAimMulti, baseentitybe, DropPos);
					}
					else if (NPCDropsint == NPCDropped) { Retire(); }
				});
				Vector3 droppoint = Vector3.zero;
				if (droppoints == null || droppoints.Count == 0)
				{
					float bounds = (ConVar.Server.worldsize / 2) - 200;
					float minx = bounds;
					float maxx = bounds;
					float minz = bounds;
					float maxz = bounds;
					float ran = UnityEngine.Random.Range(0, 2);
					if (ran == 1) { minx = bounds; maxx = bounds; minz = 0; maxz = UnityEngine.Random.Range(0, bounds); }
					if (ran == 0) { minz = bounds; maxz = bounds; minx = 0; maxx = UnityEngine.Random.Range(0, bounds); }
					droppoint = new Vector3(UnityEngine.Random.Range(minx, maxx), flyheightfloat, UnityEngine.Random.Range(minz, maxz));
					float x = UnityEngine.Random.Range(0, 2);
					float z = UnityEngine.Random.Range(0, 2);
					if (x == 1) { droppoint.x = -droppoint.x; }
					if (z == 1) { droppoint.z = -droppoint.z; }
				}
				else
				{
					droppoint = droppoints.Keys.ToList().GetRandom();
					if (droppoints[droppoint] == true) { CanDropNext = true; }
					droppoints.Remove(droppoint);
				}
				droppoint.y = TerrainMeta.HeightMap.GetHeight(droppoint) + flyheightfloat;
				return droppoint;
			}

			void FixedUpdate()
			{
				if (baseentitybe == null && plane == null) { OnDestroy(); return; }
				if (baseentitybe.Distance2D(movetopoint) < 10f && !isRetiring) { movetopoint = FindPOIvector(); }
				if (plugin.PointsOfIntrest.Count != 0)
				{
					if (NextPOITick < Time.time)
					{
						NextPOITick = Time.time + 10;
						Vector3 POI = plugin.getPOIfunction(baseentitybe.transform.position, _ServerSettings.PiratesAirPOIRadius);
						if (POI != Vector3.zero)
						{
							POI.y = TerrainMeta.HeightMap.GetHeight(POI) + _ServerSettings.PiratePlaneflyheight;
							movetopoint = POI;
							NextPOITick = Time.time + 120;
						}
					}
				}
				Vector3 targetDir = movetopoint - transform.position;
				Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, 5f * Time.deltaTime, 0.0f);
				Vector3 pos = Vector3.MoveTowards(baseentitybe.transform.position, movetopoint, flyspeedfloat * Time.deltaTime);
				if (terraincheck < Time.time)
				{
					float terrainheight = TerrainMeta.HeightMap.GetHeight(plane.transform.position);
					if (pos.y < terrainheight + flyheightfloat - 5) { pos.y += 2f; }
					else if (pos.y > terrainheight + flyheightfloat + 5) { pos.y -= 2f; }
					terraincheck = Time.time + 0.5f;
				}
				baseentitybe.transform.position = pos;
				baseentitybe.transform.rotation = Quaternion.LookRotation(newDir);
				plane.transform.position = baseentitybe.transform.position;
				plane.transform.rotation = baseentitybe.transform.rotation;
			}

			void OnDestroy()
			{
				if (plugin != null) { plugin.piratePlane--; }
				if (plane != null && !plane.IsDestroyed) { plane.Kill(); }
				if (baseentitybe != null && !baseentitybe.IsDestroyed) { baseentitybe.Kill(); }
			}
		}

		public class BasePirate : MonoBehaviour
		{
			public int Type;
			public BaseEntity _base;
			public List<ScientistNPC> npcs;
			public List<BaseEntity> entitys = new List<BaseEntity>();
			public StorageContainer itembox;

			private void Awake()
			{
				plugin.NextFrame(() =>
				{
					_base = GetComponent<BaseEntity>();
					if (_base == null) { Die(); return; }
					plugin.pirateBase++;
					plugin.ClearAreafunction(_base.transform.position, _ServerSettings.PirateBaseWallRadius);
					plugin.timer.Once(_ServerSettings.LeaveAfter, () => { if (_base != null) { CheckEmpty(); } });
					plugin.timer.Once(10, () => { if (_base != null) { CheckLoot(); } });
					plugin.PirateUintList.Add(_base.net.ID);
					entitys.Add(_base);
					entitys.AddRange(plugin.CreateWallsfunction(_base.transform.position, _ServerSettings.PirateBaseWallRadius));
					CreateBase();
					CreateStorage0();
					plugin.NextFrame(() =>
					{
						if (itembox != null) { plugin.filllootfunction(itembox.inventory, -1, _ServerSettings.PirateBaseLootMulti, _ServerSettings.PirateBaseLootString); }
						List<Vector3> pos = new List<Vector3>();
						for (int i = 0; i < _ServerSettings.PirateBaseNPCAmount; i++) { pos.Add(Vector3.zero); }
						npcs = plugin.CreateNPCs(_ServerSettings.PirateBaseNPCAmount, Type, _ServerSettings.PirateBaseNPCHealth, _ServerSettings.PirateBaseNPCKit, _ServerSettings.PirateBaseAimMulti, _base, pos);
						if (npcs == null) { _base.Kill(); }
						plugin.BaseParts.Add(itembox.net.ID);
						plugin.timer.Once(10f, () =>
						{
							foreach (ScientistNPC npc in npcs) { plugin.BailOutfunction(_base.transform.position + new Vector3(0, 4, 0), npc, Vector3.zero, _ServerSettings.PirateBaseWallRadius - 3, 5.5f, false); }
							foreach (BaseEntity be in entitys)
							{
								if (be == null || be.net == null) { continue; }
								plugin.BaseParts.Add(be.net.ID);
							}
						});
					});
				});
			}

			private void CheckLoot()
			{
				if (_base != null && itembox != null)
				{
					plugin.timer.Once(10f, () => { try { CheckLoot(); } catch { } });
					return;
				}
				CheckEmpty();
			}

			private void CreateBase()
			{
				var entity = GameManager.server.CreateEntity(floorprefab, _base.transform.position + new Vector3(0, 3.5f, 0), _base.transform.rotation);
				if (entity == null) { return; }
				entitys.Add(entity);
				entity.Spawn();
				entity.enableSaving = false;
				entity.OwnerID = (ulong)UnityEngine.Random.Range(10000000, 20000000);
				entity.SetParent(_base, true, true);
				var buildingBlock = entity as BuildingBlock;
				if (buildingBlock != null)
				{
					buildingBlock.SetGrade((BuildingGrade.Enum)4);
					buildingBlock.grounded = true;
					buildingBlock.AttachToBuilding(BuildingManager.server.NewBuildingID());
					buildingBlock.health = buildingBlock.MaxHealth();
					AttachParts(floorprefab, new Vector3(0f, 6, 0), entity.transform.rotation, buildingBlock);
					AttachParts(foundationprefab, new Vector3(0f, 3, 0), entity.transform.rotation, buildingBlock);
					AttachParts(wallprefab, new Vector3(-1.5f, 0, 0), entity.transform.rotation * Quaternion.Euler(0, 180, 0), buildingBlock);
					BaseEntity doorframe = AttachParts(wallframeprefab, new Vector3(-1.5f, 3, 0), entity.transform.rotation * Quaternion.Euler(0, 180, 0), buildingBlock);
					AttachParts(wallprefab, new Vector3(1.5f, 0, 0), entity.transform.rotation, buildingBlock);
					AttachParts(wallprefab, new Vector3(1.5f, 3, 0), entity.transform.rotation, buildingBlock);
					AttachParts(wallprefab, new Vector3(0f, 0, -1.5f), entity.transform.rotation * Quaternion.Euler(0, 90, 0), buildingBlock);
					AttachParts(wallprefab, new Vector3(0f, 3, -1.5f), entity.transform.rotation * Quaternion.Euler(0, 90, 0), buildingBlock);
					AttachParts(wallprefab, new Vector3(0f, 0, 1.5f), entity.transform.rotation * Quaternion.Euler(0, 270, 0), buildingBlock);
					AttachParts(wallprefab, new Vector3(0f, 3, 1.5f), entity.transform.rotation * Quaternion.Euler(0, 270, 0), buildingBlock);
					plugin.BaseParts.Add(plugin.CreateGarageDoorfunction(Vector3.zero, Vector3.zero, doorframe, 2433668609, entity.OwnerID, _ServerSettings.PirateBaseDoorHealth).net.ID);
					BaseEntity lantern = GameManager.server.CreateEntity(lanternentityprefab, _base.transform.position + new Vector3(0, 9.6f, 0), _base.transform.rotation);
					lantern.Spawn();
					lantern.enableSaving = false;
					lantern.OwnerID = entity.OwnerID;
					lantern.SetFlag(BaseEntity.Flags.On, true);
					entitys.Add(lantern);
					plugin.BaseParts.Add(entity.net.ID);
				}
			}

			private BaseEntity AttachParts(string prefab, Vector3 position, Quaternion rotation, BuildingBlock basepart)
			{
				var entity = GameManager.server.CreateEntity(prefab);
				if (entity == null) { return null; }
				entity.Spawn();
				entity.enableSaving = false;
				entitys.Add(entity);
				entity.SetParent(basepart, false, true);
				entity.OwnerID = basepart.OwnerID;
				var buildingBlock = entity as BuildingBlock;
				if (buildingBlock != null)
				{
					buildingBlock.SetGrade((BuildingGrade.Enum)3);
					buildingBlock.grounded = true;
					buildingBlock.AttachToBuilding(BuildingManager.server.NewBuildingID());
					buildingBlock.health = buildingBlock.MaxHealth();
				}
				entity.transform.localPosition = position;
				entity.transform.localRotation = rotation;
				return entity;
			}

			private void CreateStorage0()
			{
				itembox = GameManager.server.CreateEntity(woodboxentityprefab, _base.transform.position + new Vector3(0, 6.5f, 0)) as StorageContainer;
				plugin.DestroyGroundComp(itembox);
				plugin.DestroyMeshCollider(itembox);
				itembox.skinID = 931816387;
				itembox.Spawn();
				itembox.enableSaving = false;
				itembox.inventory.capacity = 30;
				itembox.SendNetworkUpdateImmediate();
				plugin.AddLockfunction(itembox);
			}

			public void CheckEmpty()
			{
				if (_base == null) { return; }
				if (!plugin.PlayersNearbyfunction(_base.transform.position, 25))
				{
					CancelInvoke("CheckEmpty");
					ExplodeAndDestroy();
					return;
				}
				Invoke("CheckEmpty", 15f);
			}

			public void ExplodeAndDestroy()
			{
				bool killquite = false;
				if (itembox != null) { killquite = true; }
				foreach (BaseEntity b in entitys)
				{
					if (!killquite) { plugin.Explodefunction(b); }
					Invoke("Die", 5f);
				}
			}

			private void OnDestroy()
			{
				try
				{
					enabled = false;
					CancelInvoke();
					if (plugin != null && this != null)
					{
						plugin.pirateBase--;
						if (itembox != null && !itembox.IsDestroyed) { itembox.Kill(); }
						if (entitys != null) { foreach (BaseEntity e in entitys) { if (e != null && !e.IsDestroyed) { e.transform.position = new Vector3(0, -501, 0); e.Kill(); } } }
						if (npcs != null) { foreach (ScientistNPC npc in npcs) { if (npc != null && !npc.IsDestroyed && !npc.IsDead()) { if (npc.net != null) { plugin.NPCsUintList.Remove(npc.net.ID); plugin.GroundedNPCs.Remove(npc.net.ID); } npc.transform.position = new Vector3(0, -501, 0); npc.Kill(); } } }
					}
				}
				catch { }
				try { if (_base != null && !_base.IsDestroyed) { _base.Kill(); } } catch { }
			}

			public void Die() { if (this != null) { Destroy(this); } }

			private void Explodevoid() { if (_base != null) { plugin.Explodefunction(_base); } }
		}
		#endregion
	}
}