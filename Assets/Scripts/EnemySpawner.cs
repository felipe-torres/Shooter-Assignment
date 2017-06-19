using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns enemies in given spawnpoints
/// </summary>
public class EnemySpawner : MonoBehaviour
{
	public static EnemySpawner Instance;

	public List<Enemy> EnemyPool;
	public GameObject EnemyPrefab;
	public GameObject EnemyPoolParent;

	private void Awake()
	{
		Instance = this;
	}
}
