## Description
This is a 3-tier application, with a WPF application as the front-end and 2 console applications as the back-end.
The WPF app is the client tier, the 2 console apps are the business and data tiers.
These tiers communicate with each other using WCF services, allowing for distributed computing.

The WPF app uses multithreading to constantly update the list of users and the main chat.

Originally, I created the same app (with more features) as part of a group in a university course.
This project is recreating a smaller version of that app (for now) by myself.

If you read the 'Issues' section below, you will find that this app cannot even execute.
As an alternative, I can show you the app our group made during an interview, if required.

## Issues
When I attempt to run the 'WPFApp', I get the following error:

> System.Windows.Markup.XamlParseException: ''The invocation of the constructor on type 'WPFApp.MainWindow' that matches the specified binding constraints threw an exception.' Line number '6' and line position '9'.'
> 
> Inner Exception
> 
> BadImageFormatException: Could not load file or assembly 'C:\Users\[name]\source\repos\ChatApp\WPFApp\bin\Debug\net8.0-windows\BusinessTier.exe'. Format of the executable (.exe) or library (.dll) is invalid.

I will fix this issue at another time as I need to prioritise other things.
Again, I can show you the app my group made during an interview, if required.

## Functionality

- Allows a user to create a username and enter the main chat (if it is not taken)
- The user can then view the main chat and a list of other users using the app
- The user can send a message to the main chat
