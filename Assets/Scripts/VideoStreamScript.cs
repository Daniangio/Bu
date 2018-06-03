using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class VideoStreamScript : MonoBehaviour {

	public string nextScene;
	public RawImage image;

	public VideoClip videoToPlay;

	private VideoPlayer videoPlayer;
	private VideoSource videoSource;
	private AudioSource audioSource;

	private bool eggBlinked = false;


	void Start () {
		Application.runInBackground = true;
		StartCoroutine (PlayVideo ());
	}

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			SceneManager.LoadScene (nextScene);
		}
	}

	IEnumerator PlayVideo() {

		//Add VideoPlayer to the GameObject
		videoPlayer = gameObject.AddComponent<VideoPlayer>();

		//Add AudioSource
		audioSource = gameObject.AddComponent<AudioSource>();

		//Disable Play on Awake for both Video and Audio
		videoPlayer.playOnAwake = false;
		audioSource.playOnAwake = false;
		audioSource.Pause();

		//We want to play from video clip not from url
		videoPlayer.source = VideoSource.VideoClip;

		// Vide clip from Url
		//videoPlayer.source = VideoSource.Url;
		//videoPlayer.url = "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";

		//Set Audio Output to AudioSource
		videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

		//Assign the Audio from Video to AudioSource to be played
		videoPlayer.EnableAudioTrack(0, true);
		videoPlayer.SetTargetAudioSource(0, audioSource);

		//Set video To Play then prepare Audio to prevent Buffering
		videoPlayer.clip = videoToPlay;
		videoPlayer.Prepare();



		//Wait until video is prepared
		while (!videoPlayer.isPrepared)	{
			yield return null;
		}

		gameObject.GetComponent<RawImage> ().enabled = true;

		//Assign the Texture from Video to RawImage to be displayed
		image.texture = videoPlayer.texture;

		//Play Video
		videoPlayer.Play();

		//Play Sound
		audioSource.Play();


		while (videoPlayer.isPlaying) {
			//Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));

			if (EditorSceneManager.GetActiveScene().name == "CutScene") {
				if (Mathf.FloorToInt ((float)videoPlayer.time) == 20 && eggBlinked == false) {
					eggBlinked = true;
					StartCoroutine (BlinkEgg ());
				}
			}

			yield return null;
		}

		SceneManager.LoadScene (nextScene);

	}

	private IEnumerator BlinkEgg() {
		ArduinoPersistent ap = (ArduinoPersistent)GameObject.Find ("ArduinoPersistent").GetComponent<ArduinoPersistent> ();
		for (int i = 0; i < 3; i++) {
			ap.SwitchOff ();
			yield return new WaitForSeconds (0.5f);
			ap.SetIntensity (i + 1);
			ap.SetBrightness (20 + (i + 1) * 40);
			yield return new WaitForSeconds (0.5f);
		}
		ap.SetIntensity (1);
		ap.SetBrightness (20);
	}

}
