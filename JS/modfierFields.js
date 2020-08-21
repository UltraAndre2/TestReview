/**
 * Wari modifier fields
 */
(function ($) {
    'use strict';
    
    var DATA = {
        windowInst: null
    };

    //---------------------------------
    //    EVENT LIST
    //---------------------------------
    $(document).ready(function () {
		
		// Trigger click all div
		/*$('.wt-search-list__item').click(function(){
			$(this).find('.wt-search-list__item-detail').trigger('click');
		});*/
		
        // Sort list
        $('body').on('click', '.wt-search-list__sort li.ui-menu-item', function () {
            var sortKey = $('.wt-list-sort-select').val();
            var sortUrl = $('.sort_urls a[data-sort="'+sortKey+'"]').attr('href');
            if(sortUrl !== "" && sortUrl !== "undefined") {
                location.href = sortUrl;
            }
        });

        // Type change
        $('.wt_form_search__type_input').on('ifChecked', function () {
            if($(this).val() === "a") {
                $('.wt-reserve__date_return').hide();
            } else {
                $('.wt-reserve__date_return').show();
            }

        });

        // Modify Class
        $('.wt__modify-class').click(function () {
            modifyClass();
        });

        // Modify passenger
        $('.wt-reserve__passenger-editor-btn').click(function () {
            var entity = $(this).data('entity');
            if(entity === 'adult') {
                recalcAdult($(this));
            } else if(entity === 'child') {
                recalcChild($(this));
            }
        });

        // Modify passenger window
        $('.wt__modify-passenger').click(function () {
            modifyPassenger();
        });
    });

    //---------------------------------
    //    FUNCTION LIST
    //---------------------------------

    function recalcChild($btn) {
        var $field = $('#child_count');
        var count = parseInt($field.val());

        var type = $btn.data('type');

        if(type === '-') {
            count = count - 1;
            removeChildAge();
        } else if(type === '+') {
            count = count + 1;
            addChildAge(count);
        }

        if(count < 0) {
            count = 0;
        }

        $field.val(count);
        $('.passenger-child-count').text(count);
    }

    function removeChildAge() {
        $('.wt-modal__child-age:last-child').remove();
    }

    function addChildAge(count) {
        var html = '<div class="wt-modal__child-age clearfix">\n' +
            '<div class="wt-modal__child-age_title">1 Child '+count+'</div>\n' +
            '<div class="wt-modal__child-age_input">\n' +
            '<input type="text" required="true" class="wt-input valid wt-field child_age_field" name="tx_waritravel_wari_travel_reserve_air[passenger_age][]" placeholder="Age" />\n' +
            '</div>\n' +
            '</div>';

        $('.children__age').append(html);
    }


    function recalcAdult($btn) {
        var adultCount = parseInt($('#adult_count').val());
        var type = $btn.data('type');

        if(type === '-') {
            adultCount = adultCount - 1;
        } else if(type === '+') {
            adultCount = adultCount + 1;
        }

        if(adultCount < 1) {
            adultCount = 1;
        }

        setAdultCount(adultCount);
    }

    function setAdultCount(count) {
        $('#adult_count').val(count);

        $('.passenger-adult-count').text(count);
    }

    function modifyPassenger() {
        DATA.windowInst = $.fancybox.open({
            src: "#input-block__passenger",
            type: "inline",
            buttons : ['close'],
            touch: false,
            beforeClose: function () {
				var emptyFields = $('.child_age_field').filter(function()
				{
					 return ($(this).val() == "");
				});
                if(emptyFields.length != 0){
					swal(translateObj['title_error'], translateObj['required'], "error");
					return false;
				}
            },
            afterClose: function () {
                recalculateChild();
            }
        });
    }

    function recalculateChild() {
        var childInfo = '';
        var groups = {};
            $.each($('input.child_age_field'), function () {
                if(groups[$(this).val()]) {
                    groups[$(this).val()] = groups[$(this).val()] + 1;
                } else {
                    groups[$(this).val()] = 1;
                }
            });

            console.log(groups);
        $.each(groups, function (years, count) {
            childInfo = childInfo + '<br /> '+count+' Child: ' + years + ' years';
        });

        $('.passenger-child-info').html(childInfo);
    }

    function modifyClass() {
        DATA.windowInst = $.fancybox.open({
            src: "#input-block__class",
            type: "inline",
            buttons : ['close'],
            touch: false,
            afterClose: function () {
                var $radioSelected = $('.wt-reserve__form_class:checked');
                var label = $('label[for="' + $radioSelected.attr('id') + '"]').text();

                $('.wt-input__block-class-content').text(label);
            }
        });
    }
}) (jQuery);
