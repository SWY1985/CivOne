# Contributing to CivOne

Thank you for taking time to read this and for contributing to the CivOne project.
Here's a list of important documents that you might want to read too:

* [Code of Conduct](CODE_OF_CONDUCT.md)
* [Coding guidelines](CODING_GUIDELINES.md)
* [Readme](README.md)

#### Table of contents

* [Testing](#testing)
* [Reporting bugs](#reporting-bugs)
	* [Bugs](#bugs)
	* [Features](#features)
	* [Enhancements](#enhancements)
	* [Feature requests](#feature-requests)
	* [Discussions](#discussions)
* [Contributing code](#contributing-code)
* [Contributing other resources](#contributing-other-resources)

## Testing

If you want to test CivOne, it is recommended to clone or build the latest source from GitHub, then [build and run CivOne](https://github.com/SWY1985/CivOne/wiki/How-to-build-and-run-CivOne%3F).

Alternatively, you can download preview builds for [Windows](https://www.civone.org/Download/Windows), [Linux](https://www.civone.org/Download/Linux) and [macOS](https://www.civone.org/Download/macOS) on the [CivOne website](https://www.civone.org/).

## Reporting issues

You are free to open an issue to [report a bug](#bugs), [request a features](#feature-requests) or [start a discussion](#discussions), but it is expected that you first [search if an issue already exists](https://github.com/SWY1985/CivOne/issues).

If you are sure an issue does not already exists, you can create a new issue. If a similar issue already exists, consider contributing to the existing issue. If an issue is already closed, it can be reopened if you provide valid arguments. If your issue is somewhat related, you can reference another issue by using typing in your issue description field `#<issue-number>`.

Did someone open a duplicate issue? Everyone makes mistakes, overlooking that an issue is already exists is not forbidden. Don't be rude because someone couldn't find an existing issue, but be helpful and kindly point to the existing issue.

### Bugs

An issue is considered to be a bug if any of the following conditions apply:

* The application crashes
* The application behaves differently from Sid Meier's Civilization for DOS

A bug report preferably contains any of the following information/attachments:

* Version of CivOne used when the bug was detected
* Operating System (Windows, Linux, macOS)
* Operating System configuration (OS language, keyboard layout, screen configuration/resolution, etc.)
* Platform (32-bit, 64-bit, ARM, etc.)
* A screenshot of the issue, if applicable
* A logfile, crash dump or console output screenshot

### Features

An issue is considered to be a feature if:

* The described feature from Sid Meier's Civilization for DOS is not currently available in CivOne

A feature report preferably contains any of the following:

* A screenshot from the original game
* A description of the feature
* A (link to a) description of the inner workings of Sid Meier's Civilization for DOS

### Enhancements

An issue is considered to be an enhancement if any of the following conditions apply:

* The issue is an obvious bug in Sid Meier's Civilization for DOS
* The issue extends plugin API features
* The issue describes Operating System integration features

Most of the times, enhancements will be implemented as API extention, or will be optionally configurable through the Setup screen.

### Feature requests

An issue is considered to be a feature request if any of the following conditions apply:

* The issue adds features that are not available in Sid Meier's Civilization for DOS
* The issue alters features that are available in Sid Meier's Civilization for DOS
* The issue is available in other versions of Sid Meier's Civilization (Mac, Amiga, SNES, etc.), but not in the DOS version
* It is not possible to implement the feature using the Plugin API

The goal of CivOne, at this moment, is to mimic Sid Meier's Civilization as closely as reasonably possible. Requesting new features that alter or extends features from Sid Meier's Civilization for DOS, is not forbidden, but will probably be regarded as low priority at this point in development.

New features that get added to CivOne will most likely be disabled in official builds, and will only be available as optional patch through the Setup screen.

### Discussions

An issue is considered a discussion if any of the following conditions apply:

* The issue is related to code guidelines, documentation or resources of CivOne
* The issue is related to future development of CivOne
* The issue is related to included/excluded features of CivOne releases

For general discussion, please go to the [CivOne forum thread on CivFanatics](https://forums.civfanatics.com/threads/civone-an-open-source-remake-of-civilization-1.535036/).

During discussions, always remember the [Code of Conduct](CODE_OF_CONDUCT.md).

## Contributing code

If you wish to contribute code to CivOne, kindly first read the [Coding guidelines](CODING_GUIDELINES.md). Code that does not follow the coding guidelines will not be accepted, even if it solves all bugs.

It is preferable to use Visual Studio Code for coding and debugging. That way, some code styling will already be enforced through the .vscode settings. If you prefer another code editor, make sure your contributions meet the expectations, and do not make unnecessary changes to .sln and .csproj files.

Do not, under any circumstance, commit Sid Meier's Civilization assets (PIC, PAL, TXT or EXE files) with your code.

Make changes to your own fork of CivOne first. Use clear, English descriptions for your commits. Refer to existing issues, if applicable, like this: `#<issue-number>`. When (a part of) the code is finished and the resulting code does not (appear to) break any features, you can create a Pull Request.

## Contributing other resources

At this point, other resources such as graphics, sounds and documents are not requested, but if you feel like you have something to add to the project, you are free to either create an issue or a pull request.