# OxHack.SignInKiosk
A fob-based sign-in system for Oxford Hackspace.

## Primary Components

### Message Broker
This component isn't actually part of the source code, however it is critical to the operation of the system.  All components communicated with one another through a central RabbitMQ Message Broker.

### OxHack.SignInKiosk.FobReaderService
A stand-alone service responsible for reading fob UIDs and relaying them as `FobRead` messages to the Message Broker.

This currently expects to be hosted on a Raspberry Pi and to be interacting with a IB Technologies RWD-HiTag2 chip.

### OxHack.SignInKiosk.Windows
The UI for the sign-in system.  This component subscribes to `FobRead` messages and starts one of three workflows with the interactive user:

 1. **User registration:** If fob is unrecognized, begin user registration workflow.  Once completed, the **Token-based Sign-In** workflow is started.
 2. **Token-based Sign-In:** When a known fob is read and the associated user is not already signed-in, a `SignInRequestSubmitted` message is published to the Message Broker.  A `PersonSignedIn` message is eventually returned and the user is shown a friendly "signed-in" message.
 3. **Token-based Sign-Out:** When a known fob is read and the associated user is already signed-in, a `SignOutRequestSubmitted` message is published to the Message Broker.  A `PersonSignedOut` message is eventually returned and the user is shown a friendly "signed-out" message.

Additionally, two manual workflows are provided for users to sign-in and -out without fobs.

### OxHack.SignInKiosk.CoreService
A stand-alone service responsible for processing `SignInRequestSubmitted` and `SignOutRequestSubmitted` messages.

This service is responsible for managing the overall system's persisted state (ie, who is currently signed in).  It also records an audit trail of sign-in and sign-out actions for future analysis.

### OxHack.SignInKiosk.PrinterService
A stand-alone service responsible for printing out all `PersonSignedIn` and `PersonSignedOut` messages to paper.

This service is optional, however it is useful if fire regulations require a paper copy of who is currently in the hackspace.  I still need to look into whether this is a legal requirement.

### OxHack.SignInKiosk.Web
A basic REST Api for querying the system state.

## Secondary Components

### OxHack.SignInKiosk.MessageBrokerProxyService
Unfortunately, the only bit of hardware I have access to for hosting the UI is fairly old.  As a result, its support for interacting with the message broker is limit.

Instead of recoding a RabbitMQ/MassTransit library for WinRT I decided to write this service instead.

This is a WCF service (supported by WinRT) that acts like a proxy for the Message Broker.  That's it  Hopefully I'll be able to get rid of it later.

### OxHack.SignInKiosk.Database
This one is pretty self explanatory.  This component is responsible for interactions with the system's database.  It uses EF Core internally but exposes the data to the rest of the application as Services.

### OxHack.SignInKiosk.Messaging
This component encapsulates all the messaging logic.  Subcription and Publishing logic for interacting with the Message Broker is implemented here.  Consumers

These functionalities are exposed to the rest of the application by the `MessagingClient` class.

### OxHack.SignInKiosk.Domain
A shared project containing Domain Entities, Message classes, and DTOs.
