using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsManager : MonoBehaviour
{
    [SerializeField] private int finalScore = 0;
    int[] scoresAnswers = new int[]{0,0,0,0,0,0,0,0,0};
    public TMP_Text scoreText;
    //public string base_url_2 = "https://docs.google.com/forms/u/1/d/e/1FAIpQLSdE6YN4WF3_-lYYi_nEkTRa_wN_NGoCZpe4bVAYdSZa4DFsIw/formResponse";
    [SerializeField] private  string base_url = "https://docs.google.com/forms/d/e/1FAIpQLSegBR4OTU18dTOIrN3aWVkyJNmNH6Y511g4MaLC1Mfgw7CnoQ/formResponse";
    [SerializeField] private int profileID;
    [SerializeField] private ProfileData profileData;

    [Header("UI")]
    [SerializeField] private List<Slider> scoreBars = new List<Slider>();
    [SerializeField] private RectTransform loadingPanel;
    [SerializeField] private RectTransform gifAnimationPanel;
    [SerializeField] private TMP_Text profileTitleText;
    [SerializeField] private TMP_Text profileDescriptionText;

    // Start is called before the first frame update
    void Start()
    {
        finalScore = PlayerPrefs.GetInt("FinalScore");
        for (int i = 0; i < scoresAnswers.Length; i++)
        {
            scoresAnswers[i] = PlayerPrefs.GetInt("Score_"+i);
        }
        scoreText.text = finalScore.ToString();
        SendScoreToDatabase();
    }

    void SendScoreToDatabase(){
        StartCoroutine(Post(finalScore.ToString(), scoresAnswers));
    }

    IEnumerator Post(string score, int[]scores){
        WWWForm form = new WWWForm();

        form.AddField("entry.1833841215",scores[0]);
        form.AddField("entry.654250768",scores[1]);
        form.AddField("entry.279795190",scores[2]);
        form.AddField("entry.597177502",scores[3]);
        form.AddField("entry.166471195",scores[4]);
        form.AddField("entry.1273587675",scores[5]);
        form.AddField("entry.852674542",scores[6]);
        form.AddField("entry.152251858",scores[7]);
        form.AddField("entry.1849992309",scores[8]);
        form.AddField("entry.1589677956",score);
        
        byte[] rawData = form.data;
        WWW www  = new WWW(base_url, rawData);
        StartCoroutine(LoadTimer());
        yield return www;
    }

    IEnumerator LoadTimer()
    {
        yield return new WaitForSeconds(3.0f);
        ShowResults();
    }
    void ShowResults()
    {
        if (finalScore >= 27 && finalScore <= 36) profileID = 4;
        else if (finalScore >= 21 && finalScore <= 26) profileID = 3;
        else if (finalScore >= 15 && finalScore <= 20) profileID = 2;
        else if (finalScore >= 9 && finalScore <= 13) profileID = 1;
        else profileID = 0;

        string path = "Profiles/" + profileID.ToString();
        profileData = Resources.Load(path) as ProfileData;

        if (profileData == null) print("BUGOU");
        profileTitleText.text = profileData.ProfileTitle;
        profileDescriptionText.text = profileData.ProfileDescription;
        gifAnimationPanel.transform.GetChild(1).GetComponent<Animator>().runtimeAnimatorController = profileData.GifAnimator;
        StartCoroutine(ShowGifAnimation());
    }
    IEnumerator ShowGifAnimation()
    {
        loadingPanel.gameObject.SetActive(false);
        gifAnimationPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(7.0f);
        gifAnimationPanel.gameObject.SetActive(false);
        BarsAnimations();
    }
    void BarsAnimations()
    {
        for (int i = 0; i < scoreBars.Count; i++)
        {
            scoreBars[i].GetComponent<SliderBar>().StartSliderAnimation(profileData.BarValues[i]);
        }
    }
}
