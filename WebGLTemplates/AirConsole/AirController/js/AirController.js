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
    this.vibrate = true;
    this.customData = {};
    this.gyroscopeData = {};

    this.onData = function (customData) {};
    this.onShowPage = function (newPage) {};
    this.onBecameHero = function () {};

    this.swipeEvent;
    this.panEvent;

    // call this to start running the controller
    this.init = function init(startPage, orientation, vibrate) {
        this.orientation = orientation;
        this.vibrate = vibrate;

        // init airconsole
        this.sender = new Sender();
        this.sender.init(this.orientation);

        this.sender.airconsole.onReady = function (code) {
            var t_buttons = document.querySelectorAll("[air-profile-picture]");
            for (var i = 0; i < t_buttons.length; i++) {
                var img = this.getProfilePicture(this.getDeviceId(), 512);
                document.getElementById(t_buttons[i].id).style.backgroundImage = "url('" + img +"')";
            }
        }

        // find all the pages
        this.findPages();
        // search for settings
        this.findPageSettings();
        // show the first pages
        this.showPage(startPage);
    }

    this.findPages = function findPages() {
        var t_pages = document.querySelectorAll("[air-page]");
        for (var i = 0; i < t_pages.length; i++) {
            this.addPage(t_pages[i].id);
        }
    }

    this.findPageSettings = function findPageSettings() {
        // look for gyroscope
        var t_page = document.querySelectorAll("[air-gyroscope]");
        for (var i = 0; i < t_page.length; i++) {
            this.pages[t_page[i].id].sendGyroscope = true;
        }

        // look for pan
        var pan = false;
        var t_page = document.querySelectorAll("[air-pan]");
        for (var i = 0; i < t_page.length; i++) {
            this.pages[t_page[i].id].sendPanEvents = true;
            pan = true;
        }

        // look for swipe
        var swipe = false;
        var t_page = document.querySelectorAll("[air-swipe]");
        for (var i = 0; i < t_page.length; i++) {
            this.pages[t_page[i].id].sendSwipeEvents = true;
            swipe = true;
        }

        if(pan || swipe){
            this.startHammer(pan, swipe);
        }
    }

    this.startHammer = function startHammer (pan, swipe) {
        console.log("hammer time");

        var mc = new Hammer(document);

        if(swipe){
            mc.get('swipe').set({ direction: Hammer.DIRECTION_ALL });
            mc.on("swipeleft swiperight swipeup swipedown", function(e) {
                console.log(e);
                controller.swipeEvent = {
                    type: "swipe",
                    value: e.type
                };
                controller.sendData(true);
            });
        }

        if(pan){
            mc.get('pan').set({ direction: Hammer.DIRECTION_ALL });
            mc.on("panstart panmove panend", function(e) {
                console.log(e);
                controller.panEvent = {
                    type: "pan",
                    value: {
                        vector: e.center,
                        end: e.type == "panend"
                    }
                };
                controller.sendData(e.type == "panend" || e.type == "panstart");
            });
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
    }

    this.getPage = function getPage(elementId) {
        return this.pages[elementId];
    }

    // call this to show a certain page
    this.showPage = function showPage(elementId) {
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

        this.onShowPage(this.pages[elementId]);
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

    this.sendGyroscope = false;
    this.sendSwipeEvents = false;
    this.sendPanEvents = false;

    this.findButtons = function findButtons() {
        var t_buttons = document.getElementById(this.elementId).querySelectorAll("[air-tap-btn]");
        for (var i = 0; i < t_buttons.length; i++) {
            this.addButton(t_buttons[i].id, t_buttons[i].getAttribute("air-tap-btn"), 'tap', t_buttons[i].getAttribute("air-hero"));
        }

        var t_buttons = document.getElementById(this.elementId).querySelectorAll("[air-hold-btn]");
        for (var i = 0; i < t_buttons.length; i++) {
            this.addButton(t_buttons[i].id, t_buttons[i].getAttribute("air-hold-btn"), 'hold', t_buttons[i].getAttribute("air-hero"));
        }
    }

    // registers a button to this page
    this.addButton = function addButton(elementId, key, type, hero) {
        console.log("hero: " + hero);
        if(hero == "true"){
            console.log("found hero button: " + elementId);
        }

        var button = new Button(this, elementId, key, type, hero);
        this.buttons[elementId] = button;
        return button;
    }

    this.getButton = function getButton(elementId) {
        return this.buttons[elementId];
    }

    this.getInput = function getInput() {
        var curInput = this.input;

        if(this.sendGyroscope){
            var gyro = {
                type: "gyro",
                value: this.parent.gyroscopeData
            }
            curInput["gyro"] = gyro;
        }
        if(this.sendSwipeEvents && this.parent.swipeEvent != null){
            curInput["swipe"] = this.parent.swipeEvent;
        }
        if(this.sendPanEvents && this.parent.panEvent != null){
            curInput["pan"] = this.parent.panEvent;
        }

        this.input = {};
        return curInput;
    }

    this.register = function register() {
        for (key in this.buttons) {
            this.buttons[key].register();
        }
    }

    this.unregister = function unregister() {
        for (key in this.buttons) {
            this.buttons[key].unregister();
        }
    }

    this.show = function show() {
        //console.log("showing page: " + this.elementId);
        // actually hide the page
        document.getElementById(this.elementId).style.display = 'block';
        // unregister events
        this.register();
    }

    this.hide = function hide() {
        //console.log("Hiding page: " + this.elementId);
        // actually show the page
        document.getElementById(this.elementId).style.display = 'none';
        // register events
        this.unregister();
    }

    this.updateData = function updateData () {
        var dataObjects = document.getElementById(this.elementId).querySelectorAll("[air-data]");
        for (var i = 0; i < dataObjects.length; i++) {
            dataObjects[i].innerHTML = parent.customData[dataObjects[i].getAttribute("air-data")];
        }
    }

    // a callback for this page
    this.eventCallback = function eventCallback(elementId, event) {
        this.input[this.buttons[elementId].key] = {
            type: this.buttons[elementId].type + "-button",
            value: this.buttons[elementId].value,
            event: event
        };
        this.parent.sendData(true);
    }
}

// button class
var Button = function Button(page, elementId, key, type, hero) {
    this.elementId = elementId;
    this.eventTypeDown = "click";
    this.eventTypeUp = "click";
    this.page = page;
    this.key = key;
    this.value = 0;
    this.callback = null;
    this.hero = hero;
    this.type = type;

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
            if(this.type == "tap"){
                this.eventTypeDown = "touchstart";
            }
            else {
                this.eventTypeDown = "touchstart";
                this.eventTypeUp = "touchend";
            }
        }
        else {
            if(this.type == "tap"){
                this.eventTypeDown = "click";
            }
            else {
                this.eventTypeDown = "mousedown";
                this.eventTypeUp = "mouseup";
            }
        }

        // split value from key
        var parts = this.key.split(":");
        if(parts.length > 1){
            this.key = parts[0];
            this.value = parts[1];
        }

        document.getElementById(this.elementId).addEventListener(this.eventTypeDown, this.onDownHandler.bind(this), false);

        if(this.type == "hold"){
            document.getElementById(this.elementId).addEventListener(this.eventTypeUp, this.onUpHandler.bind(this), false);
        }
    }

    this.unregister = function unregister() {
        document.getElementById(this.elementId).removeEventListener(this.eventTypeDown, this.onDownHandler.bind(this));

        if(this.type == "hold"){
            document.getElementById(this.elementId).removeEventListener(this.eventTypeUp, this.onUpHandler.bind(this));
        }
    }

    this.onDownHandler = function onDownHandler (e) {
        this.eventHandler("down");
    }

    this.onUpHandler = function onUpHandler (e) {
        this.eventHandler("up");
    }

    this.eventHandler = function eventHandler(event) {
        console.log("hero test: " + this.hero);
        if(this.hero == "true" && controller.enableHero == false){
            controller.sender.airconsole.getPremium();
            console.log("GetPremium");
            return;
        }
        console.log("test");

        if ("vibrate" in navigator) {
            navigator.vibrate = navigator.vibrate || navigator.webkitVibrate || navigator.mozVibrate || navigator.msVibrate;

            if(AirController.vibrate){
                window.navigator.vibrate(50);
            }
        }

        // send this event to the page
        this.page.eventCallback(this.elementId, event);
        // call the callback if there is one
        if (this.callback != null) this.callback(event);
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
        this.airconsole = new AirConsole({orientation: orientation, device_motion: 120});

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
                        if(!controller.enableHero && obj[this.device_id].enablehero){
                            controller.enableHero = true;
                            controller.onBecameHero();
                        }

                        controller.showPage(obj[this.device_id].view);

                        controller.customData = obj[this.device_id].customdata;
                        controller.onData(obj[this.device_id].customdata);

                        controller.currentPage.updateData();

                        document.body.className = "P" + (obj[this.device_id].playerId + 1) + " " + obj[this.device_id].class + " " + obj[this.device_id].color;
                    }
                }
                catch(e) {
                    console.log("Error handle here!");
                    console.error("parsing data: " + data.custom);
                    console.error(e);
                }
            }
        }

        this.airconsole.onDeviceMotion = function (data) {
            controller.gyroscopeData = data;
            controller.sendData(true);
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

            console.log(this.data);

            this.airconsole.message(0, this.data);

            this.lastData = this.data;
            this.data = {};

            controller.panEvent = null;
            controller.swipeEvent = null;
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

var controller = new AirController();