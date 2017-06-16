using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Bullet properties and functions
/// </summary>
public class Bullet : MonoBehaviour
{
	public float Lifetime = 5f;
	public float ScaleMultiplier = 3f;
	private Vector3 originalScale;

	private void Awake()
	{
		originalScale = transform.localScale;
	}

	private void OnEnable()
	{
		StartCoroutine(Fly());
	}

	private IEnumerator Fly()
	{
		// Reset 
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		transform.localScale = originalScale;

		// Grow scale
		transform.DOScale(ScaleMultiplier*originalScale, 0.5f).SetEase(Ease.InOutSine);

		yield return new WaitForSeconds(Lifetime);
		gameObject.SetActive(false);
	}
}
