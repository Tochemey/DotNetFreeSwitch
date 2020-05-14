.NET FreeSwitch Library
========================================
[![Build status](https://ci.appveyor.com/api/projects/status/gtd0537ge5jtfmxr?svg=true)](https://ci.appveyor.com/project/Tochemey/modfreeswitch)


## **Overview**
This library helps interact with the FreeSwitch via its mod_event_socket. For more information about the mod_event_socket refer to [FreeSwitch web site](https://freeswitch.org/confluence/display/FREESWITCH/mod_event_socket). 
The framework is written using [DotNetty](https://github.com/Azure/DotNetty).
In its current state it can help build IVR applications more quickly. 

## **Features**
The library in its current state can be used to interact with FreeSwitch easily in:
* Inbound mode [Event Socket Inbound](https://freeswitch.org/confluence/display/FREESWITCH/mod_event_socket#mod_event_socket-Inbound)
* Outbound mode [Event Socket Outbound](https://wiki.freeswitch.org/wiki/Event_Socket_Outbound)
* One good thing it has is that you can implement your own FreeSwitch message encoder and decoder if you do not want to use the built-in ones

## **License**
[Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0.txt)

## **Installation**
For the meantime there is no Nuget package available for it. Just clone it and build it and you are ready to go.

## **Example**

kindly check the Demo project in this repo
