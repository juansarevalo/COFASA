'use strict';

var ciasGridButtons = '';
var tableCias;
var dialogCias = null;

$(document).ready(function () {
    prepareCiaButtons();
    initGrid();
});

function prepareCiaButtons() {
    const bodyButtons = $('#ciasGridButtons').val();
    const tags = $('<div/>');
    tags.append(bodyButtons);

    $('#addCiaBtn').click(function(){ showCiasForm(); });
    ciasGridButtons = '<center>' + tags.html() + '<center>';
}

function bindCiasButtons() {
    $('#ciasDt tbody tr td button').unbind('click').on('click', function (event) {
       if (event.preventDefault) { event.preventDefault(); }
       if (event.stopImmediatePropagation) { event.stopImmediatePropagation(); }

        const obj = JSON.parse(Base64.decode($(this).parent().attr('data-row')));
        const action = $(this).attr('data-action');

        if (action === 'edit') { showCiasForm(obj.cod); }
       // if (action === 'delete') { deleteRecord(obj.id); }
       if (action === 'copy') { showCiasForm(obj.cod, 1); }
    });
}

function initGrid() {
    tableCias = $('#ciasDt')
        .on('draw.dt', function () {
            drawRowNumbers('#ciasDt', tableCias);
            setTimeout(function(){bindCiasButtons();},500);
        })
        .DataTable({
            'language': { url: '/plugins/jquery.dataTables/1.13.7/i18n/es-MX.json' },
            'ajax': {
                'url': '/Cias/GetCias',
                'type': 'GET',
                'datatype': 'JSON',
            },
            'columns': [
                { 'data': 'cod' },
                { 'data': 'cod' },
                { 'data': 'razonSocial' },
                { 'data': 'nomComercial' },
                { 'data': 'codCiaCore' },
                {
                    sortable: false, searchable: false,
                    render: function (data, type, row) {
                        return ciasGridButtons.replace('{data}',Base64.encode($.toJSON(row)));
                    }
                }
            ]
        });

    $('#ciasDt')
        .removeClass('display')
        .addClass('table table-bordered table-hover dataTable');
}

function startCiasValidation() {
    $('#ciasForm').validate({ // number: true
        rules: {
            // COD_CIA: { required: true, minlength: 3, maxlength: 3 },
            RAZON_SOCIAL: { required: true, minlength: 3, maxlength: 60 },
            NOM_COMERCIAL: { required: true, minlength: 3, maxlength: 60 },
        },
        messages:{
            // COD_CIA: 'Este campo es obligatorio',
            RAZON_SOCIAL: 'Este campo es obligatorio',
            NOM_COMERCIAL: 'Este campo es obligatorio',
        },
        showErrors: function(errorMap, errorList) {
            this.defaultShowErrors();
        },
        submitHandler: function (form, event) {
            isLoading('#btnSaveCia', true);
            if ($('#IS_COPYING').val()!=='') { copyCia(); }
            else { saveCia(); }
        }
    });
}

function showCiasForm(id, makeCopy) {
    const isCopying = !(typeof (makeCopy) === 'undefined');
    const isEditing = !(typeof (id) === 'undefined' || id === 0) && !isCopying;
    var title = '';

    if (isEditing) {
        title = 'Editar compañía';
    } else if (isCopying) {
        title = 'Copiar compañía';
    } else {
        title = 'Agregar compañía';
    }

    dialogCias = bootbox.dialog({
        title: title,
        message: $('#ciasFormBody').val(),
        className: 'modalLarge',
        onEscape: true
    });

    startCiasValidation();

    if (isEditing) {
        $('#COD_CIA').val(id);
        $('#btnSaveCiaText').text('Editar');
        loadOneCia(id);
    }
    if (isCopying) {
        $('#IS_COPYING').val(id);
        $('#btnSaveCiaText').text('Copiar');
        loadOneCia(id);
    }
}

function saveCia(){
    const formData = $('#ciasForm').serialize();

    $.ajax({
        url: '/Cias/SaveCia',
        type:'POST',
        data: formData,
        success:function(data){
            showToast(data.success, data.message);
            isLoading('#btnSaveCia', false);

            if(data.success){
                dialogCias.modal('hide');
                tableCias.ajax.reload();
            }
        },
        error: function (error) {
            showToast(false, 'Ocurrió un error al procesar la solicitud');
            isLoading('#btnSaveCia', false);
        }
    });
}

function copyCia(){
    const formData = $('#ciasForm').serialize();

    $.ajax({
        url: '/Cias/CopyCia',
        type:'POST',
        data: formData,
        success:function(data){
            showToast(data.success, data.message);
            isLoading('#btnSaveCia', false);

            if(data.success){
                dialogCias.modal('hide');
                tableCias.ajax.reload();
            }
        },
        error: function (error) {
            showToast(false, 'Ocurrió un error al procesar la solicitud');
            isLoading('#btnSaveCia', false);
        }
    });
}

function loadOneCia(id) {
    $.ajax({
        url: '/Cias/GetCiaById?ciaCod=' + id,
        type:'GET',
        success:function(data){
            if(data.success){
                setDataToCiaForm(data.data);
            }
        },
        error: function (error) {
            console.log(error);
            showToast(false, 'Ocurrió un error al obtener los detalles de la empresa');
        }
    });
}

function setDataToCiaForm(data) {
    $('#RAZON_SOCIAL').val(data.razonSocial);
    $('#NOM_COMERCIAL').val(data.nomComercial);
    $('#COD_CIA_CORE').val(data.codCiaCore);
}