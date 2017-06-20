using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawn enemies indefinitely in waves in given spawn points. Activated by trigger. Manages a pool.
/// </summary>
public class InfiniteWaveSpawner : WaveSpawner
{
	public static InfiniteWaveSpawner Instance;

	public Transform[] EnemySpawnPoints;
	private bool HasSpawned = false;

	/* Pool */
	public List<Enemy> EnemyPool;
	public GameObject EnemyPrefab;
	public GameObject EnemyPoolParent;
	public int MaxEnemies = 20;

	private void Awake()
	{
		Instance = this;
		EnemyPool = new List<Enemy>();

		InitPool();
	}

	public void OnTriggerEnter(Collider c)
	{
		if(c.gameObject.CompareTag("Player"))
		{
			Spawn();
		}
	}

	/// <summary>
	/// Spawns each wave, waiting for the wave's delay before starting the next one
	/// </summary>
	public override void Spawn()
	{
		if(HasSpawned) return;
		StartCoroutine(SpawnSequence());
	}
	private IEnumerator SpawnSequence()
	{
		HasSpawned = true;
		GameManager gm = GameManager.Instance;
		while(!gm.GameOver)
		{
			// Choose a spawn point
			Vector3 pos = GetRandomSpawnPoint();
			// Obtain enemy, or wait till one is available from the pool
			Enemy e;
			do
			{
				e = GetEnemy();
				yield return null;
			}
			while(e == null);

			// "Instantiate" enemy in the spawn position (copying only xz coords)
			e.transform.position = new Vector3(pos.x, e.transform.position.y, pos.z);
			//Quaternion lookRot = Quaternion.LookRotation(camera.transform.forward*100f, Vector3.up);
			//e.transform.rotation = Quaternion.Euler(0, lookRot.eulerAngles.y, 0);
			e.gameObject.SetActive(true);
			e.Spawn();

			// Wait random time
			yield return new WaitForSeconds(Random.Range(1f, 4f));
		}
		yield break;
	}

	private void InitPool()
	{
		// First check how many enemies are there already spawned (from other spawners)
		foreach (Transform child in EnemyPoolParent.transform) 
		{
			EnemyPool.Add(child.GetComponent<Enemy>()); // and add them to the list
		}
		if(EnemyPool.Count >= MaxEnemies) MaxEnemies = EnemyPool.Count;
		else
		{
			while(EnemyPool.Count < MaxEnemies)
			{				
				GameObject go = Instantiate(EnemyPrefab) as GameObject;
				EnemyPool.Add(go.GetComponent<Enemy>());
				go.SetActive(false);
				go.transform.SetParent(EnemyPoolParent.transform, false);
			}
		}
	}

	private Enemy GetEnemy()
	{
		foreach (Enemy e in EnemyPool) 
		{
			if(!e.gameObject.activeInHierarchy)
			{
				return e;
			}
		}

		return null;
	}

	/// <summary>
	/// Returns a random spawn point from the ones provided
	/// </summary>
	private Vector3 GetRandomSpawnPoint()
	{
		int r = Random.Range(0, EnemySpawnPoints.Length);
		return EnemySpawnPoints[r].position;
	}

	public void KillAllEnemies()
	{
		foreach (Enemy e in EnemyPool) 
		{
			if(e.gameObject.activeInHierarchy)
			{
				e.Die();
			}
		}
	}

}
