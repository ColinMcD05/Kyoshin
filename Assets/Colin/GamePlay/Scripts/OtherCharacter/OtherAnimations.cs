using System.Collections;
using UnityEngine;

public class OtherAnimations : MonoBehaviour
{
    public Animator animator;

    public void StartAnimations(string name)
    {
        StartCoroutine(DoAnimation(name));
    }

    public void StartAnimations(string firstTrigger, string intType, int animation)
    {
        StartCoroutine(DoAnimation(firstTrigger, intType, animation));
    }

    IEnumerator DoAnimation(string name)
    {
        yield return new WaitForSeconds(Time.fixedDeltaTime * 3);
        animator.SetTrigger(name);
    }

    IEnumerator DoAnimation(string firstTrigger, string intType, int animation)
    {
        yield return new WaitForSeconds(Time.fixedDeltaTime * 3);
        animator.SetInteger(intType, animation);
        animator.SetTrigger(firstTrigger);
    }
}
