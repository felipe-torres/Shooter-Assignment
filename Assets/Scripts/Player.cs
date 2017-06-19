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
	public bool InputEnabled = true;

	/* Shooting properties */
	public GameObject BulletPrefab;
	public GameObject BulletPoolParent;
	private List<Bullet> BulletPool;
	public Transform BulletStartTransform;
	public int MaxBulletAmmo = 20;

	/* Audio properties */
	private AudioSource audioSource;
	public AudioClip playerShoot;
	public AudioClip playerDie;
	public AudioClip playerHurt;

	/* FX */
	public ParticleSystem HitParticles;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		camera = Camera.main;
		audioSource = GetComponent<AudioSource>();

		// Init Bullet Pool
		InitBulletPool();
	}

	private void Update()
	{
		if(!InputEnabled) return;
		Rotate();
		Shoot();
	}

	private void FixedUpdate()
	{
		if(!InputEnabled) return;
		Move(); // Move processing in here since it involves physics calculations
	}

	/// <summary>
	/// Rotates player so that faces the camera forward
	/// </summary>
	private void Rotate()
	{
		Quaternion lookRot = Quaternion.LookRotation(camera.transform.forward*100f, Vector3.up);
		transform.rotation = Quaternion.Euler(0, lookRot.eulerAngles.y, 0); // Player rotates towards the camera's forward vector
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
		//rb.velocity += Physics.gravity;

	}

	private void Shoot()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			//Shoot
			Bullet b = GetBullet();
			if (b != null)
			{
				b.rb.velocity = Vector3.zero;
				b.transform.position = BulletStartTransform.position;
				b.transform.rotation = BulletStartTransform.rotation;
				b.gameObject.SetActive(true);

				// SFX
				audioSource.PlayOneShot(playerShoot, 0.5f);
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

	public void GetHit()
	{	
		HitParticles.Play();
		audioSource.PlayOneShot(playerHurt);
		
		//currentHp--;
		//HealthBar.DOFillAmount((float)currentHp/(float)MaxHp, 0.5f).SetEase(Ease.InOutSine);
		//if(currentHp <= 1) // I use here 1 to give the space to the UI's heart mask
		//	Die();
	}

	public void Die()
	{
		/*
		if (state == State.Dead) return; // What is already dead may not die again
		state = State.Dead;
		HealthBarGroup.DOFade(0f, 0.5f);
		animator.SetTrigger("Die");
		audioSource.PlayOneShot(EnemyDie);
		*/
	}


}
