using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject mainMenuPanel;

    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TextMeshProUGUI errorText;

    [SerializeField] private TextMeshProUGUI playerNameText;

    [SerializeField] private GameObject changeNamePanel;
    [SerializeField] private TMP_InputField changeNameInput;
    [SerializeField] private TextMeshProUGUI changeNameError;

    private const string PLAYER_NAME_KEY = "PLAYER_NAME";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        errorText.text = "";

        if (PlayerPrefs.HasKey(PLAYER_NAME_KEY))
        {
            OpenMainMenu();
        }
        else
        {
            OpenLogin();
        }
    }

    public void ContinueButton()
    {
        string playerName = usernameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            errorText.text = "Please enter a username";
            return;
        }

        if (playerName.Length > 16)
        {
            errorText.text = "Username cannot exceed 16 characters";
            return;
        }

        if (playerName.Length < 3)
        {
            errorText.text = "Username must be at least 3 characters";
            return;
        }

        PlayerPrefs.SetString(PLAYER_NAME_KEY,playerName);

        PlayerPrefs.Save();

        OpenMainMenu();
    }

    private void OpenLogin()
    {
        loginPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    private void OpenMainMenu()
    {
        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        UpdateProfileUI();
    }

    public string GetPlayerName()
    {
        return PlayerPrefs.GetString(PLAYER_NAME_KEY,"Player");
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey("PLAYER_NAME");

        loginPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    private void UpdateProfileUI()
    {
        playerNameText.text = GetPlayerName();
    }
    public void OpenChangeNamePanel()
    {
        changeNamePanel.SetActive(true);

        changeNameInput.text = GetPlayerName();

        changeNameError.text = "";
    }

    public void CloseChangeNamePanel()
    {
        changeNamePanel.SetActive(false);
    }

    public void SaveNewName()
    {
        string newName =changeNameInput.text.Trim();

        if (newName.Length < 3)
        {
            changeNameError.text ="Minimum 3 characters";
            return;
        }

        if (newName.Length > 16)
        {
            changeNameError.text ="Maximum 16 characters";
            return;
        }

        PlayerPrefs.SetString(PLAYER_NAME_KEY,newName);

        PlayerPrefs.Save();

        UpdateProfileUI();

        changeNamePanel.SetActive(false);
    }

}