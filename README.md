# C#/.NET Distributed Chat App

A distributed desktop chat app built in **C#/.NET Framework** using **WPF** and **Windows Communication Foundation (WCF)**. The application allows multiple users to connect through separate clients, create and join lobby rooms, communicate through lobby chat and private messages, and share files.

Developed as a group project for **COMP3008 Distributed Computing** at **Curtin University** in Semester 2, 2024.

## Features

- **User authentication** with unique usernames and logout support
- **Lobby management** allowing users to create, discover, join, and leave rooms
- **Lobby chat** with messages shared between users in the same room
- **Private messaging** between individual users within a lobby
- **Asynchronous polling** for new messages, available lobbies, and connected users
- **Concurrent client support**, allowing multiple users to interact with the services simultaneously
- **Background processing** using tasks to retrieve updates without blocking the WPF interface
- **File sharing support** for uploading text and image files
- **Three-tier distributed architecture** separating the WPF client, business service, and data service

> **Note:** File sharing was partially implemented in the submitted version. File uploads and server-side storage are present, while parts of the file listing/download workflow remain incomplete.

## Architecture

The application uses a three-tier architecture communicating over **WCF `netTcpBinding`**:

- **Client** – WPF interface for authentication, lobby management, messaging, and file sharing.
- **BusinessTier** – WCF service exposing the application's lobby and messaging operations to clients.
- **DataTier** – WCF service maintaining users, lobbies, messages, private messages, and files in memory.

The client uses a **pull-based update strategy**, with background asynchronous tasks periodically retrieving lobby, message, and user state without blocking the UI.

## Technologies

- C#
- .NET Framework 4.7.2
- WPF
- Windows Communication Foundation (WCF)
- TCP (`netTcpBinding`)
- Asynchronous programming / `Task`
- Concurrent collections

## Running

Open `DC_Assignment.sln` in Visual Studio and start the components in the following order:

1. **DataTier** – hosts `net.tcp://localhost:8100/DataService`
2. **BusinessTier** – hosts `net.tcp://localhost:8200/BusinessService`
3. **Client** – launches the WPF application

Multiple Client instances can be launched to simulate multiple connected users.

All application state is stored in memory and is reset when the data server is stopped.
