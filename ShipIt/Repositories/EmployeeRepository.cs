using Npgsql;
using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;
using ShipIt.Models.DataModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ShipIt.Repositories
{
    public interface IEmployeeRepository
    {
        int GetCount();
        int GetWarehouseCount();
        EmployeeDataModel GetEmployeeByName(string name);
        IEnumerable<EmployeeDataModel> GetEmployeesByWarehouseId(int warehouseId);
        EmployeeDataModel GetOperationsManager(int warehouseId);
        EmployeeDataModel GetEmployeeByID(int ID);
        void AddEmployees(IEnumerable<Employee> employees);
        void RemoveEmployee(string name);
        void RemoveEmployee(int ID);
    }

    public class EmployeeRepository : RepositoryBase, IEmployeeRepository
    {
        public static IDbConnection CreateSqlConnection()
        {
            return new NpgsqlConnection(ConnectionHelper.GetConnectionString());
        }

        public int GetCount()
        {

            using (IDbConnection connection = CreateSqlConnection())
            {
                var command = connection.CreateCommand();
                string EmployeeCountSQL = "SELECT COUNT(*) FROM em";
                command.CommandText = EmployeeCountSQL;
                connection.Open();
                var reader = command.ExecuteReader();

                try
                {
                    reader.Read();
                    return (int) reader.GetInt64(0);
                }
                finally
                {
                    reader.Close();
                }
            };
        }

        public int GetWarehouseCount()
        {
            using (IDbConnection connection = CreateSqlConnection())
            {
                var command = connection.CreateCommand();
                string EmployeeCountSQL = "SELECT COUNT(DISTINCT w_id) FROM em";
                command.CommandText = EmployeeCountSQL;
                connection.Open();
                var reader = command.ExecuteReader();

                try
                {
                    reader.Read();
                    return (int)reader.GetInt64(0);
                }
                finally
                {
                    reader.Close();
                }
            };
        }

        public EmployeeDataModel GetEmployeeByName(string name)
        {
            string sql = "SELECT * FROM em WHERE name = @name";
            var parameter = new NpgsqlParameter("@name", name);
            string noProductWithIdErrorMessage = string.Format("No employees found with name: {0}", name);
            return base.RunSingleGetQuery(sql, reader => new EmployeeDataModel(reader),noProductWithIdErrorMessage, parameter);
        }

        public IEnumerable<EmployeeDataModel> GetEmployeesByWarehouseId(int warehouseId)
        {

            string sql = "SELECT * FROM em WHERE w_id = @w_id";
            var parameter = new NpgsqlParameter("@w_id", warehouseId);
            string noProductWithIdErrorMessage =
                string.Format("No employees found with Warehouse Id: {0}", warehouseId);
            return base.RunGetQuery(sql, reader => new EmployeeDataModel(reader), noProductWithIdErrorMessage, parameter);
        }
        public EmployeeDataModel GetEmployeeByID(int ID)
        {
            string sql = "SELECT * FROM em WHERE personal_id = @ID";
            var parameter = new NpgsqlParameter("@ID", ID);
            string noProductWithIdErrorMessage = string.Format("No employees found with ID: {0}", ID);
            return base.RunSingleGetQuery(sql, reader => new EmployeeDataModel(reader), noProductWithIdErrorMessage, parameter);
        }

        public EmployeeDataModel GetOperationsManager(int warehouseId)
        {

            string sql = "SELECT * FROM em WHERE w_id = @w_id AND role = @role";
            var parameters = new []
            {
                new NpgsqlParameter("@w_id", warehouseId),
                new NpgsqlParameter("@role", DataBaseRoles.OperationsManager)
            };

            string noProductWithIdErrorMessage =
                string.Format("No employees found with Warehouse Id: {0}", warehouseId);
            return base.RunSingleGetQuery(sql, reader => new EmployeeDataModel(reader), noProductWithIdErrorMessage, parameters);
        }

        public void AddEmployees(IEnumerable<Employee> employees)
        {
            string sql = "INSERT INTO em (name, w_id, role, ext) VALUES(@name, @w_id, @role, @ext)";
            
            var parametersList = new List<NpgsqlParameter[]>();
            foreach (var employee in employees)
            {
                var employeeDataModel = new EmployeeDataModel(employee);
                parametersList.Add(employeeDataModel.GetNpgsqlParameters().ToArray());
            }

            base.RunTransaction(sql, parametersList);
        }
        public string AddEmployee(Employee employee)
        {

            string sql = "INSERT INTO em(name, w_id, role, ext) VALUES (@name, @w_id, @role, @ext) RETURNING personal_id";
            string CouldNotAddEmloyee =
                string.Format("Could Not Add Emloyee");
            var id = RunSingleGetQuery(sql, reader => new EmployeeDataModel(reader),CouldNotAddEmloyee);

            return id.PersonalId.ToString();
        }

        public void RemoveEmployee(string name)
        {
            string sql = "DELETE FROM em WHERE name = @name";
            var parameter = new NpgsqlParameter("@name", name);
            var rowsDeleted = RunSingleQueryAndReturnRecordsAffected(sql, parameter);
            if (rowsDeleted == 0)
            {
                throw new NoSuchEntityException("Incorrect result size: expected 1, actual 0");
            }
            else if (rowsDeleted > 1)
            {
                throw new InvalidStateException("Unexpectedly deleted " + rowsDeleted + " rows, but expected a single update");
            }
        }
        public void RemoveEmployee(int ID)
        {
            string sql = "DELETE FROM em WHERE personal_id = @ID";
            var parameter = new NpgsqlParameter("@ID", ID);
            var rowsDeleted = RunSingleQueryAndReturnRecordsAffected(sql, parameter);
            if (rowsDeleted == 0)
            {
                throw new NoSuchEntityException("Incorrect result size: expected 1, actual 0");
            }
            else if (rowsDeleted > 1)
            {
                throw new InvalidStateException("Unexpectedly deleted " + rowsDeleted + " rows, but expected a single update");
            }
        }
    }
}