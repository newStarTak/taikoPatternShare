using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testCtrl : MonoBehaviour
{
    public void ClickBtn()
    {
        ScLoadManager.Instance.LoadSceneAsync("ScStart");
    }
}
