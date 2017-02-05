var config = null;

var getQueryParams = function (name, url) {
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
        initStorage: function () {
            console.log("init");
            window.localStorage.dk = window.localStorage.dk === undefined ? JSON.stringify({}) : window.localStorage.dk;
        },
        saveConfig: function () {
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
        global: function () {
            dk.utils.initStorage();
            config = JSON.parse(window.localStorage.dk);
            $("dkheader").replaceWith('<div class="ui top fixed menu" style="position: static !important;"><a class="item" href="index.html"><strong>Data</strong>Kit</a><a class="item" href="devices.html">Devices</a><a class="item" href="config.html">Configure</a></div>');
        },
        devices: function () {
            $.get("http://" + config.dk_rpi_ip + ":" + config.dk_rpi_port + "/r/connected", function (data) {
                data.forEach(function (device) {
                    $("#deviceList").append("<tr><td>" + device.name + "</td><td>" + device.units + "</td><td class='ui checkbox'><input type='checkbox' name='" + device.id + "' device_name='" + device.name  + "'><label>Include</label></td></tr>");
                });
                $(".checkbox").checkbox();
            });
        },
        data: function() {
            //Check if devices
            //if(getQueryParams("devices") == null) window.location.href = "devices.html";
            var deviceList = getQueryParams("devices").split(",");
            console.log(deviceList);
            $.each(deviceList, function(val) {
                var did = deviceList[val];
                $.post(dk.utils.getURL() + "/r/createchannel/" + did, function(channelID) {
                    var newObj = {
                        did: deviceList[val],
                        channel: channelID
                    };
                    $.post(dk.utils.getURL() + "/r/channel/" + channelID + "/start/");
                    newObj.ws = new WebSocket("ws" + dk.utils.getURL().substring(4) + "/ws");
                    newObj.ws.onopen = function() {
                        newObj.ws.send(">" + newObj.channel + "\n");
                    };
                    newObj.ws.onmessage = function(evt) {
                        console.log(newObj.channel);
                        console.log(evt.data);
                    };
                    dk.pageFunctions.data.deviceConnections.push(newObj);
                });
            });
        }
    },
    pageFunctions: {
        devices: {
            clearDevices: function () {

            },
            loadDevices: function () {
                var deviceList = [];
                $.each($(".checkbox"), function (device) {
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


var onStart = function (conf) {
    /*config = JSON.parse(conf);
    console.log(config);*/
    var page = window.location.pathname.split("/").pop();
    page = page.slice(0, page.length - 5);
    dk.pages.global();
    /*try {
        dk.pages[page]();
    } catch (err) {
        console.error(err);
        console.warn("The '" + page + "' function may not have been defined in the dk page list");
    }*/
    dk.pages[page]();
};

onStart();
