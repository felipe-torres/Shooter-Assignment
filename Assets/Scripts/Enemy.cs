using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.AI;

/// <summary>
/// Describes the behavior for an enemy
/// </summary>
public class Enemy : MonoBehaviour
{
	private Animator animator;

	public float MovementSpeed = 2.0f;
	public int MaxHp = 5;
	private int currentHp;

	public enum State { Buried, Idle, Follow, Dead }
	public State state = State.Idle;

	// Should be the player. Exposed for scalability
	public Transform Target;
	private UnityEngine.AI.NavMeshAgent agent;
	public bool SeesTarget { get; set; }
	public bool BuryOnStart = true;

	// FX
	public ParticleSystem DeadParticles;
	public ParticleSystem HitParticles;

	// UI
	public Image HealthBar;
	public CanvasGroup HealthBarGroup;

	/* Audio properties */
	private AudioSource audioSource;
	public AudioClip EnemyDie;
	public AudioClip EnemyHurt;
	public AudioClip EnemySpawn;


	private void Awake()
	{
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		audioSource = GetComponent<AudioSource>();

		Target = GameObject.FindGameObjectWithTag("Player").transform;

		// Some enemies spawn at some point during gameplay
		if(BuryOnStart) Bury();
	}

	public void Bury()
	{
		state = State.Buried;
		agent.enabled = false;
		transform.DOMoveY(-1.68f, 0f).SetRelative(true);
		HealthBarGroup.DOFade(0f, 0.5f);
	}

	[ContextMenu ("Spawn")]
	public void Spawn()
	{
		StartCoroutine(SpawnSequence());
	}
	private IEnumerator SpawnSequence()
	{
		transform.DOMoveY(1.68f, 2.5f).SetRelative(true).SetEase(Ease.OutSine);
		DeadParticles.transform.DOLocalMoveY(1.33f, 0f);
		DeadParticles.Play();
		audioSource.PlayOneShot(EnemyHurt);
		yield return new WaitForSeconds(2.5f);
		agent.enabled = true;
		state = State.Idle;
		HealthBarGroup.DOFade(1f, 0.5f);
	}

	private void OnEnable()
	{
		currentHp = MaxHp; 
		HealthBar.fillAmount = 1;
		HealthBarGroup.alpha = 1f;
	}

	private void Update()
	{
		switch (state)
		{
		case State.Idle:
			// Search for target
			Search();
			break;
		case State.Follow:
			agent.SetDestination(Target.position);
			break;
		case State.Dead:
			break;
		}
	}

	private void Search()
	{
		if (SeesTarget)
		{
			state = State.Follow;
			animator.SetBool("Move", true);
		}
	}

	public void Attack()
	{

	}

	public void GetHit()
	{	
		HitParticles.Play();
		audioSource.PlayOneShot(EnemyHurt);
		currentHp--;
		HealthBar.DOFillAmount((float)currentHp/(float)MaxHp, 0.5f).SetEase(Ease.InOutSine);
		if(currentHp <= 1) // I use here 1 to give the space to the UI's heart mask
			Die();
	}

	public void Die()
	{
		if (state == State.Dead) return; // What is already dead may not die again
		state = State.Dead;
		HealthBarGroup.DOFade(0f, 0.5f);
		animator.SetTrigger("Die");
		audioSource.PlayOneShot(EnemyDie);
	}

	/// <summary>
	/// Effects to despawn enemy after it has been hit
	/// </summary>
	public void StartSinking()
	{
		StartCoroutine(SinkSequence());
	}
	private IEnumerator SinkSequence()
	{
		agent.enabled = false;
		transform.DOMoveY(-1f, 1.5f).SetRelative(true).SetEase(Ease.InSine).SetDelay(0.5f);
		//yield return new WaitForSeconds(0.35f);
		DeadParticles.transform.DOLocalMoveY(0.428f, 0f);
		DeadParticles.Play();
		yield return new WaitForSeconds(3.5f);
		gameObject.SetActive(false);
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.gameObject.CompareTag("Player"))
		{
			print("Trigger entered by player");
			SeesTarget = true;
		}
	}
	private void OnTriggerExit(Collider c)
	{
		if (c.gameObject.CompareTag("Player"))
		{
			SeesTarget = false;
		}
	}

	private void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.CompareTag("Player"))
		{
			c.rigidbody.AddForce((this.transform.forward+Vector3.up)*20f, ForceMode.Impulse);
			c.gameObject.GetComponent<Player>().GetHit();
		}
	}

	/// <summary>
	/// This method will make this enemy randomly accelerate (as zombies usually do)
	/// </summary>
	private void RandomyAccelerate()
	{

	}


}
