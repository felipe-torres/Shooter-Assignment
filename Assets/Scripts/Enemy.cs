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

	public enum State { Buried, Idle, Follow, Rest, Dead }
	public State state = State.Idle;

	// Should be the player. Exposed for scalability
	public Transform Target;
	private Player player;
	private UnityEngine.AI.NavMeshAgent agent;
	public Collider CollisionCollider;
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
		player = Target.GetComponent<Player>();

		// Some enemies spawn at some point during gameplay
		if(BuryOnStart) Bury();
	}

	public void Bury()
	{
		state = State.Buried;
		agent.enabled = false;
		transform.DOMoveY(-1.68f, 0f).SetRelative(true);
		HealthBarGroup.DOFade(0f, 0.5f);
		CollisionCollider.enabled = false;
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
		HealthBarGroup.DOFade(0.5f, 0.5f);
		CollisionCollider.enabled = true;
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
			if(agent.enabled && player.IsAlive) agent.SetDestination(Target.position);
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
			RandomyAccelerate();
			animator.SetBool("Move", true);
		}
	}

	public void GetHit()
	{	
		HitParticles.Play();
		audioSource.PlayOneShot(EnemyHurt);
		currentHp--;
		HealthBar.DOFillAmount((float)currentHp/(float)MaxHp, 0.5f).SetEase(Ease.InOutSine);
		if(currentHp <= 0)
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
		CollisionCollider.enabled = false;
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
		if(state != State.Follow) return;
		if (c.gameObject.CompareTag("Player"))
		{
			c.rigidbody.AddForce((this.transform.forward+Vector3.up)*2.5f, ForceMode.Impulse);
			c.gameObject.GetComponent<Player>().GetHit();
			StartCoroutine(AttackDelaySequence(Random.Range(0.5f, 2f)));
		}
	}

	/// <summary>
	/// This method will make this enemy randomly accelerate (as zombies usually do)
	/// </summary>
	private void RandomyAccelerate()
	{
		StartCoroutine(RandomyAccelerateSeq());
	}
	private IEnumerator RandomyAccelerateSeq()
	{
		while(state == State.Follow)
		{
			agent.speed = 1.5f;
			animator.speed = 1f;
			yield return new WaitForSeconds(Random.Range(1f, 5f));
			agent.speed = Random.Range(2.3f, 3f);
			animator.speed *= 2f;
			yield return new WaitForSeconds(Random.Range(2f, 5f));
		}
	}

	/// <summary>
	/// Delays the next attack
	/// </summary>
	private IEnumerator AttackDelaySequence(float time)
	{
		state = State.Rest;
		animator.SetBool("Move", false);
		yield return new WaitForSeconds(time);
		state = State.Idle;
	}


}
