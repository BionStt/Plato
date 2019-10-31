﻿// <reference path="~/js/app/plato.js" />
// <reference path="~/js/vendors/jquery.js" />

if (typeof window.jQuery === "undefined") {
    throw new Error("jQuery 3.3.1 or above Required");
}

if (typeof window.$.Plato === "undefined") {
    throw new Error("$.Plato Required");
}

(function ($, win, doc, undefined) {

    "use strict";

    var prettyprint = function () {

        var dataKey = "anchorific",
            dataIdKey = dataKey + "Id";

        var defaults = {         
        };

        var methods = {            
            init: function ($caller, methodName, func) {

                if (methodName) {
                    if (this[methodName] !== null && typeof this[methodName] !== "undefined") {
                        return this[methodName].apply(this, [$caller, func]);
                    } else {
                        alert(methodName + " is not a valid method!");
                    }
                    return null;
                }

                this.bind($caller);

            },
            bind: function ($caller) {

                console.log("prettyprint - bind");

                // convert code blocks          
                $caller.find("pre").each(function () {
                    // apply css to code block
                    var item = $(this),
                        isTidy = item.data("isTidy");
                    if (!isTidy) {

                        var lang = item.data("language"),
                            langCss = (lang ? " lang-" + lang : ""),
                            langToolTip = lang ? lang.toUpperCase() + " - " : "";

                        item.data("isTidy", "true");
                        item.addClass("prettyprint" + langCss);

                        item.click(function () {
                            
                        });

                    }
                });

                if (win.prettyPrint) {
                    win.prettyPrint();
                }


            }
        };

        return {
            init: function () {

                var options = {},
                    methodName = null,
                    func = null;          
                for (var i = 0; i < arguments.length; ++i) {
                    var a = arguments[i];
                    switch (a.constructor) {
                        case Object:
                            $.extend(options, a);
                            break;
                        case String:
                            methodName = a;
                            break;                     
                        case Function:
                            func = a;
                            break;
                    }
                }

                if (this.length > 0) {
                    // $(selector).prettyprint()
                    return this.each(function () {
                        if (!$(this).data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $(this).data(dataIdKey, id);
                            $(this).data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $(this).data(dataKey, $.extend({}, $(this).data(dataKey), options));
                        }
                        return methods.init($(this), methodName, func);
                    });
                } else {
                    // $().prettyprint()
                    var $caller = $('[data-provide="markdownBody"]');
                    if ($caller.length > 0) {
                        if (!$caller.data(dataIdKey)) {
                            var id = dataKey + parseInt(Math.random() * 100) + new Date().getTime();
                            $caller.data(dataIdKey, id);
                            $caller.data(dataKey, $.extend({}, defaults, options));
                        } else {
                            $caller.data(dataKey, $.extend({}, $caller.data(dataKey), options));
                        }
                        return methods.init($caller, methodName, func);
                    }
                }

            }
        };

    }();

    // ---------------

    $.fn.extend({
        prettyprint: prettyprint.init
    });

    // ---------------

    var app = win.$.Plato;

    app.ready(function () {

        // Locals
        var $elem = $('[data-provide="markdownBody"]'),
            $target = null,
            opts = {
                anchorTitle: app.T("Link to this section"),
                emptyText: app.T("No sections found"),
                onClick: function ($caller, $header, $link) {
                    // Track targeted header so we can update the
                    // history state after infiniteScrolls scrollEnd event
                    $target = $header;
                }
            };

        // Apply anchorific
        $elem.prettyprint(opts);

        // Activate anchorific when loaded via infiniteScroll load
        $().infiniteScroll("ready", function ($ele) {
            $elem.prettyprint(opts);
        });

    });

})(jQuery, window, document);