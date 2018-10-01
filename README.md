# VideoKioskWPF application

#### Features
This is the simple application with TrueConf SDK control and button.
Main features:
- call to random user from address book by clicking button;
- shows incoming chat messages on popup with specified display timeout;
- auto accept all incoming calls and invitations.

#### Settings
Application also contains the Setting page that is opened by adding command line argument `-config` or using hot keys `Ctrl+Shift+F12`. The Setting page contains the authorization settings(server, login and password), hardware settings (camera, microphone, speakers), chat messages display timeout setting, logging setting.

###### Main page
![Video Kiosk Main Page](https://github.com/TrueConf/VideoKioskWPF/blob/master/screenshots/Screenshot_1.png)

###### Main page in call state
![Video Kiosk Main Page In Call](https://github.com/TrueConf/VideoKioskWPF/blob/master/screenshots/Screenshot_3.png)

###### Settings page
![Video Kiosk Settings Page](https://github.com/TrueConf/VideoKioskWPF/blob/master/screenshots/Screenshot_4.png)

# How to install CallX control in Visual Studio (2017)

1. Create new Project -> Windows Forms Control Library
![Settings VS Part 1](https://github.com/TrueConf/VideoKioskWPF/blob/master/screenshots/Screenshot_5.png)

2. Open Toolbox and right click on it -> Choose Items...

![Settings VS Part 2](https://github.com/TrueConf/VideoKioskWPF/blob/master/screenshots/Screenshot_6.png)

3. Open COM Components tab in opened window and check the TrueConfCallX Class then click Ok button
![Settings VS Part 3](https://github.com/TrueConf/VideoKioskWPF/blob/master/screenshots/Screenshot_7.png)

4. Add TrueConf Control to the UserControl and set Dock property to Fill
![Settings VS Part 4](https://github.com/TrueConf/VideoKioskWPF/blob/master/screenshots/Screenshot_8.png)

5. Build project ->Interop.TrueConf_CallXLib.dll and AxInterop.TrueConf_CallXLib.dll are created in Debug (or Release) folder

6. Create new Project -> WPF App
![Settings VS Part 5](https://github.com/TrueConf/VideoKioskWPF/blob/master/screenshots/Screenshot_9.png)

7. Add Reference to dll from 5 step -> AxInterop.TrueConf_CallXLib.dll:
   - Right click on References -> Add Reference
   - Click Browse button -> open Debug (or Release) folder from previous project and choose AxInterop.TrueConf_CallXLib.dll
   - Click Add button
![Settings VS Part 6](https://github.com/TrueConf/VideoKioskWPF/blob/master/screenshots/Screenshot_10.png)

8. Open toolbox and add WindowsFormsHost control to MainWindow
![Settings VS Part 7](https://github.com/TrueConf/VideoKioskWPF/blob/master/screenshots/Screenshot_11.png)

9. Set TrueConf SDK as child for WindowsFormsHost control in MainWindow constructor after `InitializeComponent`:
```
AxTrueConfCallX tc = new AxTrueConfCallX();
windowsFormsHost.Child = tc;
```
Done.

Now you can use The CallX ActiveX control in WPF Apps

Also see https://sdk.trueconf.com/
