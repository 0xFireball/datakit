var config = null;

var dk = {
	initStorage: function() {

	},
	saveConfig: function() {
		var inputs = $("input");
		var configToSave = {};
		for(var i = 0; i < inputs.length; i++) {
			var inp = inputs[i];
			configToSave[inp.name] = inp.value;
		}
		Window.localStorage.dk.config = configToSave;
	}
};

var dkheader = Vue.component('dkheader', {
	template: '<div class="ui top fixed menu" style="position: static !important;"><a class="item" href="index.html"><strong>Data</strong>Kit</a><a class="item" href="devices.html">Devices</a><a class="item" href="config.html">Configure</a></div>'
});

var dkdevice = Vue.component('dkdevice', {
	template: '<tr><td>{{device.id}}</td><td>{{device.name}}<td>Connect</td></tr>'
});

new Vue({
	el: "#dkbody"
});

var configLoaded = function(conf) {
	config = JSON.parse(conf);
	console.log(config);
};

$.get("config.json", configLoaded);
