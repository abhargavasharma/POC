(function(module) {
    'use strict';
    module.directive('talFocusOnError', function (EVENT, $timeout) {
        return {
            restrict: 'A',
            link: function(scope, elem) {

                // CODE SOURCE: http://web.archive.org/web/20140213105950/http://itnewb.com/tutorial/Creating-the-Smooth-Scroll-Effect-with-JavaScript

                function currentYPosition() {
                    // Firefox, Chrome, Opera, Safari
                    if (window.self && window.self.pageYOffset) {
                        return window.self.pageYOffset;
                    }
                    // Internet Explorer 6 - standards mode
                    if (document.documentElement && document.documentElement.scrollTop) {
                        return document.documentElement.scrollTop;
                    }
                    // Internet Explorer 6, 7 and 8
                    if (document.body.scrollTop) {
                        return document.body.scrollTop;
                    }
                    return 0;
                }

                function elmYPosition(elm) {
                    var y = elm.offsetTop;
                    var node = elm;
                    while (node.offsetParent && node.offsetParent !== document.body) {
                        node = node.offsetParent;
                        y += node.offsetTop; 
                    }
                    return y;
                }

                function smoothScroll(elm) {
                    var startY = currentYPosition();
                    var stopY = elmYPosition(elm);
                    var distance = stopY > startY ? stopY - startY : startY - stopY;
                    if (distance < 100) {
                        scrollTo(0, stopY); return;
                    }
                    var speed = Math.round(distance / 100);
                    if (speed >= 20) {
                        speed = 20;
                    }
                    var step = Math.round(distance / 25);
                    var leapY = stopY > startY ? startY + step : startY - step;
                    var timer = 0;
                    var i;
                    var scrollTimeoutFunc = function() { window.scrollTo(0, leapY); };
                    if (stopY > startY) {
                        for (i = startY; i < stopY; i += step) {
                            $timeout(scrollTimeoutFunc, timer * speed);
                            leapY += step;
                            if (leapY > stopY) {
                                leapY = stopY;
                            }
                            timer++;
                        }
                        return;
                    }
                    for (i = startY; i > stopY; i -= step) {
                        $timeout(scrollTimeoutFunc, timer * speed);
                        leapY -= step;
                        if (leapY < stopY) {
                            leapY = stopY;
                        }
                        timer++;
                    }
                }

                // set up event handler on the form element
                scope.$on(EVENT.SUBMIT.ON_ERROR, function () {
                    $timeout(function() {
                        // find the first invalid element
                        var firstInvalid = elem[0].querySelector('.ng-invalid');

                        // if we find one, set focus
                        if (firstInvalid) {
                            smoothScroll(firstInvalid);
                        }
                    }, 100);
                });
            }
        };
    });

})(angular.module('appCustomerPortal'));