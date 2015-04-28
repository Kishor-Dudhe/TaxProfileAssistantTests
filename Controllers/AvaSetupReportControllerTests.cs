using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxProfileAssistant.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using TaxProfileAssistant.Models;
using TaxProfileAssistant.Utils;
using System.Data.Entity;

namespace TaxProfileAssistantTest.Controllers.Tests
{
    
    [TestClass()]
    public class AvaSetupReportControllerTests
    {
        AccountInfo accountInfo;
        AvaSetupReportController avaSetupReportController;
        AccountService accountService;
        TPAScaffold input;
        TaxProfileAssistantController tpaController;
        
        [TestInitialize]//Initializes data required for each test and runs once per test method.
        public void InitializeData()
        {
            //Arrange

            avaSetupReportController = new AvaSetupReportController();
            XmlReader xmlReader = XmlReader.Create(@"..\XMLTestFiles\TPA_500.xml");
            var tpa = new TPAScaffold();
            var serializer = new XmlSerializer(tpa.GetType());
            input = (TPAScaffold)serializer.Deserialize(xmlReader);

            tpaController = new TaxProfileAssistantController();
            tpaController.ControllerContext = new ControllerContext();
            string resultstr = tpaController.Post(input);
            string key = resultstr.Substring(38);
            key = key.Replace(Constants.APPLICATION_URL_DUMMY_TEXT, "/");
            accountInfo = AvaTaxProfileAssistantHelper.GetAccountInfoBySecureKey(key);
                     
            accountService = new AccountService(accountInfo.Webservice, accountInfo.UserName, accountInfo.Password, accountInfo.AccountNumber, accountInfo.LicenseKey, accountInfo.ERPName);


            TempDataDictionary TempData = new TempDataDictionary();
            avaSetupReportController.TempData.Add("AccountInfo", accountInfo);
            avaSetupReportController.TempData.Add("AccountService", accountService);
            Constants.SESSION_KEY_SELECTED_COMPANY = "4.1 Dev Company";
            avaSetupReportController.TempData.Add(Constants.SESSION_KEY_SELECTED_COMPANY, "4.1 Dev Company");
            avaSetupReportController.TempData.Add(TaxProfileAssistant.Utils.Constants.SESSION_KEY_ERP_NAME, accountInfo.ERPName);
            avaSetupReportController.TempData.Add(TaxProfileAssistant.Utils.Constants.SESSION_KEY_COMPANY_NAME, "Systems of America");

        }

        [TestMethod()]//Checks if An object of FinishSetup model is returned through view of index method.
        public void AvaSetupReportController_IndexMethod_ReturnsFinishSetupObjectTest()
        {
            //Act
            var result = avaSetupReportController.Index() as ViewResult;

            //Assert
            Assert.IsInstanceOfType(result.Model, typeof(FinishSetup), "Incorrect type of result returned!!Expected is an object of FinishSetup!");

        } 

                
        [TestMethod()]//Checks if Index method sets the appropriate field to true
        public void AvaSetupReportController_IndexMethod_SetsTheRequiredFieldsToTrue()
        {
                      
            //Act

            var result = avaSetupReportController.Index() as ViewResult ;
            var resultData=(FinishSetup)result.ViewData.Model;

            //Assert
            
            Assert.IsTrue(resultData.IsConnected, "Service Connection failed!!");
            Assert.IsTrue(resultData.IsTaxScheduledMapped, "Tax Schedule is not mapped to AVATAX!!");
            Assert.IsNotNull(resultData.NexusList, "Nexus List Cannot be empty!!");
            Assert.IsNotNull(resultData.HelpLinkList, "HelpLinks should be provided!!No Help Links generated!");
            Assert.IsNotNull(resultData.TotalCustomers, "There are no customers for the respective ERP!!");
            Assert.IsTrue(resultData.IsAddressValidationEnabled, "Address Validation not enabled!!");


        }
    }
}
