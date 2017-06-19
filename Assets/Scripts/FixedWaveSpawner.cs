using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawn enemies in predefined waves. Activated through a trigger
/// </summary>
public class FixedWaveSpawner : WaveSpawner
{
	public Wave[] Waves;
	private bool HasSpawned = false;

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
		for(int i=0; i < Waves.Length; i++)
		{
			foreach (Enemy e in Waves[i].Enemies) 
			{
				e.Spawn();
			}

			yield return new WaitForSeconds(Waves[i].Delay);
		}
	}

}
