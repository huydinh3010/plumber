
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
public class BtnIAP : MonoBehaviour
{
    [SerializeField] GameObject panelSuccess;
    [SerializeField] GameObject panelFail;
    [SerializeField] Text txtidSuccess;
    [SerializeField] Text txtidFail;
    [SerializeField] Text txtresponse;
    public void CheckSuccess(Product product, int instanceid)
    {
        if (instanceid == gameObject.GetInstanceID())
        {
            Debug.Log("Coin UP++++");
            txtidSuccess.text = product.definition.id;
            panelSuccess.SetActive(true);
        }


    }
    public void CheckFail(Product product, string response)
    {
            txtidFail.text = product.definition.id;
            // txtresponse.text = response;
            panelFail.SetActive(true);
            Debug.Log("FAilllllllllllllllllll" + response);
    }

}
