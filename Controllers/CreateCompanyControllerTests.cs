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
    public class CreateCompanyControllerTests
    {
        AccountInfo accountInfo;
        AccountService accountService;
        CreateCompanyController createCompanyController;
        TempDataDictionary TempData;
        TPAScaffold input;
        TaxProfileAssistantController tpaController;
        
        [TestInitialize]//Initializes data required for each test and runs once per test method.
        public void InitializeData()
        {
            accountInfo = new AccountInfo();
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
            createCompanyController = new CreateCompanyController();
            accountService = new AccountService(accountInfo.Webservice, accountInfo.UserName, accountInfo.Password, accountInfo.AccountNumber, accountInfo.LicenseKey, accountInfo.ERPName);
            TempData = new TempDataDictionary();
            createCompanyController.TempData.Add("AccountInfo", accountInfo);
            createCompanyController.TempData.Add("AccountService", accountService);


        }
   
        [TestMethod()]//checks if index method returns company object through view
        public void CreateCompanyController_IndexTest_CheckIfItReturnsCompanyObject()
        {
            //Act

            var result = createCompanyController.Index() as ViewResult;

            //Assert

            Assert.IsInstanceOfType((result).Model, typeof(CreateCompany), "Incorrect Type of result returned,Return type should be an object of createCompany");           
          
                 
        }

        [TestMethod()]//Checks if the method redirects to nexusSetup controller if company is created.
        public void CreateComapnyController_CreateNewCompanyTestToCheckIfItRedirectsToNexusSetupOnceCompanyIsCreated()
        {
           

            CreateCompany createCompany = new CreateCompany();
            createCompany.AccountInfoID = accountInfo.ID;
            createCompany.AddressLine1 = "900 winslow way e";
            createCompany.AddressLine2 = "";
            createCompany.AddressLine3 = "";
            createCompany.CompanyCode = Guid.NewGuid().ToString().Substring(0, 8);
            createCompany.CompanyName = "Systems of America1";
            createCompany.Country = "US";
            createCompany.StateProvince = "WA";
            createCompany.TIN = "95-3657472";
           
            createCompany.FirstName = "abbott";
            createCompany.LastName = "abbott";
            createCompany.PhoneNumber = "9999999999";
            createCompany.Fax = "9999999999";
            createCompany.Email = "abc@abc.com";
            createCompany.City = "Bainbridge Island";
            createCompany.ZipCode = "98110";
            createCompany.MobileNumber = "";
            createCompany.Title = "";
            createCompany.BIN = "";
           

            //Act
            var companyresult = createCompanyController.CreateNewCompany(createCompany);
           

            //Assert

            RedirectToRouteResult routeResult = companyresult as RedirectToRouteResult;
            Assert.AreEqual(routeResult.RouteValues["action"], "Index", "Index Method of CreateCompany Controller failed to redirect to correct action!!");
            Assert.AreEqual(routeResult.RouteValues["controller"], "NexusSetup", "Index Method of CreateCompany Controller failed to redirect to correct controller!!");
           
        }

        [TestMethod()]//Checks if the method returns correct view if company is not created
        public void CreateComapnyController_CreateNewCompanyTestToCheckIfItReturnsCreateCompanyIndexViewWhenCompanyIsNotCreated()
        {
          

            CreateCompany createCompany = new CreateCompany();
            createCompany.AccountInfoID = accountInfo.ID;
            createCompany.AddressLine1 = "900 winslow way e";
            createCompany.AddressLine2 = "";
            createCompany.AddressLine3 = "";
                   
            //Act
           
           var companyresult = createCompanyController.CreateNewCompany(createCompany) as ViewResult ;


            //Assert
            Assert.AreEqual("Index", companyresult.ViewName);
            

        }
    }
}
