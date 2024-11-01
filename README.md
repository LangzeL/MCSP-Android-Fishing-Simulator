# Fishing Simulator

Welcome to the **Fishing Simulator**! This Unity-based mobile application offers an immersive fishing experience by integrating real-time camera feeds as the game background and guiding players through interactive tutorials. Whether you're a fishing enthusiast or just seeking a relaxing game, our Fishing Simulator provides engaging gameplay and stunning visuals.

Check the Gameplay Video!
<div align="center">
  <a href="https://www.youtube.com/watch?v=-cK8UKXdrHE">
    <img src="https://img.youtube.com/vi/-cK8UKXdrHE/0.jpg" alt="video">
  </a>
</div>


## Authors

This project was developed by the COMP90018 Project Group T10-G01 at the University of Melbourne:

- **[Langze Lu](https://github.com/LangzeL)**

  - 🆔 Student ID: 1185039
  - 📧 Email: [langzel@student.unimelb.edu.au](langzel@student.unimelb.edu.au)

- **[Wenda Zhang](https://github.com/WendaZhang08)**

  - 🆔 Student ID: 1126164
  - 📧 Email: [wendaz@student.unimelb.edu.au](mailto:wendaz@student.unimelb.edu.au)

- **[Shanqing Huang](https://github.com/shanqingh)**

  - 🆔 Student ID: 1266301
  - 📧 Email: [shanqingh@student.unimelb.edu.au](mailto:shanqingh@student.unimelb.edu.au)

- **[Tianxi Peng](https://github.com/timpeng123)**

  - 🆔 Student ID: 1169385
  - 📧 Email: [tianxip@student.unimelb.edu.au](mailto:tianxip@student.unimelb.edu.au)

- **[Yilin Chen](https://github.com/6188145)**

  - 🆔 Student ID: 1239841
  - 📧 Email: [yilinc6@student.unimelb.edu.au](mailto:yilinc6@student.unimelb.edu.au)

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Compiling and Running the App](#compiling-and-running-the-app)
  - [For Android](#for-android)
- [Project Structure](#project-structure)
- [Contributing](#contributing)

## Features

- **Real-Time Camera Background**: Utilize your device's camera to create a dynamic and immersive game environment.
- **Accelerometer-Based Casting Detection**: Detects the player's casting motion using the device's accelerometer, providing a realistic fishing experience.
- **Inclinometer-Controlled Net Fishing**: Uses the device's level (inclinometer) to control the fishing net, allowing players to balance the net and catch fish intuitively.
- **Firebase Integration**: Connects to Firebase for real-time data storage, user authentication, and analytics, enhancing the game's backend capabilities.
- **Responsive UI**: Ensures optimal display across various screen sizes and orientations.
- **Interactive Tutorial**: A blinking tutorial canvas guides new players through the basic game mechanics for the first 3 seconds of each game round.

## Requirements

Before you begin, ensure you have met the following requirements:

- **Unity Version**: Unity 2020.3.8f1 LTS or later.
- **Platform**: Android only.
  - **Supported Android API Levels**: 26 (Android 8.0 Oreo) to 34 (Android 13).
- **Development Tools**:
  - **For Android**: Android SDK, JDK, and appropriate USB drivers.
- **Device Permissions**:
  - **Camera Access**: Required for displaying the camera background.
  - **Motion Sensors**: Access to accelerometer and inclinometer for detecting casting motions and controlling the fishing net.

> **Note**: The application does **not** support iOS devices.

## Installation

### 1. Clone the Repository

Clone this repository to your local machine using Git:

```bash
git clone https://github.com/yourusername/fishing-simulator-game.git
```

### 2. Open the Project in Unity

1. **Launch Unity Hub**.
2. **Add the Project**:
   - Click on the **"Add"** button.
   - Navigate to the cloned repository folder and select it.
3. **Open the Project**:
   - Once added, select the project and click **"Open"**.

### 3. Import Necessary Packages

Ensure all necessary Unity packages are imported:

- **TextMeshPro**: For advanced text rendering.
- **WebCamTexture**: For accessing the device's camera feed.
- **Input System** (Optional): For handling accelerometer and inclinometer inputs more effectively.

_Note: Unity usually prompts you to import essential packages like TextMeshPro when required._

## Compiling and Running the App

Follow the instructions below to compile and run the app on your Android device.

### For Android

#### 1. Set Up Android Build Environment

<img src="./Images/Set Up Android Build Environment.png" alt="Set Up Android Build Environment" width="600">

Ensure you have the Android Build Support module installed in Unity:

1. **Open Unity Hub**.
2. **Navigate to Installs**.
3. **Find Your Unity Version** and click the **three dots (...)**.
4. **Select "Add Modules"**.
5. **Check "Android Build Support"** and install.

#### 2. Configure Project for Android

<img src="./Images/Configure Project for Android.png" alt="Configure Project for Android" width="600">

1. **Open Build Settings**:
   - Go to **File > Build Settings**.
2. **Select Android**:
   - Choose **Android** from the **Platform** list.
   - Click **"Switch Platform"**.
3. **Player Settings**:
   - Click on **"Player Settings"**.
   - **Under "Other Settings"**:
     - **Package Name**: Ensure it follows the format `com.mcsp.fishingsimulator`.
     - **Minimum API Level**: Set to **Android 8.0 Oreo (API Level 26)**.
     - **Target API Level**: Set to **Android 13 (API Level 34)**.
     - **Scripting Backend**: Preferably set to **IL2CPP** for better performance.
     - **Internet Access**: Set to **Require** if your app needs internet access; otherwise, **Auto**.
   - **Permissions**:
     - Ensure **"Camera"** permission is added. Unity handles this automatically when using `WebCamTexture`, but verify in the **AndroidManifest.xml** if needed.

#### 3. Connect Your Android Device

1. **Enable Developer Options**:
   - On your Android device, go to **Settings > About Phone**.
   - Tap **"Build Number"** seven times to enable Developer Options.
2. **Enable USB Debugging**:
   - Go to **Settings > Developer Options**.
   - Turn on **"USB Debugging"**.
3. **Connect via USB**:
   - Use a USB cable to connect your device to the computer.
   - Allow USB debugging permissions if prompted on your device.

#### 4. Build and Run

<img src="./Images/Build and Run.png" alt="Build and Run" width="600">

1. **Open Build Settings**:
   - Go to **File > Build Settings**.
2. **Ensure "Run Device" is Selected**:
   - Your connected device should appear in the **Run Device** list.
3. **Click "Build and Run"**:
   - Unity will compile the project and install it on your device automatically.
4. **Launch the App**:
   - Once installed, the app should launch automatically on your device. If not, manually open it from your device's app drawer.

## Project Structure

Here's a brief overview of the project's folder structure:

```
FishingSimulator/
├── Assets/
│   ├── Scripts/
│   │   ├── Audio/
│   │   |   └── (Any Audio Controller Scripts)
│   │   ├── Firebase/
│   │   |   └── (Any Database Controller Scripts)
│   │   ├── MainMenu/
│   │   |   └── (Any UI Controller Scripts)
│   │   ├── SenesorTest/
│   │   |   └── (Any Sensor Controller Scripts)
│   │   └── (Other Script.cs)
│   ├── Scenes/
│   │   └── StartScene.unity
│   │   └── MainGameScene.unity
│   │   └── NetScene.unity
│   ├── Prefabs/
│   │   └── (Any prefab assets)
│   ├── UI/
│   │   ├── Fonts/
│   │   ├── Icons/
│   │   └── (Other UI elements)
│   └── (Other asset folders)
├── ProjectSettings/
│   └── (Unity project settings files)
├── Packages/
│   └── (Unity package files)
└── README.md
```

## Contributing

Contributions are welcome! Please follow these steps to contribute:

1. **Fork the Repository**.
2. **Create a New Branch**:
   - `git checkout -b feature/YourFeatureName`
3. **Commit Your Changes**:
   - `git commit -m "Add some feature"`
4. **Push to the Branch**:
   - `git push origin feature/YourFeatureName`
5. **Open a Pull Request**.

Please ensure your code follows the project's coding standards and includes appropriate comments and documentation.

---

**Enjoy fishing in the virtual world! 🎣🐟**
