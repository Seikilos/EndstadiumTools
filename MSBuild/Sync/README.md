Build script for syncing files to and from any file system using MSBuild
========================================================================

This sync system is a msbuild project that performs copying files using robocopy and
then uses Microsoft's File Checksum Integrity Verifier to ensure file copy integrity to
destinations without automatic integrity checks.

__Be aware__: This is never a two-way sync but is used to explicitly copy data from or to a remote destination.
The destination will be purged and contain only files from the source.

When copying to a remote destination (e.g. NAS) a full md5 list is created for s given directory. If files
are already broken or compromised locally they __WILL__ remain so. Those files are then transmitted to the destination,
the hash database is copied to the destination and integrity checks are executed on the destination to ensure no copy
error occured. The hash db remains there and can be used by the __VerifyHash__ target of the msbuild script or use the
python tool in the Python directory of this repository to perform integrity checks on the premises (e.g as a cron on a NAS)

_Additional info_ regarding __SyncFromDestination__: Since copying from remote destination, which typically has a wider access imposes 
an increased risk of broken or compromised data, the __SyncFromDestination__ target performs a verificaton __before__ copying files to
ensure the lowest level of integrity, copies the files and ensure integrity again.

Prerequisites
-------------

Two(+2) tools are required:

* Obviously [MSBuild](http://msdn.microsoft.com/en-us/library/0k6kkbsd.aspx), which also should be installed on Vista/7/8, check C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe or any update of the .NET framework, e.g. C:\Windows\Microsoft.NET\Framework\*\MSBuild.exe
* Custom build SyncTasks assembly which is available in the C# hive of this repository. Binary is checked in for convenience
* [Robocopy ](http://technet.microsoft.com/en-us/library/cc733145.aspx), which should be installed on Vista/7/8
* [File Checksum Integrity Verifier from Microsoft](http://support.microsoft.com/kb/841290), (it looks like an installer, but it just unpacks the executable, which is then portable). Place it alongside the nassync.msbuild.xml file.

Usage
-----

Create a msbuild project (for instance relative to the nassync.msbuild.xml file) and reference all directories from and to the backup destination

```XML
<Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <!--
    This project includes nas sync and simply contains target calls for arbitrary locations
    
    MSBuild location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
    -->
    
    <PropertyGroup>
        <Script>nassync.msbuild.xml</Script>
        <ToNas>SyncToDestination</ToNas>
        <FromNas>SyncFromDestination</FromNas>
    </PropertyGroup>
    
    <Target Name="Build">
        <MSBuild Properties="source=d:\important_data;destination=\\nas\data\my_data" Projects="$(Script)" Targets="$(ToNas)" />
        <MSBuild Properties="source=c:\my_mp3;destination=\\nas\music"                Projects="$(Script)" Targets="$(ToNas)" />

        <!-- This syncs files FROM server to local machine, performs integrity check before and after copying -->
        <MSBuild Properties="source=\\nas\music;destination=c:\my_music"              Projects="$(Script)" Targets="$(FromNas)" />
        
    </Target>
</Project>
```

Disclaimer
----------
This software performs destructive copying, so that the destination will be a perfect clone of the source (with ensured integrity).
It is __NOT__ a backup solution as syncing broken or compromized files results in both locations having broken data.
The developer is not responsible for any damage this software could potentially do. Use it on your own risk.
