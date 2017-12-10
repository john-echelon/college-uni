using CollegeUni.Data.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollegeUni.Data.EntityFrameworkCore
{
    public class ConcurrencyHelper
    {
        public static Action<IEnumerable<EntityEntry>> ResolveConflicts<TEntity>(TEntity entity, Dictionary<string, string[]> modelState) where TEntity : IEntity
        {
            Action<IEnumerable<EntityEntry>> resolveConflicts = (entries) =>
            {
                var exceptionEntry = entries.Single();
                var clientValues = exceptionEntry.CurrentValues;
                var databaseEntry = exceptionEntry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    modelState.Add("Concurrency Conflict", new string[] { "Unable to save changes. The entry was deleted by another user." });
                }
                else
                {
                    var databaseValues = (TEntity)databaseEntry.ToObject();
                    var props = databaseEntry.Properties
                                .Where(property => !object.Equals(clientValues[property.Name], databaseEntry[property.Name]) && property.Name != "RowVersion");

                    foreach (var property in props)
                    {
                        modelState.Add(property.Name, new string[] { databaseEntry[property.Name].ToString() });
                    }
                    modelState.Add("Row Version", new string[] { $"Current value: {Convert.ToBase64String(databaseValues.RowVersion)}" });
                    modelState.Add("Concurrency Conflict", new string[] { "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed." });
                    entity.RowVersion = (byte[])databaseValues.RowVersion;
                }
            };
            return resolveConflicts;
        }
    }
}
