﻿using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ShipIt.Controllers
{
    public class EmployeeController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IEmployeeRepository employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        public EmployeeResponse Get(string name)
        {
            log.Info(String.Format("Looking up employee by name: {0}", name));

            var employee = new Employee(employeeRepository.GetEmployeeByName(name));

            log.Info("Found employee: " + employee);
            return new EmployeeResponse(employee);
        }

        public EmployeeResponse Get(int warehouseId)
        {
            log.Info(String.Format("Looking up employee by id: {0}", warehouseId));

            var employees = employeeRepository
                .GetEmployeesByWarehouseId(warehouseId)
                .Select(e => new Employee(e));

            log.Info(String.Format("Found employees: {0}", employees));
            
            return new EmployeeResponse(employees);
        }

        public Response Post([FromBody]AddEmployeesRequest requestModel)
        {
            List<Employee> employeesToAdd = requestModel.Employees;

            if (employeesToAdd.Count == 0)
            {
                throw new MalformedRequestException("Expected at least one <employee> tag");
            }

            log.Info("Adding employees: " + employeesToAdd);

            employeeRepository.AddEmployees(employeesToAdd);

            log.Debug("Employees added successfully");

            return new Response() { Success = true };
        }
        public EmployeeResponse GetEmployeeByID(int ID)
        {
            var employees = new Employee(employeeRepository.GetEmployeeByID(ID));

            return new EmployeeResponse(employees);
        }

        public void Delete([FromBody]RemoveEmployeeRequest requestModel)
        {
            string name = requestModel.Name;
            int personalID = requestModel.PersonalID;
            if (name == null && personalID == 0)
            {
                throw new MalformedRequestException("Unable to parse name or personal ID from request parameters");
            }
            if (name == null)
            {
                try
                {
                    employeeRepository.RemoveEmployee(personalID);
                }
                catch (NoSuchEntityException)
                {
                    throw new NoSuchEntityException("No employee exists with ID: " + personalID);
                }
            }
            else
            {
                try
                {
                    employeeRepository.RemoveEmployee(name);
                }
                catch (NoSuchEntityException)
                {
                    throw new NoSuchEntityException("No employee exists with name: " + name);
                }
            }
        }
    }
}
