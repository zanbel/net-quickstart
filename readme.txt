Zuora API .NET Quickstart

INTRODUCTION
------------

Thank you for downloading the Zuora QuickStart .NET Toolkit.  This download contains code designed 
to help you begin using Zuora APIs.

REQUIREMENTS
------------

Microsoft Visual Studio 2010 or 
Microsoft Visual C# Express Edition 

Instructions may vary slightly based on development environment. All attempts to ensure compatibility 
have been made.

CONTENTS
--------

This sample zip contains:

    /readme.txt - This file
    /zuora.17.0.wsdl - The latest version of the WSDL
    /ZuoraSignUp.sln - The Visual Studio Project Solution File 
    /ZuoraSignUp/ZuoraInterface.cs - Zuora API Interface for common calls/use cases
    /ZuoraSignUp/ZuoraInterfaceTest.cs - Test program for Zuora API Interface
    /ZuoraSignUp/SignUp.* - Files for a "Sign Up" Page for use in an existing website
    /ZuoraSignUp/Web.config.* - Configuration for running a test webserver

DOCUMENTATION & SUPPORT
-----------------------

API Documentation is available at http://developer.zuora.com

PRE-REQUISITES
--------------

The following are pre-requisites to successfully run the sample code:

1. A Zuora Tenant
2. A Zuora User
    a.) With the User Role Permission to create Invoices 
        (http://knowledgecenter.zuora.com/index.php/Z-Billing_Admin#Manage_User_Roles)
3. A Product created with a Rate Plan & Rate Plan Component 
   (http://knowledgecenter.zuora.com/index.php/Product_Catalog), with
    a.) The Effective Period (Start & End) of the Product/Rate Plan not expired 
        (start < today and end > today)
    b.) An Accounting Code specified on the Rate Plan Component 
        (Update ZuoraInterfaceTest.ACCOUNTING_CODE with the code you specify)
4. A Zuora Gateway set up 
   (http://knowledgecenter.zuora.com/index.php/Z-Payments_Admin#Setup_Payment_Gateway)
    a.) Either Authorize.net, CyberSource, PayPal Payflow Pro (production or test)
    b.) The setting "Verify new credit card" disabled

SETTING UP THE PROJECT
----------------------

1. Unzip the files contained in the quickstart_net.zip file to a folder on you hard drive.  
2. Double-click on the ZuoraSignUp.sln
3. In the Solution Explorer, right-click on ZuoraSignUp and choose "Add Web Reference"
4. In the URL prompt, type in the path to the zuora.17.0.wsdl file and click on the green arrow.
5. Once the ZuoraService is found, type in a Web reference name of "zuora", and click "Add Reference"'
6. Click on Build->Rebuild ZuoraSignUp

NOTE: If you have custom fields or other additional functionality exposed to your tenant, your
Tenant's WSDL may be different than the one provided in this sample. To get your tenant-specific WSDL,
log into the application and go to "Settings->Z-Billing Settings->Download the Zuora WSDL"

RUNNING THE EXAMPLES
--------------------

There are two examples to run in this Quickstart.

    1. Basic Tests - Run the executable version, which ensures your Zuora configuration is correct
    2. Sign Up Page - Sample ASP & .NET code to show a website Sign Up (Order) Page

Prior to running the examples, configure the code to work with your Zuora Credentials and
Product Catalog. To do this, perform the following:

    1. Open ZuoraInterface.cs and set your USERNAME, PASSWORD and ENDPOINT 
        a. USERNAME should be a name in the format of an email address
        b. ENDPOINT is a URL of the form https://www.zuora.com/apps/services/a/17.0. If you are 
           testing on Zuora's sandbox, make sure you have the appropriate URL
    2. Open ZuoraInterfaceTest.cs and set your ACCOUNTING_CODE. This value should be an
       an actual Accounting Code specified on a Rate Plan Component in your Product Catalog.
       For simplicity, it is recommended you make the Rate Plan Component with this value a 
       One-Time charge.

BASIC TESTS
------------

The basic tests are designed to run as a stand-alone application. If these tests pass, it
confirms that your configuration is correct.

The ZuoraInterfaceTest file contains a main() method that will execute the test. This will run 
through a basic CRUD of an Account and a subscribe() call, which is the equivalent of a new order.

SIGN-UP PAGE
------------

The sign-up page sample demonstrates a simple new customer sign-up for your public-facing website.
It queries your product catalog (product/rate plan/charges) to show the products available
to customers. After clicking on the "Sign Up" button, a new customer is created in Zuora with a
corresponding subscription.

For simplicity, the order defaults all quantities to 1. You can customize the page to allow user-input
of quantities.

To run the sample, click on SignUp.aspx in the project and press F5 to begin debugging.


