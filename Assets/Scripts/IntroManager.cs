using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    private TextApparition textApparition;
    [SerializeField] private List<GameObject> images;
    [SerializeField] private Text uiIntroTxt;
    private List<string> introText;
    [SerializeField] private GameObject imageNext;
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameManager gamemanager;

    public AudioClip IntroMusic;

    private int index = 0;
    private bool canClick = false;
    //private bool introFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        introText = new List<string>();
        introText.Add("You always knew that your wife was full of envy");
        introText.Add("Yet, you didn't imagine that she would go so far");
        introText.Add("To sell her soul to achieve her goals");
        introText.Add("She was no longer there, but her body remained alive");
        introText.Add("And so the devil appeared, asking if you could redeem her soul for a hefty sum");
        introText.Add("A sum you could collect in the Pan's Temple");

        textApparition = GetComponent<TextApparition>();
        TextApparition.uiIntroTxt = uiIntroTxt;
        SetNextIntroDisplay();
        TextApparition.onFinishText += this.OnFinishedText;
        SoundManager.Instance.PlayMusic(IntroMusic,true);
    }

    public void OnFinishedText()
    {
        canClick = true;
        imageNext.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canClick)
        {
            index++;
            SetNextIntroDisplay();
        }
    }

    private void SetNextIntroDisplay()
    {
        if (index < introText.Count)
        {
            canClick = false;
            imageNext.SetActive(false);
            if (index > 0 && index - 1 < images.Count && images[index - 1] != null)
                images[index - 1].SetActive(false);
            textApparition.DisplayText(introText[index]);
            if (index < images.Count && images[index]!=null)
            {
                images[index].SetActive(true);
            }
        }
        else
        {
            introPanel.SetActive(false);
            SoundManager.Instance.FadeOutMusic(1,0);
            this.enabled = false;
            TextApparition.onFinishText -= this.OnFinishedText;
            gamemanager = GetComponent<GameManager>();
            gamemanager.enabled = true;
        }
    }
}
