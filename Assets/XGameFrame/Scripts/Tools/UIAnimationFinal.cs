using System;
using UnityEngine;
using UnityEngine.UI;

//该脚本主要用于播放序列帧
public class UIAnimationFinal : MonoBehaviour
{
    public Sprite[] SpriteArray;
    public float m_sep = 0.05f;

    private Image m_Image;
    private float m_delta = 0;
    private int m_curFrame = 0;
    public bool isStatr = false;
    public bool isLoop = false;
    public Action _CallBackAction=null;
    public int FrameCount
    {
        get
        {
            return SpriteArray.Length;
        }
    }

    private void OnEnable()
    {
        PlayAnitiom(0);
    }
    
    private void PlayAnitiom(int frame = 0)
    {
        if (m_Image == null)
        {
            m_Image = this.GetComponent<Image>();
        }
        if (frame >= FrameCount)
        {
            frame = 0;
            m_curFrame = frame;
        }
        m_Image.overrideSprite = SpriteArray[frame];
        //m_Image.SetNativeSize();
        isStatr = true;
    }

    void Update()
    {
        if (!isStatr)
        {
            return;
        }
        m_delta += Time.deltaTime;
        if (m_delta > m_sep)
        {
            m_delta = 0;
            m_curFrame++;
            if (isLoop)
            {
                PlayAnitiom(m_curFrame);
            }
            else if (m_curFrame == FrameCount)
            {
                isStatr = false;
                OnAnimationComplete();
            }
            else
            {
                PlayAnitiom(m_curFrame);
            }
        }
    }

    private void OnAnimationComplete()
    {
        this.gameObject.SetActive(false);
        if (_CallBackAction != null)
        {
            _CallBackAction();
        }
    }
    

}
