The primary purpose of the iTunes Toolbox is to synchronize the ratings imprisoned in iTunes with the files on your hard disk.

## Goals ##

- Sync ratings from iTunes to Windows Files
- Sync ratings from Windows Files to iTunes
- Delete files rated one (1) from both Windows & iTunes
- Rename files (Format: Artist [Album] - Title)

## Usage ##

### Informational Options ###
```
/D		Display track information
```


### Updating Options ###
```
/W		Update ratings in Windows files (iTunes => File)
/I		Update ratings in iTunes Tracks (File => iTunes)
/REN	Rename Windows files. Format: (Artist [Album] - Title)
/DEL	Delete & Remove tracks with a rating of one (1)
```


### Logging Options ###
```
/L		Log all messages to the console (equivalent to /LI /LW /LE)
/LI		Log informational messages to the console
/LW		Log warning messages to the console
/LE		Log error messages to the console
```


### Examples ###

```
Console /D				Display iTunes track information on the console
Console /W /DEL /L		Update ratings in Windows files (from iTunes), delete all tracks rated one (1) and log all messages to the console
Console /REN /LW /LE	Rename all files using the format Artist [Album] - Title and log only warnings and errors to the console
```

## General ##

The solution has been developed in Visual Studio 2010 using the C# language.
The executable requires Windows and the .NET Framework 4.0.

Tested on Windows 7 using iTunes 10.0.1.22
Last edited Oct 19 2010 at 1:45 PM