
# Server REST API doc

The server exposes a REST API to allow it to be controlled remotely

All routes are relative to `server_address` with the `/r` prefix

## REST routes

### Connected devices

GET `/connected/`

Gets a JSON array with all the connected devices.
Each device has an `id` field. This is the device ID.

### Channel creation/deletion

POST `/createchannel/{deviceId}`

- `deviceId` - the ID of the device (obtained through the connected devices route)

Returns a UID that is the channel ID. This should be stored for all further operations
on the channel.

POST `/destroychannel/{channelId}`

- `channelId` - the channelid obtained from channel creation

Destroyes the channel associated with the id

### Data collection (requires a channel id)

`id` will refer to the channel ID

POST `/channel/{id}/start`

Tells the server to start receiving data on that channel. It is stored on the server and can be obtained later.

POST `/channel/{id}/stop`

Tells the server to stop receiving data on that channel.

GET `/channel/{id}/getdata`

Receives all the data currently on the channel. This is an array
of JSON objects, in the form:

```js
[
    {
        "tag": "some_tag",
        "timestamp": 1459309227,
        "data": 3.141593
    },
    // ... etc ...
]
```
