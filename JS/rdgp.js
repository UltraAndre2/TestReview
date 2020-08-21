/**
 * RGDP cookie bar
 */
(function ($, RGDP) {
    'use strict';
    var WINDOW;
    var CONFIG;
    var ACCEPT_ALL = true;

    //---------------------------------
    //    EVENT LIST
    //---------------------------------
    $(document).ready(function () {
        // INIT CONFIG
        initConfig();

        // Exit if non display banner
        if(!CONFIG.display_banner) {
            return;
        }

        // Open widow
        $('#wari-rgdp').dialog({
            modal: true,
            autoOpen: true,
            resizable: true,
            draggable: false,
            show: { effect: "fadeIn", duration: 200 },
            dialogClass: 'cookie-dialog',
            open: function(){
            }
        });

        // Events
        $('.wari-rgdp__btn-params').click(function (e) {
            e.preventDefault();
            ACCEPT_ALL = false;
            $('.wari-rgdp__setting').toggleClass('active');
        });

        $('.wari-rgdp__btn-accept').click(function (e) {
            e.preventDefault();

            $.ajax({
                url: $('#wari-rgdp__ajax_link').attr("href"),
                type: 'POST',
                data: getAcceptData(),
                success: function (re) {
                    $('#wari-rgdp').dialog('close');
                }
            });
        });
    });

    /**
     * Create Accept data
     *
     * @returns {{cookie_media: *, cookie_analytics: *, cookie_customization: *}|{cookie_media: boolean, cookie_analytics: boolean, cookie_customization: boolean}|*}
     */
    function getAcceptData() {
        if(ACCEPT_ALL) {
            return {
                "cookie_media": true,
                "cookie_analytics": true,
                "cookie_customization": true
            };
        }

        return  {
            "cookie_media": $('#input-cookie-media').prop('checked'),
            "cookie_analytics": $('#input-cookie-analytics').prop('checked'),
            "cookie_customization": $('#input-cookie-customization').prop('checked')
        };
    }

    /**
     * Create data for dataLayer push
     *
     * @returns {{"'privacy_policy'": *}}
     */
    function getDataLayerPush() {
        var data = getAcceptData();
        data.display_banner = false;

        data.cookie_media = ConvertInGTMVar(data.cookie_media);
        data.cookie_analytics = ConvertInGTMVar(data.cookie_analytics);
        data.cookie_customization = ConvertInGTMVar(data.cookie_customization);

        return {"'privacy_policy'": data};
    }

    /**
     * @return {string}
     */
    function ConvertInGTMVar(val) {
        if(val) {
            return "selected_true";
        }

        return "selected_false";
    }

    /**
     * Get options
     */
    function initConfig() {
        if(RGDP.display_banner === "1") {
            RGDP.display_banner = true;
        } else {
            RGDP.display_banner = false;
        }

        CONFIG = RGDP;
    }
})(jQuery, RGDP);
