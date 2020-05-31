function desactivefilieres() {
    if (document.getElementById('F1').checked == true) {
        document.getElementById('F2').checked = false;
        document.getElementById('F2').checked = false;


    }

    if (document.getElementById('T1').checked == true) {
        document.getElementById('T2').checked = false;
        document.getElementById('T3').checked = false;


    }



    if (document.getElementById('D2').checked == true) {
        document.getElementById('D1').checked = false;
        document.getElementById('D3').checked = false;


    }

    if (document.getElementById('P2').checked == true) {
        document.getElementById('P1').checked = false;
        document.getElementById('P3').checked = false;

    }



    if (document.getElementById('F2').checked == true) {
        document.getElementById('F1').checked = false;
        document.getElementById('F3').checked = false;


    }

    if (document.getElementById('T2').checked == true) {
        document.getElementById('T1').checked = false;
        document.getElementById('T3').checked = false;


    }



    if (document.getElementById('D2').checked == true) {
        document.getElementById('D1').checked = false;
        document.getElementById('D3').checked = false;


    }

    if (document.getElementById('P2').checked == true) {
        document.getElementById('P1').checked = false;
        document.getElementById('P3').checked = false;


    }



    if (document.getElementById('F3').checked == true) {
        document.getElementById('F1').checked = false;
        document.getElementById('F2').checked = false;


    }

    if (document.getElementById('T3').checked == true) {
        document.getElementById('T2').checked = false;
        document.getElementById('T1').checked = false;


    }



    if (document.getElementById('D3').checked == true) {
        document.getElementById('D2').checked = false;
        document.getElementById('D1').checked = false;


    }

    if (document.getElementById('P3').checked == true) {
        document.getElementById('P2').checked = false;
        document.getElementById('P1').checked = false;


    }

}

$(".date").datepicker({
    format: 'dd/mm/yyyy'
});