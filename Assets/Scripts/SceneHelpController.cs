using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SceneHelpController : MonoBehaviour
{
    public Button btnSound;
    public Button btnRemove;
    public Button btnConstruct;
    public Text txtPoints;
    public Text txtCoins;
    public Sprite[] s_sounds;


    public GameObject[] pipes;
    public int rotate_speed;

    private GameObject valve;
    private int row;
    private int col;
    private int length;
    private int[] l_pipes = {5, 1, 2, 1, 2, 1, 1, 6};
    private int[] pos_y = {0, 1, 2, 2, 2, 3, 4, 5};
    private int[] pos_x = {0, 0, 0, 1, 2, 2, 2, 2};
    private int[] l_angles = {2, 0, 0, 1, 2, 0, 0, 0};
    private int[] u_y = {2, 3};
    private int[] u_x = {0, 2};
    private List<GameObject> list_results;
    private List<int> list_dirs;

    private void Awake()
    {
        if (GameData.Instance.sound_on) btnSound.GetComponent<Image>().sprite = s_sounds[1];
        else btnSound.GetComponent<Image>().sprite = s_sounds[0];
        btnRemove.interactable = GameData.Instance.coins >= 50;
        btnConstruct.interactable = GameData.Instance.coins >= 25;
        txtCoins.text = GameData.Instance.coins.ToString();
        txtPoints.text = GameData.Instance.points.ToString();
    }

    
    private void setupLevel()
    {
        row = 6;
        col = 4;
        length = l_pipes.Length;
        for(int i = 0; i < length; i++)
        {
            Vector3 position;
            position.z = 0;
            position.y = ((row / 2 - pos_y[i]) - 0.5f) * 1.4f;
            position.x = ((pos_x[i] - col / 2) + 0.5f) * 1.4f;
            
            GameObject go = Instantiate(pipes[l_pipes[i] - 1], position, Quaternion.Euler(0f, 0f, -l_angles[i] * 90f));
            go.GetComponent<PipeProperties>().rotation = l_angles[i];
            go.GetComponent<BoxCollider2D>().enabled = false;
            if (l_pipes[i] == 5) valve = go;
            
            
        }
        
    }

    private IEnumerator rotatePipe(GameObject gameObject, float speed)
    {
        float angle = -90;
        PipeProperties pp = gameObject.GetComponent<PipeProperties>();

        pp.rotation = (pp.rotation + 1) % 4;
        while (angle < 0)
        {
            gameObject.transform.eulerAngles -= new Vector3(0f, 0f, speed);
            angle += speed;
            yield return 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
