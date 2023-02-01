# ezr² Net4.8

## What is ezr² Net4.8?
**This is not the main repo of ezr².** This is a clone of ezr² made with `DotNet 4.8` and `C# 9.0` as a `class library`,
to be used in other programs. Check out ezr² [here](https://github.com/Uralstech/ezrSquared).

## Progress
**For those confused by the versioning: 1st place -> Major; 2nd place -> Feature; 3rd place -> Function; 4th place -> Library; 5th place -> Patch**

### Released
**Check the [GitHub Commits](https://github.com/Uralstech/ezrSquaredNet4.8/commits) for all changes in source code**

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