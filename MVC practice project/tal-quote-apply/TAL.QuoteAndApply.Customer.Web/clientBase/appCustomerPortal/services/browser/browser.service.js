'use strict';

angular.module('appCustomerPortal').service('talBrowserService', function () {
    var isDevice = {
        Android: function () {
            return navigator.userAgent.match(/Android/i);
        },
        AndroidChrome: function () {
            return window.chrome && navigator.userAgent.match(/Android/i);
        },
        BlackBerry: function () {
            return navigator.userAgent.match(/BlackBerry/i);
        },
        iOS: function () {
            return navigator.userAgent.match(/iPhone|iPad|iPod/i);
        },
        Opera: function () {
            return navigator.userAgent.match(/Opera Mini/i);
        },
        Windows: function () {
            return navigator.userAgent.match(/IEMobile/i);
        },
        any: function () {
            var isany = (isDevice.Android() || isDevice.BlackBerry() || isDevice.iOS() || isDevice.Opera() || isDevice.Windows());
            return (isany !== null);
        },
        touch: function () {
            return 'ontouchstart' in window || 'onmsgesturechange' in window;
        }
    };

    // Checks if the device supports touch.
    // https://ctrlq.org/code/19616-detect-touch-screen-javascript
    this.isTouch = function () {
        return (('ontouchstart' in window) || (navigator.MaxTouchPoints > 0) || (navigator.msMaxTouchPoints > 0));
    };

    this.isMobileDevice = function () {
        return isDevice.any();
    };

    this.detectIe = function () {
        var ie = (function () {
            var undef,
              v = 3,
              div = document.createElement('div'),
              all = div.getElementsByTagName('i');

            while (div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->', all[0]) { }

            return (v > 4) ? v : undef;
        }()),

        htmlElement = document.documentElement;

        if (ie) { htmlElement.className += ' is-ie'; }
        if (ie < 8) { htmlElement.className += ' lt-ie8'; }
        if (ie < 9) { htmlElement.className += ' lt-ie9'; }
        if (ie < 10) { htmlElement.className += ' lt-ie10'; }

        // For IE10, the value of the Version token ("MSIE") was changed to "10.0". The value of the Trident token ("Trident") became "6.0".
        if (/MSIE 10/i.test(navigator.userAgent)) {
            htmlElement.className += ' is-ie is-ie10';
        }

        if (/rv:11.0/i.test(navigator.userAgent)) {
            htmlElement.className += ' is-ie is-ie11';
        }

        //Try with just 'Edge'. Remove /12. this might pick up any version of edge.
        if (/Edge/i.test(navigator.userAgent)) {
            htmlElement.className += ' is-ie is-edge';
        }
    };
});