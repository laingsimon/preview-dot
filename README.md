# preview-dot
A drawing previewer for [DOT format](https://en.wikipedia.org/wiki/DOT_(graph_description_language)) drawings in Windows Explorer and Outlook's preview panes.

![](https://github.com/laingsimon/preview-dot/blob/master/Screenshot.png)

### Installation & Uninstallation
#### System requirements
- [x] Microsoft Windows 7 or later (32bit or 64bit)
- [x] Microsoft .net Framework 4.6 installed (**prior** to running `install.bat`), [available here](https://www.microsoft.com/en-gb/download/details.aspx?id=48130)
- [x] GraphViz installed, [available here](https://graphviz.gitlab.io/_pages/Download/Download_windows.html)

* To install 
   * download the [latest release](https://github.com/laingsimon/preview-dot/releases)
   * extract the contents and run `install.bat` (right click and choose "Run as an administrator")
   * You must also install GraphViz, [downloads available here](https://graphviz.gitlab.io/_pages/Download/Download_windows.html)
* To uninstall 
   * Open the the folder `c:\Program Files\PreviewDot`
   * run `uninstall.bat` (right click and choose "Run as an administrator")
   * Uninstall GraphViz manually

### Features
* Preview a DOT format file with `.gv` extensions (in any application that has preview panes, e.g. Explorer, Outlook)
* Print the drawing from the preview
* Zoom in and move around the drawing
* Copy drawing to clipboard

### Resources
* Other resources are [available here](https://www.graphviz.org/resources/)
