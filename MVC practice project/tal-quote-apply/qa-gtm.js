// create dummy element for events
(function(){
    var dtmEventDiv = document.createElement('div');
    dtmEventDiv.setAttribute('id', "talDtmHolder");
    dtmEventDiv.setAttribute('style','display:none');
    document.body.appendChild(dtmEventDiv);
})();

function() {

    var onDomReady = function(callBack) {
        if(document.readyState === "complete") {
            callBack();
        }
        else 
        {
            document.addEventListener("DOMContentLoaded", callBack, false);
        }
    };

    digitalData = digitalData || {};
    digitalData.events = digitalData.events || {};
    digitalData.events.push = function() {
         var args = Array.prototype.slice.call(arguments);

            args.forEach(function(arg) {
                process(arg);
            });

            function process(evt) {
                var sJSONBuff = '';
                if (typeof(evt) != 'object') {
                    sJSONBuff = '{"type": "analytics", "event": "' + evt + '"}';
                    evt = JSON.parse(sJSONBuff);
                }              

                onDomReady(function() {
                     var event = document.createEvent("CustomEvent");
                     event.initCustomEvent(evt.type, true, false, evt);
                     document.getElementById("talDtmHolder").dispatchEvent(event);
                });
            }
    }

    