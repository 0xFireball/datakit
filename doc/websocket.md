
# Websocket protocol

Specification version 0.1

## Chronological explanation

### Setup

After initiating the websocket connection, the user sends
a message to "connect" to a channel, consisting of a `'>'`
character immediately followed by the channel ID to subscribe
to (and with a newline after the command, of course). Example:


`>aa6b2ec9a9294724be0a42e617fc86ce` + NEWLINE

### After that

The server will then automatically forward every new data point
to the user over the websocket, in the same format as the sensor
sent it. Example:

`>16|1337.1337` + NEWLINE