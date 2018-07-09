# NetLib
NetLib is a c\# network library without any external dependencies, built with .NET 4.7.1  
It makes the long and hard process to create a network protocol a bit easier.  
### Usage
Although the project is aimed at custom protocols, it is helpful when integrating with already existing protocols.  
Classes such as `ByteIntegrity`, `Crypt`, `Compression`, `Base64`, have all the functions a protocol needs.  
There are 2 Clients a `TcpClient` and an `SSLClient`.  
With servers you can choose between ssl and non-ssl servers as well as single or multi client handling servers.  
Multi client handling servers have a unique client ID system, so you can focus on the messages of a client.  
ByteIntegrity turns Tcp from a stream into a message protocol by sending the length of the message as a prefix, handling the message construction in the background.  
### Events
NetLib is almost fully event based, you can subscribe to events such as ClientConnected, BytesReceived, LineReceived, ClientDisconnected.  
Also based on your needs simple direct read/write methods are available, NOTE: they block the execution on the calling thread until a message is received.  
We also support async read calls, so your form based application can run without freezing.  
### Summary
In short NetLib is an awesome protocol building library, highliy customizable and easy to control.  
Just clone the repo, open it with Visual Studio, build the project (CTRL + SHIFT + B) to get a .dll file you can import to other projects.  
For information about contribution, please read the CONTRIBUTING.md file.  

Happy protocol design!