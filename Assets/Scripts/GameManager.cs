using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using DG.Tweening;
using UnityEngine.SceneManagement;

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

	public bool SkipTitle = false;

	public bool Paused { get; set; }
	public bool GameWon { get; set; }
	public bool GameOver { get; set; }

	private void Awake()
	{
		Instance = this;
	}

	// Use this for initialization
	private void Start()
	{
		// Turn off input for the player and show title screen
		if (!SkipTitle)
		{
			player.InputEnabled = false;
			ui = UIManager.Instance;
			ui.ToggleTitle(true);
		}
		else
		{
			// Turn off mouse and lock it
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	public void StartGame()
	{
		StartCoroutine(StartSeq());
	}
	/// <summary>
	/// Initial cutscene to show game's objectives
	/// </summary>
	private IEnumerator StartSeq()
	{
		// Hide Title Screen
		ui.ToggleTitle(false, 0.5f);

		// Turn off mouse and lock it
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		// Show initial text
		ui.SetCenterText("You are having THE worst dream!");
		yield return new WaitForSeconds(3f);
		ui.SetCenterText("You must get to the clock and wake yourself up!");

		flc.enabled = false;
		pcfwc.enabled = false;
		Vector3 camPos = Camera.main.transform.position;
		Quaternion camRot = Camera.main.transform.rotation;
		Camera.main.transform.DOMove(TutorialObjectiveCamPoint.position, 2.5f).SetEase(Ease.InOutSine);
		Camera.main.transform.DORotate(TutorialObjectiveCamPoint.rotation.eulerAngles, 2.5f).SetEase(Ease.InOutSine);
		// Turn off mouse and lock it
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		yield return new WaitForSeconds(5f);

		Camera.main.transform.DOMove(camPos, 2.5f).SetEase(Ease.InOutSine);
		Camera.main.transform.DORotate(camRot.eulerAngles, 2.5f).SetEase(Ease.InOutSine);

		yield return new WaitForSeconds(2.5f);
		flc.enabled = true;
		pcfwc.enabled = true;

		player.InputEnabled = true;
		ui.SetCenterText("Use WASD to move");
		yield return new WaitForSeconds(3f);
		ui.SetCenterText("Use left click or space to shoot!");
		yield return new WaitForSeconds(4.5f);
		ui.SetCenterText("");
	}

	private void Update()
	{
		CheckPause();
	}

	/// <summary>
	/// Pauses the game
	/// </summary>
	private void CheckPause()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ui.TogglePause(!Paused, 0.5f);
		}
	}

	/// <summary>
	/// Reloads this level.
	/// </summary>
	public void RestartLevel()
	{	
		StartCoroutine(RestartLevelSeq());
	}
	private IEnumerator RestartLevelSeq()
	{		
		GameOver = true;
		// Win or Loose logic here
		if(GameWon)
		{
			// Kill all enemies
			//Object.FindObjectsOfType(typeof(Enemy))
			print("Won game");
		}
		else
		{
			print("Lost game");
		}
		yield return new WaitForSeconds(5f);
		SceneManager.LoadScene(0);
	}

	public void ExitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
		Application.OpenURL(webplayerQuitURL);
#else
		Application.Quit();
#endif
	}
}
