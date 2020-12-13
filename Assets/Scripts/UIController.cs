using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour {

    /// <summary>
    /// Win text mask
    /// </summary>
    private const string MASK = "Team {0} wins!";

    /// <summary>
    /// Button start new battle
    /// </summary>
    [SerializeField]
    private GameObject StartButton = null;

    /// <summary>
    /// Text forr round result
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI WinText = null;

    /// <summary>
    /// Show/hide start button
    /// </summary>
    public void SwitchButton (bool value) {
        StartButton.SetActive (value);
        WinText.text = "";
    }

    /// <summary>
    /// Show win result
    /// </summary>
    /// <param name="team">Team id</param>
    public void ShowResult (Team team) {
        WinText.text = string.Format (MASK, team);
    }

}