using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XGameTools
{
    public class PlayUIAnimation : MonoBehaviour
    {
        private Image image_play;
        [SerializeField] private Sprite[] anim;
        private void Awake()
        {
            image_play = GetComponent<Image>();
        }
        private void OnEnable()
        {
            StartCoroutine(PlayAnim());
        }

        IEnumerator PlayAnim()
        {
            for (int i = 0; i < anim.Length; i++)
            {
                yield return new WaitForSeconds(0.05f);
                image_play.sprite = anim[i];
            }
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            image_play.sprite = anim[0];
        }
    }
}


