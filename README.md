# ASP.NET-Identity-SQLite-EntityFramework
Implementation of ASP.NET Identity membership based on SQLite database and Entity Framework OR/M

# Basic Info
This is an implementation of ASP.NET Identity membership with Entity Framework and SQLite database. For my purpose there was unnecessary multi roles strategy (one user can have more than 1 role), which is default in Identity. So here I have a single role strategy (one user - one role).

# Implementation details
As a starting point was taken ASP.NET Identity 2 from MVC 5 project.
This project is based on SQLite database, project link: https://sqlite.org/;
Also it uses System.Data.SQLite ADO.NET provider for SQLite, project link: https://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki

# How to install SQLite database and System.Data.SQLite provider correctly
If you want to use SQLite AND Entity Framework, first thing that you should do is to install SQLite database on your computer if you don't have it. To do so, you should download from https://sqlite.org/download.html sqlite-shell-...zip and sqlite-dll...zip files and run both installers. When you are happy with this go to https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki and choose right package of System.Data.SQLite based on your system architecture (32 bit/64 bit) and version of .NET framework installed on your computer. 

NOTE: It's very important to install setup-bundle package due to it is fully than the other setup package. For example, I run .NET Framework of version 4.5.1 and my system is 64 bit-based. So I have to choose sqlite-netFx451-setup-bundle-x64-2013-{current version}.exe package. When you are happy with this you can set up your project. In Visual Studio go to package manger and search for System.Data.SQLite. 

! Important Thing ! You MUST install nuget System.Data.SQLite package of the same version as you installed it from offical site.
For example, if your installation package of System.Data.SQLite provider which you downloaded from offical site was sqlite-netFx451-setup-bundle-x64-2013-1.0.94.0.exe, you MUST install nuget package of version 1.0.94.0. Select 'System.Data.SQLite (x86/x64)' package and it will also install for you all additional packages for Entity Framework and others. When it completed go to your .config file an find 2 new sections: entityFramework and system.data. Unfotunately, when you are installing package from nuget it creates some wrong nodes in this sections. Change it manually. It should be something like this:

```xml
<entityFramework>
    <defaultConnectionFactory type="System.Data.Entity.Infrastructure.SqlConnectionFactory, EntityFramework" />
    <providers>
      <provider invariantName="System.Data.SQLite.EF6" type="System.Data.SQLite.EF6.SQLiteProviderServices, System.Data.SQLite.EF6, Version={your version}, Culture=neutral" />
      <provider invariantName="System.Data.SQLite" type="System.Data.SQLite.EF6.SQLiteProviderServices, System.Data.SQLite.EF6, Version={your version}, Culture=neutral" />
    </providers>
  </entityFramework>
  <system.data>
    <DbProviderFactories>
      <remove invariant="System.Data.SQLite" />
      <remove invariant="System.Data.SQLite.EF6" />
      <add name="SQLite Data Provider" invariant="System.Data.SQLite" description=".NET Framework Data Provider for SQLite" type="System.Data.SQLite.SQLiteFactory, System.Data.SQLite" />
      <add name="SQLite Data Provider (Entity Framework 6)" invariant="System.Data.SQLite.EF6" description=".NET Framework Data Provider for SQLite (Entity Framework 6)" type="System.Data.SQLite.EF6.SQLiteProviderFactory, System.Data.SQLite.EF6" />
    </DbProviderFactories>
  </system.data>
```

If you have installed System.Data.SQLite of version 1.0.94.0, you should have this number in Version={your version} attribute of 'provider' nodes instead of {your version} (e.g. Version=1.0.94.0).

And the last step of preparation. Go to this page https://visualstudiogallery.msdn.microsoft.com/0e313dfd-be80-4afb-b5e9-6e74d369f7a1/ and install Visual Studio Add-In for SQL Server Compact/SQLite. With this toolbox you can connect to your database from Visual Studio.

Only when you have done with all this steps you can connect to your SQLite database (or create a new one) and create Entity Framework model

# How to use this/your own Identity implementation
Include in your project custom Identity implementation and change all using directive from ASP.NET.Identity to this custom one.
Delete constructor from 'something'Context class (I think it should be ApplicationContext) because it is unnecessary now.
Now you should use IdentityUser and IdentityRole classes from your implementation instead of previous ones from Microsoft's ASP.NET.Identity

Now it all should work! Enjoy!
