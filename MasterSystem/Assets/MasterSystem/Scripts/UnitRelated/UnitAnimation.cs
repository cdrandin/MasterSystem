using UnityEngine;
using System.Collections;

public class UnitAnimation : MonoBehaviour {

	Animator anim;
	int attackHash = Animator.StringToHash("Attack");
	int staggerHash = Animator.StringToHash("Stagger");
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	public void AttackAnimation()
	{
		//stateInfo = anim.GetCurrentAnimatorStateInfo(0);

		anim.SetTrigger(attackHash);

	}

	public void StaggerAnimation()
	{
		//stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		
		anim.SetTrigger(staggerHash);
		
	}
}
