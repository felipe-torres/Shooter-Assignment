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

	public RectTransform OptionsGroup;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		// Init elements
		CenterText.DOFade(0f, 0f);
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

}
