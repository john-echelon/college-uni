using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CollegeUni.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeUni.Api.Services
{
    public class ConcurrencyHelper
    {
        public static Action<IEnumerable<EntityEntry>> ResolveConflicts<TEntity>(TEntity entity, ModelStateDictionary modelState) where TEntity : IRowVersion 
        {
            Action<IEnumerable<EntityEntry>> resolveConflicts = (entries) =>
            {
                var exceptionEntry = entries.Single();
                var clientValues = exceptionEntry.CurrentValues;
                var databaseEntry = exceptionEntry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    modelState.AddModelError(string.Empty, "Unable to save changes. The course was deleted by another user.");
                }
                else
                {
                    var databaseValues = (TEntity)databaseEntry.ToObject();
                    var props = databaseEntry.Properties
                                .Where(property => !object.Equals(clientValues[property.Name], databaseEntry[property.Name]) && property.Name != "RowVersion");

                    foreach (var property in props)
                    {
                        modelState.AddModelError(property.Name, databaseEntry[property.Name].ToString());
                    }
                    modelState.AddModelError("Row Version", $"Current value: {Convert.ToBase64String(databaseValues.RowVersion)}");
                    modelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed.");
                    entity.RowVersion = (byte[])databaseValues.RowVersion;
                }
            };
            return resolveConflicts;
        }
    }
}
