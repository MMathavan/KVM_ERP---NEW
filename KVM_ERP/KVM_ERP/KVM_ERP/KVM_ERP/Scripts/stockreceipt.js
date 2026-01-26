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







function total() {
    i = 0;
    TRANDWGHT = 0;
    TRANGWGHT = 0;

  //  alert("1");

    $('.SRDID').each(function () {

       // alert("2");

        TRANDQTY = $(".SRD_WGHT")[i].value;

      //  alert("3");

        j = 0;
      
        TRANDWGHT = parseFloat(TRANDQTY).toFixed(3);
       // alert(TRANDWGHT);
        TRANGWGHT = (parseFloat(TRANGWGHT) + parseFloat(TRANDWGHT)).toFixed(3);
        
        if (isNaN(TRANDWGHT))
            TRANDWGHT = 0;

        i++;

    });

    if (isNaN(TRANGWGHT))
        TRANGWGHT = 0;

    $("#SRMGAMT").val(TRANGWGHT);

 
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
