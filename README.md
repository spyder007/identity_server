# Spydersoft Identity Server Instance

## Application Settings

## Upgrade Notes

When upgrading the ASP.NET Identity Packages or the Duende IdentityServer Packages, it may be necessary to add additional DB migrations.  This can be done by going into the Package Manager Console and typing the following:

``` powershell
Add-Migration My_New_Migration -Context My_ContextName
```

where **My_ContextName** is one of the following:

* ApplicationDbContext
* PersistedGrantDbContext
* ConfigurationDbContext

If changes are required, a migration will be generated.  The first time the application runs, the migration will be applied to the database if needed.
