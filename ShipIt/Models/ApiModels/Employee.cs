using ShipIt.Models.DataModels;
using System;
using System.Text;

namespace ShipIt.Models.ApiModels
{
    public class Employee
    {
        public string Name { get; set; }
        public int WarehouseId { get; set; }
        public EmployeeRole role { get; set; }
        public string Ext { get; set; }
        public int PersonalId { get; set; }

        public Employee(EmployeeDataModel dataModel)
        {
            Name = dataModel.Name;
            WarehouseId = dataModel.WarehouseId;
            role = MapDatabaseRoleToApiRole(dataModel.Role);
            Ext = dataModel.Ext;
            PersonalId = dataModel.PersonalId;
        }

        private EmployeeRole MapDatabaseRoleToApiRole(string databaseRole)
        {
            if (databaseRole == DataBaseRoles.Cleaner)
            {
                return EmployeeRole.CLEANER;
            }

            if (databaseRole == DataBaseRoles.Manager)
            {
                return EmployeeRole.MANAGER;
            }

            if (databaseRole == DataBaseRoles.OperationsManager)
            {
                return EmployeeRole.OPERATIONS_MANAGER;
            }

            if (databaseRole == DataBaseRoles.Picker)
            {
                return EmployeeRole.PICKER;
            }

            throw new ArgumentOutOfRangeException("DatabaseRole");
        }

        //Empty constructor needed for Xml serialization
        public Employee()
        {
        }

        public override String ToString()
        {
            return new StringBuilder()
                    .AppendFormat("name: {0}, ", Name)
                    .AppendFormat("warehouseId: {0}, ", WarehouseId)
                    .AppendFormat("role: {0}, ", role)
                    .AppendFormat("ext: {0}", Ext)
                    .AppendFormat("PersonalId: {0}", PersonalId)
                    .ToString();
        }
    }
}