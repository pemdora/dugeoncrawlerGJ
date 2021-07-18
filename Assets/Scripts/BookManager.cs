using UnityEngine;
using TMPro;

public class BookManager : MonoBehaviour
{
    [Header("BOOK")]
    [SerializeField] private Book bookScript;
    [SerializeField] private GameObject bookInputFieldPanel;
    [SerializeField] private TMP_InputField inputFieldBook;
    [SerializeField] private Sprite page6;
    [SerializeField] private GameObject bookSucess;

    [SerializeField] private GameObject bookFeedbacks;

    [HideInInspector] public bool bookFieldWriting = false;

    public void OnFLip()
    {
        if (bookScript.currentPage == 6)
            bookInputFieldPanel.SetActive(true);
        else
            bookInputFieldPanel.SetActive(false);
    }

    // Input field to guess
    public void AnswerTextBook()
    {
        string answer = (inputFieldBook.text).ToLower();
        answer = answer.Trim();
        if (answer == "")
            return;
        
        if (answer == "coffee")
        {
            bookScript.bookPages[6] = page6;
            bookSucess.SetActive(true);
            GameManager.instance.BookAnswerSucess(true); 
        }
        else
        {
            GameManager.instance.BookAnswerSucess(false);
            inputFieldBook.GetComponent<TMP_InputField>().text = "";
        }
    }
    public void OnSelectnputField()
    {
        bookFieldWriting = true;
    }

    public void OnDeSelectnputField()
    {
        bookFieldWriting = false;
    }

    public void SetWinMessage()
    {
        bookFeedbacks.SetActive(true);
    }
}
