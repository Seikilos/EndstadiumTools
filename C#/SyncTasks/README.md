SyncTasks
======================

A collection of helpers for the sync msbuild script

FilterFcivXml Task
-------------------
MSBuild supports filtering of files to not be copied to destination. However fciv does not properly support filtering files.
This tasks traverse the hash xml file and removes all files according to the same filter