using ShipIt.Models.ApiModels;
using System;
using System.Data;

namespace ShipIt.Models.DataModels
{
    public class EmployeeDataModel : DataModel
    {
        [DatabaseColumnName("name")]
        public string Name { get; set; }
        [DatabaseColumnName("w_id")]
        public int WarehouseId { get; set; }
        [DatabaseColumnName("role")]
        public string Role { get; set; }
        [DatabaseColumnName("ext")]
        public string Ext { get; set; }
        [DatabaseColumnName("personal_id")]
        public int PersonalId { get; set; }

        public EmployeeDataModel(IDataReader dataReader) : base(dataReader)
        { 
        }

        public EmployeeDataModel()
        { 
        }

        public EmployeeDataModel(Employee employee)
        {
            Name = employee.Name;
            WarehouseId = employee.WarehouseId;
            Role = MapApiRoleToDatabaseRole(employee.role);
            Ext = employee.Ext;
            PersonalId = employee.PersonalId;
        }

        private string MapApiRoleToDatabaseRole(EmployeeRole employeeRole)
        {
            if (employeeRole == EmployeeRole.CLEANER)
            {
                return DataBaseRoles.Cleaner;
            }

            if (employeeRole == EmployeeRole.MANAGER)
            {
                return DataBaseRoles.Manager;
            }

            if (employeeRole == EmployeeRole.OPERATIONS_MANAGER)
            {
                return DataBaseRoles.OperationsManager;
            }

            if (employeeRole == EmployeeRole.PICKER)
            {
                return DataBaseRoles.Picker;
            }

            throw new ArgumentOutOfRangeException("EmployeeRole");
        }
    }
}