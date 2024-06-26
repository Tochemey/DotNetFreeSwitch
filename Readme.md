# DotNetFreeSwitch

[![Stability: Maintenance](https://masterminds.github.io/stability/maintenance.svg)](https://masterminds.github.io/stability/maintenance.html)

_DotNetFreeSwitch up to the latest release is considered feature complete and mature. 
No future feature development is planned, though bugs and security issues are fixed._

[![GitHub Workflow Status (branch)](https://img.shields.io/github/actions/workflow/status/Tochemey/DotNetFreeSwitch/ci.yml?branch=main&style=flat-square)](https://github.com/Tochemey/ModFreeSwitch/actions/workflows/ci.yml)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=flat-square)](https://opensource.org/licenses/Apache-2.0)
[![Nuget](https://img.shields.io/nuget/v/DotNetFreeSwitch?style=flat-square)](https://www.nuget.org/packages/DotNetFreeSwitch/)

## Introduction

DotNetFreeSwitch is a socket library that helps interact with the FreeSwitch via its [mod_event_socket](https://freeswitch.org/confluence/display/FREESWITCH/mod_event_socket).
It has been written to run both on .Net8 .Net7 and .Net6 as well. The library is written using [DotNetty](https://github.com/Azure/DotNetty).

## Features

The library in its current state can be used to interact with FreeSwitch easily in:

* Inbound mode [Event Socket Inbound](https://freeswitch.org/confluence/display/FREESWITCH/mod_event_socket#mod_event_socket-Inbound)
* Outbound mode [Event Socket Outbound](https://wiki.freeswitch.org/wiki/Event_Socket_Outbound)
* One good thing it has is that you can implement your own FreeSwitch message encoder and decoder if you do not want to use the built-in ones.

## Installation

```bash
 dotnet add package DotNetFreeSwitch
```

## Example

kindly check the Demo project in this repo.

## Contribution policy

Contributions via GitHub pull requests are gladly accepted from their original author. Along with
any pull requests, please state that the contribution is your original work and that you license
the work to the project under the project's open source license. Whether or not you state this
explicitly, by submitting any copyrighted material via pull request, email, or other means you
agree to license the material under the project's open source license and warrant that you have the
legal authority to do so.
