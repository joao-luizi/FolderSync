# FolderSync

## Overview

This program was made in response to a test task in an application i made in 2023.
Implementing this Test Task seemed a good learning opportunity.

The objective was to implement a solution that synchronizes two folders: source and 
replica.

The specifications were:
* Synchronization must be one-way: after the synchronization content of the replica folder should be modified to exactly match content of the source folder; 

* Synchronization should be performed periodically;
 
* File creation/copying/removal operations should be logged to a file and to the console output; 

* Folder paths, synchronization interval and log file path should be provided using the command line arguments; 

* It is undesirable to use third-party libraries that implement folder synchronization; 

* It is allowed (and recommended) to use external libraries implementing other well-known algorithms. For example, there is no point in implementing yet another function that calculates MD5 if you need it for the task – it is perfectly  acceptable to use a third-party (or built-in) library.



Planned Features:
1. Clarify the **State Pattern**
2. Add a GUI
3. Implement a custom Logger [using Microsoft.Extensions.Logging](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging?view=dotnet-plat-ext-7.0)