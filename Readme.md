DotNetFreeSwitch
========================================

[![build](https://github.com/Tochemey/ModFreeSwitch/actions/workflows/ci.yml/badge.svg)](https://github.com/Tochemey/ModFreeSwitch/actions/workflows/ci.yml)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)


## **Introduction**

DotNetFreeSwitch is a socket library that helps interact with the FreeSwitch via its [mod_event_socket](https://freeswitch.org/confluence/display/FREESWITCH/mod_event_socket). 
It has been written to run both on dotnet core 3.1 and 5.0 as well. The library is written using [DotNetty](https://github.com/Azure/DotNetty).

## **Features**

The library in its current state can be used to interact with FreeSwitch easily in:
* Inbound mode [Event Socket Inbound](https://freeswitch.org/confluence/display/FREESWITCH/mod_event_socket#mod_event_socket-Inbound)
* Outbound mode [Event Socket Outbound](https://wiki.freeswitch.org/wiki/Event_Socket_Outbound)
* One good thing it has is that you can implement your own FreeSwitch message encoder and decoder if you do not want to use the built-in ones


## **Installation**

```
 PM> Install-Package DotNetFreeSwitch
```

## **Example**

kindly check the Demo project in this repo.

## Contribution policy ##

Contributions via GitHub pull requests are gladly accepted from their original author. Along with
any pull requests, please state that the contribution is your original work and that you license
the work to the project under the project's open source license. Whether or not you state this
explicitly, by submitting any copyrighted material via pull request, email, or other means you
agree to license the material under the project's open source license and warrant that you have the
legal authority to do so.
