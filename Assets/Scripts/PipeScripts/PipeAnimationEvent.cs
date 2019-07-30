using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeAnimationEvent : MonoBehaviour
{
    private void Start()
    {
        EventDispatcher.Instance.RegisterListener(EventID.SkipAnimation, speedUp);
    }

    public void startNextAnimation()
    {
        PipeProperties c_p = GetComponent<PipeProperties>();
        if (c_p == null) c_p = GetComponentInParent<PipeProperties>();
        PipeProperties n_p = c_p.next[c_p.temp].GetComponent<PipeProperties>();
        int anim_rotation = n_p.animRotation[n_p.n_Line * 4 + (c_p.nextIn[c_p.temp] - n_p.rotation + 4) % 4];
        string anim_state = n_p.animState[n_p.n_Line * 4 + (c_p.nextIn[c_p.temp] - n_p.rotation + 4) % 4];
        n_p.n_Line++;
        c_p.temp++;
        n_p.GetComponent<Animator>().Play(anim_state);
        
        n_p.transform.eulerAngles -= new Vector3(0f, 0f, anim_rotation * 90);
        n_p.rotation = (n_p.rotation - anim_rotation) % 4;
    }

    public void endOfPipeLines()
    {
        EventDispatcher.Instance.PostEvent(EventID.PipeAnimationEnd, null);
        AudioManager.Instance.Stop("water");
    }

    private void speedUp(object param)
    {
        GetComponent<Animator>().speed += 2f;
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EventID.SkipAnimation, speedUp);
    }
}
