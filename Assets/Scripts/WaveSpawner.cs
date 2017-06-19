using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines waves of enemies to spawn
/// </summary>
public abstract class WaveSpawner : MonoBehaviour
{
	[System.Serializable]
	public struct Wave
	{
		public Enemy[] Enemies;
		public float Delay;
	}

	public abstract void Spawn();
}
