using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    #region Variables
    
    [SerializeField]
    private             Question[]          _questions              = null;
    public              Question[]          Questions               { get { return _questions; } }

    [SerializeField]    GameEvents          events                  = null;

    [SerializeField]    Animator            timerAnimtor            = null;
    [SerializeField]    TextMeshProUGUI     timerText               = null;
    [SerializeField]    Color               timerHalfWayOutColor    = Color.yellow;
    [SerializeField]    Color               timerAlmostOutColor     = Color.red;
    private             Color               timerDefaultColor       = Color.white;
    [SerializeField]
    private             List<AnswerData>    PickedAnswers           = new List<AnswerData>();
    private             List<int>           FinishedQuestions       = new List<int>();
    private             int                 currentQuestion         = 0;

    private             int                 timerStateParaHash      = 0;

    private             IEnumerator         IE_WaitTillNextRound    = null;
    private             IEnumerator         IE_StartTimer           = null;

    //private Question _currentQuestion = null;
    //public Question CurrentQuestion { get { return _currentQuestion; } }

    public int questionId = 0;
    public int[] savedAnswers;

    public GameObject finishButton;
    public GameObject nextButton;

    private             bool                IsFinished
    {
        get
        {
            return (FinishedQuestions.Count < Questions.Length) ? false : true;
        }
    }

    #endregion

    #region Default Unity methods

    /// <summary>
    /// Function that is called when the object becomes enabled and active
    /// </summary>
    void OnEnable()
    {
        events.UpdateQuestionAnswer += UpdateAnswers;
    }
    /// <summary>
    /// Function that is called when the behaviour becomes disabled
    /// </summary>
    void OnDisable()
    {
        events.UpdateQuestionAnswer -= UpdateAnswers;
    }

    /// <summary>
    /// Function that is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    void Awake()
    {
        events.CurrentFinalScore = 0;
    }
    /// <summary>
    /// Function that is called when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        events.StartupHighscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);

        timerDefaultColor = timerText.color;
        LoadQuestions();

        timerStateParaHash = Animator.StringToHash("TimerState");

        var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(seed);

        savedAnswers = new int[8] {-1, -1, -1,-1,-1,-1,-1,-1};

        Display();
    }

    #endregion

    /// <summary>
    /// Function that is called to update new selected answer.
    /// </summary>
    public void UpdateAnswers(AnswerData newAnswer)
    {
        //if (Questions[currentQuestion].GetAnswerType == Question.AnswerType.Single)
        //{
            foreach (var answer in PickedAnswers)
            {
                if (answer != newAnswer)
                {
                    answer.Reset();
                }
            }
            PickedAnswers.Clear();
            PickedAnswers.Add(newAnswer);
            savedAnswers[questionId] = newAnswer.answerId;

        //}
        //else
        //{
        //    bool alreadyPicked = PickedAnswers.Exists(x => x == newAnswer);
        //    if (alreadyPicked)
        //    {
        //        PickedAnswers.Remove(newAnswer);
        //    }
        //    else
        //    {
        //        PickedAnswers.Add(newAnswer);
        //    }
        //}
    }

    /// <summary>
    /// Function that is called to clear PickedAnswers list.
    /// </summary>
    public void EraseAnswers()
    {
        PickedAnswers = new List<AnswerData>();
    }

    /// <summary>
    /// Function that is called to display new question.
    /// </summary>
    void Display()
    {
        EraseAnswers();

        var question = GetQuestion();

        if (events.UpdateQuestionUI != null)
        {
            events.UpdateQuestionUI(question);
        } else { Debug.LogWarning("Ups! Something went wrong while trying to display new Question UI Data. GameEvents.UpdateQuestionUI is null. Issue occured in GameManager.Display() method."); }

        //MARCAR RESPOSTA ESCOLHIDA SE HOUVER
        if(savedAnswers[questionId] != -1)
        {
            //events.UpdatePreviousAnswer(savedAnswers[questionId]);
            StartCoroutine("IEUpdatePrevious");
        }

        //if (question.UseTimer)
        //{
        //    UpdateTimer(question.UseTimer);
        //}
    }

    IEnumerator IEUpdatePrevious(){
        yield return new WaitForSeconds(0.1f); 
        events.UpdatePreviousAnswer(savedAnswers[questionId]);
    }
    /// <summary>
    /// Function that is called to accept picked answers and check/display the result.
    /// </summary>
    public void Accept()
    {
        questionId++;
        if (questionId >= _questions.Length-1) questionId = _questions.Length-1;
        Display();
        TurnButtons();

        //UpdateTimer(false);
        //Não é necessário verificar se está correta
        //bool isCorrect = CheckAnswers();
        //´Não é necessário adicionar em questões já respondidas
        //FinishedQuestions.Add(currentQuestion);

        //UpdateScore((isCorrect) ? Questions[currentQuestion].AddScore : -Questions[currentQuestion].AddScore);

        if (IsFinished)
        {
            SetHighscore();
        }

        //var type 
        //    = (IsFinished) 
        //    ? UIManager.ResolutionScreenType.Finish 
        //    : (isCorrect) ? UIManager.ResolutionScreenType.Correct 
        //    : UIManager.ResolutionScreenType.Incorrect;

        //if (events.DisplayResolutionScreen != null)
        //{
        //    events.DisplayResolutionScreen(type, Questions[currentQuestion].AddScore);
        //}

        //AudioManager.Instance.PlaySound((isCorrect) ? "CorrectSFX" : "IncorrectSFX");

        //if (type != UIManager.ResolutionScreenType.Finish)
        //{
        //    if (IE_WaitTillNextRound != null)
        //    {
        //        StopCoroutine(IE_WaitTillNextRound);
        //    }
        //    IE_WaitTillNextRound = WaitTillNextRound();
        //    StartCoroutine(IE_WaitTillNextRound);
        //}
    }
    void TurnButtons()
    {
        if (questionId == _questions.Length - 1)
        {
            finishButton.SetActive(true);
            nextButton.SetActive(false);
        }
        else
        {
            finishButton.SetActive(false);
            nextButton.SetActive(true);
        }
    }
    public void PreviousQuestion()
    {
        questionId--;
        if (questionId <= 0) questionId = 0;
        Display();

        TurnButtons();
    }

        #region Timer Methods

        void UpdateTimer(bool state)
        {
            switch (state)
            {
                case true:
                    IE_StartTimer = StartTimer();
                    StartCoroutine(IE_StartTimer);

                    timerAnimtor.SetInteger(timerStateParaHash, 2);
                    break;
                case false:
                    if (IE_StartTimer != null)
                    {
                        StopCoroutine(IE_StartTimer);
                    }

                    timerAnimtor.SetInteger(timerStateParaHash, 1);
                    break;
            }
        }
        IEnumerator StartTimer()
        {
            var totalTime = Questions[currentQuestion].Timer;
            var timeLeft = totalTime;

            timerText.color = timerDefaultColor;
            while (timeLeft > 0)
            {
                timeLeft--;

                AudioManager.Instance.PlaySound("CountdownSFX");

                if (timeLeft < totalTime / 2 && timeLeft > totalTime / 4)
                {
                    timerText.color = timerHalfWayOutColor;
                }
                if (timeLeft < totalTime / 4)
                {
                    timerText.color = timerAlmostOutColor;
                }

                timerText.text = timeLeft.ToString();
                yield return new WaitForSeconds(1.0f);
            }
            Accept();
        }
        IEnumerator WaitTillNextRound()
        {
            yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
            Display();
        }

        #endregion
    /// <summary>
    /// Function that is called to check currently picked answers and return the result.
    /// </summary>
    bool CheckAnswers()
    {
        if (!CompareAnswers())
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// Function that is called to compare picked answers with question correct answers.
    /// </summary>
    bool CompareAnswers()
    {
        if (PickedAnswers.Count > 0)
        {
            List<int> c = Questions[currentQuestion].GetCorrectAnswers();
            List<int> p = PickedAnswers.Select(x => x.AnswerIndex).ToList();

            var f = c.Except(p).ToList();
            var s = p.Except(c).ToList();

            return !f.Any() && !s.Any();
        }
        return false;
    }

    /// <summary>
    /// Function that is called to load all questions from the Resource folder.
    /// </summary>
    void LoadQuestions()
    {
        Object[] objs = Resources.LoadAll("Questions", typeof(Question));
        _questions = new Question[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            _questions[i] = (Question)objs[i];
        }

        for (int i = 0; i < _questions.Length; i++)
        {
           RandomizeAnswers(_questions[i]);
        }
    }

    void RandomizeAnswers(Question q){
        int count = q._answers.Length;
        Answer[] tempAnswers = q._answers;
        //Answer[] randAnswers = null;

        for (int i = 0; i < count; i++) {
            //int randomIndex = Random.Range(i, tempAnswers.Length);
            //Answer temp = tempAnswers[randomIndex];
            //randAnswers[i] = temp;
            //q[i] = q[randomIndex];
            //q[randomIndex] = temp;
            
            Answer tmp = q._answers[i];
            int r = Random.Range(i, q._answers.Length);
            q._answers[i] = q._answers[r];
            q._answers[r] = tmp;
        }
        //Debug.Log(q._answers);
    }

    /// <summary>
    /// Function that is called restart the game.
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    /// <summary>
    /// Function that is called to quit the application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Function that is called to set new highscore if game score is higher.
    /// </summary>
    private void SetHighscore()
    {
        var highscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);
        if (highscore < events.CurrentFinalScore)
        {
            PlayerPrefs.SetInt(GameUtility.SavePrefKey, events.CurrentFinalScore);
        }
    }
    /// <summary>
    /// Function that is called update the score and update the UI.
    /// </summary>
    private void UpdateScore(int add)
    {
        events.CurrentFinalScore += add;

        if (events.ScoreUpdated != null)
        {
            events.ScoreUpdated();
        }
    }

    #region Getters

    Question GetQuestion()
    {
        var randomIndex = questionId;
        currentQuestion = randomIndex;

        return Questions[currentQuestion];
    }

    //int GetRandomQuestionIndex()
    //{
    //    var random = 0;
    //    if (FinishedQuestions.Count < Questions.Length)
    //    {
    //        do
    //        {
    //            random = UnityEngine.Random.Range(0, Questions.Length);
    //        } while (FinishedQuestions.Contains(random) || random == currentQuestion);
    //    }
    //    return random;
    //}

    #endregion

    public void ResultsScreen()
    {
        SceneManager.LoadScene("Results");
    }
}