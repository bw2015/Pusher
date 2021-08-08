!function (a, b) { "function" == typeof define && define.amd ? define([], b) : "undefined" != typeof module && module.exports ? module.exports = b() : a.ReconnectingWebSocket = b() }(this, function () { function a(b, c, d) { function l(a, b) { var c = document.createEvent("CustomEvent"); return c.initCustomEvent(a, !1, !1, b), c } var e = { debug: !1, automaticOpen: !0, reconnectInterval: 1e3, maxReconnectInterval: 3e4, reconnectDecay: 1.5, timeoutInterval: 2e3 }; d || (d = {}); for (var f in e) this[f] = "undefined" != typeof d[f] ? d[f] : e[f]; this.url = b, this.reconnectAttempts = 0, this.readyState = WebSocket.CONNECTING, this.protocol = null; var h, g = this, i = !1, j = !1, k = document.createElement("div"); k.addEventListener("open", function (a) { g.onopen(a) }), k.addEventListener("close", function (a) { g.onclose(a) }), k.addEventListener("connecting", function (a) { g.onconnecting(a) }), k.addEventListener("message", function (a) { g.onmessage(a) }), k.addEventListener("error", function (a) { g.onerror(a) }), this.addEventListener = k.addEventListener.bind(k), this.removeEventListener = k.removeEventListener.bind(k), this.dispatchEvent = k.dispatchEvent.bind(k), this.open = function (b) { h = new WebSocket(g.url, c || []), b || k.dispatchEvent(l("connecting")), (g.debug || a.debugAll) && console.debug("ReconnectingWebSocket", "attempt-connect", g.url); var d = h, e = setTimeout(function () { (g.debug || a.debugAll) && console.debug("ReconnectingWebSocket", "connection-timeout", g.url), j = !0, d.close(), j = !1 }, g.timeoutInterval); h.onopen = function () { clearTimeout(e), (g.debug || a.debugAll) && console.debug("ReconnectingWebSocket", "onopen", g.url), g.protocol = h.protocol, g.readyState = WebSocket.OPEN, g.reconnectAttempts = 0; var d = l("open"); d.isReconnect = b, b = !1, k.dispatchEvent(d) }, h.onclose = function (c) { if (clearTimeout(e), h = null, i) g.readyState = WebSocket.CLOSED, k.dispatchEvent(l("close")); else { g.readyState = WebSocket.CONNECTING; var d = l("connecting"); d.code = c.code, d.reason = c.reason, d.wasClean = c.wasClean, k.dispatchEvent(d), b || j || ((g.debug || a.debugAll) && console.debug("ReconnectingWebSocket", "onclose", g.url), k.dispatchEvent(l("close"))); var e = g.reconnectInterval * Math.pow(g.reconnectDecay, g.reconnectAttempts); setTimeout(function () { g.reconnectAttempts++, g.open(!0) }, e > g.maxReconnectInterval ? g.maxReconnectInterval : e) } }, h.onmessage = function (b) { (g.debug || a.debugAll) && console.debug("ReconnectingWebSocket", "onmessage", g.url, b.data); var c = l("message"); c.data = b.data, k.dispatchEvent(c) }, h.onerror = function (b) { (g.debug || a.debugAll) && console.debug("ReconnectingWebSocket", "onerror", g.url, b), k.dispatchEvent(l("error")) } }, 1 == this.automaticOpen && this.open(!1), this.send = function (b) { if (h) return (g.debug || a.debugAll) && console.debug("ReconnectingWebSocket", "send", g.url, b), h.send(b); throw "INVALID_STATE_ERR : Pausing to reconnect websocket" }, this.close = function (a, b) { "undefined" == typeof a && (a = 1e3), i = !0, h && h.close(a, b) }, this.refresh = function () { h && h.close() } } return a.prototype.onopen = function () { }, a.prototype.onclose = function () { }, a.prototype.onconnecting = function () { }, a.prototype.onmessage = function () { }, a.prototype.onerror = function () { }, a.debugAll = !1, a.CONNECTING = WebSocket.CONNECTING, a.OPEN = WebSocket.OPEN, a.CLOSING = WebSocket.CLOSING, a.CLOSED = WebSocket.CLOSED, a });
!function () {
    class goeasy {

        constructor(config) {
            let t = this;
            t.config = config;
            t.channels = [];
            t.time = new Date().getTime();

            t.ws = new ReconnectingWebSocket("wss://" + config.host + "/?host=" + location.host);

            t.ws.onopen = () => {
                t.success = true;
                t.init = null;

                // 如果发生重连则自动订阅频道
                if (t.channels.length) {
                    t.channels.forEach(channel => {
                        t.subscribe({
                            channel: channel
                        });
                    });
                } else if (config.onConnected) {
                    config.onConnected();
                }
            };

            t.ws.onmessage = data => {
                // 心跳信息
                if (data.data === "1") {
                    t.time = new Date().getTime();
                    return;
                }
                t.onmessage(data);

            }

            t.ws.onerror = () => {

            };

            // 心跳检测
            setInterval(() => {
                if (new Date().getTime() - t.time > 10 * 1000) {
                    t.ws.close();
                }
            }, 1000);
        }

        subscribe(options) {
            let t = this;
            if (arguments.length === 2) {
                options = {
                    channel: arguments[0],
                    onMessage: arguments[1]
                }
            }
            if (!t.success || !t.init) {
                setTimeout(t.subscribe.bind(t), 500, options);
                return;
            }
            if (!t.channels.includes(options.channel)) {
                t.channels.push(options.channel);
            }
            t.ws.send(JSON.stringify({
                action: "subscribe",
                channel: options.channel,
                sid: t.init.sid
            }));
            if (!t.message) t.message = new Object();
            if (options.onMessage) {
                t.message[options.channel] = options.onMessage;
            }
        }

        unsubscribe(channel) {

        };

        onmessage(data) {
            let t = this,
                message = JSON.parse(data.data);
            switch (message.action) {
                // 初始化
                case "InitResponse":
                    t.init = message;
                    if (!t.ping) {
                        t.ping = setInterval(() => {
                            t.ws.send("0");
                        }, t.init.pingInterval);
                    }
                    break;
                // 订阅成功
                case "SubscribeResponse":

                    break;
                // 收到消息推送
                case "MessageResponse":
                    t.message[message.channel] && t.message[message.channel](message);
                    break;
            }
        };
    };



    window["GoEasy"] = goeasy;
}();