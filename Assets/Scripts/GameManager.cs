using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using DG.Tweening;

/// <summary>
/// Controls the main flow of the game
/// </summary>
public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public Player player;
	private UIManager ui;

	public FreeLookCam flc;
	public ProtectCameraFromWallClip pcfwc;
	public Transform TutorialObjectiveCamPoint;

	private void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	private void Start()
	{
		ui = UIManager.Instance;
		//StartCoroutine(StartSeq());

		// Turn off mouse and lock it
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	/// <summary>
	/// Initial cutscene to show game's objectives
	/// </summary>
	private IEnumerator StartSeq()
	{
		// Turn off input for the player
		player.InputEnabled = false;
		// Show initial text
		ui.SetCenterText("You are having THE worst dream!");
		yield return new WaitForSeconds(3f);
		ui.SetCenterText("You must get to the clock and wake yourself up!");
		yield return new WaitForSeconds(5f);

		ui.SetCenterText("Debes llegar a la meta para ganar!");
		flc.enabled = false;
		pcfwc.enabled = false;
		Vector3 camPos = Camera.main.transform.position;
		Quaternion camRot = Camera.main.transform.rotation;
		Camera.main.transform.DOMove(TutorialObjectiveCamPoint.position, 2.5f).SetEase(Ease.InOutSine);
		Camera.main.transform.DORotate(TutorialObjectiveCamPoint.rotation.eulerAngles, 2.5f).SetEase(Ease.InOutSine);

		yield return new WaitForSeconds(5f);

		Camera.main.transform.DOMove(camPos, 2.5f).SetEase(Ease.InOutSine);
		Camera.main.transform.DORotate(camRot.eulerAngles, 2.5f).SetEase(Ease.InOutSine);

		yield return new WaitForSeconds(2.5f);
		flc.enabled = true;
		pcfwc.enabled = true;

		player.InputEnabled = true;
		ui.SetCenterText("Usa WASD para moverte");
		yield return new WaitForSeconds(3f);
		ui.SetCenterText("Puedes usar Espacio para saltar!");
		yield return new WaitForSeconds(3f);
		ui.SetCenterText("");
	}
}
