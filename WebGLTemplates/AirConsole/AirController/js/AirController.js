// Main class
var AirController = function AirController() {
    this.fps = 9;
    // contains a list of all pages
    this.pages = {};
    this.currentPage = null;
    this.timer = null;
    //this.airconsole = null;
    this.sender = null;
    this.enableHero = false;
    this.orientation ;
    this.vibrate;

    // call this to start running the controller
    this.init = function init(startPage, orientation, vibrate) {
        this.orientation = orientation;
        this.vibrate = vibrate;

        // find all the pages
        this.findPages();
        // show the first pages
        this.showPage(startPage);
        // init airconsole
        this.sender = new Sender();
        this.sender.init(this.orientation);
    }

    this.findPages = function findPages() {
        t_pages = document.querySelectorAll("[air-page]");
        for (var i = 0; i < t_pages.length; i++) {
            this.addPage(t_pages[i].id);
        }
    }

    this.sendData = function sendData (button) {
        if (this.currentPage == null) return;
        var t_input = this.getInput();

        if (Object.keys(t_input).length > 0 ) {
            if(button){
                this.sender.sendNow(t_input);
            }
            else {
                this.sender.setData(t_input);
            }
        }
    }

    // call this to stop running the controller
    this.stop = function stop() {
        clearTimeout(this.timer);
    }

    this.getInput = function getInput() {
        return this.currentPage.getInput();
    }

    // registers a page
    this.addPage = function addPage(elementId) {

        // create the page
        var page = new Page(elementId, this);
        // save the page
        this.pages[elementId] = page;
        // hide it :)
        document.getElementById(elementId).style.display = 'none';
        return page;
    }

    this.findPageButtons = function findPageButtons (page) {
        // find buttons
        page.findButtons();
        // find the joysticks
        page.findJoysticks();
    }

    this.getPage = function getPage(elementId) {
        return this.pages[elementId];
    }

    // call this to show a certain page
    this.showPage = function showPage(elementId) {
        //("showpage: " + elementId);
        // unregister old page
        if (this.currentPage != null) {
            if(elementId == this.currentPage.elementId) return;
            this.currentPage.hide();
        }
        // register new page
        this.currentPage = this.pages[elementId];

        if(!this.currentPage.isLoaded){
            this.currentPage.isLoaded = true;
            this.findPageButtons(this.currentPage);
        }

        this.pages[elementId].show();
    }


}

// page class
var Page = function Page(elementId, parent) {
    // the id of the element
    this.elementId = elementId;
    this.input = {};
    this.parent = parent;
    this.isLoaded = false;

    // contains a list of all buttons
    this.buttons = {};
    this.joysticks = {};

    this.findButtons = function findButtons() {
        var t_buttons = document.getElementById(this.elementId).querySelectorAll("[air-btn]");
        for (var i = 0; i < t_buttons.length; i++) {
            console.log(t_buttons[i].id);
            this.addButton(t_buttons[i].id, t_buttons[i].getAttribute("air-btn"), t_buttons[i].getAttribute("air-hero"));
        }
    }

    // registers a button to this page
    this.addButton = function addButton(elementId, key, hero) {
        if(hero){
            console.log("found hero button: " + elementId);
        }

        var button = new Button(this, elementId, key, hero);
        this.buttons[elementId] = button;
        return button;
    }

    this.getButton = function getButton(elementId) {
        return this.buttons[elementId];
    }

    this.findJoysticks = function findJoysticks() {
        var t_joysticks = document.getElementById(this.elementId).querySelectorAll("[air-joystick]");

        for (var i = 0; i < t_joysticks.length; i++) {
            this.addJoystick(t_joysticks[i].id, t_joysticks[i].getAttribute("air-joystick"));
        }
    }

    this.addJoystick = function addJoystick(elementId, key) {
        var joystick = new Joystick(this, elementId, key);
        joystick.init();

        this.joysticks[elementId] = joystick;
        return joystick;
    }

    this.getInput = function getInput() {
        var curInput = this.input;

        for (var key in this.joysticks) {
            if(this.joysticks[key].diff > 5 ){
                var input = this.joysticks[key].input;
                this.joysticks[key].lastSend = this.joysticks[key].input;

                var tJoystick = {
                    type: "vector",
                    value: {
                        x: input.x,
                        y: input.y
                    }
                };
                curInput[this.joysticks[key].key] = tJoystick;
            }
        }

        this.input = {};
        return curInput;
    }

    this.register = function register() {
        for (key in this.buttons) {
            this.buttons[key].register();
        }
        for (key in this.joysticks) {
            this.joysticks[key].register();
        }
    }

    this.unregister = function unregister() {
        for (key in this.buttons) {
            this.buttons[key].unregister();
        }
        for (key in this.joysticks) {
            this.joysticks[key].unregister();
        }
    }

    this.show = function show() {
        // actually hide the page
        document.getElementById(this.elementId).style.display = 'block';
        // unregister events
        this.register();
    }

    this.hide = function hide() {
        // actually show the page
        document.getElementById(this.elementId).style.display = 'none';
        // register events
        this.unregister();
    }

    // a callback for this page
    this.eventCallback = function eventCallback(elementId, e) {
        this.input[this.buttons[elementId].key] = {
            type: "button",
            value: this.buttons[elementId].value
        };
        this.parent.sendData(true);
    }
}

// button class
var Button = function Button(page, elementId, key, hero) {
    this.elementId = elementId;
    this.eventType = "click";
    this.page = page;
    this.key = key;
    this.value = 0;
    this.callback = null;
    this.hero = hero;

    this.bind = function bind(page) {
        this.page = page;
    }

    this.addCallback = function addCallback(callback) {
        this.callback = callback;
        return this;
    }

    this.register = function register() {
        //console.log("register: " + this.key);
        if (isEventSupported('touchstart')) {
            this.eventType = "touchstart";
        }
        else {
            this.eventType = "click";
        }

        // split value from key
        var parts = this.key.split(":");
        if(parts.length > 1){
            this.key = parts[0];
            this.value = parts[1];
        }

        document.getElementById(this.elementId).addEventListener(this.eventType, this.eventHandler.bind(this), false);
    }

    this.unregister = function unregister() {
        document.getElementById(this.elementId).removeEventListener(this.eventType, this.eventHandler.bind(this));
    }

    this.eventHandler = function eventHandler(e) {
        console.log("hero: "+this.hero);
        if(this.hero == "true" && controller.enableHero == false){
            console.log("request hero");
            controller.sender.airconsole.getPremium();
            return;
        }

        if ("vibrate" in navigator) {
            navigator.vibrate = navigator.vibrate || navigator.webkitVibrate || navigator.mozVibrate || navigator.msVibrate;

            if(AirController.vibrate){
                window.navigator.vibrate(50);
            }
        }

        // send this event to the page
        this.page.eventCallback(this.elementId, e);
        // call the callback if there is one
        if (this.callback != null) this.callback();
    }
}

var Joystick = function Joytstick(page, elementId, key) {
    this.elementId = elementId;
    this.page = page;
    this.key = key;
    this.joytsick = null;
    this.input = new Victor(0, 0);
    this.lastSend = new Victor(0, 0);
    this.active = false;
    this.diff = 0;

    this.init = function init () {
        //console.log("register: " + this.key);
        this.joystick = new VirtualJoystick({
            container: document.getElementById(this.elementId),
            mouseSupport: true,
            limitStickTravel: true,
            stickRadius: 50
        });

        window.setInterval((function (self) {         //Self-executing func which takes 'this' as self
            return function () {   //Return a function in the context of 'self'
                self.run(); //Thing you wanted to run as non-window 'this'
            }
        })(this), 1 / 30 * 1000);
    }

    this.register = function register() {
        this.active = true;
    }

    this.unregister = function unregister () {
        this.active = false;
    }

    this.run = function run () {
        if(!this.active) return;

        this.input = new Victor(this.joystick.deltaX(), this.joystick.deltaY());
        this.diff = this.angleBetween(this.input, this.lastSend);

        // sudden change
        if(this.diff > 45 || this.diff < -45){
            controller.sendData(true);
        }
    }

    this.angleBetween = function angleBetween (vector1, vector2) {
        var rad = Math.atan2(vector2.y, vector2.x) - Math.atan2(vector1.y, vector1.x);
        var angle = rad  * 180 / Math.PI;

        if(angle > 180){
            angle -= Math.ceil(angle / 180) * 180;
        }
        else if (angle < -180) {
            angle += Math.ceil((angle *-1) / 180) * 180;
        }

        if (angle < 0) angle *= -1;

        return angle;
    }
}

// Sender class
var Sender = function Sender() {
    this.rtcFpsCap = 12;
    this.rtcFps = 8;

    this.fpsCap = 9;
    this.fps = 7;

    this.data = {};
    this.lastData = {};
    this.interval = 0;
    this.lastSendTimestamp = 0;
    this.sendImmediate = 0;
    this.airconsole = null;

    this.webRtcTimeout = 10;
    this.webRtcTimestamp = 0;

    this.init = function init (orientation) {
        this.airconsole = new AirConsole({orientation: orientation});

        this.airconsole.onDeviceStateChange = function (deviceId, data) {
            var customIsDeclared = true;
            try{ data.custom; }
            catch(e) {
                customIsDeclared = false;
            }

            if (!customIsDeclared || typeof data.custom === 'undefined' || data.custom === null) {
                //console.log("data.custom empty");
            }
            else {
                try {
                    var obj = JSON.parse(data.custom);
                    //console.log(this.device_id);
                    if(obj[this.device_id]){
                        controller.enableHero = obj[this.device_id].enablehero;
                        controller.showPage(obj[this.device_id].view)
                        document.body.className = "P" + (obj[this.device_id].playerId + 1) + " " + obj[this.device_id].class + " " + obj[this.device_id].color;
                        //console.log("whoop whoop");
                    }
                }
                catch(e) {
                    console.log("Error handle here!");
                    console.error("parsing data: " + data.custom);
                    console.error(e);
                }
            }
        }

        this.interval = 1 / this.fps * 1050;

        if (!Date.now) {
            Date.now = function() { return new Date().getTime(); }
        }

        this.lastSendTimestamp = Date.now();
        this.webRtcTimestamp = Date.now();

        // this runs the code at 60 fps
        window.setInterval((function (self) {         //Self-executing func which takes 'this' as self
            return function () {   //Return a function in the context of 'self'
                self.run(); //Thing you wanted to run as non-window 'this'
            }
        })(this), 1 / 60 * 1000);
    }

    this.run = function run () {
        var curTimestamp = Date.now();
        // return true if the last send item is longer than this.interval ago
        if(curTimestamp - this.lastSendTimestamp > this.interval){

            this.send();
        }

        if(curTimestamp - this.webRtcTimestamp > 2000 && this.webRtcTimeout > 0){
            this.webRtcTimeout--;
            this.webRtcTimestamp = Date.now();
            if(this.airconsole.devices[this.airconsole.getDeviceId()] != null && this.airconsole.devices[this.airconsole.getDeviceId()].rtc == 2){
                console.log("Using webRTC");

                this.webRtcTimeout = 0;
                this.fpsCap = this.rtcFpsCap;
                this.fps = this.rtcFps;

                this.interval = 1 / this.fps * 1050;
            }
            else {
                console.log("no webRTC: " + this.webRtcTimeout);
            }
        }
    }

    this.setData = function setData (data) {
        for (var key in data) {
            this.data[key] = data[key];
        }
    }

    this.send = function send () {

        this.lastSendTimestamp = Date.now();

        if (Object.keys(this.data).length > 0 ) {
            this.airconsole.message(0, this.data);
            console.log(this.data);
            this.lastData = this.data;
            this.data = {};
        }
        else {
            //console.log("no data to send");
        }
    }

    this.sendNow = function sendNow (data) {
        this.setData(data);

        // there's room to send data
        if(this.sendImmediate + this.fps < this.fpsCap){
            // up the count
            this.sendImmediate++;

            // make sure it deques after a second
            window.setInterval((function (self) {         //Self-executing func which takes 'this' as self
                return function () {   //Return a function in the context of 'self'
                    self.deque(); //Thing you wanted to run as non-window 'this'
                }
            })(this), 1000);

            // send the data
            this.send();
        }
        else {
            //console.log("limit reached, queuing");
        }
    }

    this.deque = function deque () {
        this.sendImmediate--;
    }
}

var isEventSupported = (function(){
    var TAGNAMES = {
        'select':'input','change':'input',
        'submit':'form','reset':'form',
        'error':'img','load':'img','abort':'img'
    }
    function isEventSupported(eventName) {
        var el = document.createElement(TAGNAMES[eventName] || 'div');
        eventName = 'on' + eventName;
        var isSupported = (eventName in el);
        if (!isSupported) {
            el.setAttribute(eventName, 'return;');
            isSupported = typeof el[eventName] == 'function';
        }
        el = null;
        return isSupported;
    }
    return isEventSupported;
})();
