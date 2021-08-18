## Welcome to the new WLED app! (v2.0-alpha)

A brand new app for Android, iOS, and Windows to discover and control your [WLED](https://github.com/Aircoookie/WLED) devices easily!

![Screenshots of various menus in the WLED App, rendered on a Galaxy S20.](app-screenshots.png)

### How to Download:

- Android: See Releases.
- UWP: See Releases.
- iPhone: Available on TestFlight [here](https://testflight.apple.com/join/7X5jBzH1).

### Features:

- Native UX
- Automatic Device Detection via mDNS
- Access and control your lights from one list
- Hide and delete devices
- Automatic Theme switching based on Device Theme

### Planned Features:

- Automatic setup when connected to WLED hotspot
- UWP Support
- Native settings interface when connected to v0.13 and higher
- Better support for Palettes



### FAQ:

#### My WLED lights are not found!

Ensure your phone/device is in the same network as the ESP.  
The "mDNS address" WiFi settings entry in WLED must be unique for every device.  
Try restarting both the ESP and your device.  
Attempt manually adding the device via its IP.

#### Can I control the lights when I'm not home?

In the default configuration this is not possible, the devices need to be in the same local network.  
However, you can either use a VPN to connect to your home network (if your router offers this option) or use a [port forwarding](https://github.com/Aircoookie/WLED/wiki/Remote-Access-and-IFTTT).  
Keep in mind that this exposes your light(s) to the public internet, so please be aware that it is not a secure solution.  
If you want to risk it, at least take the precaution of enabling the WLED [OTA lock](https://github.com/Aircoookie/WLED/wiki/Security) feature and, if possible, only connect it to a guest network.  

#### Why are you making this app? X app/service/the WLED server is perfectly fine!

I started developing this app as an exercise to learn C#. As I am growing ever more enthusiastic of the WLED project, I thought I would combine my willingness to learn a new language with one of the projects I love the most at the moment, and build something to give back to the community. I don't believe there is an absolute need for a native and official WLED app, but I still felt like it would be a welcome addition.
