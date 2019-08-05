using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImoSysSDK.Demo
{

    public class PopupManager : MonoBehaviour
    {
        [SerializeField] GameObject popup;
        // Use this for initialization
        public void ClosePopUp()
        {
            popup.SetActive(false);
        }

        public void ShowPopUp()
        {
            popup.SetActive(true);
        }
    }
}