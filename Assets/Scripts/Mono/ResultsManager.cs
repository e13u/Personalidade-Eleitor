using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsManager : MonoBehaviour
{
    int finalScore = 0;
    int[] scoresAnswers = new int[]{0,0,0,0,0,0,0,0,0};
    public TMP_Text scoreText;
    //public string base_url_2 = "https://docs.google.com/forms/u/1/d/e/1FAIpQLSdE6YN4WF3_-lYYi_nEkTRa_wN_NGoCZpe4bVAYdSZa4DFsIw/formResponse";
    public string base_url = "https://docs.google.com/forms/d/e/1FAIpQLSegBR4OTU18dTOIrN3aWVkyJNmNH6Y511g4MaLC1Mfgw7CnoQ/formResponse";
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
        //Debug.Log(www.text);
        yield return www;
    }
}
