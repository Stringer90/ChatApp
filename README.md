## Description
This is a 3-tier application, with a WPF application as the front-end and 2 console applications as the back-end.
The WPF app is the client tier, the 2 console apps are the business and data tiers.
These tiers communicate with each other using WCF services, allowing for distributed computing.

The WPF app uses asynchronous programming and multithreading to constantly update the list of users and the main chat.

Originally, I created the same app (with more features) as part of a group in a university course.
This project is recreating a smaller version of that app (for now) by myself.

## Functionality

- Allows a user to create a username and enter the main chat (if it is not taken)
- The user can then view the main chat and a list of other users using the app
- The user can send a message to the main chat

## Note

I had issues with the 'WPFApp' project. It has been replaced with 'WPFClient' (seen as 'Client' in Visual Studio 2022).
