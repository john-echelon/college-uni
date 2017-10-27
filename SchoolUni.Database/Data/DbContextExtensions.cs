using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Rest.TransientFaultHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchoolUni.Database.Data
{
    public enum RefreshConflict
    {
        StoreWins = 0,

        ClientWins = 1,

        MergeClientAndStore =2,
    }

    public static partial class DbContextExtensions
    {
        public static int SaveChanges(
         this DbContext context, Action<IEnumerable<EntityEntry>> resolveConflicts, int retryCount = 3, bool userResolveConflict = false)
        {
            bool ignoreSave = false;
            if (retryCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryCount));
            }

            for (int retry = 1; retry < retryCount; retry++)
            {
                try
                {
                    return context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException exception) when (retry < retryCount)
                {
                    resolveConflicts(exception.Entries);
                    if(userResolveConflict == true)
                    {
                        ignoreSave = true;
                        break;
                    }
                }
            }
            if (ignoreSave)
                return 0;
            return context.SaveChanges();
        }

        public static EntityEntry Refresh(this EntityEntry tracking, RefreshConflict refreshMode)
        {
            switch (refreshMode)
            {
                case RefreshConflict.StoreWins:
                    {
                        // When entity is already deleted in database, Reload sets tracking state to Detached.
                        // When entity is already updated in database, Reload sets tracking state to Unchanged.
                        tracking.Reload(); // Execute SELECT.
                                           // Hereafter, SaveChanges ignores this entity.
                        break;
                    }
                case RefreshConflict.ClientWins:
                    {
                        PropertyValues databaseValues = tracking.GetDatabaseValues(); // Execute SELECT.
                        if (databaseValues == null)
                        {
                            // When entity is already deleted in database, there is nothing for client to win against.
                            // Manually set tracking state to Detached.
                            tracking.State = EntityState.Detached;
                            // Hereafter, SaveChanges ignores this entity.
                        }
                        else
                        {
                            // When entity is already updated in database, refresh original values, which go to in WHERE clause.
                            tracking.OriginalValues.SetValues(databaseValues);
                            // Hereafter, SaveChanges executes UPDATE/DELETE for this entity, with refreshed values in WHERE clause.
                        }
                        break;
                    }
                case RefreshConflict.MergeClientAndStore:
                    {
                        PropertyValues databaseValues = tracking.GetDatabaseValues(); // Execute SELECT.
                        if (databaseValues == null)
                        {
                            // When entity is already deleted in database, there is nothing for client to merge with.
                            // Manually set tracking state to Detached.
                            tracking.State = EntityState.Detached;
                            // Hereafter, SaveChanges ignores this entity.
                        }
                        else
                        {
                            // When entity is already updated, refresh original values, which go to WHERE clause.
                            PropertyValues originalValues = tracking.OriginalValues.Clone();
                            tracking.OriginalValues.SetValues(databaseValues);
                            // If database has an different value for a property, then retain the database value.
#if EF
                            var props = databaseValues.PropertyNames // Navigation properties are not included.
                                .Where(property => !object.Equals(originalValues[property], databaseValues[property]));
#else
                            var props = databaseValues.Properties // Navigation properties are not included.
                                .Where(property => !object.Equals(originalValues[property.Name], databaseValues[property.Name]));

                            //(props as List<IProperty>).ForEach(property => tracking.Property(property.Name).IsModified = false);
                            foreach(var property in props) { 
                                if(!object.Equals(originalValues[property.Name], databaseValues[property.Name]))
                                {
                                    tracking.Property(property.Name).IsModified = false;
                                }
                            }
#endif
                            // Hereafter, SaveChanges executes UPDATE/DELETE for this entity, with refreshed values in WHERE clause.
                        }
                        break;
                    }
            }
            return tracking;
        }
    }
}
