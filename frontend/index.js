var config = null;

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
        }
    },
    pages: {
        global: function() {
          config = JSON.parse(window.localStorage.dk); 
            dk.utils.initStorage();
            $("dkheader").replaceWith('<div class="ui top fixed menu" style="position: static !important;"><a class="item" href="index.html"><strong>Data</strong>Kit</a><a class="item" href="devices.html">Devices</a><a class="item" href="config.html">Configure</a></div>');
        },
        devices: function() {

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
        console.warn("The '" + page + "' function has not been defined in the dk page list");
    }
};

$.get("config.json", configLoaded);
