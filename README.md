<img align="right" height="192px" src="https://raw.githubusercontent.com/Valyreon/Subloader/master/subtitles.png"/>

# Subloader
![](https://img.shields.io/badge/Price-Free-brightgreen.svg)
![](https://img.shields.io/badge/License-MIT-blue.svg)
![](https://img.shields.io/badge/Release-1.2.0-blue.svg)

**Subloader** is a simple and minimalistic software written in **C#** and **.NET Core 3.0** that enables you to quickly find and download subtitles for your movies and TV Shows. It interfaces with **Opensubtitles** database by using REST API for searching and downloading subtitles.

Subloader searches subtitles by using a special file hash. This enables the user to get the best possible search results for their video file. **Installer** will also add an entry to **right click context menu** of **.avi**, **.mkv** and **.mp4** files for easy access. **Note:** This feature might not work on Windows 7.

Search and download is provided by **[Opensubtitles](http://www.opensubtitles.org/)**. Big thanks to their team for this great project. Please consider showing your appreciation by **[supporting them](https://www.opensubtitles.org/en/support)**.

### Installation

Subloader requires **[.NET Framework 4.6.1](https://www.microsoft.com/net/download/thank-you/net461)** to run. It was developed in Visual Studio Community 2017.

Download the **[latest version of Subloader](https://github.com/Valyreon/Subloader/releases)**, run the setup and that's it. You can now right click your video file and get your subtitles in a matter of seconds. Enjoy!

### Usage

Subloader can be opened from Start Menu, and then using the 'Open' button you can choose a video file. Search will begin immediately. 

After the search is complete, you can select a subtitle from the list and click 'Download'. 

When you click 'Download' Subloader will download subtitle in the same directory where video file is, under the same name and appropriate extension. If there is an existing subtitle with the same name, it will overwrite it.

Depending on the time of day, opensubtitles server will sometimes be busy and you will get a 'Server error'. In that case, just click 'Refresh' until it responds.

Installer will also add 'Find subtitles' menu in right click context menu of .avi, .mkv and .mp4 files for easy access.

SubLoader now has a working UI for configuring wanted subtitle languages. You can access it via 'Settings' button on the main form in the bottom right corner.

### Acknowledgments

Icon made by **[Freepik](https://www.flaticon.com/authors/freepik)** from **[Flaticon](https://www.flaticon.com )**.

If you find **Subloader** useful, consider donating at **[PayPal](https://www.paypal.me/valyreon)**.
### Screenshots
![](https://raw.githubusercontent.com/Valyreon/Subloader/master/screenshot.png)
![](https://raw.githubusercontent.com/Valyreon/Subloader/master/screenshot2.png)
### To do
- ~~**Language settings window**~~
  
  ~~Subloader now reads from lang.cfg in install directory for languages if lang.cfg exists. Window for configuring the filter from UI is next.~~
  
- ~~**Move to MVVM**~~
  
  ~~Refactor and restructure Subloader so it uses Model-View-ViewModel design.~~
  
- **Interfacing with more subtitle databases**
  
License
----

This free software is released under **[MIT License](https://opensource.org/licenses/MIT)**.
