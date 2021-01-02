using UnityEngine;
using UnityEngine.UI;

public class MaterialsUnlock : MonoBehaviour
{
    public int ScoreToUnlock = 1;
    public Material SelfMaterial;
    public Text ScoreToUnlockCaption;
    //public GameObject Cube;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("score") > ScoreToUnlock)
        {
            GetComponent<MeshRenderer>().material = SelfMaterial;
            ScoreToUnlockCaption.gameObject.SetActive(false);
        }
        else ScoreToUnlockCaption.text = "Need " + ScoreToUnlock + "\ncube score";
    }

}
