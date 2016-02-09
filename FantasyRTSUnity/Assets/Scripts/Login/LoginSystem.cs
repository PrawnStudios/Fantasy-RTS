using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class LoginSystem : MonoBehaviour
{
    public static string Email = "";
    public static string userName = "";
    public static string firstName = "";
    public static string surname = "";
    public static string password = "";
    public static string cPassword = "";
    public static string confirmPassword = "";
    public static string cEmail = "";
    public static string cUserName = "";

    public string currentMenu = "Login";

    private string CreateAccountUrl = "http://prawnstudios.com/accounts/register.php";
    private string LoginUrl = "http://prawnstudios.com/accounts/login.php";

    //GUI
    public float X = 300;
    public float Y = 65;
    public float Width = 400;
    public float Height = 450;

    void OnGUI()
    {
        //if our current menu = login, the display the login menu
        //by calling our logingui function. Else, display the create account gui by calling its function
        if (currentMenu == "Login")
        {
            LoginGUI();
        }
        else if (currentMenu == "CreateAccount")
        {
            CreateAccountGUI();
        }
    }
    //this method will login accounts
    void LoginGUI()
    {
        GUI.Box(new Rect(300, 65, (Screen.width / 4) + 400, (Screen.height / 4) + 450), "Login");

        //creates a new account window
        if (GUI.Button(new Rect(500, 440, 110, 25), "Create Account"))
        {
            currentMenu = "CreateAccount";
        }
        //log user in
        if (GUI.Button(new Rect(700, 440, 110, 25), "Log In"))
        {
            StartCoroutine("LoginAccount"); //login
        }
        
        //Offline Mode
        if (GUI.Button(new Rect(700, 500, 110, 25), "Offline Mode"))
        {
            SceneManager.LoadScene("MainMenu");
            PlayerPrefs.SetInt("Offline Mode", 1);
            PlayerPrefs.SetString("Username", "Offline Mode");
        }

        //Close the game
        if (GUI.Button(new Rect(600, 550, 110, 25), "Close Game"))
        {
            Application.Quit();
        }

        GUI.Label(new Rect(565, 260, 188, 25), "Username:");
        userName = GUI.TextField(new Rect(565, 280, 188, 25), userName);

        GUI.Label(new Rect(565, 320, 188, 25), "Password:");
        password = GUI.PasswordField(new Rect(565, 340, 188, 25), password, "*"[0], 25);

    }
    //This method creates the gui for the account
    void CreateAccountGUI()
    {
        GUI.Box(new Rect(300, 65, (Screen.width / 4) + 400, (Screen.height / 4) + 450), "Account Creation");

        if (GUI.Button(new Rect(500, 440, 110, 25), "Create Account"))
        {
            if (confirmPassword == cPassword)
            {
                StartCoroutine("CreateAccount"); //Create account
            }
            currentMenu = "CreateAccount";
        }

        if (GUI.Button(new Rect(700, 440, 110, 25), "Back"))
        {
            currentMenu = "Login";
        }

        GUI.Label(new Rect(565, 80, 188, 25), "First name:");
        firstName = GUI.TextField(new Rect(565, 100, 188, 25), firstName);

        GUI.Label(new Rect(565, 140, 188, 25), "Surname:");
        surname = GUI.TextField(new Rect(565, 160, 188, 25), surname);

        GUI.Label(new Rect(565, 200, 188, 25), "Email:");
        cEmail = GUI.TextField(new Rect(565, 220, 188, 25), cEmail);

        GUI.Label(new Rect(565, 260, 188, 25), "Username:");
        cUserName = GUI.TextField(new Rect(565, 280, 188, 25), cUserName);

        GUI.Label(new Rect(565, 320, 188, 25), "Password:");
        cPassword = GUI.PasswordField(new Rect(565, 340, 188, 25), cPassword, "*"[0], 25);

        GUI.Label(new Rect(565, 380, 188, 25), "Repeat Password:");
        confirmPassword = GUI.PasswordField(new Rect(565, 400, 188, 25), confirmPassword, "*"[0], 25);

    }

    IEnumerator CreateAccount()
    {
        Debug.Log("Account Creation Called");
        //this is what sends messages to our php script
        WWWForm form = new WWWForm();
        //the fields are the variables we are sending
        form.AddField("FirstName", firstName);
        form.AddField("Surname", surname);
        form.AddField("Username", cUserName);
        form.AddField("Email", cEmail);
        form.AddField("Password", cPassword);
        form.AddField("ConfirmPassword", confirmPassword);
        WWW CreateAccountWWW = new WWW(CreateAccountUrl, form);
        //wait for php to send something back to unity
        yield return CreateAccountWWW;

        if (CreateAccountWWW.error != null)
        {
            Debug.LogError("Cannot connect to account creation server.");
        }
        else
        {
            string CreateAccountReturn = CreateAccountWWW.text;
            if (CreateAccountReturn == "Success")
            {
                Debug.Log("Success: Account created!");
                currentMenu = "Login";
            }
            else { Debug.Log("Error Creating Account"); }
        }

    }

    //logins to account
    IEnumerator LoginAccount()
    {
        // add our values that will go into the php script
        WWWForm form = new WWWForm();
        //make sure the "email" and "password" are spelt the same in php script
        form.AddField("Username", userName);
        form.AddField("Password", password);
        //conect to our url, and put in our form
        WWW LoginAccountWWW = new WWW(LoginUrl, form);
        //make sure we get the returning information before we continue.
        yield return LoginAccountWWW;

        if (LoginAccountWWW.error != null)
        {
            Debug.LogError("Cannot connect to login server.");
        }
        else
        {
            string logText = LoginAccountWWW.text;
            if (logText == "Success")
            {
                Debug.Log("Logging in");
                Debug.Log(logText + "fully logged in!");
                SceneManager.LoadScene("MainMenu");
                PlayerPrefs.SetInt("Offline Mode", 0);
                PlayerPrefs.SetString("Username", userName);
            }
            else
            {
                Debug.Log("Invalid username or password");
            }
        }
    }
}
