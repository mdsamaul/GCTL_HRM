using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GCTL.UI.Core.Helpers
{
    public static class ModelStateHelpers
    {
        public static string TryValidate(this ModelStateDictionary modelState)
        {
            string result = string.Empty;
            if (!modelState.IsValid)
            {
                var errror = modelState.Values.FirstOrDefault(x => x.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid);
                var errror2 = modelState.Values.FirstOrDefault(x => x.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)?.Errors;
                var message = modelState.Values.FirstOrDefault(x => x.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)?.Errors.FirstOrDefault()?.ErrorMessage;
                result = message;
            }

            return result;
        }
    }
}
