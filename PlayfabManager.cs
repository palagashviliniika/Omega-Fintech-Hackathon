using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Linq;
using UnityEngine.UI;
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

    [SerializeField] private Sprite[] cardDesignMain;
    [SerializeField] private Sprite[] transferIcon;
    [SerializeField] private Sprite[] requestIcon;
    [SerializeField] private Sprite[] inAndOutIcon;
    [SerializeField] private Sprite[] bankIcon;
    [SerializeField] private Image[] mainCardImage;
    [SerializeField] private GameObject requestObject;
    [SerializeField] private GameObject NewInfoObject;
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
        OnLoginGetPlayerPlayfabID();
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
        PlayerPrefs.SetString("myplayfabID", result.AccountInfo.PlayFabId);
        myplayfabID = result.AccountInfo.PlayFabId;
        StartCoroutine(KeepRequestingInfo());
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
        if (pageToOpen == 3)
        {
            UpdateUIElements(false,3);
        }
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
        Cards temp = new Cards();
        temp.cardIBAN = IBANInputField.text;
        root.cardsList.Add(temp);
        UpdateUserInfo();
        OpenPage(3);
    }
    #endregion
    public IEnumerator KeepRequestingInfo()
    {
        while (true)
        {
            RequestMyInfo();
            yield return new WaitForSeconds(10f);
        }
    }
    public void RequestMyInfo()
    {
        Debug.Log("Trying to retrieve info with: " + myplayfabID);
        var request = new GetUserDataRequest
        {
            PlayFabId = myplayfabID
        };
        PlayFabClientAPI.GetUserData(request, OnRequestMyInfoSuccess, OnRequestMyInfoError);
    }
    public void OnRequestMyInfoSuccess(GetUserDataResult result)
    {
        if(result.Data.Count == 0)
        {
            UpdateUIElements(true,activePage);
        }
        else
        {
            root =JsonUtility.FromJson<PaymentRoot>(result.Data.ElementAt(0).Value.Value);
            UpdateUIElements(false,activePage);
        }
    }
    public void OnRequestMyInfoError(PlayFabError error)
    {

    }
    public void UpdateUserInfo()
    {
        string temp = JsonUtility.ToJson(root);
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "info" ,temp }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnUpdateUserInfoSuccess, OnUpdateUserInfoError);
    }
    public void OnUpdateUserInfoSuccess(UpdateUserDataResult result)
    {
        Debug.Log("user data has been updated successfuly");
    }
    public void OnUpdateUserInfoError(PlayFabError error)
    {

    }
    public void UpdateUIElements(bool blank,int page)
    {
        if (blank)
        {

        }
        else
        {
            if(page == 3)
            {
                if(root.cardsList.Count == 0)
                {

                }
                else
                {
                    card.transform.GetChild(0).gameObject.SetActive(false);
                    int temp = root.cardsList.Count - 1;
                    card.transform.GetChild(1).GetComponent<TMP_Text>().text = root.cardsList[temp].cardIBAN;
                    switch (root.cardsList[temp].cardIBAN.Substring(5, 2))
                    {
                        case "BG":
                            mainCardImage[0].sprite = cardDesignMain[0];
                            mainCardImage[1].sprite = transferIcon[0];
                            mainCardImage[2].sprite = requestIcon[0];
                            mainCardImage[3].sprite = inAndOutIcon[0];
                            mainCardImage[4].sprite = bankIcon[0];
                            break;
                        case "TB":
                            mainCardImage[0].sprite = cardDesignMain[1];
                            mainCardImage[1].sprite = transferIcon[1];
                            mainCardImage[2].sprite = requestIcon[1];
                            mainCardImage[3].sprite = inAndOutIcon[1];
                            mainCardImage[4].sprite = bankIcon[1];
                            break;
                        case "LB":
                            mainCardImage[0].sprite = cardDesignMain[2];
                            mainCardImage[1].sprite = transferIcon[2];
                            mainCardImage[2].sprite = requestIcon[2];
                            mainCardImage[3].sprite = inAndOutIcon[2];
                            mainCardImage[4].sprite = bankIcon[2];
                            break;
                    }
                    if (root.transferRequestList.Count > 0)
                    {
                        requestObject.SetActive(true);
                    }
                    if (root.newTransferList.Count > 0)
                    {
                        NewInfoObject.SetActive(true);
                    }
                }
            }
        }
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
