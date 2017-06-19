using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// The Alarm Clock is the objective to "destroy" in the game
/// </summary>
public class AlarmClock : MonoBehaviour
{
	public int MaxHp;
	private int currentHp;
	private Vector3 originalRot;

	// FX
	public ParticleSystem AlarmParticles;
	public ParticleSystem HitParticles;

	// UI
	public Image HealthBar;
	public CanvasGroup HealthBarGroup;

	/* Audio properties */
	private AudioSource audioSource;
	public AudioClip HitSFX;
	public AudioClip AlarmSFX;

	public bool AlarmTriggered { get; set; }

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		originalRot = transform.eulerAngles;
	}

	// Use this for initialization
	private void Start ()
	{
		currentHp = MaxHp;
	}

	/// <summary>
	/// Called when it has been hit by a bullet
	/// </summary>
	public void GetHit()
	{
		DOTween.Kill("ClockHitTween");
		transform.DOShakeRotation(0.5f, 20f, 20, 100f, false).SetId("ClockHitTween");
		transform.DORotate(originalRot, 0.25f).SetDelay(0.5f).SetId("ClockHitTween");

		HitParticles.Play();
		
		if(AlarmTriggered) return;

		currentHp--;
		HealthBar.DOFillAmount((float)currentHp/(float)MaxHp, 0.5f).SetEase(Ease.InOutSine);
		if(currentHp <= 0)
			SoundAlarm();
	}

	/// <summary>
	/// Called when Hp reaches 0. Activates a gameover: victory
	/// </summary>
	public void SoundAlarm()
	{		
		AlarmTriggered = true;
		AlarmParticles.Play();
		audioSource.PlayOneShot(AlarmSFX);

		GameManager.Instance.GameWon = true;
		GameManager.Instance.RestartLevel();
	}
}
