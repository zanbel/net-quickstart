/*    Copyright (c) 2010 Zuora, Inc.
 *
 *   Permission is hereby granted, free of charge, to any person obtaining a copy of 
 *   this software and associated documentation files (the "Software"), to use copy, 
 *   modify, merge, publish the Software and to distribute, and sublicense copies of 
 *   the Software, provided no fee is charged for the Software.  In addition the
 *   rights specified above are conditioned upon the following:
 *
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 *
 *   Zuora, Inc. or any other trademarks of Zuora, Inc.  may not be used to endorse
 *   or promote products derived from this Software without specific prior written
 *   permission from Zuora, Inc.
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL
 *   ZUORA, INC. BE LIABLE FOR ANY DIRECT, INDIRECT OR CONSEQUENTIAL DAMAGES
 *   (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 *   LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 *   ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 *   (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 *   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZuoraSignUp.zuora;

namespace ZuoraSignUp
{
    public class ZuoraInterfaceTest : ZuoraInterface
    {

        private static string ACCOUNTING_CODE = "TestNewComponent";

        static void Main(string[] args)
        {
            ZuoraInterfaceTest test = new ZuoraInterfaceTest(USERNAME, PASSWORD, ENDPOINT);
            test.testAccountCRUD();
            test.testSubscribe();

        }


        public ZuoraInterfaceTest (string un, string pw, string endpoint) : base(un, pw, endpoint) {
        }

        private void testAccountCRUD()
        {
            string accId = createAccount(true);

            Account accountQuery = queryAccount(accId);
            print("Create Active Account: " + accId);
            // print(toString(accountQuery));

            Account accUpdate = new Account();
            accUpdate.Id = accId;
            accUpdate.Name = "testUpdate" + DateTime.Now.Ticks;

            update(accUpdate);

            accountQuery = queryAccount(accId);
            print("Updated Account: " + accId);
            // print(toString(accountQuery));

            bool deleted = delete("Account", accId);
            print("Deleted Account: " + deleted);

        }


        protected void testSubscribe()
        {

            ProductRatePlanCharge charge = queryChargeByAccountingCode(ACCOUNTING_CODE);

            Account acc = makeAccount();
            Contact con = makeContact();
            PaymentMethod pm = makePaymentMethod();
            Subscription subscription = makeSubscription("New Signup" + DateTime.Now.Ticks, null);

            SubscriptionData sd = new SubscriptionData();
            sd.Subscription = subscription;

            RatePlanData[] subscriptionRatePlanDataArray = makeRatePlanData(new ProductRatePlanCharge[]{charge});
            sd.RatePlanData = subscriptionRatePlanDataArray;

            SubscribeRequest sub = new SubscribeRequest();
            sub.Account = acc;
            sub.BillToContact = con;
            sub.PaymentMethod = pm;
            sub.SubscriptionData = sd;

            SubscribeRequest[] subscribes = new SubscribeRequest[1];
            subscribes[0] = sub;

            SubscribeResult[] result = binding.subscribe(subscribes);
            String message = createMessage(result[0]);
            print(message);

        }

        protected Account makeAccount()
        {

            long time = DateTime.Now.Ticks;
            string name = "SomeAccount" + time;
            return makeAccount(name, "USD");
        }


        public Contact makeContact()
        {
            long time = DateTime.Now.Ticks;
            return makeContact("First" + time, "Last" + time, "contact@test.com", "415 555 1212", "52 Vexford Lane", null, "Anaheim", "CA", "USA", "92808");
        }


        protected PaymentMethod makePaymentMethod()
        {

            DateTime theDate = DateTime.Now;
            theDate.AddYears(1);

            return makePaymentMethod("First Last", "517 Country Drive", "Anaheim", "CA", "USA", "92808", "Visa", "4111111111111111", theDate.Month, theDate.Year);

        }



        public string createAccount(bool active)
        {

            // create account
            Account acc1 = makeAccount();
            string accountId = create(acc1);

            if (active)
            {

                // create contact
                Contact con = makeContact();
                con.AccountId = accountId;
                string contactId = create(con);

                PaymentMethod pm = makePaymentMethod();
                pm.AccountId = accountId;
                string pmId = create(pm);

                // set required active fields and activate
                Account accUpdate = new Account();
                accUpdate.Id = accountId;
                accUpdate.Status = "Active";
                accUpdate.SoldToId = contactId;
                accUpdate.BillToId = contactId;
                accUpdate.AutoPay = true;
                accUpdate.PaymentTerm = "Due Upon Receipt";
                accUpdate.DefaultPaymentMethodId = pmId;
                update(accUpdate);
            }

            return accountId;
        }

    }
}