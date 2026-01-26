function add_autocomplete_grid($obj, controls) {
    var row_idx = $("#TDETAIL_IDX").val();
    var oldFn = $.ui.autocomplete.prototype._renderItem;
    $.ui.autocomplete.prototype._renderItem = function (ul, item) {
        var re = new RegExp(this.term, "i");
        var t = item.label.replace(re, "<strong style='font-weight:bold;background:#C7DFFE;border-radius:2px;border:1px solid #98B8E1'>" + this.term + "</strong>");
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };
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
                                case 6:
                                    jsonArg.xparam4 = data;
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
                    case 6:
                        $("." + value)[myRow].value = ui.item.xparam4;
                        break
                }
             

                count++
            });

             //var CHRGDON = $(".CHRGDON")[myRow].value;
            
             //var TRANDREFID = $(".TRANDREFID")[myRow].value;
             //var TARIFFMID = $(".TARIFFMID").val();
             // //alert(TARIFFMID);
             //if(CHRGDON != "4") {
             //    $tr.find(".TRANDRATE").attr('readonly', 'readonly');
             //    base = window.location.origin;
             //    var url = base + "/OP_Estimate/GetAmount/" +TRANDREFID + "-" + TARIFFMID;
             //    $.ajax({
             //        url: url,
             //        async: false,
             //        success: function (data) {

             //            $(".TRANDRATE")[myRow].value=data;

             //        }


             //    });//---End of Post--------

             //    $("#add_detail").click();
             //    $(".TRANDREFNAME")[myRow + 1].focus();
             //    //$(".TMPVALIDT")[myRow].value="-1";
             //}
             //else {
                
             //    $tr.find(".TRANDRATE").removeAttr('readonly', 'readonly');
             //    base = window.location.origin;
             //    var url = base + "/OP_Estimate/GetAmount/" + TRANDREFID + "-" + TARIFFMID;
               
             //    $.ajax({
             //        url: url,
             //        async: false,
             //        success: function (data) {
                       
             //            $(".TRANDRATE")[myRow].value = data;

             //        }


             //    });//---End of Post--------
             //    $tr.find(".TRANDRATE").focus();
             //    $("#add_detail").click();
             //   // $(".TMPVALIDT")[myRow].value = "-1";
             //}
            
             total();
           
             $tr.find(".TRANDID").val("0");
            





            return false




         

        },
        messages: {
            noResults: "",
            results: ""
        }
    })
}