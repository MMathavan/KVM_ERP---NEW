function sel_text(obj, dest) {



    row = obj.parentNode.parentNode.rowIndex;


    var modelLength = obj.options[obj.selectedIndex].text;




    $("." + dest)[row - 1].value = modelLength;


    var selid = '/Common/CostFactorDetail/' + obj.value;



    $.getJSON(selid, function (data) {
        var items = [];


        $.each(data, function (key, val) {
            if (val.CFMODE == "0") val.CFMODE = "+";
            else if (val.CFMODE == "1") val.CFMODE = "-";
            $(".CFMODE")[row - 1].value = val.CFMODE;
            $(".CFEXPR")[row - 1].value = val.CFEXPR;
            $(".CFTYPE")[row - 1].value = val.CFTYPE;
            $(".DORDRID")[row - 1].value = val.DORDRID;

        });

        total();

    });







}


function del_factor(obj) {

    row = obj.parentNode.parentNode.rowIndex;

    document.getElementById("CFACTOR").deleteRow(row);

    total();
    return false;

}


function round(value, exp) {
    if (typeof exp === 'undefined' || +exp === 0)
        return Math.round(value);

    value = +value;
    exp = +exp;

    if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0))
        return NaN;

    // Shift
    value = value.toString().split('e');
    value = Math.round(+(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp)));

    // Shift back
    value = value.toString().split('e');
    return +(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp));
}



function total() {
    TRANNAMT = 0;
    TRANGAMT = 0;
    ATRANGAMT = 0;
    CF_ROW_TOTAL = 0;
    i = 0;
    TRANDGAMT = 0;
    TRANDNAMT = 0;

    TRANCGSTAMT = 0;
    TRANSGSTAMT = 0;
    TRANIGSTAMT = 0;

    TRANBTYPE = 0;// $(".TRANBTYPE").val();

    //var select = document.getElementById("TRANTID");
    //var TRANTID = select.options[select.selectedIndex].value;

    $('.INVDID').each(function () {



        TRANDQTY = $(".INVDQTY")[i].value;
        TRANDBRATE = $(".INVDRATE")[i].value;

        //TRANDCGSTAMT = 0;
        //TRANDSGSTAMT = 0;
        //TRANDIGSTAMT = 0

        TRANDCGSTEXPRN = 9;//parseFloat($(".TRANDCGSTEXPRN")[i].value);
        TRANDSGSTEXPRN = 9;//parseFloat($(".TRANDSGSTEXPRN")[i].value);
        TRANDIGSTEXPRN = 0;

        var GROSSAMT = eval(TRANDBRATE) * eval(TRANDQTY);

        //if (TRANDCGSTEXPRN > 0)
        //    TRANDCGSTAMT = parseFloat((GROSSAMT * TRANDCGSTEXPRN) / 100).toFixed(3);

        //if (TRANDSGSTEXPRN > 0)
        //    TRANDSGSTAMT = parseFloat((GROSSAMT * TRANDSGSTEXPRN) / 100).toFixed(3);

        //if (TRANDIGSTEXPRN > 0)
        //    TRANDIGSTAMT = parseFloat((GROSSAMT * TRANDIGSTEXPRN) / 100).toFixed(3);

        j = 0;
        TRANDTAMT = 0;


        $('.CFNO').each(function () {

            DORDRID = $('.CFNO')[j].value;
            DEDEXPRN = $('.CF')[j].value;

       


            switch (DORDRID) {
                case "2":
                    break;
                case "3":
                    break;
                case "4":
                    break;
                case "5":
                    break;
                case "6":
                    break;
                case "7":
                    break;
            }
            j++;
        });

  
        
        TRANDGAMT = (parseFloat(TRANDQTY) * parseFloat(TRANDBRATE)).toFixed(2);
        TRANDGAMT = Math.round(TRANDGAMT, 0).toFixed(2)

        TRANGAMT = (parseFloat(TRANGAMT) + parseFloat(TRANDGAMT)).toFixed(2);
        
        if (isNaN(TRANDGAMT))
            TRANDGAMT = 0;
       // if (DORDRID == 7) {  }
        $(".INVDGAMT")[i].value = TRANDGAMT;
        $(".INVDNAMT")[i].value = TRANDGAMT;


        i++;

    });

    if (isNaN(TRANGAMT))
        TRANGAMT = 0;

    $("#INVGAMT").val(TRANGAMT);

    i = 0;
    $('.TAX').each(function () {


        temp = 0;

        temp_tax = TRANGAMT;


        if (i > 0)
            temp_tax = eval(TRANGAMT) + eval(CF_ROW_TOTAL);



        if ($(".CFTYPE")[i].value == 1)
            temp = eval(temp_tax) * eval($(".CFEXPR")[i].value) / 100;
        else
            temp = eval($(".CFEXPR")[i].value).toFixed(2);




        switch ($(".DORDRID")[i].value) {
            case "4":

                if (i < 1) {
                    temp = 0;
                    alert("Correct Excise Order");

                    break;

                }
                ex = $(".DORDRID")[i - 1].value;
                ex = ex.trim();

                if ($(".CFTYPE")[i].value != 0)
                    temp = eval($(".CFAMOUNT")[i - 1].value) * eval($(".CFEXPR")[i].value) / 100;
                else
                    temp = eval($(".CFEXPR")[i].value);


                if (ex != "3") {
                    temp = 0;
                    alert("Correct Excise Order");
                }

                break;
            case "5":

                if (i < 2) {
                    temp = 0;
                    alert("Correct Excise Order");

                    break;

                }

                ex = $(".DORDRID")[i - 2].value;
                ex = ex.trim();

                sec = $(".DORDRID")[i - 1].value;
                sec = ex.trim();


                if ($(".CFTYPE")[i].value != 0)
                    temp = eval($(".CFAMOUNT")[i - 2].value) * eval($(".CFEXPR")[i].value) / 100;
                else
                    temp = eval($(".CFEXPR")[i].value);


                if (ex != "3" && sec != "4") {
                    temp = 0;
                    alert("Correct Excise Order");
                }



                break;
        }





        if (isNaN(temp))
            temp = 0;


        if ($(".CFMODE")[i].value == "+")
            CF_ROW_TOTAL = eval(CF_ROW_TOTAL) + eval(temp);
        else
            CF_ROW_TOTAL = eval(CF_ROW_TOTAL) - eval(temp);

        $(".CFAMOUNT")[i].value = eval((temp)).toFixed(2);
        i++;
    });
    TRANNAMT = (eval(TRANGAMT) + eval(CF_ROW_TOTAL)).toFixed(2);
    //alert(TRANNAMT);
    

    if (TRANDCGSTEXPRN > 0)
        TRANCGSTAMT = parseFloat((TRANNAMT * TRANDCGSTEXPRN) / 100).toFixed(2);

    if (TRANDSGSTEXPRN > 0)
        TRANSGSTAMT = parseFloat((TRANNAMT * TRANDSGSTEXPRN) / 100).toFixed(2);

    if (TRANDIGSTEXPRN > 0)
        TRANIGSTAMT = parseFloat((TRANNAMT * TRANDIGSTEXPRN) / 100).toFixed(2);

    TRANCGSTAMT = Math.round(TRANCGSTAMT, 0).toFixed(2)
    TRANSGSTAMT = Math.round(TRANSGSTAMT, 0).toFixed(2)
    TRANIGSTAMT = Math.round(TRANIGSTAMT, 0).toFixed(2)

    TRANCGSTAMT =  parseFloat(TRANCGSTAMT).toFixed(2);
    TRANSGSTAMT =  parseFloat(TRANSGSTAMT).toFixed(2);
    TRANIGSTAMT =  parseFloat(TRANIGSTAMT).toFixed(2);

    //alert(TRANCGSTAMT);
    //alert(TRANSGSTAMT);
    //alert(TRANIGSTAMT);

    TRANNAMT = (eval(TRANNAMT) + eval(TRANCGSTAMT) + eval(TRANSGSTAMT) + eval(TRANIGSTAMT)).toFixed(2);

    if (isNaN(TRANNAMT))
        TRANNAMT = 0;

    $("#INVNAMT").val(TRANNAMT);
    $("#INVCGSTAMT").val(TRANCGSTAMT);
    $("#INVSGSTAMT").val(TRANSGSTAMT);
    $("#INVIGSTAMT").val(TRANIGSTAMT);

}


$(document).ready(function () {

    function sleep(milliseconds) {
        var start = new Date().getTime();
        for (var i = 0; i < 1e7; i++) {
            if ((new Date().getTime() - start) > milliseconds) {
                break;
            }
        }
    }



    $(document).on("click", "#cfact", function () {


        var $tableBody = $('#CFACTOR').find("tbody"),
$trLast = $tableBody.find("tr:last");

        $("#cf_dynamic").css("display:block");


        var tax_param = "";
        i = 0;


        $('.TAX').each(function () {

            tax_param = tax_param + $.trim(this.value) + ",";


            pos = $('#CFACTOR tr').eq($('#CFACTOR tr').length - 1);


            idx = $('#CFACTOR tr').length - 2;

            desc = $(".CFDESC")[idx].value;





            pos.find('td:eq(1)').html("<input type=text value=" + this.value + " id='TAX' class='TAX' name='TAX' style='display:none' >" + desc + "<input type=text style='border:none' readonly=readonly value='" + desc + "' name=CFDESC id='CFDESC' class='CFDESC hide'> ");





            i++;
        });



        var formData = { term: tax_param }; //Array 

        $.ajax({
            url: "/Common/CostFactor",
            type: "POST",
            data: formData,
            success: function (data, textStatus, jqXHR) {
                //data - response from server

                if (data.length != 0)
                    $trLast.after("<tr class='item-row'><Td class='col-lg-1'> <button href='#'   type='button' onclick='del_factor(this)' class='btn btn-default dfact'><i class=glyphicon-minus></i> </button>  </td> <td>" + data + " </td><td class='col-lg-2'><input  type=text value='' name=CFAMOUNT id='CFAMOUNT' class='CFAMOUNT form-control' readonly='readonly'> </td>  </TD></tr>");




                total();


            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });



        total();
        return false;

    });

});
