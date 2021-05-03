.NET FreeSwitch Library
========================================
[![build](https://github.com/Tochemey/ModFreeSwitch/actions/workflows/ci.yml/badge.svg)](https://github.com/Tochemey/ModFreeSwitch/actions/workflows/ci.yml)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)


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

This code is open source software licensed under the [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0.txt)

## **Installation**
For the meantime there is no Nuget package available for it. Just clone it and build it and you are ready to go.

## **Example**

kindly check the Demo project in this repo

## Contribution policy ##

Contributions via GitHub pull requests are gladly accepted from their original author. Along with
any pull requests, please state that the contribution is your original work and that you license
the work to the project under the project's open source license. Whether or not you state this
explicitly, by submitting any copyrighted material via pull request, email, or other means you
agree to license the material under the project's open source license and warrant that you have the
legal authority to do so.
