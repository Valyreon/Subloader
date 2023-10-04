<img align="right" height="192px" src="https://raw.githubusercontent.com/Valyreon/Subloader/master/subtitles.png"/>

# Subloader
![](https://img.shields.io/badge/Price-Free-brightgreen.svg)
![](https://img.shields.io/badge/License-MIT-blue.svg)
![](https://img.shields.io/badge/Release-1.6.0-blue.svg)

**Subloader** is a simple and minimalistic software written in **C#** and **.NET 6** that enables you to quickly find and download subtitles for your movies and TV Shows. It interfaces with **Opensubtitles** database by using REST API for searching and downloading subtitles.

Subloader searches subtitles by using a special file hash. This enables the user to get the best possible search results for their video file. **Installer** will also add an entry to **right click context menu** of **.avi**, **.mkv** and **.mp4** files for easy access.

The application now also includes a **Subloader CLI** tool that can search through a directory and download subtitles for all found video files. It is included in the installer.

Search and download is provided by **[Opensubtitles](http://www.opensubtitles.com/)**. Big thanks to their team, please consider showing your appreciation by **[supporting them](https://www.opensubtitles.com/en/support_us/)**.

### Installation

#### Windows
Download the **[latest version of Subloader](https://github.com/Valyreon/Subloader/releases/latest)**, run the setup and that's it. You can now right click your video file and get your subtitles in a matter of seconds. Enjoy!

There is also now a portable version of the subloader you can find in the link above. It will keep the config file in the same directory as the executable.

If the CLI component is selected, installer will also add a **subloader-cli** executable and add the Subloader installation directory to PATH environment variable.

Subloader requires **[.NET 6 Runtime](https://dotnet.microsoft.com/download)** to run and this will probably be included with your Windows. If not, you will be prompted to download it on starting the app.

_I don't guarantee that the app will work properly on versions of Windows before Windows 10._

#### Linux
I am currently implementing the Subloader for Linux using Avalonia UI. Once I'm finished I will add a deb package and linux executable to the release.

### Usage

<p align="center"><img src="./Screenshots/mainView.png?raw=true" title="file sharing" align="center" hspace="5" vspace="5">

Subloader can be opened from Start Menu, and then using the 'File' button you can choose a video file. Search will begin immediately. Installer will also add 'Find subtitles' menu in right click context menu of .avi, .mkv and .mp4 files for easy access:

<p align="center"><img src="./Screenshots/contextMenu.png?raw=true" title="file sharing" align="center" hspace="5" vspace="5">

Note that only one instance of Subloader can be opened. If you search for a new video's subtitles from the context menu while Subloader is opened, the results will be shown in the already opened instance. That way you don't have to close the Subloader between searches.

You can also search manually without file selection, by using the Search button and entering the search parameters.

<p align="center"><img src="./Screenshots/searchView.png?raw=true" title="file sharing" align="center" hspace="5" vspace="5">

After the search is complete, you can select a subtitle from the list and double-click it or press Enter to download. If results are by searching without selected video file you will be prompted for file location upon download.

When searching for subtitles by selecting the video file, **_the text of subtitle entries in the result table which have a matching movie hash will be bolded._** Also the initial sort of the subtitle entries will put the entries which have the most similar Release name to the video file name on top (sorted by Levenshtein distance).

When you double-click an item in the table Subloader will download the subtitle. The name and location of the file depends on Settings, default (without options below checked) behaviour is to download subtitles into a file that is the same name as the video file with a different extension.

### CLI Usage



### Wiki

You can find more details about the usage and implementation of Subloader on the **[Wiki pages](https://github.com/Valyreon/Subloader/wiki)**.

### Acknowledgments

Icon made by **[Freepik](https://www.flaticon.com/authors/freepik)** from **[Flaticon](https://www.flaticon.com )**.

License
----

This free software is released under a modified **[MIT License](https://opensource.org/licenses/MIT)**.
