using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Controls all elements in the canvas
/// </summary>
public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	public Text CenterText;
	public CanvasGroup TitleGroup;
	public CanvasGroup PauseGroup;

	public RectTransform OptionsGroup;

	private void Awake()
	{
		Instance = this;
		// Init elements
		CenterText.DOFade(0f, 0f);
		ToggleTitle(false);
		TogglePause(false);
	}

	public void SetCenterText(string s)
	{
		StartCoroutine(SetCenterTextSeq(s));
	}
	private IEnumerator SetCenterTextSeq(string s)
	{
		if (CenterText.text != "")
		{
			CenterText.DOFade(0.0f, 0.5f);
			yield return new WaitForSeconds(1.0f);
		}
		if (s != "")
		{
			CenterText.text = s;
			CenterText.DOFade(1.0f, 0.5f);
		}
		else
		{
			CenterText.DOFade(0.0f, 0.5f);
			yield return new WaitForSeconds(0.5f);
			CenterText.text = s;
		}
		yield return null;
	}

	public void ShowOptions()
	{
		OptionsGroup.DOAnchorPosX(2f, .5f).SetEase(Ease.InOutBack);
	}

	public void HideOptions()
	{
		OptionsGroup.DOAnchorPosX(301f, .5f).SetEase(Ease.InOutBack);
	}

	public void ToggleTitle(bool toggle, float time = 0)
	{		
		TitleGroup.DOFade(toggle ? 1f : 0f, time);
		TitleGroup.blocksRaycasts = toggle;
		TitleGroup.interactable = toggle;
	}

	public void TogglePause(bool toggle, float time = 0)
	{				
		Time.timeScale = toggle ? 0f : 1f;
		PauseGroup.DOFade(toggle ? 1f : 0f, time).SetUpdate(UpdateType.Normal, true);
		PauseGroup.blocksRaycasts = toggle;
		PauseGroup.interactable = toggle;

		// Turn off mouse and lock it
		Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
		Cursor.visible = toggle;
	}

	public void Resume()
	{
		TogglePause(false, 0.5f);
	}

}
