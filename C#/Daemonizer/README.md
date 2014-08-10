Daemonizer
===============

Simple C# tool that spawns a given process without shell as current user with arbitrary args.
If process exists with exit code != 0 the output of the spawned process will be stored to disk
and the user will receive a message box.
