Daemonizer
===============

Simple C# tool that spawns a given process without shell as current user with arbitrary args.
If process exits with exit code != 0 the output of the spawned process will be saved to disk
and the user will receive a message box.


For convenience: A compiled release version of the Daemonizer is checked-in under bin