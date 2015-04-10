﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using SimpleJSON;
using Debug = FFP.Debug;

/*!
 *	Manages audio for all levels. This class pretty much just loads and plays sounds, on a (timed or untimed) repeat if desired.
 *	\todo implementation + code
 *	written by Ace
 */
public class AudioManager : MonoBehaviour {

	#region singletonEnforcement
	private static AudioManager _instance;

	public static AudioManager instance {
		get {
			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<AudioManager>();

				//Dont distroy on new scene load
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}
	#endregion

	public string soundListFilePath = "/Resources/Json/audioListData.json";
	public string soundFilePath = "/StreamingAssets/Audio/";

	public AudioMixer masterMixer;

	Dictionary<string, bool> ambianceDict = new Dictionary<string, bool>();
	Dictionary<string, bool> effectDict = new Dictionary<string, bool>();
	Dictionary<string, bool> musicDict = new Dictionary<string, bool>();
	Dictionary<string, bool> voiceDict = new Dictionary<string, bool>();


	//! Unity Start function
    void Start() {
		Debug.Warning("audio", "SoundManager has started");
		
		#region singletonCreation
		if(_instance == null) {
			//If I am the first instance, make me the Singleton
			_instance = this;
			DontDestroyOnLoad(this);
		} else {
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if(this != _instance)
				Destroy(this.gameObject);
		}
		#endregion

		if(!loadListFromFile(soundListFilePath)) {
			Debug.Error("audio", "JSON file did not load");
		} else
			Debug.Warning("audio", "JSON file loaded");

		

	}

	//! Unity Update function
	//void Update() {
	//}

	#region json file reading
	//!checks JSON file exists and loads it
	public bool loadListFromFile(string path) {		//checks to make sure json file is there
		if(!System.IO.File.Exists(Application.dataPath + path)) {
			Debug.Error("audio", "File does not exist: " + path);
			return false;
		}
		string json = System.IO.File.ReadAllText(Application.dataPath + path);
		return loadListFromJson(json);
	}

	//!checks JSON file isnt empty and loads data into audio dictionaries
	public bool loadListFromJson(string json) {		
		JSONNode soundData = JSON.Parse(json);
		if(soundData == null) {
			Debug.Error("audio", "Json file is empty");
			return false;
		}
		
		JSONNode audioNode = soundData["audio"];
		string[] types = { "ambiance", "effect", "music", "voice" };

		foreach(string type in types) {
			JSONArray tempArray = audioNode[type].AsArray;
			foreach(JSONNode j in tempArray) {
				var name = j[0];				//j[0] is name of object
				bool priority = j[1].AsBool;	//j[1] priority of object
				addSound(type, name, priority);
			}
		}
		return true;
	}
	#endregion 
	
	#region General Sound Functions
	//!Adds sound info to Dictionaries
	public void addSound(string type, string name, bool priority){
		switch(type.ToLower()) {
				case "ambiance":
					ambianceDict.Add(name, priority);
					break;
				case "effect":
					effectDict.Add(name, priority);
					break;
				case "music":
					musicDict.Add(name, priority);
					break;
				case "voice":
					voiceDict.Add(name, priority);
					break;
				default:
					Debug.Log("audio", type + "is not an option, or you spelled it wrong");
					break;
			}
		} 

	//!checks if sounds in dictionary
	public bool findSound(string type, string name) {
		switch(type.ToLower()) {
			case "ambiance":
				if(ambianceDict.ContainsKey(name))
					return true;
				else
					return false;
			case "effect":
				if(effectDict.ContainsKey(name))
					return true;
				else
					return false;
			case "music":
				if(musicDict.ContainsKey(name))
					return true;
				else
					return false;
			case "voice":
				if(voiceDict.ContainsKey(name))
					return true;
				else
					return false;
			default:
				Debug.Log("audio", type + "is not an option, or you spelled it wrong");
				return false;
		}
	}

	//!Load single sound, must include file name and extension
	public AudioClip loadSound(string type, string name) {
		AudioClip tempSound;
		string path = Application.dataPath + soundFilePath;

		if(!findSound(type, name)) {
			tempSound = null;
		} else {
			switch(type.ToLower()) {
				case "ambiance":
					path = path + "Ambiance/" + name;
					tempSound = AudioClip.Create(path, )
					ambianceDict.Add(name, priority);
					break;
				case "effect":
					effectDict.Add(name, priority);
					break;
				case "music":
					musicDict.Add(name, priority);
					break;
				case "voice":
					voiceDict.Add(name, priority);
					break;
				default:
					Debug.Log("audio", type + "is not an option, or you spelled it wrong");
					break;
			}
		}
			

		return tempSound;
	}

	/*playMe(this, type, name){
		AudioClip sound;
		
		if(!sound)
			Debug.Warning("audio", "there was a problem loading " + name);
	}*/

	#endregion


	#region Sound volume
	[EventVisible]
	//!Change volume for groups in MasterMixer
	public void changeVol(string name, float level){
		switch(name.ToLower()){
			case "master":
				masterMixer.SetFloat ("MasterVol", level);
				break;
			case "ambiance":
				masterMixer.SetFloat ("MaxAmbianceVol", level);
				break;
			case "effect":
				masterMixer.SetFloat ("MaxEffectVol", level);
				break;
			case "music":
				masterMixer.SetFloat ("MaxMusicVol", level);
				break;
			case "voice":
				masterMixer.SetFloat ("MaxVoiceVol", level);
				break;
			case "reset":
				masterMixer.ClearFloat ("MasterVol");
				masterMixer.ClearFloat ("MaxAmbianceVol");
				masterMixer.ClearFloat ("MaxEffectVol");
				masterMixer.ClearFloat ("MaxMusicVol");
				masterMixer.ClearFloat ("MaxVoiceVol");
				break;
			default:
				Debug.Log("audio", name + "is not an option, or you spelled it wrong");
				break;
		}

	}

	//!Check the current volume levels in MasterMixer
	public float seeVol(string name){
		float level = 100;
		switch(name.ToLower()) {
			case "master":
				masterMixer.GetFloat ("MasterVol", out level);
				return level;
			case "ambiance":
				masterMixer.GetFloat ("MaxAmbianceVol", out level);
				return level;
			case "effect":
				masterMixer.GetFloat ("MaxEffectVol", out level);
				return level;
			case "music":
				masterMixer.GetFloat ("MaxMusicVol", out level);
				return level;
			case "voice":
				masterMixer.GetFloat ("MaxVoiceVol", out level);
				return level;
			default:
				Debug.Log("audio", name + "is not an option, or you spelled it wrong");
				return 300;
		}

	}
	
	//public void setObjectVolume(string objectName, float objectLvl){
		//needs work
	//}


	#endregion

	#region playFunct
	//set up each type of sound play functions

	//generic idea for sending and playing an audio on object
	//AudioManager.playMe(this, type, "fire");
	//bool playMe(monobehavior mono, string name){
	//sound = Mono.gameObject.addComponent<AudioSource>;
	//as.clip;
	//}

	#endregion


}		//end of audio manager



/*Ideas/todo
 * 
 * json sound list - DONE
 *	json file: filename, type of sound, priority always load or on use load
 *	http://jsonformatter.curiousconcept.com/ - format json, may not be simplejson
 *	http://json.org/example
 *	look at checkpointManager.cs & AchievementManager.cs
 *	http://wiki.unity3d.com/index.php/SimpleJSON
 *	ALL TYPE HAS BEEN SET TO LOWERCASE AND INPUT HAS BEEN CONVERTED VIA ToLower()
 * 
 * 
 * Audio should come from streaming Assets
 *   http://docs.unity3d.com/Manual/StreamingAssets.html
 *  
 * 
 * check sounds are on list and ready to load
 *   make sure file paths are correct either in JSON or hardcoded
 * Add priority sounds into game at game start
 * 
 * 
 * when called play sound or throw error
 * gui should call sound manager for volume changes
 * 
 * 
 * play functions
 *	void play sound
 *	void pause sound
 *	bool is playing
 *	void stop playing
 *	void set loop sound
 *	
 *   pause all, group sounds
 * 
 *  play on player, if given no object play on player
 *  play on object, if given object play on object
 *  
 * local object sound script should call sound manager
 * local script should create audio source & delete when done playing
 * 
 *  Custom sound inspector window
 *   http://unity3d.com/learn/tutorials/modules/intermediate/editor/building-custom-inspector
 *   on inspector gui - only in manager
 *     display list of active sounds
 *     print as text/cant interract
 *     total time & time left of each
 * 
*/