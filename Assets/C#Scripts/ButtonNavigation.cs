using UnityEngine;

public class ButtonNavigation : MonoBehaviour
{
    public GameObject homePagePanel;
    public GameObject tinderswipepanel;
    public GameObject welcomePanel;
    public GameObject itsAMatch;
    public GameObject notForYou;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void changeThat(string nameOfPanel)
    {
        homePagePanel.SetActive(nameOfPanel == "Home Page Panel");
        tinderswipepanel.SetActive(nameOfPanel == "Tinder swipe Panel");
        welcomePanel.SetActive(nameOfPanel == "Welcome panel");
        itsAMatch.SetActive(nameOfPanel == "Its a Match Panel");
        notForYou.SetActive(nameOfPanel == "NotForYouPanel");
        
    }
}
