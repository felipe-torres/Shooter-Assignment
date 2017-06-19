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
	public ConstantForce cf;
	public Rigidbody rb;
	public float constantRelativeForce = 20f;

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
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		transform.localScale = originalScale;

		// Force
		rb.useGravity = false;
		cf.relativeForce = Vector3.forward*constantRelativeForce;

		// Grow scale
		transform.DOScale(ScaleMultiplier*originalScale, 0.5f).SetEase(Ease.InOutSine);

		yield return new WaitForSeconds(Lifetime);
		gameObject.SetActive(false);
	}

	private void OnCollisionEnter(Collision c)
	{
		if(c.gameObject.CompareTag("Enemy"))
		{
			//print("Bullet hit enemy!");
			cf.relativeForce = Vector3.zero;
			rb.velocity = Vector3.zero;
			rb.useGravity = true;
			gameObject.SetActive(false);
			// FX
			c.gameObject.GetComponent<Enemy>().GetHit();
		}
		else if(c.gameObject.CompareTag("AlarmClock"))
		{
			cf.relativeForce = Vector3.zero;
			rb.velocity = Vector3.zero;
			rb.useGravity = true;
			gameObject.SetActive(false);
			c.gameObject.GetComponent<AlarmClock>().GetHit();
		}
		else
		{
			//print("Bullet hit something: "+c.gameObject.name);
			cf.relativeForce = Vector3.zero;
			rb.velocity = Vector3.zero;
			rb.useGravity = true;
		}
	}
}
