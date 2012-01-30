using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Boy_Scouts_Scheduler;
using Boy_Scouts_Scheduler.Controllers;

namespace Boy_Scouts_Scheduler.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual("Modify this template to kick-start your ASP.NET MVC application.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Help()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Help() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Import()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Import() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Schedules()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Schedules() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
