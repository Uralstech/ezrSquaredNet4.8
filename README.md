# ezr² Net4.8

## What is ezr² Net4.8?
**This is not the main repo of ezr².** This is a clone of ezr² made with `DotNet 4.8` and `C# 9.0` as a `class library`,
to be used in other programs. Check out ezr² [here](https://github.com/Uralstech/ezrSquared).

## NOTICE
**ezr² Net4.8 (prerelease-1.5.1.0.0) is only equal to ezr² prerelease-1.3.2.0.0.**

## Progress
**For those confused by the versioning: 1st place -> Major; 2nd place -> Feature; 3rd place -> Quality of Life; 4th place -> Library; 5th place -> Patch**

### Released
**Check the [GitHub Commits](https://github.com/Uralstech/ezrSquaredNet4.8/commits) for all changes in source code**

* **prerelease-1.5.1.0.0** - [16-02-23]
    * runtimeRunError message parity with ezr²

* **prerelease-1.5.0.0.0** - [15-02-23]
    * Added flag to stop program at next visit function call
    * Better errors with new runtimeRunError and interruptError classes

* **prerelease-1.4.1.0.0** - [15-02-23]
    * Fixed a count loop error message
    * Better error message for else if statements

* **prerelease-1.4.0.0.2** - [12-02-23]
    * Fixed a bug in error messages

* **prerelease-1.4.0.0.1** - [12-02-23]
    * Fixed `remove` function in character_lists
    * Fixed `in` expression in lists

* **prerelease-1.4.0.0.0** - [12-02-23]
    * Parity to ezrSquared
 
* **prerelease-1.3.0.0.1** - [01-02-23]
    * Fixed operators `remove` and `get` in character_list, list and array - ezr² would crash if input was a float

* **prerelease-1.3.0.0.0** - [01-02-23]
    * Added `remove` operation to character_list
    * Removed `remove_at` function in character_list and list as they are redundant - use the `remove` operation

* **prerelease-1.2.0.1.0** - [31-01-23]
    * Added back IO library
    * Fixed bug in interpreter - visit_objectDefinitionNode now returns Task\<runtimeResult>

* **prerelease-1.2.0.0.1** - [30-01-23]
    * Fixed bug in `character_list` type - all `storedValue` references should now be casted to List\<char>
    * Fixed hashing and comparison for all types

* **prerelease-1.2.0.0.0** - [30-01-23]
    * Removed all dynamic variables

* **prerelease-1.1.0.0.0** - [27-01-23]
    * Converted all interpreter functions to async functions

* **prerelease-1.0.0.0.2** - [26-01-23]
    * Removed useless using for IO library

* **prerelease-1.0.0.0.1** - [26-01-23]
    * Fixed `array` slicing

* **prerelease-1.0.0.0.0** - [26-01-23]
    * Initial release!