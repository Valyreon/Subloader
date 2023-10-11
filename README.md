<img align="right" height="192px" src="https://raw.githubusercontent.com/Valyreon/Subloader/master/subtitles.png"/>

# Subloader
![](https://img.shields.io/badge/Price-Free-brightgreen.svg)
![](https://img.shields.io/badge/License-MIT-blue.svg)
![](https://img.shields.io/badge/Release-1.6.0-blue.svg)

**Subloader** is a simple and minimalistic application that enables you to quickly find and download subtitles for your movies and TV Shows.

The application now also features a **Subloader CLI** tool that can search through a directory and download subtitles for all found video files. It is included in the installer.

This app was built with **.NET 6**, using **C#** and **WPF**. Search and download is provided by **[Opensubtitles API](http://www.opensubtitles.com/)**.

---

### Installation

#### Windows

You can install Subloader by using the **[latest installer](https://github.com/Valyreon/Subloader/releases/latest)**.

In the link above you can also find a portable versions of the app, both GUI and the CLI tool.

##### Scoop
If you use [Scoop package manager](https://scoop.sh/) for Windows, you can also install Subloader by running:

`scoop install https://raw.githubusercontent.com/Valyreon/Subloader/master/InstallerFiles/Scoop/subloader.json`

#### Linux
I am currently in the process of implementing the Subloader for Linux using Avalonia UI. Once I'm finished I will add a deb package and Linux executable to the release.

---

### Usage

<p align="center"><img src="./Screenshots/mainView.png?raw=true" title="file sharing" align="center" hspace="5" vspace="5">

Subloader can be opened from Start Menu, and then using the 'File' button you can choose a video file. Search will begin immediately. Installer will also add 'Find subtitles' menu in right click context menu of .avi, .mkv and .mp4 files for easy access:

<p align="center"><img src="./Screenshots/contextMenu.png?raw=true" title="file sharing" align="center" hspace="5" vspace="5">

Note that only one instance of Subloader can be opened. If you search for a new video's subtitles from the context menu while Subloader is opened, the results will be shown in the already opened instance. That way you don't have to close the Subloader between searches.

You can also search manually without file selection, by using the Search button and entering the search parameters:

<p align="center"><img src="./Screenshots/searchView.png?raw=true" title="file sharing" align="center" hspace="5" vspace="5">

After the search is complete, you can select a subtitle from the list and double-click it or press Enter to download. If results are by searching without selected video file you will be prompted for file location upon download.

When searching for subtitles by selecting the video file, **_the text of subtitle entries in the result table which have a matching movie hash will be bolded._** Also the initial sort of the subtitle entries will put the entries which have the most similar Release name to the video file name on top (sorted by Levenshtein distance).

The location of the downloaded subtitle for a video file will depend on the user's Settings.

#### Login

The new OpenSubtitles REST API <span style="color:red">**limits the number of allowed downloads based on the level of your account on their website**</span>. You can find the details in the table **[here](https://www.opensubtitles.com/en/support_us/)** although the values listed there are not always up to date.

For example, at the time of writing this, anonymous users are able to download 5 subtitle files daily and logged in users 20 files daily. Paying **VIP users** can download up to **1000 files** daily and use the VIP API endpoint, with other levels in between.

In the Settings > Login tab you will be able to login and see some details of your account:

<p align="center"><img src="./Screenshots/loggedInView.png?raw=true" title="file sharing" align="center" hspace="5" vspace="5">

---

### CLI Usage

If you installed subloader CLI tool with the Installer, you will be able to run the tool from any directory by opening a Command Prompt/PowerShell window. Currently there are 3 commands:

* **dir** - Download subtitles for all video files in the directory (can be recursive)
* **file** - Download subtitle for a single file
* **languages** - Used for finding appropriate language codes

You can find all the commands and paramaters description by using -h or --help parameter. You can use it on root or specific command, for example:

<p align="center"><img src="./Screenshots/terminalDirHelp.png?raw=true" title="file sharing" align="center" hspace="5" vspace="5">

Command run example:

<p align="center"><img src="./Screenshots/dirRunCommand.png?raw=true" title="file sharing" align="center" hspace="5" vspace="5">

---

### Wiki

You can find more details about the usage, settings and implementation of Subloader on the **[Wiki pages](https://github.com/Valyreon/Subloader/wiki)**.

### Acknowledgments

Icon made by **[Freepik](https://www.flaticon.com/authors/freepik)** from **[Flaticon](https://www.flaticon.com )**.

License
----

This free software is released under a modified **[MIT License](https://opensource.org/licenses/MIT)**.
