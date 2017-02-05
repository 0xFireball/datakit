var config = null;

var getQueryParams = function(name, url) {
    if (!url) {
        url = window.location.href;
    }

    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
};

var dk = {
    utils: {
        initStorage: function() {
            window.localStorage.dk = window.localStorage.dk === undefined ? {} : window.localStorage.dk;
        },
        saveConfig: function() {
            var inputs = $("input");
            var configToSave = {};
            for (var i = 0; i < inputs.length; i++) {
                var inp = inputs[i];
                configToSave[inp.name] = inp.value;
            }
            window.localStorage.dk = JSON.stringify(configToSave);
        },
        getURL: function() {
            return "http://" + config.dk_rpi_ip + ":" + config.dk_rpi_port;
        }
    },
    pages: {
        global: function() {
            config = JSON.parse(window.localStorage.dk);
            dk.utils.initStorage();
            $("dkheader").replaceWith('<div class="ui top fixed menu" style="position: static !important;"><a class="item" href="index.html"><strong>Data</strong>Kit</a><a class="item" href="devices.html">Devices</a><a class="item" href="config.html">Configure</a></div>');
        },
        devices: function() {
            $.get("http://" + config.dk_rpi_ip + ":" + config.dk_rpi_port + "/r/connected", function(data) {
                data.forEach(function(device) {
                    $("#deviceList").append("<tr><td>" + device.name + "</td><td>" + device.units + "</td><td class='ui checkbox'><input type='checkbox' name='" + device.id + "'><label>Include</label></td></tr>");
                });
                $(".checkbox").checkbox();
            });
        },
        data: function() {
            //Check if devices
            if(getQueryParams("devices") == null) window.location.href = "devices.html"
            var deviceList = getQueryParams("devices").indexOf(",") > -1 ? getQueryParams("devices").split(",") : getQueryParams("devices");
            $.each(deviceList, function(val) {
                var did = deviceList[val];
                $.post(dk.utils.getURL() + "/r/createchannel/" + did, function(channelID) {
                    var newObj = {
                        did: deviceList[val],
                        channel: channelID
                    };
                    newObj.ws = new WebSocket("ws" + dk.utils.getURL().substring(4) + "/ws");
                    newObj.ws.onopen = function() {
                        console.log("Opening: " + newObj.channel);
                        newObj.ws.send(">" + newObj.channel + "\n");
                    };
                    newObj.ws.onmessage = function(evt) {
                        console.log(evt.data);
                    };
                    dk.pageFunctions.data.deviceConnections.push(newObj);
                });
            });
        }
    },
    pageFunctions: {
        devices: {
            clearDevices: function() {

            },
            loadDevices: function() {
                var deviceList = [];
                $.each($(".checkbox"), function(device) {
                    device = $($(".checkbox")[device]);
                    if (device.checkbox('is checked')) {
                        deviceList.push(device.children().first().attr('name'));
                    }
                });
                var url = deviceList.join(',');
                window.location.href = "data.html?devices=" + url;
            }
        },
        data: {
            deviceConnections: []
        }
    }
};


var configLoaded = function(conf) {
    config = JSON.parse(conf);
    console.log(config);
    var page = window.location.pathname.split("/").pop();
    page = page.slice(0, page.length - 5);
    dk.pages.global();
    try {
        dk.pages[page]();
    } catch (err) {
        console.error(err);
        console.warn("The '" + page + "' function may not have been defined in the dk page list");
    }
};

var ws = new WebSocket("ws://localhost:5000/ws");

ws.onopen = function() {
    // Web Socket is connected, send data using send()
    console.log("sending");
    ws.send(">1f45df1250ca41f8a08eb6e9f71bda90c\n");
};

ws.onmessage = function(evt) {
    var received_msg = evt.data;
    console.log(received_msg);
};

ws.onclose = function() {
    // websocket is closed.
};

$.get("config.json", configLoaded);
