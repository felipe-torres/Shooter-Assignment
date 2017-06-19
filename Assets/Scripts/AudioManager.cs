using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls audio: ambience, music, sfx
/// </summary>
public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;

	public AudioSource BGM_Audio;
	public AudioSource SFX_Audio;

	private void Awake()
	{
		Instance = this;
	}

	
}
