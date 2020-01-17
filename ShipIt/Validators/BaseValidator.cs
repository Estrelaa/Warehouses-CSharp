using System;
using System.Collections.Generic;

namespace ShipIt.Validators
{
    public abstract class BaseValidator<T>
    {
        List<string> errors;

        protected BaseValidator()
        {
            errors = new List<string>();
        }

        public void Validate(T target)
        {
            DoValidation(target);
        }

        protected abstract void DoValidation(T target);

        void AddError(String error)
        {
            errors.Add(error);
        }

        void AddErrors(List<String> errors)
        {
            this.errors.AddRange(errors);
        }

        /**
         * Object validators
         */

        void AssertNotNull(String fieldName, Object value)
        {
            if (value == null)
            {
                AddError(string.Format("Field {0} cannot be null", fieldName));
            }
        }

        /**
         * String validators
         */

        protected void AssertNotBlank(string fieldName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError(string.Format("Field {0} cannot be blank", fieldName));
            }
        }

        protected void AssertNumeric(string fieldName, string value)
        {
            if (!double.TryParse(value, out double D))
            {
                AddError(string.Format("Field {0} must be numeric", fieldName));
            }
        }

        protected void AssertMaxLength(String fieldName, string value, int maxLength)
        {
            if (value.Length > maxLength)
            {
                AddError(string.Format("Field {0} must be shorter than {1} characters", fieldName, maxLength));
            }
        }

        protected void AssertExactLength(string fieldName, string value, int exactLength)
        {
            if (value.Length != exactLength)
            {
                AddError(string.Format("Field {0} must be exactly {1} characters", fieldName, exactLength));
            }
        }

        /**
         * Numeric validators
         */

        protected void AssertNonNegative(string fieldName, int value)
        {
            if (value < 0)
            {
                AddError(string.Format("Field {0} must be non-negative", fieldName));
            }
        }

        protected void AssertNonNegative(string fieldName, float value)
        {
            if (value < 0)
            {
                AddError(string.Format("Field {0} must be non-negative", fieldName));
            }
        }

        /**
         * Specific validators
         */

        protected void ValidateGtin(string value)
        {
            AssertNotBlank("gtin", value);
            AssertNumeric("gtin", value);
            AssertMaxLength("gtin", value, 13);
        }

        protected void ValidateGcp(String value)
        {
            AssertNotBlank("gcp", value);
            AssertNumeric("gcp", value);
            AssertMaxLength("gcp", value, 13);
        }

        protected void validateWarehouseId(int warehouseId)
        {
            AssertNonNegative("warehouseId", warehouseId);
        }
    }
}