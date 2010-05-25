using System;
using ZuoraSignUp.zuora;

namespace ZuoraSignUp
{

    public partial class SignUpPage : System.Web.UI.Page
    {

        private ZuoraInterface zuora = new ZuoraInterface();
        String _pId = null;
        String _prpId = null;
        String[] _chargeIds = null;

        protected void Page_Load(object sender, EventArgs e)
        {

            _pId = getValue(Products);
            _prpId = getValue(RatePlans);
            _chargeIds = getValues(Charges);

            // clear it out and load dynamically 
            Products.Items.Clear();
            RatePlans.Items.Clear();
            Charges.Items.Clear();

            // show initial product selection
            System.Web.UI.WebControls.ListItem item = new System.Web.UI.WebControls.ListItem();
            item.Text = "-- SELECT ONE -- ";
            item.Value = null;
            Products.Items.Add(item);

            Product[] products = zuora.queryProducts();
            if (products != null && products.Length > 0)
            {
                for (int i = 0; i < products.Length; i++)
                {
                    Product obj = products[i];
                    item = new System.Web.UI.WebControls.ListItem();
                    item.Text = obj.Name;
                    item.Value = obj.Id;
                    if (obj.Id == _pId)
                    {
                        item.Selected = true;
                    }
                    Products.Items.Add(item);
                }

                // if only one item, select it.
                if (products.Length == 1)
                {
                    _pId = products[0].Id;
                }
            }

            if (ZuoraInterface.isValidId(_pId))
            {
                ProductRatePlan[] prp = zuora.queryRatePlansByProduct(_pId);
                if (prp != null && prp.Length > 0)
                {
                    for (int i = 0; prp.Length > 0 && i < prp.Length; i++)
                    {
                        ProductRatePlan obj = prp[i];
                        item = new System.Web.UI.WebControls.ListItem();
                        item.Text = obj.Name;
                        item.Value = obj.Id;
                        if (obj.Id == _prpId)
                        {
                            item.Selected = true;
                        }
                        RatePlans.Items.Add(item);
                    }

                    // if only one item, select it.
                    if (prp.Length == 1){
                        _prpId = prp[0].Id;
                    }
                }
            } 
            else
            {
                item = new System.Web.UI.WebControls.ListItem();
                item.Text = "-- SELECT A PRODUCT ABOVE -- ";
                item.Value = null;
                RatePlans.Items.Add(item);
            }

            if (ZuoraInterface.isValidId(_pId) && ZuoraInterface.isValidId(_prpId)) 
            {
                ProductRatePlanCharge[] charges = zuora.queryChargesByProductRatePlan(_prpId);
                if (charges != null && charges.Length > 0)
                {
                    for (int i = 0; charges.Length > 0 && i < charges.Length; i++)
                    {
                        ProductRatePlanCharge obj = charges[i];
                        item = new System.Web.UI.WebControls.ListItem();
                        item.Text = obj.Name + (obj.AccountingCode != null ? " (" + obj.AccountingCode + ")" : "");
                        item.Value = obj.Id;
                        item.Selected = true;
                        Charges.Items.Add(item);
                    }

                    // if only one item, select it.
                    if (charges.Length == 1)
                    {
                        _chargeIds = new String[1];
                        _chargeIds[0] = charges[0].Id;
                    }
                }
            }
            else
            {
                item = new System.Web.UI.WebControls.ListItem();
                item.Text = "-- SELECT A RATE PLAN ABOVE -- ";
                item.Value = null;
                Charges.Items.Add(item);
            }

        }

        protected void SignUp_Click(object sender, EventArgs e)
        {

            int month = Int32.Parse(getValue(CreditCardExpirationMonth));
            int year = Int32.Parse(getValue(CreditCardExpirationYear));
            String ccType = CreditCardType.Items[CreditCardType.SelectedIndex].Value;

            ProductRatePlanCharge[] charges = new ProductRatePlanCharge[_chargeIds.Length];
            for (int i = 0; i < _chargeIds.Length; i++)
            {
                ProductRatePlanCharge charge = new ProductRatePlanCharge();
                charge.Id = _chargeIds[i];
                charge.ProductRatePlanId = _prpId;
                charges[i] = charge;
            }

            String subscriptionName = Name.Value + " New Signup (" + DateTime.Now.Ticks + ")";

            SubscribeResult result = zuora.subscribe(
                subscriptionName,
                charges,
                Name.Value,
                FirstName.Value,
                LastName.Value,
                WorkEmail.Value,
                WorkPhone.Value,
                Address1.Value,
                Address2.Value,
                City.Value,
                State.Value,
                Country.Value,
                PostalCode.Value,
                CreditCardType.Value,
                CreditCardNumber.Value,
                CreditCardHolderName.Value,
                month,
                year            
            );

            String message = ZuoraInterface.createMessage(result);

            Response.Write(message);

        }

        private string getValue(System.Web.UI.HtmlControls.HtmlSelect select)
        {
            String value = null;
            if (select != null && select.Items != null && select.Items.Count > 0 && select.SelectedIndex >= 0)
            {
                value = select.Items[select.SelectedIndex].Value;
            }
            return value;
        }


        private string getValue(System.Web.UI.WebControls.DropDownList select)
        {
            String value = null;
            if (select != null && select.Items != null && select.Items.Count > 0 && select.SelectedIndex >= 0)
            {
                value = select.Items[select.SelectedIndex].Value;
            }
            return value;
        }


        private string[] getValues(System.Web.UI.WebControls.ListBox select)
        {
            int numSelected = 0;
            for (int i = 0; i < select.Items.Count; i++ )
            {
                if (select.Items[i].Selected){
                    numSelected++;
                }
            }
            String[] values = new String[numSelected];
            int index = 0;
            for (int i = 0; i < select.Items.Count; i++)
            {
                if (select.Items[i].Selected)
                {
                    values[index++] = select.Items[i].Value;
                }
            }
            return values;
        }

    }
}