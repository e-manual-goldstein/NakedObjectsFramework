Naked Objects Framework
=======================

Full documentation of how use the framework (typically starting from the Template projects) is contained in the Developer Manual (within the Documentation folder).
There is no need to download and build the source, as the recommended way to use the framework is via the published NuGet and NPM packages. (However there are details in the Developer Manual on how to build the source for those that want to.)

This branch contains the current release. These are the principal changes from the previous release.

- All assemblies will now be compiled against .NET Core 3
- Updating of framework source code to use C#8 capabilities
- Removal of dependency on System.Dynamic, due to concerns that that might be cause of (very rare) runtime errors.
- Removal of redundant code
- Restructuring of packages

Under the new package structure, we will be publishing just two NuGet packages:

- NakedObjects.Server (currently 11.0.0-rc01, soon to be released as version 11.0.0)
- NakedObjects.ProgrammingModel (8.0.0.)	 
	Note however that this is identical to 7.0.4 - the current release - except compiled against .NET Core 3,
	so no changes to domain object model code are required. (Members obsoleted since 7.0 have now been removed).
	
- The Naked Objects Client is built from a set of NPM packages. 

The Client and Server communicate via  RESTful API, which remains at version 1.1.




