using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProfileData", menuName = "Profile/new Profile")]
public class ProfileData : ScriptableObject
{
    [SerializeField] private string profileTitle;
    [SerializeField] public string ProfileTitle { get { return profileTitle; } }
    [SerializeField] private string profileDescription;
    [SerializeField] public string ProfileDescription { get { return profileDescription; } }
    [SerializeField] private List<int> barValues;
    [SerializeField] public List<int> BarValues { get { return barValues; } }

    [SerializeField] private RuntimeAnimatorController gifAnimator;
    [SerializeField] public RuntimeAnimatorController GifAnimator { get { return gifAnimator; } }
}
