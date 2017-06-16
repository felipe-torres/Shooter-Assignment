using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main controller for the player
/// </summary>
public class Player : MonoBehaviour
{
	private Animator animator;
	private Rigidbody rb;
	private Camera camera;

	public float MovementSpeed = 2.0f;

	/* Shooting properties */
	public GameObject BulletPrefab;
	public GameObject BulletPoolParent;
	private List<Bullet> BulletPool;
	public Transform BulletStartTransform;
	public int MaxBulletAmmo = 20;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		camera = Camera.main;

		// Init Bullet Pool
		InitBulletPool();
	}

	private void Update()
	{
		Rotate();
		Shoot();
	}

	private void FixedUpdate()
	{
		Move(); // Move processing in here since it involves physics calculations
	}

	/// <summary>
	/// Rotates player so that faces the camera forward
	/// </summary>
	private void Rotate()
	{
		transform.LookAt(camera.transform.forward * 100f); // Player rotates towards the camera's forward vector
	}

	/// <summary>
	/// Process movement for the player
	/// </summary>
	private void Move()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		// Animate movement
		animator.SetFloat("Move", Mathf.Clamp(Mathf.Abs(h) + Mathf.Abs(v), 0f, 1f));

		// Movement is done by forces
		rb.velocity = Vector3.zero;
		rb.AddForce(MovementSpeed * transform.TransformDirection(new Vector3(h, 0f, v)), ForceMode.VelocityChange);

	}

	private void Shoot()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			//Shoot
			Bullet b = GetBullet();
			if (b != null)
			{
				b.transform.position = BulletStartTransform.position;
				b.transform.rotation = BulletStartTransform.rotation;
				b.gameObject.SetActive(true);
			}
		}
	}

	private void InitBulletPool()
	{
		BulletPool = new List<Bullet>();
		for (int i = 0; i < MaxBulletAmmo; i++)
		{
			GameObject b = Instantiate(BulletPrefab);
			b.transform.SetParent(BulletPoolParent.transform);
			b.SetActive(false);
			BulletPool.Add(b.GetComponent<Bullet>());
		}
	}

	private Bullet GetBullet()
	{
		foreach (Bullet b in BulletPool)
		{
			if (!b.gameObject.activeInHierarchy)
			{
				return b; // Found an inactive bullet
			}
		}

		// All bullets are occuppied, return nothing
		return null;
	}


}
