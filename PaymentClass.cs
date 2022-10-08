using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PaymentRoot
{
    public List<Cards> cardsList = new List<Cards>();
    public List<Transfers> expenseList = new List<Transfers>();
    public List<NewTransfers> newTransferList = new List<NewTransfers>();
    public List<TransferRequests> transferRequestList = new List<TransferRequests>();
}
public class Cards
{
    public string cardIBAN;
}
public class Transfers
{
    public string amount;
    public string phoneNumber;
    public string sendTo;
    public bool income;
    public string IBAN;
}
public class NewTransfers
{
    public string amount;
    public string phoneNumber;
    public string sendTo;
    public bool income;
    public string IBAN;
}
public class TransferRequests
{
    public string amount;
    public string phoneNumber;
    public string sendTo;
    public bool income;
    public string IBAN;
}
public class PaymentClass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
