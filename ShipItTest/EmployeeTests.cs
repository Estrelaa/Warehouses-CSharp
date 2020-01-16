using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShipIt.Controllers;
using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Repositories;
using ShipItTest.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShipItTest
{
    [TestClass]
    public class EmployeeControllerTests : AbstractBaseTest
    {
        EmployeeController employeeController = new EmployeeController(new EmployeeRepository());
        EmployeeRepository employeeRepository = new EmployeeRepository();

        private const string NAME = "Gissell Sadeem";
        private const int WAREHOUSE_ID = 1;

        [TestMethod]
        public void TestRoundtripEmployeeRepository()
        {
            onSetUp();
            var employee = new EmployeeBuilder().CreateEmployee();
            employeeRepository.AddEmployees(new List<Employee>() {employee});
            Assert.AreEqual(employeeRepository.GetEmployeeByName(employee.Name).Name, employee.Name);
            Assert.AreEqual(employeeRepository.GetEmployeeByName(employee.Name).Ext, employee.Ext);
            Assert.AreEqual(employeeRepository.GetEmployeeByName(employee.Name).WarehouseId, employee.WarehouseId);
        }

        [TestMethod]
        public void TestGetEmployeeByName()
        {
            onSetUp();
            var employeeBuilder = new EmployeeBuilder().setName(NAME);
            employeeRepository.AddEmployees(new List<Employee>() {employeeBuilder.CreateEmployee()});
            var result = employeeController.Get(NAME);

            var correctEmployee = employeeBuilder.CreateEmployee();
            Assert.IsTrue(EmployeesAreEqual(correctEmployee, result.Employees.First()));
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void TestGetEmployeesByWarehouseId()
        {
            onSetUp();
            var employeeBuilderA = new EmployeeBuilder().setWarehouseId(WAREHOUSE_ID).setName("A");
            var employeeBuilderB = new EmployeeBuilder().setWarehouseId(WAREHOUSE_ID).setName("B");
            employeeRepository.AddEmployees(new List<Employee>() { employeeBuilderA.CreateEmployee(), employeeBuilderB.CreateEmployee() });
            var result = employeeController.Get(WAREHOUSE_ID).Employees.ToList();

            var correctEmployeeA = employeeBuilderA.CreateEmployee();
            var correctEmployeeB = employeeBuilderB.CreateEmployee();

            Assert.IsTrue(result.Count == 2);
            Assert.IsTrue(EmployeesAreEqual(correctEmployeeA, result.First()));
            Assert.IsTrue(EmployeesAreEqual(correctEmployeeB, result.Last()));
        }

        [TestMethod]
        public void TestGetNonExistentEmployee()
        {
            onSetUp();
            try
            {
                employeeController.Get(NAME);
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (NoSuchEntityException e)
            {
                Assert.IsTrue(e.Message.Contains(NAME));
            }
        }

        [TestMethod]
        public void TestGetEmployeeInNonexistentWarehouse()
        {
            onSetUp();
            try
            {
                var employees = employeeController.Get(WAREHOUSE_ID).Employees.ToList();
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (NoSuchEntityException e)
            {
                Assert.IsTrue(e.Message.Contains(WAREHOUSE_ID.ToString()));
            }
        }

        [TestMethod]
        public void TestAddEmployees()
        {
            onSetUp();
            var employeeBuilder = new EmployeeBuilder().setName(NAME);
            var addEmployeesRequest = employeeBuilder.CreateAddEmployeesRequest();

            var response = employeeController.Post(addEmployeesRequest);
            var databaseEmployee = employeeRepository.GetEmployeeByName(NAME);
            var correctDatabaseEmploye = employeeBuilder.CreateEmployee();

            Assert.IsTrue(response.Success);
            Assert.IsTrue(EmployeesAreEqual(new Employee(databaseEmployee), correctDatabaseEmploye));
        }

        [TestMethod]
        public void TestDeleteEmployees()
        {
            onSetUp();
            var employeeBuilder = new EmployeeBuilder().setName(NAME);
            employeeRepository.AddEmployees(new List<Employee>() { employeeBuilder.CreateEmployee() });

            var removeEmployeeRequest = new RemoveEmployeeRequest() { Name = NAME };
            employeeController.Delete(removeEmployeeRequest);

            try
            {
                employeeController.Get(NAME);
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (NoSuchEntityException e)
            {
                Assert.IsTrue(e.Message.Contains(NAME));
            }
        }

        [TestMethod]
        public void TestDeleteNonexistentEmployee()
        {
            onSetUp();
            var removeEmployeeRequest = new RemoveEmployeeRequest() { Name = NAME };

            try
            {
                employeeController.Delete(removeEmployeeRequest);
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (NoSuchEntityException e)
            {
                Assert.IsTrue(e.Message.Contains(NAME));
            }
        }

        [TestMethod]
        public void TestAddDuplicateEmployee()
        {
            onSetUp();
            var employeeBuilder = new EmployeeBuilder().setName(NAME);
            employeeRepository.AddEmployees(new List<Employee>() { employeeBuilder.CreateEmployee() });
            var addEmployeesRequest = employeeBuilder.CreateAddEmployeesRequest();

            try
            {
                employeeController.Post(addEmployeesRequest);
            }
            catch (Exception)
            {
                Assert.IsTrue(false); //means that we could not add a dup, so fail the test
            }
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void TestGetEmployeeByID()
        {
            onSetUp();
            // Create an employee
            var employeeBuilder = new EmployeeBuilder().setName(NAME);
            var addEmployeesRequest = employeeBuilder.CreateAddEmployeesRequest();

            // Add them to the database 
            employeeController.Post(addEmployeesRequest);
            //Get them from the database and find their ID 
            var databaseEmployee = employeeController.Get(NAME);
            var ID = databaseEmployee.Employees.Last().PersonalId;

            //Find them by their PersonalID
            var databaseEmployeeByID = employeeController.GetEmployeeByID(ID);

            //Check that they are the same person
            Assert.AreEqual(databaseEmployee.Employees.First().PersonalId, databaseEmployeeByID.Employees.First().PersonalId);

        }
        [TestMethod]
        public void TestDeleteEmployeeByID()
        {
            onSetUp();
            // Create an employee
            var employeeBuilder = new EmployeeBuilder().setName(NAME);
            var addEmployeesRequest = employeeBuilder.CreateAddEmployeesRequest();

            // Add them to the database 
            employeeController.Post(addEmployeesRequest);
            //Get them from the database and find their ID 
            var databaseEmployee = employeeController.Get(NAME);
            var ID = databaseEmployee.Employees.Last().PersonalId;

            var removeEmployeeRequest = new RemoveEmployeeRequest() { PersonalID = ID };
            employeeController.Delete(removeEmployeeRequest);

            try
            {
                employeeController.GetEmployeeByID(ID);
                Assert.IsTrue(false); //Means that it found the employee so fail the test
            }
            catch 
            {
                Assert.IsTrue(true);
            }
            

        }

        private bool EmployeesAreEqual(Employee A, Employee B)
        {
            return A.WarehouseId == B.WarehouseId
                   && A.Name == B.Name
                   && A.role == B.role
                   && A.Ext == B.Ext;
        }
    }
}
