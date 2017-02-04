
# DataKit protocol

Specification version 0.1

connection is over tcp, language agnostic protocol

## Chronological explanation

### Connection setup

Connection setup consists of two parts: a HELLO packet and an ACK packet.
All data is followed by a UNIX NEWLINE! (`\n`)

#### The HELLO packet

The client sends a bunch of data containing metadata.

Format:

Each of these sections, separated by a `|` character;

1) `$H` - a basic hello packet identifier
1) a name (alphanumeric string)
1) a data collection type (see data collection types - NOTE: for now just use `stream`)

Example:

`$H|Super Sensor 1.5 someunits|stream` + NEWLINE

**followed by a UNIX newline (`\n`). The NEWLINE is the terminator of the message, and MUST be sent**

#### The ACK packet

The server receives the hello and tells the client: OK, i know you're here! you're now registered!

Format:

1) `ACK`
1) A UID (alphanumeric string)

Example:

`ACK` + NEWLINE

### Standby stage

At this point, the client and server have established a connection, and the client is waiting for a signal to start streaming data to the server.
