var config = null;
var colorArray = ["#c0392b", "#e67e22", "#f1c40f", "#34495e", "#9b59b6", "#3498db", "#2ecc71", "#1abc9c"];
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
            window.localStorage.dk = window.localStorage.dk === undefined ? JSON.stringify({}) : window.localStorage.dk;
        },
        saveConfig: function () {
            var inputs = $("input");
            var configToSave = {};
            for (var i = 0; i < inputs.length; i++) {
                var inp = inputs[i];
                configToSave[inp.name] = inp.value;
            }
            $.get("http://" + configToSave.dk_rpi_ip + ":" + config.dk_rpi_port + "/r/connected").done(function() {
                $(".positive.message").removeClass("hidden");
                $(".negative.message").addClass("hidden");
            }).fail(function() {
                $(".negative.message").removeClass("hidden");
                $(".positive.message").addClass("hidden");
            });
            window.localStorage.dk = JSON.stringify(configToSave);
            $(".positive.message").removeClass("hidden");
        },
        getURL: function() {
            return "http://" + config.dk_rpi_ip + ":" + config.dk_rpi_port;
        }
    },
    pages: {
        global: function () {
            dk.utils.initStorage();
            config = JSON.parse(window.localStorage.dk);
            $("dkheader").replaceWith('<div class="ui top fixed menu" style="position: static !important;"><a class="item" href="index.html"><strong>Data</strong>Kit</a><a class="item" href="config.html">Configure</a><a class="item" href="devices.html">Devices</a></div>');
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
            //if(getQueryParams("devices") === null) window.location.href = "devices.html";

            //Assemble Chart
            dk.pageFunctions.data.init();

            var deviceList = getQueryParams("devices").split(",");
            var deviceName = getQueryParams("names").split(",");
            console.log(deviceList);
            $.each(deviceList, function(val) {
                var did = deviceList[val];
                $.post(dk.utils.getURL() + "/r/createchannel/" + did, function(channelID) {
                    var newObj = {
                        did: deviceList[val],
                        channel: channelID,
                        name: deviceName[val]
                    };
                    //$.post(dk.utils.getURL() + "/r/channel/" + channelID + "/start/");
                    newObj.ws = new WebSocket("ws" + dk.utils.getURL().substring(4) + "/ws");
                    newObj.ws.onopen = function() {
                        newObj.ws.send(">" + newObj.channel + "\n");
                    };
                    newObj.ws.onmessage = function(evt) {
                        evt.data = JSON.parse(evt.data);
                        evt.data = JSON.parse(evt.data)
                        realData = JSON.parse(evt.data);
                        if(dk.pageFunctions.data.dataNames.indexOf(newObj.name) <= -1) dk.pageFunctions.data.dataNames.push(newObj.name + "_" + realData.tag);
                        var datasetIdx = dk.pageFunctions.data.dataNames.indexOf(newObj.name + "_" + realData.tag);
                        var targSet = dk.pageFunctions.data.data.datasets[datasetIdx].data;
                        dk.pageFunctions.data.data.datasets[datasetIdx].label = newObj.name;
                        if(targSet.firstMark === undefined) targSet.firstMark = realData["timestamp"]
                        targSet.push({x: realData["timestamp"] - targSet.firstMark, y: realData["data"]});
                        if(targSet.length > dk.pageFunctions.data.config.maxPoints) delete targSet.shift();
                        setTimeout(dk.pageFunctions.data.update, 100);


                        //insertDataIntoChart(evt.data)
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
                var deviceName = [];
                $.each($(".checkbox"), function (device) {
                    device = $($(".checkbox")[device]);
                    if (device.checkbox('is checked')) {
                        deviceList.push(device.children().first().attr('name'));
                        deviceName.push(device.children().first().attr('device_name'));
                    }
                });
                var url = deviceList.join(',');
                var url2 = deviceName.join(',');
                window.location.href = "data.html?devices=" + url + "&names=" + url2;
            }
        },
        data: {
            deviceConnections: [],
            dataNames: [],
            config: {
                maxPoints: 20,
                scale: 100
            },
            chart: null,
            colorArray: ["#c0392b", "#e67e22", "#f1c40f", "#34495e", "#9b59b6", "#3498db", "#2ecc71", "#1abc9c"],
            init: function() {
                dk.pageFunctions.data.chart = new Chart(document.getElementById("myLineChart"), {
                    type: 'line',
                    data: dk.pageFunctions.data.data,
                    options: dk.pageFunctions.data.options
                });
            },
            update: function() {
                dk.pageFunctions.data.chart = new Chart(document.getElementById("myLineChart"), {
                    type: 'line',
                    data: dk.pageFunctions.data.data,
                    options: dk.pageFunctions.data.options
                });
            },
            options: {
                legend: {
                    display: true,
                    position: "right"
                },
                scales: {
                    xAxes: [{
                        type: 'linear',
                        position: 'bottom'
                    }]
                },
                animation: false
            },
            data:{
                datasets: [{
                    label: "",
                    fill: false,
                    borderColor: colorArray[0],
                    data: [
                    ]
                }, {
                    label: "",
                    fill: false,
                    borderColor: colorArray[5],
                    data: [
                    ]
                }, {
                    label: "",
                    fill: false,
                    borderColor: colorArray[5],
                    data: [
                    ]
                }, {
                    label: "",
                    fill: false,
                    borderColor: colorArray[5],
                    data: [
                    ]
                }]
            },
            start: function() {
                $("#dkstart").addClass("disabled");
                $("#dkstop").removeClass("disabled");
                var conns = dk.pageFunctions.data.deviceConnections;
                for(var i = 0; i < conns.length; i++) {
                    $.post(dk.utils.getURL() + "/r/channel/" + conns[i].channel + "/start/");
                }

            },
            stop: function() {
                $("#dkstart").removeClass("disabled");
                $("#dkstop").addClass("disabled");
                var conns = dk.pageFunctions.data.deviceConnections;
                for(var i = 0; i < conns.length; i++) {
                    $.post(dk.utils.getURL() + "/r/channel/" + conns[i].channel + "/stop/");
                }

            },
            updateSettings: function() {
                dk.pageFunctions.data.config.maxPoints = parseInt($("#dkdatapts").val());
            }
        }
    }
};


var onStart = function (conf) {
    /*config = JSON.parse(conf);
    console.log(config);*/
    var page = window.location.pathname.split("/").pop();
    page = page.slice(0, page.length - 5);
    dk.pages.global();
    try {
        dk.pages[page]();
    } catch (err) {
        console.error(err);
        console.warn("The '" + page + "' function may not have been defined in the dk page list");
    }
    //dk.pages[page]();
};

onStart();
