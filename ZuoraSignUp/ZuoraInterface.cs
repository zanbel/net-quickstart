using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZuoraSignUp.zuora;

namespace ZuoraSignUp
{
    public class ZuoraInterface
    {

        public static string USERNAME = "<your username>";
        public static string PASSWORD = "<your password>";
        public static string ENDPOINT = "https://www.zuora.com/apps/services/a/17.0";

        private static Boolean GENERATE_INVOICE = false;
        private static Boolean PROCESS_PAYMENTS = false;

        protected zuora.ZuoraService binding;

        public ZuoraInterface()
            : this(USERNAME, PASSWORD, ENDPOINT)
        { }

        public ZuoraInterface(String username, String password, String endpoint)
        {

            //create a new instance of the web service proxy class 
            binding = new zuora.ZuoraService();
            binding.Url = endpoint;

            login(username, password);

        }


        protected bool login(String username, String password)
        {

            try
            {
                //execute the login placing the results  
                //in a LoginResult object 
                LoginResult loginResult = binding.login(username, password);

                //set the session id header for subsequent calls 
                binding.SessionHeaderValue = new zuora.SessionHeader();
                binding.SessionHeaderValue.session = loginResult.Session;

                //reset the endpoint url to that returned from login 
                // binding.Url = loginResult.ServerUrl;

                print("Session: " + loginResult.Session);
                print("ServerUrl: " + loginResult.ServerUrl);

                return true;
            }
            catch (Exception ex)
            {
                //Login failed, report message then return false 
                Console.WriteLine("Login failed with message: " + ex.Message);
                return false;
            }
        }



        protected string create(zObject acc)
        {
            SaveResult[] result = binding.create(new zObject[] { acc });
            return result[0].Id;
        }

        protected string update(zObject acc)
        {
            SaveResult[] result = binding.update(new zObject[] { acc });
            return result[0].Id;
        }

        protected bool delete(String type, string id)
        {

            DeleteResult[] result = binding.delete(type, new string[] { id });
            return result[0].success;

        }

        protected Account queryAccount(string accId)
        {
            QueryResult qResult = binding.query("SELECT id, name, accountnumber FROM account WHERE id = '" + accId + "'");
            Account rec = (Account)qResult.records[0];
            return rec;
        }

        // BUGBUG: deal with expiration later
        public Product[] queryProducts()
        {

            QueryResult result = binding.query("select Id, Name FROM Product");
            zObject[] recs = (zObject[])result.records;
            Product[] res = null;
            if (result.size > 0 && recs != null && recs.Length > 0)
            {
                res = new Product[recs.Length];
                for (int i = 0; i < recs.Length; i++)
                {
                    res[i] = (Product)recs[i];
                }

            }
            return res;

        }

        // BUGBUG: deal with expiration later
        public ProductRatePlan[] queryRatePlansByProduct(String productId)
        {

            QueryResult result = binding.query("select Id, Name FROM ProductRatePlan where ProductId = '" + productId + "'");
            zObject[] recs = (zObject[])result.records;
            ProductRatePlan[] res = null;
            if (result.size > 0 && recs != null && recs.Length > 0)
            {
                res = new ProductRatePlan[recs.Length];
                for (int i = 0; i < recs.Length; i++)
                {
                    res[i] = (ProductRatePlan)recs[i];
                }

            }
            return res;

        }

        private static String CHARGE_SELECT_LIST = "Id, Name, AccountingCode, DefaultQuantity, Type, Model, ProductRatePlanId";

        // BUGBUG: deal with expiration later
        public ProductRatePlanCharge[] queryChargesByProductRatePlan(String prpId)
        {

            QueryResult result = binding.query("select "+CHARGE_SELECT_LIST+" from ProductRatePlanCharge where ProductRatePlanId = '" + prpId + "'");
            zObject[] recs = (zObject[])result.records;
            ProductRatePlanCharge[] res = null;
            if (result.size > 0 && recs != null && recs.Length > 0)
            {
                res = new ProductRatePlanCharge[recs.Length];
                for (int i = 0; i < recs.Length; i++)
                {
                    res[i] = (ProductRatePlanCharge)recs[i];
                }

            }
            return res;

        }

        public ProductRatePlanCharge queryChargeById(String id)
        {

            QueryResult result = binding.query("select " + CHARGE_SELECT_LIST + " from ProductRatePlanCharge where Id = '" + id + "'");
            ProductRatePlanCharge rec = (ProductRatePlanCharge)result.records[0];
            return rec;

        }

        public ProductRatePlanCharge queryChargeByAccountingCode(String accountingCode)
        {

            QueryResult result = binding.query("select " + CHARGE_SELECT_LIST + " from ProductRatePlanCharge where AccountingCode = '" + accountingCode + "'");
            ProductRatePlanCharge rec = (ProductRatePlanCharge)result.records[0];
            return rec;

        }

        protected Account makeAccount(string Name, string CurrencyIso)
        {
            Account acc = new Account();
            acc.Name = Name;
            acc.Currency = CurrencyIso;
            acc.Status = "Draft";
            acc.PaymentTerm = "Due Upon Receipt";
            acc.Batch = "Batch1";
            acc.BillCycleDay = 1;
            acc.BillCycleDaySpecified = true;
            acc.AllowInvoiceEdit = true; 
            acc.AutoPay = false;

            // uncomment if you want to specify your own account number 
            // acc.AccountNumber = "ACC-" + DateTime.Now.Ticks;

            // optional values
            // acc.CustomerServiceRepName = "CSR Dude";
            // acc.SalesRepName = "Sales Dude";
            // acc.PurchaseOrderNumber = "PO-" + time;
            // acc.CrmId = "SFDC-" + time;
            return acc;
        }


        protected Contact makeContact(string FirstName, string LastName, string WorkEmail, string WorkPhone, string Address1, string Address2, string City, string State, string Country, string PostalCode)
        {
            Contact con = new Contact();
            con.FirstName = FirstName;
            con.LastName = LastName;
            con.Address1 = Address1;
            con.City = City;
            con.State = State;
            con.Country = Country;
            con.PostalCode = PostalCode;
            con.WorkEmail = WorkEmail;
            con.WorkPhone = WorkPhone;
            return con;
        }


        /**
         * Creates a Subscription object reading the values from the property
         * @return
         */
        protected Subscription makeSubscription(String subscriptionName, String subscriptionNotes)
        {

            DateTime calendar = DateTime.Now;

            Subscription sub = new Subscription();
            sub.Name = subscriptionName;
            sub.Notes = subscriptionNotes;

            sub.TermStartDate = calendar;
            sub.ContractEffectiveDate = calendar;
            sub.ContractAcceptanceDate = calendar;
            sub.ServiceActivationDate = calendar;

            sub.InitialTerm = 12;
            sub.InitialTermSpecified = true;
            sub.RenewalTerm = 12;
            sub.RenewalTermSpecified = true;

            return sub;
        }

        protected RatePlanData[] makeRatePlanData(ProductRatePlanCharge[] charges)
        {

            RatePlanData[] data = new RatePlanData[charges.Length];

            for (int i = 0; i < charges.Length; i++)
            {
                ProductRatePlanCharge charge = charges[i];
                RatePlanData ratePlanData = new RatePlanData();

                RatePlan ratePlan = new RatePlan();
                ratePlanData.RatePlan = ratePlan;
                ratePlan.AmendmentType = "NewProduct";
                ratePlan.ProductRatePlanId = charge.ProductRatePlanId;

                RatePlanChargeData ratePlanChargeData = new RatePlanChargeData();
                ratePlanData.RatePlanChargeData = new RatePlanChargeData[] { ratePlanChargeData };

                RatePlanCharge ratePlanCharge = new RatePlanCharge();
                ratePlanChargeData.RatePlanCharge = ratePlanCharge;

                ratePlanCharge.ProductRatePlanChargeId = charge.Id;
                // if it has a default quantity, default to 1
                if (charge.DefaultQuantitySpecified)
                {
                    ratePlanCharge.Quantity = 1;
                }
                ratePlanCharge.QuantitySpecified = true;
                ratePlanCharge.TriggerEvent = "ServiceActivation";

                data[i] = ratePlanData;
            }

            return data;
        }


        protected PaymentMethod makePaymentMethod(
            string HolderName,
            string Address,
            string City,
            string State,
            string Country,
            string PostalCode, 
            string CreditCardType, 
            string CreditCardNumber, 
            int CreditCardExpirationMonth, 
            int CreditCardExpirationYear)
            
        {
            PaymentMethod pm = new PaymentMethod();
            pm.Type = "CreditCard";
            pm.CreditCardType = CreditCardType;
            pm.CreditCardAddress1 = Address;
            pm.CreditCardCity = City;
            pm.CreditCardState = State;
            pm.CreditCardPostalCode = PostalCode;
            pm.CreditCardCountry = Country;
            pm.CreditCardHolderName = HolderName;
            pm.CreditCardExpirationYear = CreditCardExpirationYear;
            pm.CreditCardExpirationYearSpecified = true;
            pm.CreditCardExpirationMonth = CreditCardExpirationMonth;
            pm.CreditCardExpirationMonthSpecified = true;
            pm.CreditCardNumber = CreditCardNumber;

            return pm;
        }

        internal SubscribeResult subscribe(
            String SubscriptionName,
            ProductRatePlanCharge[] charges,
            string Name, 
            string FirstName, 
            string LastName, 
            string WorkEmail, 
            string WorkPhone, 
            string Address1, 
            string Address2, 
            string City, 
            string State, 
            string Country, 
            string PostalCode, 
            string CreditCardType, 
            string CreditCardNumber, 
            string CreditCardHolderName, 
            int CreditCardExpirationMonth,
            int CreditCardExpirationYear)
        {

            Account acc = makeAccount(Name, "USD");
            Contact con = makeContact(FirstName, LastName, WorkEmail, WorkPhone, Address1, Address2, City, State, Country, PostalCode);

            PaymentMethod pm = makePaymentMethod(CreditCardHolderName, Address1, City, State, Country, PostalCode, CreditCardType, CreditCardNumber, CreditCardExpirationMonth, CreditCardExpirationYear);
            Subscription subscription = makeSubscription(SubscriptionName, null);

            SubscriptionData sd = new SubscriptionData();
            sd.Subscription = subscription;

            RatePlanData[] subscriptionRatePlanDataArray = makeRatePlanData(charges);
            sd.RatePlanData = subscriptionRatePlanDataArray;

            SubscribeOptions options = new SubscribeOptions();
            options.GenerateInvoice = GENERATE_INVOICE;
            options.ProcessPayments = PROCESS_PAYMENTS;

            SubscribeRequest sub = new SubscribeRequest();
            sub.Account = acc;
            sub.BillToContact = con;
            sub.PaymentMethod = pm;
            sub.SubscriptionData = sd;
            sub.SubscribeOptions = options;

            SubscribeRequest[] subscribes = new SubscribeRequest[1];
            subscribes[0] = sub;

            SubscribeResult[] result = binding.subscribe(subscribes);

            return result[0];


        }


        /**
          * Creates the print format of the subscribe result value.
          * @param resultArray
          * @return
          */
        public static String createMessage(SubscribeResult result)
        {
            String resultString = null;
            if (result != null)
            {
                if (result.Success)
                {
                    resultString = resultString + "<b>Subscribe Result: Success</b>";
                    resultString = resultString + "<br>&nbsp;&nbsp;Account Id: " + result.AccountId;
                    resultString = resultString + "<br>&nbsp;&nbsp;Account Number: " + result.AccountNumber;
                    resultString = resultString + "<br>&nbsp;&nbsp;Subscription Id: " + result.SubscriptionId;
                    resultString = resultString + "<br>&nbsp;&nbsp;Subscription Number: " + result.SubscriptionNumber;
                    resultString = resultString + "<br>&nbsp;&nbsp;Invoice Number: " + result.InvoiceNumber;
                    resultString = resultString + "<br>&nbsp;&nbsp;Payment Transaction: " + result.PaymentTransactionNumber;
                }
                else
                {
                    resultString = resultString + "<b>Subscribe Result: Failed</b>";
                    Error[] errors = result.Errors;
                    if (errors != null)
                    {
                        foreach (Error error in errors)
                        {
                            resultString = resultString + "<br>&nbsp;&nbsp;Error Code: " + error.Code;
                            resultString = resultString + "<br>&nbsp;&nbsp;Error Message: " + error.Message;
                        }
                    }
                }
            }
            return resultString;
        }


        public static Boolean isValidId(string id)
        {
            return id != null && id.Length == 32;
        }

        protected void print(string p)
        {
            Console.WriteLine(p);
        }
    }
}
