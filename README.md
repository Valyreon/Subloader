
# Subloader

**Subloader** is a simple and minimalistic software written in **C#** that enables you to quickly find and download subtitles for your movies and TV Shows. It interfaces with **Opensubtitles** database by using XMLRPC API for searching and downloading subtitles.

Subloader searches subtitles by using a special file hash. This enables the user to get the best possible search results for their video file. **Installer** will also add an entry to **right click context menu** of **.avi**, **.mkv** and **.mp4** files for easy access.

Search and download is provided by **[Opensubtitles](http://www.opensubtitles.org/)**. Big thanks to their team for this great project. Please consider showing your appreciation by **[supporting them](https://www.opensubtitles.org/en/support)**.

### Installation

Subloader requires **.NET Framework 4.6.1** to run. 

Download the **[latest version of Subloader](https://github.com/Valyreon/Subloader/releases)**, run the setup and that's it. You can now right click your video file and get your subtitles in a matter of seconds. Enjoy!

### Contributions

Subloader works thanks to following open-source projects:

* **[XML-RPC.NET](http://xml-rpc.net/)** - a library for implementing XML-RPC Services and clients in the .NET environment

Icon made by **[Freepik](https://www.flaticon.com/authors/freepik)** from **[Flaticon](www.flaticon.com )**.

If you find **Subloader** useful, consider donating at **[PayPal](https://www.paypal.me/valyreon)**.
### Screenshots
![Subtitles search](https://raw.githubusercontent.com/Valyreon/Subloader/master/Subloaderscreen.PNG)

### To do
- **Language filter**
  
  I have started implementing this but stopped because during testing I realized that not all subtitles at the database are             properly tagged. I will probably implement some simple text file setting for language on client side, and not by using OS API, to avoid this problem and avoid adding clutter to the UI.
  
License
----

This free software is released under **[MIT License](https://opensource.org/licenses/MIT)**.
