using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyS : MonoBehaviour 
{
	// Container for enemy data
	public EnemyData enemyData; 
	
	// Visual Data
	public SpriteRenderer spriteRenderer; // enemy sprite
	public Animator animator; // animates sprite
	
	// Propertes 
	private int _health; // health property field
	public int health
	{
		get
		{
			return _health; // returns field
		}
		set
		{
			_health = value; // allocates field
		}
	}
	
	private int _stagger; // stagger property field
	public int stagger
	{
		get
		{
			return _stagger;
		}
		set
		{
			_stagger = value;
		}
	}
	
	private float _charge; // charge property field
	public float charge
	{
		get
		{
			return _charge;
		}
		set
		{
			_charge = value;
		}
	}
	
	// UI Objects 
	public GameObject healthBar;
	public GameObject staggerBar;
	public GameObject chargeBar;
	
	// AI State
	public EnemyAI AI;
	
	// Setup function
	public void Setup()
	{
		// Attaches sprite to SpriteRenderer
		spriteRenderer.sprite = enemyData.sprite;
		
		// Sets AI
		AI = EnemyAI.GetAIFromString(enemyData.AIType, enemyData.AIParameters);
		
		// Sets properies
		health = enemyData.maxHP;
		stagger = enemyData.maxStagger;
		charge = 0;
	}
}
