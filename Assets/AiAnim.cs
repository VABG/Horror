using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAnim : MonoBehaviour
{
    public GameObject lookAt;
    Animator anim;
    [SerializeField] Transform neckBone;
    [SerializeField] Transform headBone;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Attack()
    {
        anim.SetTrigger("Attack");
    }

    void LookIK()
    {
        if (lookAt)
        {
            headBone.LookAt(lookAt.transform.position);
            Quaternion.Slerp(neckBone.transform.rotation, headBone.transform.rotation, .5f);
            headBone.LookAt(lookAt.transform.position);
        }
    }

    private void LateUpdate()
    {
        LookIK();
    }
}
