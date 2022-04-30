<img align="right" height="192px" src="https://raw.githubusercontent.com/Valyreon/Subloader/master/subtitles.png"/>

# Subloader
![](https://img.shields.io/badge/Price-Free-brightgreen.svg)
![](https://img.shields.io/badge/License-MIT-blue.svg)
![](https://img.shields.io/badge/Release-1.5.0-blue.svg)

**Subloader** is a simple and minimalistic software written in **C#** and **.NET 6** that enables you to quickly find and download subtitles for your movies and TV Shows. It interfaces with **Opensubtitles** database by using REST API for searching and downloading subtitles.

Subloader searches subtitles by using a special file hash. This enables the user to get the best possible search results for their video file. **Installer** will also add an entry to **right click context menu** of **.avi**, **.mkv** and **.mp4** files for easy access. User can also manually input strings to query the API. It will also follow Windows 10 accent color for UI main color.

Search and download is provided by **[Opensubtitles](http://www.opensubtitles.org/)**. Big thanks to their team, please consider showing your appreciation by **[supporting them](https://www.opensubtitles.org/en/support)**.

### Installation

Subloader requires **[.NET 6 Runtime](https://dotnet.microsoft.com/download)** to run. It was developed in Visual Studio Community 2022.

Download the **[latest version of Subloader](https://github.com/Valyreon/Subloader/releases)**, run the setup and that's it. You can now right click your video file and get your subtitles in a matter of seconds. Enjoy!

#### Manual installation

To compile Subloader yourself you don't have to install Visual Studio, just .NET 6 SDK. Go to the Subloader folder that contains the solution, open terminal and run:

```
dotnet publish SubloaderWpf -p:PublishSingleFile=true --no-self-contained -r win-x64 -o .
```

This will output the compiled exe file in the current directory. Generated pdb files are not neccessary.

### Usage

Subloader can be opened from Start Menu, and then using the 'Open' button you can choose a video file. Search will begin immediately. Installer will also add 'Find subtitles' menu in right click context menu of .avi, .mkv and .mp4 files for easy access. You can also search manually without file selection, by using the Search button, entering the title and/or relevant info and pressing Enter(or Search button in the modal window).

After the search is complete, you can select a subtitle from the list and double-click it or press Enter to download. 

When you double-click an item in the table Subloader will download the subtitle. The name and location of the file depends on Settings, default (without options below checked) behaviour is to download subtitles into a file that is the same name as the video file with a different extension.

Depending on the time of day, Opensubtitles server will sometimes be busy and you will get a 'Service temporarily unavailable'. In that case, try clicking 'Refresh' a few times.

In **Settings** you can configure wanted **subtitle languages** as well as other options:

* **Always on top** - When checked, Subloader will stay above other windows.
* **Allow multiple downloads** - If this option is checked Subloader will allow downloading multiple subtitle files named *movie-title.lang-id.format*, for example: *matrix-reloaded.eng.srt*. If user downloads multiple subtitles from the same language, the files will be named *movie-name.(1).lang-id.format*.
* **Download to Subs folder** - This option allows user to download subtitles into a separate Subs folder in the same directory as the file. This option can be used only if option above is checked as well. The naming of the files is the same as for the option above.
* **Overwrite same language files** - If checked, when user downloads multiple subtitles of the same language Subloader won't create multiple files named *movie-name.(1).lang-id.format*, *movie-name.(2).lang-id.format* etc. but overwrite the existing *movie-title.lang-id.format* file with the last downloaded subtitle.

### Acknowledgments

Icon made by **[Freepik](https://www.flaticon.com/authors/freepik)** from **[Flaticon](https://www.flaticon.com )**.

### Screenshots
| | |
|:-------------------------:|:-------------------------:|
|  <img width="1604" alt="screen shot 2017-08-07 at 12 18 15 pm" src="https://raw.githubusercontent.com/Valyreon/Subloader/master/screenshot3.png">|<img width="1604" alt="screen shot 2017-08-07 at 12 18 15 pm" src="https://raw.githubusercontent.com/Valyreon/Subloader/master/screenshot2.png">|

### To do
- ~~**Language settings window**~~
  
  ~~Subloader now reads from lang.cfg in install directory for languages if lang.cfg exists. Window for configuring the filter from UI is next.~~
  
- ~~**Move to MVVM**~~
  
  ~~Refactor and restructure Subloader so it uses Model-View-ViewModel design.~~
  
- **Interfacing with more subtitle databases**

- ~~**Migrate to .NET 6**~~

- ~~**Add manual search functionality**--
  
License
----

This free software is released under **[MIT License](https://opensource.org/licenses/MIT)**.
