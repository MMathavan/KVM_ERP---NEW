
//..grid
function split(val) {//...autocomplete function
    return val.split(/,\s*/)
}
function extractLast(term) {
    return split(term).pop()
}
function add_autocomplete_grid($obj, controls) {
    var oldFn = $.ui.autocomplete.prototype._renderItem;
    $.ui.autocomplete.prototype._renderItem = function (ul, item) {
        var re = new RegExp(this.term, "i");
        var t = item.label.replace(re, "<strong style='font-weight:bold;background:#C7DFFE;border-radius:2px;border:1px solid #98B8E1'>" + this.term + "</strong>");
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };
    var row_idx = $("#TDETAIL_IDX").val();




    $obj.autocomplete({
        source: function (request, response) {

            $.ajax({
                url: $obj.data("autocomplete-url"),
                type: "POST",
                dataType: "json",
                max: 10,
                scrollable: true,
                data: {
                    term: request.term
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        count = 0;
                        item_str = "";
                        var jsonArg = new Object();
                        count = 0;
                        $.each(item, function (i, data) {
                            switch (count) {
                                case 0:
                                    jsonArg.label = data;
                                    jsonArg.value = data;
                                    break;
                                case 1:
                                    jsonArg.id = data;
                                    break;
                                case 2:
                                    jsonArg.desc = data;
                                    break;
                                case 3:
                                    jsonArg.xparam1 = data;
                                    break;
                                case 4:
                                    jsonArg.xparam2 = data;
                                    break;
                                case 5:
                                    jsonArg.xparam3 = data;
                                    break
                            }
                            count++
                        });
                        return jsonArg
                    }))
                }
            })
        },
        search: function () {
            var term = extractLast(this.value);
            if (term.length < 1) {
                return false
            }
        },
        select: function (event, ui) {
            var $tr = $obj.closest('tr');
            var myRow = $tr.index();
            $(this).val(ui.item.label);
            count = 0;
            $.each(controls.split(','), function (index, value) {
                switch (count) {
                    case 1:
                        $("." + value)[myRow].value = ui.item.id;
                        break;
                    case 2:
                        $("." + value)[myRow].value = ui.item.desc;
                        break;
                    case 3:
                        $("." + value)[myRow].value = ui.item.xparam1;
                        break;
                    case 4:
                        $("." + value)[myRow].value = ui.item.xparam2;
                        break;
                    case 5:
                        $("." + value)[myRow].value = ui.item.xparam3;
                        break
                }
                count++
            });
            //alert("fff");
            //alert($tr.find(".ITEMDESC").val);
            $(".ITEMDESC")[myRow].value = "";
            $(".TRANDREFNAME")[myRow].value = "";
            //$(".UNITID")[myRow].value = "";
            //$(".TRANDREFID")[myRow].value = "";
            return false
        },
        messages: {
            noResults: "",
            results: ""
        }
    })
}
//......
function split(val) {//...cascaded autocomplete function
    return val.split(/,\s*/)
}
function extractLast(term) {
    return split(term).pop()
}
function add_autocomplete_grid_cascade($obj, controls) {
    var oldFn = $.ui.autocomplete.prototype._renderItem;
    $.ui.autocomplete.prototype._renderItem = function (ul, item) {
        var re = new RegExp(this.term, "i");
        var t = item.label.replace(re, "<strong style='font-weight:bold;background:#C7DFFE;border-radius:2px;border:1px solid #98B8E1'>" + this.term + "</strong>");
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };
    var row_idx = $("#TDETAIL_IDX").val();

    //alert($(".MTRLID")[row_idx].value);


    $obj.autocomplete({
        source: function (request, response) {

            $.ajax({
                url: $obj.data("autocomplete-url"),
                type: "POST",
                dataType: "json",
                max: 10,
                scrollable: true,
                data: {
                    term: request.term + ";" + $(".MTRLID")[row_idx].value
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        count = 0;
                        item_str = "";
                        var jsonArg = new Object();
                        count = 0;
                        $.each(item, function (i, data) {
                            switch (count) {
                                case 0:
                                    jsonArg.label = data;
                                    jsonArg.value = data;
                                    break;
                                case 1:
                                    jsonArg.id = data;
                                    break;
                                case 2:
                                    jsonArg.desc = data;
                                    break;
                                case 3:
                                    jsonArg.xparam1 = data;
                                    break;
                                case 4:
                                    jsonArg.xparam2 = data;
                                    break;
                                case 5:
                                    jsonArg.xparam3 = data;
                                    break
                            }
                            count++
                        });
                        return jsonArg
                    }))
                }
            })
        },
        search: function () {
            var term = extractLast(this.value);
            if (term.length < 1) {
                return false
            }
        },
        select: function (event, ui) {
            var $tr = $obj.closest('tr');
            var myRow = $tr.index();
            $(this).val(ui.item.label);
            count = 0;
            $.each(controls.split(','), function (index, value) {
                switch (count) {
                    case 1:
                        $("." + value)[myRow].value = ui.item.id;
                        break;
                    case 2:
                        $("." + value)[myRow].value = ui.item.value;
                        break;
                    case 3:
                        $("." + value)[myRow].value = ui.item.xparam1;
                        break;
                    case 4:
                        $("." + value)[myRow].value = ui.item.xparam2;
                        break;
                    case 5:
                        $("." + value)[myRow].value = ui.item.xparam3;
                        break
                }
                count++
            });

            var base = window.location.origin;
            var curl = base + "/PurchaseOrder/GetPrice/" + $tr.find(".TRANDREFID").val() + ";" + $(".TRANREFID").val();

            $.ajax({
                type: 'get',
                url: curl, // we are calling json method
                dataType: 'json',
                success: function (data) {
                    if (data != "") {
                        $tr.find(".TRANDRATE").val(data[0].TRANDRATE);
                    }
                    else {
                        $tr.find(".TRANDRATE").val(0);
                    }
                },
                error: function (ex) {
                    alert('Failed to data ' + ex);
                }
            });

            return false
        },
        messages: {
            noResults: "",
            results: ""
        }
    })
}