using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeAnimationEvent : MonoBehaviour
{
    public void stopAnimation()
    {
        
        GetComponent<Animator>().enabled = false;
    }

    public void startNextAnimation()
    {
        GameController gameController = GameObject.Find("ScriptController").GetComponent<GameController>();
        gameController.index++;
        int next_in = gameController.list_dir[gameController.index];
        GameObject nextObj = gameController.result[gameController.index];
        PipeProperties pp = nextObj.GetComponent<PipeProperties>();
        int anim_rotation = pp.anim_rotation[pp.n_line * 4 + (next_in - pp.rotation + 4) % 4];
        string anim_state = pp.anim_state[pp.n_line * 4 + (next_in - pp.rotation + 4) % 4];
        pp.n_line++;
        
        nextObj.GetComponent<Animator>().Play(anim_state);
        
        nextObj.GetComponent<Animator>().enabled = true;
        nextObj.transform.eulerAngles -= new Vector3(0f, 0f, anim_rotation * 90);
        pp.rotation = (pp.rotation - anim_rotation) % 4;
    }

    public void endOfPipeLines()
    {
        GetComponent<Animator>().enabled = false;
        GameObject.Find("ScriptController").GetComponent<GameController>().game_over = true;
    }
}
