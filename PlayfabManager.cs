using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Linq;
public class PlayfabManager : MonoBehaviour
{
    //variables

    #region AuthenticationStuff

    [SerializeField] private TMP_InputField MobilePhoneInputTextLogin;

    [SerializeField] private TMP_InputField PasswordInputTextLogin;

    [SerializeField] private TMP_InputField MobilePhoneInputTextRegister;

    [SerializeField] private TMP_InputField PasswordInputTextRegister;

    [SerializeField] private TMP_InputField PasswordRepeatInputTextRegister;

    private bool AcceptedTermOfService = false;

    private string myplayfabID;
    #endregion

    #region PageStuff

    [SerializeField] private GameObject[] pages;

    private int activePage=0;

    [SerializeField] private GameObject TermsOfServiceGameObject;

    #endregion
    [SerializeField] private TMP_InputField IBANInputField;

    PaymentRoot root = new PaymentRoot();

    [SerializeField] private GameObject card;
    
    //methods
    private void Start()
    {

    }
    #region RegisterRegion
    public void RegisterAccount()
    {
        string temp = PasswordInputTextRegister.text;
        Debug.Log("Trying To Register with login: " + MobilePhoneInputTextRegister.text + " password: " + PasswordInputTextRegister.text + " | " + temp);
        if(PasswordInputTextRegister.text == PasswordRepeatInputTextRegister.text && AcceptedTermOfService)
        {
            var request = new RegisterPlayFabUserRequest
            {
                Username = MobilePhoneInputTextRegister.text,
                Password = PasswordInputTextRegister.text,
                RequireBothUsernameAndEmail = false
            };
            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterAccountSuccess, OnRegisterAccountError);
        }    
    }
    public void OnRegisterAccountSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Register Success");
        OnLoginGetPlayerPlayfabID();
        OpenPage(3);
    }
    public void OnRegisterAccountError(PlayFabError error)
    {
        Debug.Log("Failed To Register Reason: " + error.ErrorMessage + " | " +error.ErrorDetails + " | " +error.Error  );
    }
    #endregion

    #region LoginRegion
    public void LoginAccount()
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = MobilePhoneInputTextLogin.text,
            Password = PasswordInputTextLogin.text
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginAccountSuccess, OnLoginAccountError);
    }
    public void OnLoginAccountSuccess(LoginResult result)
    {
        Debug.Log("Login Success");
        PlayerPrefs.SetString("UserName",MobilePhoneInputTextLogin.text);
        OpenPage(3);
    }
    public void OnLoginAccountError(PlayFabError error)
    {
        Debug.Log("Failed To Login Reason: " + error.ErrorMessage + " | " + error.ErrorDetails + " | " + error.Error);
    }
    public void OnLoginGetPlayerPlayfabID()
    {
        var request = new GetAccountInfoRequest
        {
            Username = PlayerPrefs.GetString("UserName")
        };
        PlayFabClientAPI.GetAccountInfo(request, OnGetPlayfabIDSuccess, OnGetPlayfabIDError);
    }
    public void OnGetPlayfabIDSuccess(GetAccountInfoResult result)
    {
        myplayfabID = result.AccountInfo.PlayFabId;

    }
    public void OnGetPlayfabIDError(PlayFabError error)
    {
        Debug.Log(error.ErrorMessage);
    }
    #endregion

    #region PageManipulation
    public void OpenPage(int pageToOpen)
    {
        Debug.Log("trying to open page " + pageToOpen);
        pages[pageToOpen].SetActive(true);
        Debug.Log("trying to close page " + activePage);
        pages[activePage].SetActive(false);
        activePage = pageToOpen;
    }

    public void TermsOfServiceButton()
    {
        if (!AcceptedTermOfService)
        {
            Debug.Log("entered accept");
            AcceptedTermOfService = true;
            TermsOfServiceGameObject.transform.GetChild(0).gameObject.SetActive(false);
            TermsOfServiceGameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("entered unaccept");
            AcceptedTermOfService = false;
            TermsOfServiceGameObject.transform.GetChild(0).gameObject.SetActive(true);
            TermsOfServiceGameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    public void AddCard()
    {

    }
    #endregion
    public IEnumerator KeepRequestingInfo()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            RequestMyInfo();
        }
    }
    public void RequestMyInfo()
    {
        var request = new GetUserDataRequest
        {
            PlayFabId = myplayfabID
        };
        PlayFabClientAPI.GetUserData(request, OnRequestMyInfoSuccess, OnRequestMyInfoError);
    }
    public void OnRequestMyInfoSuccess(GetUserDataResult result)
    {
        if(result.Data.ElementAt(0).Value.Value == null)
        {
            UpdateUIElements(true);
        }
        else
        {
            root =JsonUtility.FromJson<PaymentRoot>(result.Data.ElementAt(0).Value.Value);
        }
    }
    public void OnRequestMyInfoError(PlayFabError error)
    {

    }
    public void UpdateUIElements(bool blank)
    {

    }
    #region payze send
    //simulating payze request
    public void SendRequestToPerson()
    {

    }
    public void SendRequestPaymentToPayze()
    {
        
    }

    #endregion
}
