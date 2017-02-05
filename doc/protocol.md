
# DataKit protocol

Specification version 0.1

connection is over tcp, language agnostic protocol

## Chronological explanation

### Connection setup

Connection setup consists of two parts: a HELLO packet and an ACK packet.
The client sends the server a HELLO, and the server will respond with an ACK indicating that the connection is now valid.
All data is followed by a UNIX NEWLINE! (`\n`)

#### The HELLO packet

The client sends a bunch of data containing metadata.

Format:

Each of these sections, separated by a `|` character;

1. `$H` - a basic hello packet identifier
1. a name (alphanumeric string)
1. units
1. data type (we don't know why yet but it will probably be useful)
1. a data collection type (see data collection types - NOTE: for now just use `stream`)

Example:

`$H|Super Sensor 1.5 someunits|meters_per_second|lol|stream` + NEWLINE

**followed by a UNIX newline (`\n`). The NEWLINE is the terminator of the message, and MUST be sent**

#### The ACK packet

The server receives the hello and tells the client: OK, i know you're here! you're now registered!

Format:

1. `ACK`
1. A UID (alphanumeric string)

Example:

`ACK|asdjfalskdjfajsrhuip3wtr87q23tg` + NEWLINE

### Standby stage

At this point, the client and server have established a connection, and the client is waiting for a signal to start streaming data to the server.

The client has two states now, SENDING and STANDBY.
Initially, it is in STANDBY mode. When it receives a START it switches to SENDING mode until it receives a STOP.

Clients need to send a heartbeat packet at least every 10 seconds or they will be UNREGISTERED!

#### The HEARTBEAT packet

Example:

`$P`

#### The START packet

Sets the client to SENDING mode when the client receives this

Example:

`START`

#### The STOP packet

Example:

Sets the client to STANDBY mode when the client receives this

`STOP`

### Data Transfer stage

When in SENDING mode, the client runs a loop like this (pseudocode):

```pseudo
while SENDING:
    GET DATA
    SEND DATA OVER TCP
    WAIT TIME
```

#### The DATA packet

Format:

1. `>` character
1. _the following, delimited by_ `|` _chars:_
   1. Tag (string)
   1. UNIX timestamp
   1. The data value (float)

Example:

`>some_tag|1459309227|3.141593` + NEWLINE
