using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScale : MonoBehaviour
{
    private Button button;
    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        if (transform.gameObject.activeInHierarchy)
        {
            StartCoroutine(DoButtonScale());
        }
    }
    private Vector3 ScaleMinVec=Vector3.one*0.9f;
    private IEnumerator DoButtonScale()
    {
        transform.DOScale(ScaleMinVec, 0.1f);
        yield return new WaitForSeconds(0.1f);
        transform.DOScale(Vector3.one, 0.1f);
    }

}
